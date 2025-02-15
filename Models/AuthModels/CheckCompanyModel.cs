namespace Fidebill_clientes_back.Models.AuthModels;
public class CheckCompanyModel
{
    public required string IdEmpresa { get; set; }
    public string? RutaLogo { get; set; }
    public required string ColorPrincipal { get; set; }
}