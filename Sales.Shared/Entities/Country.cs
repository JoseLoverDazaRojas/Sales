namespace Sales.Shared.Entities
{
    #region Import

    using System.ComponentModel.DataAnnotations;

    #endregion Import

    /// <summary>
    /// The class Country
    /// </summary>

    public class Country
    {

        #region Attributes

        public int Id { get; set; }

        [Display(Name = "País")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        #endregion Attributes

    }
}