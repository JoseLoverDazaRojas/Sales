namespace Sales.API.Helpers
{

    #region Import

    using Sales.Shared.DTOs;

    #endregion Import

    /// <summary>
    /// The class QueryableExtensions
    /// </summary>

    public static class QueryableExtensions
    {

        #region Methods

        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)
                .Take(pagination.RecordsNumber);
        }

        #endregion Methods

    }
}