using Microsoft.EntityFrameworkCore;
using Shop.Models;

//Uma classe

namespace Shop.Data 
{

  public class DataContext : DbContext
  {

      public DataContext(DbContextOptions<DataContext> options) : base(options)
      {
          
      }

    //Os DbSet sao as representações de memoria do banco de dados, ele vai buscar no banco tabelas do produto, categoria, e user atraves dos modelos criados, e vai mapear os itens pro modelo

    //Eles vao permitir que a gente utilize o Crud , sao os responsaveis pelas açoes que tomamos dentro do banco

      public DbSet<Product> Products {get; set;}
      public DbSet<Category> Categories {get; set;}
      public DbSet<User> Users {get; set;}


  }


}