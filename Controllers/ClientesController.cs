using System.Text.Json;
using Dapper;
using fidebill_clientes_back.Models.ClientesModels;
using Fidebill_clientes_back.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fidebill_clientes_back.Controllers;

[Authorize]
[ApiController]
[Route("vistaclientes")]
public class ClientesController(Repository repository) : ControllerBase
{
    [HttpGet("obtenerlocales")]
    public async Task<IActionResult> ObtenerLocales()
    {
        string idEmpresa = User.FindFirst("idEmpresa").Value;
        DynamicParameters dynamicParameters = new();
        dynamicParameters.Add("@id_empresa", idEmpresa);
        try
        {
            List<BranchModel> sucursales = await repository.GetListByProcedure<BranchModel>("obtener_locales", dynamicParameters);
            if (sucursales == null)
            {
                return NoContent();
            }
            return Ok(sucursales);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { error = true, Message = "No se pudo obtener los locales. Contacte con un administrador." });
        }
    }

    [HttpGet("obtenertransacciones")]
    public async Task<IActionResult> ObtenerTransacciones()
    {
        string idEmpresa = User.FindFirst("idEmpresa").Value;
        string idCliente = User.FindFirst("idCliente").Value;
        DynamicParameters dynamicParameters = new();
        dynamicParameters.Add("@id_empresa", idEmpresa);
        dynamicParameters.Add("@id_cliente", idCliente);
        try
        {
            List<TransaccionesModel> compras = await repository.GetListByProcedure<TransaccionesModel>("obtener_transacciones", dynamicParameters);
            return Ok(new { error = false, compras });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { error = true, Message = "No se pudo obtener las operaciones, por favor, contacte un administrador" });
        }
    }

    [HttpGet("obtenerbeneficios")]
    public async Task<IActionResult> ObtenerBeneficios([FromQuery] int id)
    {
        string idEmpresa = User.FindFirst("idEmpresa").Value;
        DynamicParameters dynamicParameters = new();
        dynamicParameters.Add("@id_empresa", idEmpresa);

        try
        {
            List<ObtenerBeneficiosModel> beneficios = await repository.GetListByProcedure<ObtenerBeneficiosModel>("obtener_beneficios", dynamicParameters);

            var beneficiosAgrupados = beneficios
                .OrderBy(b => b.FechaFin)
                .GroupBy(b => new
                {
                    b.IdBeneficio,
                    b.Tipo,
                    b.PorcentajeReintegro,
                    b.Descripcion,
                    b.Dias,
                    b.FechaInicio,
                    b.FechaFin,
                    b.UrlImagen
                })
                .Select(g => new ObtenerBeneficiosAgrupadosModel
                {
                    IdBeneficio = g.Key.IdBeneficio,
                    Tipo = g.Key.Tipo,
                    PorcentajeReintegro = g.Key.PorcentajeReintegro,
                    Descripcion = g.Key.Descripcion,
                    Dias = JsonSerializer.Deserialize<bool[]>(g.Key.Dias),
                    FechaInicio = g.Key.FechaInicio,
                    FechaFin = g.Key.FechaFin,
                    UrlImagen = g.Key.UrlImagen,

                    IdsUsuariosEmpresas = g.Select(x => new ObtenerBeneficiosAgrupadosModel.UsuarioEmpresaInfo
                    {
                        IdUsuarioEmpresa = x.IdUsuarioEmpresa,
                        NombreUsuarioEmpresa = x.NombreUsuarioEmpresa
                    })
                    .Where(x => x.IdUsuarioEmpresa.HasValue)
                    .Distinct()
                    .ToList()
                })
                .Where(b => b.IdsUsuariosEmpresas == null || b.IdsUsuariosEmpresas.Count == 0 || b.IdsUsuariosEmpresas.Any(x => x.IdUsuarioEmpresa == id))
                .ToList();

            return Ok(new { error = false, beneficiosAgrupados });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { error = true, Message = "No se pudo obtener las operaciones, por favor, contacte un administrador" });
        }
    }

}
