using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using HR_Taxonomy_Change_Management_Tests.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HR_Taxonomy_Change_Management_Tests.Repository
{
    public class TaxonomyRepositoryTests : IDisposable
    {
        public SqliteConnection? _connection { get; set; }
        public TaxonomyContext mockContext { get; set; }
        public ILogger<TaxonomyRepository> TaxonomyLogger { get; set; }

        public TaxonomyRepositoryTests()
        {
            var x = new TestDbContext();
            mockContext = x.CreateContext();
            _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            TaxonomyLogger = Mock.Of<ILogger<TaxonomyRepository>>();
        }

        [Fact]
        public async Task ApplyAddChangesAsync_ReturnsInt()
        {
            TaxonomyRepository taxRepo = new(mockContext, TaxonomyLogger);
            var taxonomyList = await taxRepo.GetAllTaxonomyAsync().ConfigureAwait(false);
            var index = (new Random().Next(0, taxonomyList.Count()-1));
            var parentId = taxonomyList[index].ParentId;
            var ownerId = taxonomyList[index].OwnerId;

            Taxonomy taxonomy = new()
            {
                Name = "Test",
                ParentId = parentId,
                OwnerId= ownerId
            };

            List<Taxonomy> taxonomies = new();
            taxonomies.Add(taxonomy);
            var result = await taxRepo.ApplyAddChangesAsync(taxonomies).ConfigureAwait(false);

            Assert.True(result > 0);

        }

        public void Dispose()
        {
            _connection.Close();
        }

        [Fact]
        public async Task GetAllTaxonomyAsync_ReturnsListOfTaxonomy()
        {
            TaxonomyRepository taxRepo = new(mockContext, TaxonomyLogger);
            var taxonomyList = await taxRepo.GetAllTaxonomyAsync();

            Assert.IsType<List<Taxonomy>>(taxonomyList);
            Assert.NotNull(taxonomyList);
        }
    }
}
