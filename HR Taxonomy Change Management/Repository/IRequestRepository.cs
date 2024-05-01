using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Repository
{
    public interface IRequestRepository
    {
        Task<List<Request>> GetAllRequestsAsync();
        Task<Request> GetRequestAsync(int requestId);
        Task<List<RequestType>> GetAllRequestTypesAsync();
        Task<List<Request>> GetAllRequestsAsync(int changePeriodId);
        Task<int> CreateRequestAsync(Request request);
        Task<bool> UpdateRequestAsync(Request request);
        Task<List<RequestStatusType>> GetAllStatusTypesAsync();
        Task<int> CloseRequestAsync(RequestStatus status);
        Task<List<Request>> GetRequestsAsync(string submitUser);
        Task<int> CountOfOpenRequestsbyPeriod(int changePeriodId);
        Task<Request> GetRequestByChangeAsync(int changeId);
        Task<List<Request>> GetRequestsAsyncByChangePeriod(int changePeriodId);
        Task<List<int>> GetAllRequestIds();
    }
}
