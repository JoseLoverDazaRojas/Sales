namespace Sales.API.Interfaces
{

    #region Import

    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The interface IGenericUnitOfWork
    /// </summary>

    public interface IGenericUnitOfWork<T> where T : class
    {

        #region Methods

        public Task<IEnumerable<T>> GetAsync();

        public Task<T> GetAsync(int id);

        public Task<Response<T>> AddAsync(T model);

        public Task<Response<T>> UpdateAsync(T model);

        public Task DeleteAsync(int id);

        #endregion Methods

    }
}