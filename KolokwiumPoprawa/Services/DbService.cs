using System.Data.Common;
using KolokwiumPoprawa.Models;
using Microsoft.Data.SqlClient;

namespace KolokwiumPoprawa.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ClientDto?> GetClient(int clientId)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        var querry = """
                     SELECT client.ID, client.FirstName, client.LastName, 
                            client.Address, c.VIN, col.Name, m.Name, cr.DateFrom, cr.DateTo,
                            c.PricePerDay
                     FROM cars c
                     JOIN models m ON m.ID = c.ModelID
                     JOIN colors col ON col.ID = c.ColorID
                     LEFT JOIN car_rentals cr ON cr.CarID = c.ID
                     LEFT JOIN clients client ON client.ID = cr.ClientID
                     WHERE client.ID = @id
                     """;
        
        await using var cmd = new SqlCommand(querry, connection);
        cmd.Parameters.AddWithValue("@id", clientId);

        await using var reader = await cmd.ExecuteReaderAsync();
        ClientDto? client = null;

        while (await reader.ReadAsync())
        {
            if (client == null)
            {
                client = new ClientDto()
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Address = reader.GetString(3),
                    Rentals = new List<ClientCarDto>()
                };
            }

            if (!reader.IsDBNull(4))
            {
                client.Rentals.Add(new ClientCarDto()
                {
                    Car = new CarDto()
                    {
                        Vin = reader.GetString(4),
                        Color = reader.GetString(5),
                        Model = reader.GetString(6),
                        PricePerDay = reader.GetInt32(9)
                    },
                    DateFrom = reader.GetDateTime(7),
                    DateTo = reader.GetDateTime(8)
                });
            }
        }
        
        return client;
    }

    public async Task<string?> AddClientCarAsync(CreateClient request)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        await connection.OpenAsync();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.CommandText = "SELECT PricePerDay FROM cars WHERE ID = @carId";
            command.Parameters.AddWithValue("@carId", request.CarId);
            var price = await command.ExecuteScalarAsync();
            if (price is not null)
            {
                await transaction.RollbackAsync();
                return "Auto nie istnieje";
            }

            command.Parameters.Clear();

            DateTime data1 = request.DateFrom;
            DateTime data2 = request.DateTo;
            TimeSpan roznica = data2.Subtract(data1);


            var totalPrice = roznica.Days * (int)price;

            command.CommandText = """
                                      INSERT INTO clients (FistName, LastName, Address)
                                      VALUES (@FirstName, @LastName, @Address)
                                  """;
            command.Parameters.AddWithValue("@FirstName", request.Client.FirstName);
            command.Parameters.AddWithValue("@LastName", request.Client.LastName);
            command.Parameters.AddWithValue("@Address", request.Client.Address);
            await command.ExecuteNonQueryAsync();
            
            command.Parameters.Clear();
            command.CommandText = "SELECT ID FROM clients WHERE FirstName = @FristName AND LastName = @LastName AND Address = @Address";
            command.Parameters.AddWithValue("@FirstName", request.Client.FirstName);
            command.Parameters.AddWithValue("@LastName", request.Client.LastName);
            command.Parameters.AddWithValue("@Address", request.Client.Address);
            var clientInserterdID = await command.ExecuteScalarAsync();
            
            command.Parameters.Clear();
            command.CommandText = """
                                      INSERT INTO car_rentals (DateFrom, DateTo, TotalPrice, ClientID)
                                      VALUES (@DateFrom, @DateTo, @TotalPrice, @ClientID)
                                  """;
            command.Parameters.AddWithValue("@DateFrom", request.DateFrom);
            command.Parameters.AddWithValue("@DateTo", request.DateTo);
            command.Parameters.AddWithValue("@TotalPrice", totalPrice);
            command.Parameters.AddWithValue("@ClientID", (int)clientInserterdID);
            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return "dodano";
        }
        catch
        {
            await transaction.RollbackAsync();
            return "nie udalo sie dodac";
        }
    }
}