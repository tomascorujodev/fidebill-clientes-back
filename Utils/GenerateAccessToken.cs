using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fidebill_clientes_back.Models.AuthModels;
using Microsoft.IdentityModel.Tokens;

namespace Fidebill_clientes_back.Utils;
public class Jwt(IConfiguration configuration)
{
  private readonly IConfiguration _configuration = configuration;
  public JwtSecurityToken GenerateAccessToken(UserLogedModel userLogedModel)
  {
    var claims = new List<Claim>
      {
          new("idCliente", userLogedModel.IdCliente.ToString()),
          new("nombre", userLogedModel.Nombre),
          new("apellido", userLogedModel.Apellido),
          new("genero", userLogedModel.Genero),
          new("email", userLogedModel.Email ??  ""),
          new("direccion", userLogedModel.Direccion ?? ""),
          new("telefono", userLogedModel.Telefono ?? ""),
          new("fechaNacimiento", userLogedModel.Telefono ?? ""),
          new("puntos", userLogedModel.Puntos.ToString()),
          new("idEmpresa", userLogedModel.IdEmpresa.ToString()),
          new("empresa", userLogedModel.UrlClientes),
      };

    var token = new JwtSecurityToken(
        issuer: _configuration["JwtSettings:Issuer"],
        audience: _configuration["JwtSettings:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(120),
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])), SecurityAlgorithms.HmacSha256)
    );

    return token;
  }
}