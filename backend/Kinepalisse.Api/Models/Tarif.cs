namespace Kinepalisse.Api.Models;

public class Tarif
{
    public int IdTarif { get; set; }
    public string TypeTarif { get; set; } = "";
    public decimal Prix { get; set; }
}
