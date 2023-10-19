namespace Sales.API.Interfaces
{
    using Sales.Shared.Entities;

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

        public Task<Country> GetCountryAsync(int id);

        public Task<State> GetStateAsync(int id);

        #endregion Methods

    }
}