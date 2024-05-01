using HR_Taxonomy_Change_Management.Migrations;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using HR_Taxonomy_Change_Management_Tests.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HR_Taxonomy_Change_Management_Tests.Repository
{
    public class AdminRepositoryTests : IDisposable
    {
        public SqliteConnection? _connection { get; set; }
        public TaxonomyContext mockContext { get; set; }
        public ILogger<AdminRepository> AdminLogger { get; set; }

        public AdminRepositoryTests()
        {
            var x = new TestDbContext();
            mockContext = x.CreateContext();
            _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            AdminLogger = Mock.Of<ILogger<AdminRepository>>();
        }

        [Fact]
        public async Task GetAllChangePeriodsAsync_ReturnsChangePeriodList()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var result = await adminRepo.GetAllChangePeriodsAsync().ConfigureAwait(false);

            Assert.IsAssignableFrom<List<ChangePeriod>>(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetCurrentChangePeriodAsync_ReturnsSingleChangePeriod()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var result = await adminRepo.GetCurrentChangePeriodAsync().ConfigureAwait(false);

            Assert.IsAssignableFrom<ChangePeriod>(result);
            Assert.True(result != null);
            Assert.True(!result.IsDeleted);
        }

        [Fact]
        public async Task GetChangePeriodsAsync_ReturnsListOfChangePeriods()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var result = await adminRepo.GetChangePeriodsAsync().ConfigureAwait(false);

            Assert.IsAssignableFrom<List<ChangePeriod>>(result);
            Assert.True(result.Any());
            Assert.False(result.Where(x => x.IsDeleted == true).Any());
        }

        [Fact]
        public async Task GetChangePeriodByDateAsync_ReturnsSingleChangePeriod()
        {
            DateTime changeDate = DateTime.UtcNow;
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var changePeriodList = await adminRepo.GetAllChangePeriodsAsync().ConfigureAwait(false);
            var random = new Random();
            var index = random.Next(0, changePeriodList.Count());
            var changePeriod = changePeriodList[index];
            
            var result = await adminRepo.GetChangePeriodAsync(changePeriod.StartDate.AddDays(1)).ConfigureAwait(false);

            Assert.IsAssignableFrom<ChangePeriod>(result);
            Assert.True(result != null);
            Assert.True(!result.IsDeleted);
        }

        [Fact]
        public async Task GetChangePeriodByIdAsync_ReturnsSingleChangePeriod()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            List<int> changePeriodIdList = await adminRepo.GetChangePeriodIdsAsync();   
            Random random = new Random();
            int index = random.Next(0, changePeriodIdList.Count -1);
            var result = await adminRepo.GetChangePeriodAsync(changePeriodIdList[index]).ConfigureAwait(false);

            Assert.IsAssignableFrom<ChangePeriod>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetChangePeriodByIdAsync_ReturnsEmpty()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var changePeriodId = 0;
            var result = await adminRepo.GetChangePeriodAsync(changePeriodId).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddChangePeriodAsync_ReturnsInt()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            ReqestDataInMemory dataGen = new();
            var changePeriod = dataGen.GenerateChangePeriod(mockContext);
            changePeriod.ChangePeriodId = 0;
            changePeriod.Requests = new List<Request>();
            var result = await adminRepo.AddChangePeriodAsync(changePeriod);

            Assert.True(result != 0);
        }

        [Fact]
        public async Task UpdateChangePeriodAsync_ReturnsUpdatedChangePeriod()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var changePeriodList = await adminRepo.GetChangePeriodIdsAsync().ConfigureAwait(false);
            var index = new Random().Next(0, changePeriodList.Count()-1);
            var changePeriod = await adminRepo.GetChangePeriodAsync(changePeriodList[index]).ConfigureAwait(false);
            changePeriod.ModifyDate = DateTime.Now;
            changePeriod.ModifyUser = "tester";

            await adminRepo.UpdateChangePeriodAsync(changePeriod);
            var updatedChangePeriod = await adminRepo.GetChangePeriodAsync(changePeriod.ChangePeriodId).ConfigureAwait(false);
#pragma warning disable CS8629 // Nullable value type may be null.
            Assert.True(updatedChangePeriod.ModifyDate.Value.Date == DateTime.Now.Date );
            Assert.True(updatedChangePeriod.ModifyUser == "tester");
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        [Fact]
        public async Task CloseChangePeriodAsync_ReturnsBool()
        {
            AdminRepository adminRepo = new AdminRepository(mockContext, AdminLogger);
            var changePeriodList = await adminRepo.GetChangePeriodIdsAsync().ConfigureAwait(false);
            var index = new Random().Next(0, changePeriodList.Count() - 1);
            var changePeriod = await adminRepo.GetChangePeriodAsync(changePeriodList[index]).ConfigureAwait(false);

            await adminRepo.CloseChangePeriodAsync(changePeriod.ChangePeriodId).ConfigureAwait(false);
            var closedChangePeriod = await adminRepo.GetChangePeriodAsync(changePeriod.ChangePeriodId);

            Assert.True(closedChangePeriod.IsClosed == true);
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}

