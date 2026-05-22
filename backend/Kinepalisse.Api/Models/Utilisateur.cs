namespace Kinepalisse.Api.Models;

public class Utilisateur
{
    public int IdUtilisateur { get; set; }
    public string Email { get; set; } = "";
    public string MotDePasseHash { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime DateCreation { get; set; }
}
