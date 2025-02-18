using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Fidebill_clientes_back.Repositories;
using Dapper;
using Fidebill_clientes_back.Models.AuthModels;
using Microsoft.AspNetCore.Authorization;
using Fidebill_clientes_back.Utils;

namespace Fidebill_clientes_back.Controllers;

[ApiController]
[Route("Authclientes")]
public class AuthController(IConfiguration configuration, Repository repository) : ControllerBase
{
  private readonly Jwt jwt = new(configuration);
  private readonly IConfiguration _configuration = configuration;

  [HttpPost("Login")]
  public async Task<IActionResult> Login(LogInModel model)
  {
    try
    {
      DynamicParameters dynamicParameters = new();
      dynamicParameters.Add("@documento", model.Documento);
      dynamicParameters.Add("@id_empresa", model.IdEmpresa);
      UserLogedModel? result = await repository.GetOneByProcedure<UserLogedModel>("validar_usuario_cliente", dynamicParameters);
      if(result == null)
      {
        return Unauthorized(new { error = true, message = "El usuario y contraseña son incorrectos" });
      }
      if((result.Clave == null && model.Password == model.Documento.ToString()) || (!string.IsNullOrEmpty(result.Clave) && BCrypt.Net.BCrypt.Verify(model.Password, result.Clave))){
        JwtSecurityToken claims = jwt.GenerateAccessToken(result);
        string token = new JwtSecurityTokenHandler().WriteToken(claims);
        return Ok(new { error = false, token });
      }
      else
      {
        return Unauthorized(new { error = true, message = "El usuario y contraseña son incorrectos" });
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return StatusCode(500, new { error = true, message = "Ha ocurrido un error al momento de realizar la peticion, por favor contacte con el administrador" });
    }
  }

  [HttpPost("Registrarse")]
  public async Task<IActionResult> Registrarse(NewUserModel cliente)
  {
    try
    {
      DynamicParameters dynamicParameters = new();
      dynamicParameters.Add("@email", cliente.Email);
      dynamicParameters.Add("@id_empresa", cliente.IdEmpresa);
      NewUserValidationModel result = await repository.GetOneByProcedure<NewUserValidationModel>("validar_correo", dynamicParameters);

      if (result != null)
      {
        if (result.Email == cliente.Email)
        {
          return Conflict(new { error = true, Message = "El correo se encuentra en uso" });
        }
        else if (result.Documento == cliente.Documento)
        {
          return Conflict(new { error = true, Message = "El documento ya se encuentra registrado" });
        }
        else
        {
          return StatusCode(500, new { error = true, Message = "Ha ocurrido un error, por favor, contacte con un administrador" });
        }
      }
      string hashedPassword = BCrypt.Net.BCrypt.HashPassword(cliente.Password, BCrypt.Net.BCrypt.GenerateSalt(12));

      IEnumerable<string> palabras = cliente.Nombre.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => char.ToUpper(p[0]) + p[1..].ToLower());
      cliente.Nombre = string.Join(" ", palabras);

      palabras = cliente.Apellido.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                      .Select(p => char.ToUpper(p[0]) + p[1..].ToLower());
      cliente.Apellido = string.Join(" ", palabras);

      dynamicParameters = new();
      dynamicParameters.Add("@nombre", cliente.Nombre);
      dynamicParameters.Add("@apellido", cliente.Apellido);
      dynamicParameters.Add("@Documento", cliente.Documento);
      dynamicParameters.Add("@fecha_nacimiento", cliente.FechaNacimiento);
      dynamicParameters.Add("@Genero", cliente.Genero);
      dynamicParameters.Add("@email", cliente.Email);
      dynamicParameters.Add("@direccion", cliente.Direccion);
      dynamicParameters.Add("@telefono", cliente.Telefono);
      dynamicParameters.Add("@clave", hashedPassword);
      dynamicParameters.Add("@id_empresa", cliente.IdEmpresa);
      dynamicParameters.Add(
          "@fecha_alta",
          TimeZoneInfo.ConvertTimeFromUtc(
              DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time")
          )
      );
      int affectedRows = await repository.InsertByProcedure("registro_cliente", dynamicParameters);
      if (affectedRows > 0)
      {
        return Ok(new { error = false });
      }
      else
      {
        Console.WriteLine("No se pudo registrar el cliente en Auth/Registrarse");
        return StatusCode(500, new { error = true, Message = "Verifique los datos ingresados" });
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return StatusCode(500, new { error = true, message = "No se pudo hacer el ingreso. Por favor, contacte con un administrador." });
    }
  }

  [HttpGet("checkempresa")]
  public async Task<IActionResult> CheckEmpresa([FromQuery]string empresa)
  {
    try
    {
      DynamicParameters dynamicParameters = new();
      dynamicParameters.Add("@empresa", empresa);
      CheckCompanyModel? result = await repository.GetOneByProcedure<CheckCompanyModel?>("check_empresa", dynamicParameters);
      if (result != null)
      {
        return Ok(new { error = false, response = result });
      }
      else
      {
        return NotFound( new { error = true, Message = "La empresa no existe" });
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return StatusCode(500, new { error = true, message = "Ocurrio un problema al intentar cargar la pagina" });
    }
  }

  [HttpGet("cambiarclave")]
  [Authorize]
  public async Task<IActionResult> CambiarClave(string clave)
  {
    try
    {
      string idEmpresa = User.FindFirst("idEmpresa").Value;
      DynamicParameters dynamicParameters = new();
      dynamicParameters.Add("@id_empresa", idEmpresa);
      dynamicParameters.Add("@clave", clave);
      CheckCompanyModel? result = await repository.GetOneByProcedure<CheckCompanyModel?>("cambiar_clave", dynamicParameters);
      if (result != null)
      {
        return Ok(new { error = false, response = result });
      }
      else
      {
        return NotFound( new { error = true, Message = "La empresa no existe" });
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return StatusCode(500, new { error = true, message = "Ocurrio un problema al intentar cargar la pagina" });
    }
  }

  [HttpGet("validatetoken")]
  [Authorize]
  public async Task<IActionResult> ValidateToken()
  {
    return Ok("Acceso autorizado.");
  }

}