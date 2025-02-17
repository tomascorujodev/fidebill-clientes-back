namespace Fidebill_clientes_back.Models.AuthModels;
public class UserLogedModel
{
    public int IdCliente { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Genero { get; set; }
    public required string Email { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public int Puntos { get; set; }
    public string? Clave { get; set; }
    public int? IdEmpresa { get; set; }
    public required string UrlClientes { get; set; }

}