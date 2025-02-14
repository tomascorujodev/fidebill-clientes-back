namespace fidebill_clientes_back.Models.ClientesModels;

public class VerClientesModel
{
    public required string IdCliente { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Documento { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public required string Genero { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public required string TipoCliente { get; set; }
    public decimal? Puntos { get; set; }
}
