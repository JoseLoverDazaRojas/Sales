namespace Sales.Shared.DTOs
{

    /// <summary>
    /// The class PaginationDTO
    /// </summary>

    public class PaginationDTO
    {

        #region Attributes

        public int Id { get; set; }

        public int Page { get; set; } = 1;

        public int RecordsNumber { get; set; } = 10;

        public string? Filter { get; set; }

        #endregion Attributes

    }
}