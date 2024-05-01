using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using HR_Taxonomy_Change_Management_Tests.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using Request = HR_Taxonomy_Change_Management.Repository.Model.Request;

namespace HR_Taxonomy_Change_Management_Tests.Repository
{
    public class RequestRepositoryTests : IDisposable
    {
        public SqliteConnection? _connection { get; set; }
        public TaxonomyContext mockContext { get; set; }
        public ILogger<RequestRepository> RequestLogger { get; set; }
        public ILogger<ChangeRepository> ChangeLogger { get; set; }
        public ILogger<AdminRepository> AdminLogger { get; set; }

        public RequestRepositoryTests()
        {
            var x = new TestDbContext();
            mockContext = x.CreateContext();
            _connection = new SqliteConnection("Datasource=:memory:");
            _connection.Open();

            RequestLogger = Mock.Of<ILogger<RequestRepository>>();
            ChangeLogger = Mock.Of<ILogger<ChangeRepository>>();
            AdminLogger = Mock.Of<ILogger<AdminRepository>>();
        }

        public void Dispose()
        {
            _connection.Close();
        }

        [Fact]
        public async Task GetAllRequests_ReturnsAll()
        {
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var requestList = await repo.GetAllRequestsAsync().ConfigureAwait(false);
            Assert.IsAssignableFrom<IEnumerable<Request>>(requestList);
            Assert.True(requestList.Count() > 0);
        }

        [Fact]
        public async Task GetAllRequests_ReturnsAllFromPeriod()
        {
            var changePeriodId = 1;
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var requestList = await repo.GetAllRequestsAsync(changePeriodId).ConfigureAwait(false);
            Assert.IsAssignableFrom<IEnumerable<Request>>(requestList);
            Assert.True(requestList.Count() > 0);
        }

        [Fact]
        public async Task GetRequestById_ReturnsOne()
        {
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var requestIdList = await repo.GetAllRequestIds().ConfigureAwait(false);
            var index = new Random().Next(0, requestIdList.Count() - 1);
            var requestId = requestIdList.ElementAt(index);

            var request = await repo.GetRequestAsync(requestId).ConfigureAwait(false);
            Assert.IsAssignableFrom<Request>(request);
            Assert.Equal(requestId, request.RequestId);
        }

        [Fact]
        public async Task GetRequestById_ReturnsException()
        {
            var requestId = 0;
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            await Assert.ThrowsAsync<Exception>(() => repo.GetRequestAsync(requestId));
        }

        [Fact]
        public async Task GetRequestByUserId_ReturnsAll()
        {
            var user = "v-reedchris@microsoft.com";
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var requests = await repo.GetRequestsAsync(user);
            Assert.IsAssignableFrom<List<Request>>(requests);
            Assert.Contains(requests, request => request.SubmitUser == user);
        }

        [Fact]
        public async Task GetRequestByUserId_ReturnsException()
        {
            var user = string.Empty;
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            await Assert.ThrowsAsync<Exception>(() => repo.GetRequestsAsync(user));
        }

        [Fact]
        public async Task GetRequestByChangeAsync_ReturnsOne()
        {
            ChangeRepository changeRepo = new ChangeRepository(mockContext, ChangeLogger);
            var changeIdList = await changeRepo.GetAllChangeIds().ConfigureAwait(false);
            var index = new Random().Next(0, changeIdList.Count() - 1);
            var changeId = changeIdList.ElementAt(index);
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var request = await repo.GetRequestByChangeAsync(changeId);
            Assert.IsAssignableFrom<Request>(request);
            Assert.Contains(changeId, request.ChangeDetail.Select(x => x.ChangeDetailId));
        }

        [Fact]
        public async Task GetRequestByChangeAsync_ReturnsException()
        {
            var changeId = 0;
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            await Assert.ThrowsAsync<Exception>(() => repo.GetRequestByChangeAsync(changeId));
        }

        [Fact]
        public async Task GetAllRequestTypes_ReturnsAll()
        {
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var result = await repo.GetAllRequestTypesAsync();
            Assert.IsAssignableFrom<List<RequestType>>(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetAllStatusTypes_ReturnsAll()
        {
            RequestRepository repo = new RequestRepository(mockContext, RequestLogger);
            var result = await repo.GetAllStatusTypesAsync();
            Assert.IsAssignableFrom<List<RequestStatusType>>(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task CreateNewRequst_ReturnsNewRequest()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            var dataGen = new ReqestDataInMemory();
            var request = await dataGen.GenerateRequestAsync(mockContext).ConfigureAwait(false);

            var result = await requestRepo.CreateRequestAsync(request).ConfigureAwait(false);
            var savedRequest = await requestRepo.GetRequestAsync(request.RequestId);
            Assert.IsType<Request>(savedRequest);
            Assert.True(request.RequestId == savedRequest.RequestId);
        }

        [Fact]
        public async Task UpdateRequst_ReturnsBool()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            var requestIdList = await requestRepo.GetAllRequestIds();
            var requestId = requestIdList[new Random().Next(0, requestIdList.Count() - 1)];
            var request = await requestRepo.GetRequestAsync(requestId).ConfigureAwait(false);
            request.Change = "Updated Change Detail";
            request.ModifyUser = "test@user.com";
            request.ModifyDate = DateTime.UtcNow;

            var result = await requestRepo.UpdateRequestAsync(request).ConfigureAwait(false);
            var savedRequest = await requestRepo.GetRequestAsync(request.RequestId);
            Assert.True(result);
            Assert.IsType<Request>(savedRequest);
            Assert.True(request.ModifyDate == savedRequest.ModifyDate);
            Assert.True(request.ModifyUser == savedRequest.ModifyUser);
            Assert.True(request.Change == savedRequest.Change);
        }

        [Fact]
        public async Task CloseRequst_ReturnsBool()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            var requestIdList = await requestRepo.GetAllRequestIds();
            var requestId = requestIdList[new Random().Next(0, requestIdList.Count() - 1)];
            var request = await requestRepo.GetRequestAsync(requestId).ConfigureAwait(false);

            var requestStatus = new RequestStatus()
            {
                RequestId = requestId,
                RequestStatusTypeId = (int)RequestStatusEnum.Closed,
                SubmitUser = "user@test.com",
                StatusDate = DateTime.UtcNow,
            };

            var result = await requestRepo.CloseRequestAsync(requestStatus).ConfigureAwait(false);
            var closedRequest = await requestRepo.GetRequestAsync(request.RequestId);
            Assert.True(result > 0);
            Assert.IsType<Request>(closedRequest);
            Assert.True(requestStatus.StatusDate == closedRequest.RequestStatuses.OrderByDescending(x => x.StatusDate).Select(x => x.StatusDate).First());
            Assert.True(requestStatus.SubmitUser == closedRequest.RequestStatuses.OrderByDescending(x => x.StatusDate).Select(x => x.SubmitUser).First());
        }

        [Fact]
        public async Task CloseRequst_ReturnsException()
        {
            RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
            var requestIdList = await requestRepo.GetAllRequestIds();
            var requestId = requestIdList[new Random().Next(0, requestIdList.Count() - 1)];
            var request = await requestRepo.GetRequestAsync(requestId).ConfigureAwait(false);

            var requestStatus = new RequestStatus()
            {
                RequestId = requestId,
                RequestStatusTypeId = (int)RequestStatusEnum.Open,
                SubmitUser = "user@test.com",
                StatusDate = DateTime.UtcNow,
            };

            await Assert.ThrowsAsync<Exception>(() => requestRepo.CloseRequestAsync(requestStatus));
        }

        //[Fact]
        //public async Task CountofOpenRequestsByPeriod_ReturnsCount()
        //{
        //    RequestRepository requestRepo = new RequestRepository(mockContext, RequestLogger);
        //    AdminRepository adminRepository = new AdminRepository(mockContext, AdminLogger);
        //    var periods = await adminRepository.GetAllChangePeriodsAsync().ConfigureAwait(false);
        //    var periodId = periods[new Random().Next(0, periods.Count() - 1)].ChangePeriodId;

        //    var result = await requestRepo.CountOfOpenRequestsbyPeriod(periodId).ConfigureAwait(false);

        //    var requests = await requestRepo.GetRequestsAsyncByChangePeriod(periodId).ConfigureAwait(false);
        //    var openCount = requests.Where(x => x.RequestStatuses.OrderByDescending(x => x.StatusDate).First().RequestStatusTypeId == (int)RequestStatusEnum.Open).Count();

        //    Assert.IsType<List<Request>>(requests);
        //    Assert.Equal(requests.Count(), openCount);

        //}

    }
}