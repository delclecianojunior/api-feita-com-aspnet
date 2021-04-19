using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{

  public class User
  {

    [Key]

    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo é obrigatorio")]
    [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
    [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

    public string UserName { get; set; }

    [Required(ErrorMessage = "Este campo é obrigatorio")]
    [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres")]
    [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres")]

    public string PassWord { get; set; }

    public string Role { get; set; }
  }
}