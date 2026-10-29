using System.ComponentModel.DataAnnotations;
using GestionOrdinateurs.Shared.src.enums;

public class User
{
    public int Id { get; set; }

    [Required]
	public string Login { get; set; } = string.Empty;

    public RoleEnum Role { get; set; }
}