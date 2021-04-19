using System.ComponentModel.DataAnnotations;
//Biblioteca acima que serve para fazer os datas de verificação de chave e de campo

using System.ComponentModel.DataAnnotations.Schema;
//Biblioteca acima referente a todos os esquemas do sql serve, como nome da tabela, tipo de dado

namespace Shop.Models
{

  //[Table("Categoria")] //Criando o nome da tabela do Banco, Se eu nao crio o banco vai criar sozinho
  public class Category
  {
    [Key] //Chamado de data notation , serve para permitir criar os datas da linha 12 a 14
    //[Column("Cat_ID")] //Criando nome da coluna,Se eu nao crio o banco vai criar sozinho


    public int Id { get; set; }

    //Esses comandos abaixos serão necessarios apra criar no banco de dadoso campo id, ou melhor eu estou limitando a quantidade de caracteres que o usuario precisa colocar que no caso seria minimo 3 e maximo 60
    //[DataType("nvarchar")] //Posso criar o DataType e setar ele com o tipo que eu quiser tambem

    [Required(ErrorMessage = "Este campo é obrigatório")]
    [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
    [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]


    public string Title { get; set; }
  }
}