namespace Sales.Shared.DTOs
{

    #region Import

    using Sales.Shared.Enums;

    #endregion Import

    /// <summary>
    /// The class OrderDTO
    /// </summary>

    public class OrderDTO
    {

        #region Attributes

        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;

        #endregion Attributes

    }
}