namespace Fidebill_clientes_back.Models.AuthModels;
public class NewUserValidationModel
{
    public required string Documento { get; set; }
    public string? Email { get; set; }

}