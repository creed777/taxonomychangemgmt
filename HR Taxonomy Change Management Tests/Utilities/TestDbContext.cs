using AutoFixture;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HR_Taxonomy_Change_Management_Tests.Utilities
{
    public class TestDbContext
    {
        public SqliteConnection? _connection { get; set; }
        public DbContextOptions<TaxonomyContext>? _contextOptions { get; set; }

        public TestDbContext() { }

        private List<ChangeDetail> Changes = new List<ChangeDetail>();

        /// <summary>
        /// Creates the Taxonomy db context and loads the data
        /// </summary>
        /// <returns>TaxonomyContext</returns>
        public TaxonomyContext CreateContext()
        {
            var context = CreateDatabase();
            context = LoadRequestData(context);
            context = LoadTaxonomyData(context);

            return context;
        }

        /// <summary>
        /// Creates the in-memory database
        /// </summary>
        /// <returns>TaxonomyContext</returns>
        public TaxonomyContext CreateDatabase()
        {
            _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
                .UseSqlite(_connection)
                .Options;

            var context = new TaxonomyContext(_contextOptions);

            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Gets request data 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>TaxonomyContext</returns>
        public TaxonomyContext LoadRequestData(TaxonomyContext context)
        {
            var memoryData = new ReqestDataInMemory();
            context.RequestType.AddRange(memoryData.AllRequestTypes());
            context.RequestStatusType.AddRange(memoryData.AllRequestStatusTypes());
            context.ChangeStatusType.AddRange(memoryData.AllChangeStatusTypes());
            context.ChangePeriod.AddRange(memoryData.AllChangePeriods());

            var requests = memoryData.AllRepositoryRequests();
            foreach (Request r in requests)
            {
                context.Entry(r).State = EntityState.Added;

                foreach (ChangeDetail c in r.ChangeDetail)
                {
                    context.Entry(c).State = EntityState.Added;
                }
            }

            context.Request.AddRange(requests);
            var changes = requests.Select(r => r.ChangeDetail).First();
            foreach(ChangeDetail change in changes)
            {
                context.ChangeDetail.AddRange(changes);
            }

            context.SaveChanges();
            return context;
        }

        public TaxonomyContext LoadTaxonomyData(TaxonomyContext context)
        {
            var memoryData = new ReqestDataInMemory();
            var owners = memoryData.AllTaxonomyOwners();
            var taxonomies = memoryData.AllTaxonomies();
            foreach(Taxonomy t in taxonomies)
            {
                context.Entry(t).State = EntityState.Added;
            }
            context.TaxonomyOwner.AddRange(owners);
            context.Taxonomy.AddRange(taxonomies);
            context.SaveChanges();
            return context;
        }
    }
}
