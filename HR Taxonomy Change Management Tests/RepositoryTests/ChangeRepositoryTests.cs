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
    public class ChangeRepositoryTests : IDisposable
    {
        public SqliteConnection? _connection { get; set; }
        public TaxonomyContext mockContext { get; set; }
        public ILogger<ChangeRepository> ChangeLogger { get; set; }
        public ILogger<RequestRepository> RequestLogger { get; set; }

        public ChangeRepositoryTests() 
        {
            var x = new TestDbContext();
            mockContext = x.CreateContext();
            _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            ChangeLogger = Mock.Of<ILogger<ChangeRepository>>();
            RequestLogger = Mock.Of<ILogger<RequestRepository>>();
        }

        public void Dispose()
        {
            _connection.Close();
        }

        [Fact]
        public async Task GetRequestChanges_ReturnsChangesForRequest()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            ChangeRepository changeRepo = new ChangeRepository(mockContext, ChangeLogger);
            var requestList = await requestRepo.GetAllRequestIds().ConfigureAwait(false);
            var index = new Random().Next(0, requestList.Count() - 1);
            var requestId = requestList.ElementAt(index);

            var changeList = await changeRepo.GetRequestChangesAsync(requestId).ConfigureAwait(false);
            Assert.IsAssignableFrom<List<ChangeDetail>>(changeList);
            Assert.True(changeList.Any());
        }

        [Fact]
        public async Task GetRequestChanges_ReturnsException()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            ChangeRepository changeRepo = new ChangeRepository(mockContext, ChangeLogger);
            var requestList = await requestRepo.GetAllRequestIds().ConfigureAwait(false);
            var requestId = 0;
            Assert.ThrowsAsync<Exception>(() => changeRepo.GetRequestChangesAsync(requestId));           
        }

        [Fact]
        public async Task GetChangeStatusType_ReturnsAll()
        {
            ChangeRepository changeRepo = new ChangeRepository(mockContext, ChangeLogger);
            var statusTypeList = await changeRepo.GetAllStatusTypesAsync().ConfigureAwait(false);
            Assert.IsAssignableFrom<List<ChangeStatusType>>(statusTypeList);
            Assert.True(statusTypeList.Any());
        }

        [Fact]
        public async Task AddChangeStatus_ReturnsSuccess()
        {
            ChangeRepository changeRepo = new ChangeRepository(mockContext, ChangeLogger);
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            var requestIdList = await requestRepo.GetAllRequestIds().ConfigureAwait(false);
            var index = (new Random()).Next(0, requestIdList.Count()-1);
            var requestId = requestIdList[index];

            var change = (await changeRepo.GetRequestChangesAsync(requestId).ConfigureAwait(false)).First();

            var status = new ChangeStatus
            {
                ChangeDetailId = change.ChangeDetailId,
                StatusDate = DateTime.UtcNow,
                SubmitUser = "user",
                ChangeStatusTypeId = (int)ChangeStatusEnum.Add,
                ReviewText = "text",
                Changes = new()
            };

        }

    }
}
