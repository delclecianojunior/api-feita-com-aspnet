using System.ComponentModel.DataAnnotations;

//Professor chamou essa classe de mapeamento 

namespace Shop.Models
{
  public class Product
  {

    [Key]
    public int Id { get; set; }

    //Mesmas limitaçoes criadas na classe Caregory caso queira entender cada uma va ate ela
    //Nos datas que tiverem range o numero precisa ser maior que 1 como esta sendo especificado neles

    [Required(ErrorMessage = "Este campo é obrigatório")]
    [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
    [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

    public string Title { get; set; }
    [MaxLength(1024, ErrorMessage = "Este campo deve conter no máximo 1024 caracteres")]

    public string Description { get; set; }

    [Required(ErrorMessage = "Este campo é obrigatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]

    public decimal Price { get; set; }

    [Required(ErrorMessage = "Este campo é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Categoria inválida")]

    public int CategoryId { get; set; }
    //Esse data é utilizado para saber o id de qual categoria o produto pertence

    public Category Category { get; set; }
    //Esse data  é utilizado para quando o usuario quer saber o titulo da referencia categoria, ele retorna o objeto de categoria    
  }
}