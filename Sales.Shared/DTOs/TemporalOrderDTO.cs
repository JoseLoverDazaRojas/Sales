namespace Sales.Shared.DTOs
{

    #region Import
        
    #endregion Import

    /// <summary>
    /// The class TemporalOrderDTO
    /// </summary>

    public class TemporalOrderDTO
    {

        #region Attributes

        public int Id { get; set; }

        public int ProductId { get; set; }

        public float Quantity { get; set; } = 1;

        public string Remarks { get; set; } = string.Empty;

        #endregion Attributes

    }
}