namespace Sales.API.UnitsOfWork
{

    #region Import

    ﻿using Sales.API.Interfaces;
    using Sales.Shared.Entities;
    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The class GenericUnitOfWork
    /// </summary>

    public class GenericUnitOfWork<T> : IGenericUnitOfWork<T> where T : class
    {

        #region Attributes

        private readonly IGenericRepository<T> _repository;

        #endregion Attributes

        #region Constructor

        public GenericUnitOfWork(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        #endregion Constructor

        #region Methods

        public async Task<IEnumerable<T>> GetAsync() => await _repository.GetAsync();

        public async Task<T> GetAsync(int id) => await _repository.GetAsync(id);

        public async Task<Response<T>> AddAsync(T model) => await _repository.AddAsync(model);

        public async Task<Response<T>> UpdateAsync(T model) => await _repository.UpdateAsync(model);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<Country> GetCountryAsync(int id) => await _repository.GetCountryAsync(id);

        public async Task<State> GetStateAsync(int id) => await _repository.GetStateAsync(id);

        #endregion Methods

    }
}