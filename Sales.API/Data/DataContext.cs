namespace Sales.API.Data
{
    #region Import

    using Microsoft.EntityFrameworkCore;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class DataContext
    /// </summary>

    public class DataContext : DbContext
    {

        #region Constructor

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #endregion Constructor

        #region Entities

        public DbSet<Country> Countries { get; set; }

        #endregion Entities

        #region Model

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
        }

        #endregion Model

    }
}
