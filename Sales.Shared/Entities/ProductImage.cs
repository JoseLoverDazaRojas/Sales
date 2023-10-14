namespace Sales.Shared.Entities
{

    #region Import

    using System.ComponentModel.DataAnnotations;

    #endregion Import

    /// <summary>
    /// The class ProductImage
    /// </summary>

    public class ProductImage
    {

        #region Attributes

        public int Id { get; set; }

        public Product Product { get; set; } = null!;

        public int ProductId { get; set; }

        [Display(Name = "Imagen")]
        public string Image { get; set; } = null!;

        #endregion Attributes

    }
}