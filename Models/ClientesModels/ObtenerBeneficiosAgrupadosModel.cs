namespace fidebill_clientes_back.Models.ClientesModels;
public class ObtenerBeneficiosAgrupadosModel
{
    public required int IdBeneficio { get; set; }
    public required string Tipo { get; set; }
    public int? PorcentajeReintegro { get; set; }
    public required string Descripcion { get; set; }
    public required bool[] Dias { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? UrlImagen { get; set; }
    public List<UsuarioEmpresaInfo>? IdsUsuariosEmpresas { get; set; }

    public class UsuarioEmpresaInfo
    {
        public int? IdUsuarioEmpresa { get; set; }
        public string? NombreUsuarioEmpresa { get; set; }
    }
}
