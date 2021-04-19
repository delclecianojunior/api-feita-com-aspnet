using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Services;

namespace Shop.Controllers
{

  [Route("users")]
  public class UserController : Controller
  {

    //AsNoTracking()
    //ToListAsync()
    //manager , significa que o gerente pode ver a lista de funcionarios nesse metodo get

    [HttpGet]
    [Route("")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<User>>> Get(
      [FromServices] DataContext context)
    {
      var users = await context
      .Users
      .AsNoTracking()
      .ToListAsync();
      return users;
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    //[Authorize = "manager"] significa que so o gerente pode criar
    public async Task<ActionResult<User>> Post(
      [FromServices] DataContext context,
      [FromBody] User model)
    {
      //Verifica se os dados são válidos
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        //Força o usuario a ser sempre "funcionario"
        model.Role = "employee";

        context.Users.Add(model);
        await context.SaveChangesAsync();

        //Esconde a senha
        model.Password = "";
        return model;
      }

      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível criar o usuário" });
      }
    }

    //
    //Na parte de Authorize abaixo apenas o gerente pode alterar um outro usuario por conta do manager

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<User>> Put(
      [FromServices] DataContext context,
      int id,
      [FromBody] User model)
    {
      //Verifica se os dados são válidos
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //Verifica se o ID informado é o mesmo do modelo
      if (id != model.Id)
      {
        return NotFound(new { message = "Usuário não encontrado" });
      }

      try
      {
        context.Entry(model).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return model;
      }

      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível criar o usuário" });
      }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<dynamic>> Authenticate(
      [FromServices] DataContext context,
      [FromBody] User model)
    {
      var user = await context.Users
      .AsNoTracking()
      .Where(x => x.Username == model.Username && x.Password == model.Password)
      .FirstOrDefaultAsync();

      if (user == null)
      {
        return NotFound(new { message = "Usuário ou Não foi possível criar o usuáriosenha inválidos" });
      }

      var token = TokenService.GenerateToken(user);

      //Esconde a senha
      user.Password = "";
      return new
      {
        user = user,
        token = token
      };
    }

  }
}

//Essa classe é destinada para criar o usuario e fazer autenticação tanto no UserName como na senha , é o controlador do dataNotation User