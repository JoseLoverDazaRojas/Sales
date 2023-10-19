namespace Sales.UnitTest.Shared
{

    #region Import

    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;

    #endregion Import

    /// <summary>
    /// The class ExceptionalDataContext
    /// </summary>

    public class ExceptionalDataContext : DataContext
    {

        #region Constructor

        public ExceptionalDataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        #endregion Constructor

        #region Methods

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Test Exception");
        }

        #endregion Methods

    }
}