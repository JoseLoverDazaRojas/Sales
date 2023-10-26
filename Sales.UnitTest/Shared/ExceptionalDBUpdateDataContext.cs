namespace Sales.UnitTest.Shared
{

    #region Import

    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;

    #endregion Import

    /// <summary>
    /// The class ExceptionalDBUpdateDataContext
    /// </summary>

    public class ExceptionalDBUpdateDataContext : DataContext
    {

        #region Constructor

        public ExceptionalDBUpdateDataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #endregion Constructor

        #region Methods

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new DbUpdateException("Test Exception");
        }

        #endregion Methods

    }
}