namespace Sales.Shared.Entities
{

    #region Import

    #endregion Import

    /// <summary>
    /// The class ProductCategory
    /// </summary>

    public class ProductCategory
    {

        #region Attributes

        public int Id { get; set; }

        public Product Product { get; set; } = null!;

        public int ProductId { get; set; }

        public Category Category { get; set; } = null!;

        public int CategoryId { get; set; }

        #endregion Attributes

    }
}