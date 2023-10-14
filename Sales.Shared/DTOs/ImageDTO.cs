namespace Sales.Shared.DTOs
{

    #region Import

    using System.ComponentModel.DataAnnotations;

    #endregion Import

    /// <summary>
    /// The class ImageDTO
    /// </summary>

    public class ImageDTO
    {

        #region Attributes

        [Required]
        public int ProductId { get; set; }

        [Required]
        public List<string> Images { get; set; } = null!;

        #endregion Attributes

    }
}