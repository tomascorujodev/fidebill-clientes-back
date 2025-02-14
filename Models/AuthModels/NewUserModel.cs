namespace Fidebill_clientes_back.Models.AuthModels;
public class NewUserModel
{
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Documento { get; set; }
    public required DateOnly FechaNacimiento { get; set; }
    public required string Genero { get; set; }
    public required string Email { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public required string Password { get; set; }
    public required int IdEmpresa { get; set; }

}