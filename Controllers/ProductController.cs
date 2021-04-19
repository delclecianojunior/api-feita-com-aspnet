using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;


//
//O metodo Include colocado no primeiro HttpGet serve para criar uma categoria, alem de retornar categorias ja criadas
//No segundo HttpGet , ele esta fazendo o select do id do produto, alem incluir a categoria dentro do produto

namespace Shop.Controllers
{
    [Route("products")]
     public class ProductController : ControllerBase
     {
       
       //Colocando o AllowAnonymous podemos listar os produtos de forma anonima
       
         [HttpGet]
         [Route("")] //Essa rota vai pegar a rota raiz "products"
         [AllowAnonymous]
         public async Task<ActionResult<List<Product>>> Get(
               [FromServices] DataContext context)
         {
            var products = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .ToListAsync();
             return products;
         }

         [HttpGet]
         [Route("{id:int}")]
         [AllowAnonymous]
         public async Task<ActionResult<Product>> GetById(
           [FromServices] DataContext context, int id)
        {
          var product = await context
          .Products
          .Include(x => x.Category)
          .AsNoTracking()
          .FirstOrDefaultAsync(x => x.Id == id);
          return product;
        }

  //Nessa rota teremos então products/categories/1
   //Eu qudro listar todos os produtos da categoria 1
   //Metodo Where , serve para fazer o filtro, eu quero anexar todos os produtos onde o o id da categoria for igual ao id do corpo da requisiçao
   //Where sera usado para compor as querys
   [HttpGet]
   [Route("categories/{id:int}")]
   [AllowAnonymous]
   public async Task<ActionResult<List<Product>>> GetByCategory(
      [FromServices] DataContext context, int id)         
    {
       var products = await context
       .Products
       .Include(x => x.Category)
       .AsNoTracking()
       .Where(x => x.CategoryId == id)
       .ToListAsync();
       return products;
    }

//Metodo Post o metodo para criar , ele sera utilizado para verificar se o model esta valido ou nao, caso nao esteja retorna uma messagem de erro , ou error 404
//O funionario pode criar um produto pois tem AllowAnonymous que da permissao pra ele fazer isso
//O Authorize ta dizendo que os funcionarios podem criar um produto

  [HttpPost]
  [Route("")]
  [Authorize(Roles = "employee")]
  public async Task<ActionResult<Product>> Post(
        [FromServices] DataContext context,
        [FromBody] Product model)
  {
     if(ModelState.IsValid)
     {
       context.Products.Add(model);
       await context.SaveChangesAsync();
       return model;
     }

     else 
     {
       return BadRequest(ModelState);
     }
  }


   }
}