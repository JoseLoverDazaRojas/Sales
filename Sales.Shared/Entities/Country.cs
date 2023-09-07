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
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States { get; set; }

        [Display(Name = "Estados/Departamentos")]
        public int StatesNumber => States == null ? 0 : States.Count;

        #endregion Attributes

    }
}