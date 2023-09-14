namespace Sales.API.Interfaces
{

    #region Import

    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The interface IGenericRepository
    /// </summary>

    public interface IGenericRepository<T> where T : class
    {

        #region Methods

        Task<Response<T>> GetAsync(int id);

        Task<Response<IEnumerable<T>>> GetAsync();

        Task<Response<T>> AddAsync(T entity);

        Task<Response<T>> DeleteAsync(int id);

        Task<Response<T>> UpdateAsync(T entity);

        #endregion Methods

    }
}