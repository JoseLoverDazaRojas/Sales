namespace Sales.Shared.Entities
{

    #region Import

    using System.ComponentModel.DataAnnotations;

    #endregion Import

    /// <summary>
    /// The class City
    /// </summary>

    public class City
    {

        #region Attributes

        public int Id { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        public int StateId { get; set; }

        public State? State { get; set; }

        public ICollection<User>? Users { get; set; }

        #endregion Attributes

    }
}