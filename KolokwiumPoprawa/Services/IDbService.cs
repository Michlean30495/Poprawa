using KolokwiumPoprawa.Models;

namespace KolokwiumPoprawa.Services;

public interface IDbService
{
    Task<ClientDto?> GetClient(int clientId);
    
    Task<string?> AddClientCarAsync(CreateClient request);
}