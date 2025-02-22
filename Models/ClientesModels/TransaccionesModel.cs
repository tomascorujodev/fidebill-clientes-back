namespace fidebill_clientes_back.Models.ClientesModels;

public class TransaccionesModel
{
    public required int IdTransaccion { get; set; }
    public required string Tipo { get; set; }
    public required DateTime Fecha { get; set; }
    public required int Monto { get; set; }
    public decimal Puntos { get; set; }
    public required bool Estado { get; set; }
    public required string DireccionLocal { get; set; }
}
