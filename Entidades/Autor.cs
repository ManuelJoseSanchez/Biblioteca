using System.ComponentModel.DataAnnotations;
using BibliotecaAPI.Validaciones;

namespace BibliotecaAPI.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(10,ErrorMessage ="El campo {0} solo permite {1} carateres o menos")]
        //[PrimeraLetraMayucula]
        public required string Nombre { get; set; }

        public List<Libro> libros { get; set; } = new List<Libro>();


        /* validacion por modelo*/
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra =  Nombre[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula - por modelo",
                    new string[]{ nameof(Nombre) });
                }
            }
        }

        /**
        [Range(18, 120)] validamos un rango de numeros
        [CreditCard] validamos el formanto de una targerta
        [Url] validamos que se una url
        **/

    }
}