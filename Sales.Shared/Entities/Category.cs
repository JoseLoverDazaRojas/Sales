namespace Sales.Shared.Entities
{

    #region Import

    using System.ComponentModel.DataAnnotations;

    #endregion Import

    /// <summary>
    /// The class Category
    /// </summary>

    public class Category
    {

        #region Attributes

        public int Id { get; set; }

        [Display(Name = "Categoria")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        #endregion Attributes

    }
}