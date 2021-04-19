using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

//Essa classe é destinada para alimentação da API , criando usuarios, gerente, categorias e produto para os data Notations
//Em vez de fazer pelo cide, estamos utilizando uma classe de controller

namespace Shop.Controllers
{

  [Route("v1")]
  public class HomeControoler : Controller
  {
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<dynamic>> Get(
      [FromServices] DataContext context)
    {
      var employee = new User { Id = 1, Username = "robin", Password = "robin", Role = "employee" };
      var manager = new User { Id = 2, Username = "batman", Password = "batman", Role = "manager" };
      var category = new Category { Id = 1, Title = "Informática" };
      var product = new Product { Id = 1, Category = category, Title = "Mouse", Price = 299, Description = "Mouse Gamer" };
      context.Users.Add(employee);
      context.Users.Add(manager);
      context.Categories.Add(category);
      context.Products.Add(product);
      await context.SaveChangesAsync();

      return Ok(new { message = "Dados configurados" });
    }
  }
}