namespace Sales.API.Repositories
{

    #region Import

    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Intertfaces;
    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The class GenericRepository
    /// </summary>

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        #region Attributes

        private readonly DataContext _context;
        private readonly DbSet<T> _entity;

        #endregion Attributes

        #region Constructor

        public GenericRepository(DataContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }

        #endregion Constructor

        #region Methods

        public async Task<Response<T>> AddAsync(T entity)
        {
            _context.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                return DbUpdateExceptionResponse(dbUpdateException);
            }
            catch (Exception exception)
            {
                return ExceptionResponse(exception);
            }
        }

        public async Task<Response<T>> DeleteAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row != null)
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    WasSuccess = true,
                };
            }
            return new Response<T>
            {
                WasSuccess = false,
                Message = "Registro no encontrado"
            };
        }

        public async Task<Response<T>> GetAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row != null)
            {
                return new Response<T>
                {
                    WasSuccess = true,
                    Result = row
                };
            }
            return new Response<T>
            {
                WasSuccess = false,
                Message = "Registro no encontrado"
            };
        }

        public async Task<Response<IEnumerable<T>>> GetAsync()
        {
            return new Response<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await _entity.ToListAsync()
            };
        }

        public async Task<Response<T>> UpdateAsync(T entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                return DbUpdateExceptionResponse(dbUpdateException);
            }
            catch (Exception exception)
            {
                return ExceptionResponse(exception);
            }
        }

        private Response<T> ExceptionResponse(Exception exception)
        {
            return new Response<T>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }

        private Response<T> DbUpdateExceptionResponse(DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
            {
                return new Response<T>
                {
                    WasSuccess = false,
                    Message = "Ya existe el registro que estas intentando crear."
                };
            }
            else
            {
                return new Response<T>
                {
                    WasSuccess = false,
                    Message = dbUpdateException.InnerException.Message
                };
            }
        }

        #endregion Methods

    }
}