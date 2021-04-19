using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens; //Vai validar se tem uma chave vai servir tanto pra intancia de token como pra Symmetric



//Essa classe Startup é uma classe para inicializar tudo , ou melhor é a classe de serviços é onde começa tudo

//Na linha 29 o serviço utiliza um lambda pra informar qual banco dedados o usuario quer usar nesse caso foi escolhido o UseInMemoryDatabase, lembrando que temos que utilizar a biblioteca EntityFrameworkCore

//Depois de informar o banco precisamos tornar o dataContext disponivel para os controller, ou dizendo melhor injenção de dependencias, pra controler funcionar ele precisa de uma dataContext

//Utilizando o AddScoped, eu estou garantindo pro codigo que eu so tenho um DataContext por requisiçao, e toda vez que uma requisiçao pra aplicação ele vai cria rum dataContext da memoria, toda vez que uma controller pedir um dataContext ele vai mandar o memso dataContext da memoria 

//Em vez de utilizar o metodo UseInMemoryDatabase ele preferiu usar os comandos do sql server por isso deixei como comentario a linha 36

//Entao o metodo do sql server é o UseSqlServer, substituto do UseMemory

//O AddResponseCompression ele vai comprimir o arquivo json antes de mandar pra tela, vai pegar a informaçao e vai zipar ele na tela  e depois o html vai descompactar

//Depois na parte de ResponseCompressionDefaults , ele vai comprimir tudo que for application/json 

//O metodo AddResponseCaching();, ele vai adicionar por padrao o cabechalo de cache  na minha aplicação , la no postman

//Metodo AddCors() , serve para nao ter problemas com itens de transform , se tentar fazer requsiçoes no localhost ae nao funciona

namespace Shop
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }


    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      services.AddResponseCompression(options =>
      {
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
      });

      //services.AddResponseCaching();
      services.AddControllers();

      //Parte de Autehnticação 
      var key = Encoding.ASCII.GetBytes(Settings.Secret);
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
      services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
      services.AddScoped<DataContext, DataContext>(); //Injeção de dependencia 

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Api", Version = "v1" });
      });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        //O objeto acessa o metodo UseDeveloper Para poder retornar algum erro caso ele nao consiga conectar na porta do banco
      }

      app.UseHttpsRedirection();
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
      });


      app.UseRouting();

      app.UseCors(x => x
      .AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader());

      app.UseAuthentication();
      app.UseAuthorization(); //Esse metodo vai ser responsavel por dizer o que os roles podem fazer na aplicação 

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

  }
}





/*
Comentarios importantes 
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

    //Se estiver em produção e quiser chavear alguma coisa , voce utiliza o objeto 'env', Agora caso eu queira saber de alguma coisa da aplicação eu utilizo o  objeto 'app'

    //Se estivermos em tempo de desenvolvimento utilize o metodo IsDevelopment no if abaixo, o if serve para exibir os erros de banco em tempo de desenvolvimento

     services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop", Version = "v1" });
      });


   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        //O objeto acessa o metodo UseDeveloper Para poder retornar algum erro caso ele nao consiga conectar na porta do banco
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

  app.UseAuthentication() // Autneticação serve pra dizer quem o usuario é

  app.UseAuthorization(); //Esse metodo vai ser responsavel por dizer o que os roler  podem fazer na aplicação 


  */