namespace Fidebill_clientes_back.Models.AuthModels;
public class LogInModel
{
    public required string Documento { get; set; }
    public required string Password { get; set; }
    public required string IdEmpresa { get; set; }
}