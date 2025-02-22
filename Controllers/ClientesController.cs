using Dapper;
using fidebill_clientes_back.Models.ClientesModels;
using Fidebill_clientes_back.Models;
using Fidebill_clientes_back.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fidebill_clientes_back.Controllers;

[Authorize]
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
}
