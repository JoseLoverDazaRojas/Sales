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

        public Task<T> GetAsync(int id);

        public Task<IEnumerable<T>> GetAsync();

        public Task<Response<T>> AddAsync(T entity);

        public Task<Response<T>> UpdateAsync(T entity);

        public Task DeleteAsync(int id);
        
        #endregion Methods

    }
}