using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using System;

//EndPoint => URL
//https://localhost: 5000
//https://localhost:5001/categories quando for o meu servidor vai ser o meu dominio no lugar desse aqui , lembrando que em produção sempre sera https
//Para utilizar as requisiçoes de https sao utilizados os chamados de verbo ou melhor metodos Crud (get,post,put,delete)
//Dica as requisiçoes post, put e delete eles possuem corpo(body) entao eu consigo passar mais informaçoes 
//Utilizando o versetion v1/ fica mais facil de criar outras rotas


[Route("v1/categories")] 
//Para definir uma rota basta fazer isso aqui e na rota de baixo significa que uma rota ficara dentro de outra rota 

public class CategoryController : ControllerBase {
  
  //Para trabalhar de forma paralela basta utilizar o task
  //Existe tambem o ActionResult, ele serve para trazer resultados em formato que a tela espera, ou melhor ele traaz status de erros
  //Podemos entao ter um Task de ActionResult com uma categoria dentro dos dois
  //E na frente da task colocamos async para ele indetificar que o metodo vai ser assincrono, ele cria threds paralelas e serve para nao travar a execução principal
  //Podemos tambem criar uma lista de categorias, entao foi posto um list na frente de category
  //O metodo AsNoTracking() serve para jogar o conteudo criado na tela, tudo que for leitura e necessario usar ele
 //ToListAsync seve para executar a query
 //O get vai servir para retornar todas as categorias construidas pelo post
 //Colocando AllowAnonymous voce está permitindo que  todos usuario ou  gerente possa fazer alteraçoes nos metodos 
 

//O Metodo ResponseCache, ele ta dizendo que ele esta cacheando apenas no metodo Get, a location esta dizendo que vai armazenar em qualquer lugar com uma duração de 30 minutos

//Para cachear basta fazer o seguinte  // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    [HttpGet] //Essa linha serve para identificar que é um metodo HttpGet
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    public async Task<ActionResult<List<Category>>> Get(
      [FromServices]DataContext context)  
    {
      var categories = await context.Categories.AsNoTracking().ToListAsync();
      return Ok(categories);
    }

    //Sempre que eu colocar um elemento dentro de chaves {id} ele vai encarar que é um parametro
    //Dica c# e uma linguagem tipada entao sempre coloque o tipo na frente do parametro
    //Se dentro do parametro voce colocar :int voce esta dizendo que so é permitido passar numeros inteiros ae quando for rodar no postman vai dar erro 404, chamado de restrição de rota
    //Rest apis trabalham com json
    //FirstOrDefaultAsync irar ter difenca no metodo GetById, a diferença e que ele vai ter o metodo AsNoTracking antes pra nao armazenar nenhuma informaçao adicional 
    //Eu acho que esse metodo reseta todos as coleçoes 

    [HttpGet]
    [Route("{id:int}")] 
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id,
       [FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(category);
    }


    //Existe um cara no asp net chamado de modelBinder ele é o responsavel por ligar o json com o c#, necessario para converter o objeto de json em objeto c#
    //Com o ActionResult adicionado no metodo precisamos converter model para ActionResult
    //Para converter basta colocarmos ok e o model dentro Ok(model)
    //Existe a propiedade ModelState que serve para armazenar o estado do modelo passado , ele pega o que esta no modelState e verifica se esta valido lembrando que tem o negado na frente do modelState , se nao tiver valido eu vou retornar um BadRequest , ou melhor retorna uma mensagem pro usuario de requisiçao invalida
    //Se caso estiver valida retorne o modelo
    //Lembrando que o teste esta sendo feito com o json de 'title' entao se nao passar um texto nele , ele retorna os erros dos datas notations , isso so é possivel pois passamos o ModelState no BadRequest
    //Foi adicionado o FromServices pra identificar que o DataContext vem da ckasse serviços
    //Depois precisamos Adicionar uma nova categoria no metodo post , basta utilizar o context.Categories.Add(model), ae podemos adicionar a model que ja e categoria dentro de context.Categories -> DbSet, mas nao é so isos precisamos persitir para que ele vai
    //Entao utilizamos um propiedade chamada de SaveChangesAsync(),                context.SaveChangesAsync() , lembrando de por o await na frente 
    //Como estamos utilizando uma forma assincrona precisamos do await
    //O await serve pra Aguardar ate que as mudanças sejam salvas
    //O Authorize passando roles = employee ta dizendo que apenas os funcionarios podem alterar o metodo post

    [HttpPost] 
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post(
       [FromBody]Category model,
       [FromServices]DataContext context)  
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        } 
         //Utilizando o Try catch podemos tratar os erros que vem do banco
        
         try 
         {
            context.Categories.Add(model);
            await  context.SaveChangesAsync();
            return Ok(model);
         }
         catch
         {
           return BadRequest(new {message = "Não foi possivel criar a categoria"});
         }
    }


    //O metodo put é a mesclagem do metodo get com o metodo post entao passamos parametros no route e no put alem de passar tambem o fromBody no metodo put
    //Se utilizar o ActionResult no metodo put ele me permite passar NotFound() caso o id  da requisiçao seja diferente do id do corpo, tornando mais elegante
     //No if abaixo se eu informar um id diferente me retorna um erro
    //Agora quando é verificado um erro é retornado uma mensagem , podemos fazer isso com o new {} retornando um objeto
    //No ModelState se eu nao passar nada em algum campo , o BadRequest me retorna um erro , esses erros vem la dos datasnotations criados na Model
    //Caso seja um id igual me retorna o modelo do corpo da requisição
    //Para Atualizar uma categoria basta utilizar o context.Entry<Category>
    //Lembrando que sempre que estiver utilizando um metodo assincrono utilizar o awayt e o SaveChangesAsync()
    //O Authorize  passando roles = employee ta dizendo que apenas os funcionarios podem alterar o metodo put

    
      [HttpPut] 
      [Route("{id:int}")]
      [Authorize(Roles = "employee")]
      public async Task<ActionResult<List<Category>>> Put(int id, 
      [FromBody]Category model,
      [FromServices]DataContext context) 
    {
      //Verifica se o ID informado é o mesmo do modelo
         if(model.Id != id)
         {
           return NotFound(new {message = "Categoria não encontrada "});
         }
     
      //Verifica se os dados são válidos
       if(!ModelState.IsValid)
       {
         return BadRequest(ModelState);
       }

      try 
      {
         context.Entry<Category>(model).State = EntityState.Modified;
         await context.SaveChangesAsync();
         return Ok(model);
      }

      catch(DbUpdateConcurrencyException)
      {
          return BadRequest(new {message = "Este registro ja foi atualizado"});
      }    

      catch(Exception)  
      { 
          return BadRequest(new {message = "Não foi possivel atualizar a categoria"});
      }

    }

   //O metodo delete so é necessario passar o id pela rota
   //Devemos colcoar o awayt na frente do metodo First pois se nao ele vai passar direto
   //
   //FirstOrDefaultAsync , esse metodo serve para buscar uma categoria dado uma expressao
   //Passado a expressao x , onde o x e a categoria se o id da categoria for igual ao id do corpo 
  //Caso a categoria venha nula , retorne um NotFound com a mensagem categoria nao encotrada
  //Caso nao seja o elemento pode ser removido, sendo feito o try catch
  //No metodo Remove passamos a category
  //Mas antes de fazer isso tudo é criado a categoria pra so depois ser feito a remoção


    [HttpDelete] 
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Delete(
      int id,
      [FromServices]DataContext context) 
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if(category == null)
        {
          return NotFound(new {message = "Categoria não encontrada"});
        }

     try 
     {
       context.Categories.Remove(category);
       await context.SaveChangesAsync();
       return Ok(new {message = "Categoria removida com sucesso"});
     }
      catch(Exception)
      {
         return BadRequest(new {message = "Não foi possivel remover a categoria"});
      } 
    }
}

//Dica importante os funcionarios podem criar alterar e deletar uma categoria pois estou utilizando Authorizate roles = employee eles tem permissao

/*
   Metodo get que retorna uma id coloquei aqui em baixo porque esta tendo alteração no metodo
    [HttpGet]
    [Route("{id:int}")] 
    public  GetById(int id) //Para criar esse parametro e necessario por o tipo tmb
    {
      return " GET " + id.ToString(); //Converte o id de inteiros em string para aparecer no postman
    }


//Metodo Post 
    //Para postar algo e necessario o nome da classe que voce quer mas a chamada da model, passe isso dentro do metodo Post , o metodo precisa ser do tipo Category pois o model nao é uma string
    //Utilizando o FromBody ele ta dizendo que virar uma categoria no post da requisição
    //A requisição de um post vai vim de duas maneiras em cabeçalho e corpo da requisiçao
    //Existe um cara no asp net chamado de modelBinder ele é o responsavel por ligar o json com o c#, necessario para converter o objeto de json em objeto c#
    

    [HttpPost] 
    [Route("")]
    public Category Post([FromBody]Category model)  
    {
      return model;
    }


//Metodo put
    //O metodo put é a mesclagem do metodo get com o metodo post entao passamos parametros no route e no put alem de passar tambem o fromBody no metodo put
    //No if abaixo se eu informar um id diferente na requisiçao e colocar outro id no corpo da requisição ele vai me retornar null e me retornar o erro 202, caso seja igual retorne minha model
    //Se utilizar o ActionResult no metodo put ele me permite passar NotFound() caso o id  da requisiçao seja diferente do id do corpo, tornando mais elegante

    
    [HttpPut] 
    [Route("{id:int}")]
    public Category Put(int id, [FromBody]Category model) 
    {
         if(model.Id == id)
         {
           return model;
         }

         return null;
    }

   //O metodo delete so é necessario passar o id pela rota

    [HttpDelete] 
    [Route("{id:int}")]
    public string Delete() 
    {
      return "DELETE";
    }

*/