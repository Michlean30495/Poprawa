namespace KolokwiumPoprawa.Models;

public class CreateClient
{
    public ClientAddDto Client { get; set; }
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}