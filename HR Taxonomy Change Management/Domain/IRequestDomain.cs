using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Domain
{
    public interface IRequestDomain
    {
        Task<List<RequestDTO>> GetAllRequestsAsync();
        Task<List<RequestDTO>> GetAllRequestsAsync(int changePeriodId);
        Task<RequestDTO> GetRequestAsync(int requestId);
        List<RequestDTO> MapRequestToRequestDTO(List<Request> requestList);
        Task<int> CreateRequestAsync(RequestDTO requestDTO);
        Task<Request> MapRequestDTOToRequest(RequestDTO requestList);
        Task<bool> UpdateRequestAsync(RequestDTO requestDTO);
        Task<List<RequestDTO>> GetRequestsAsync(string submitUser);
        Task<List<RequestStatusType>> GetAllRequestStatusTypeAsync();
        Task<RequestDTO> GetRequestByChangeAsync(int changeId);
    }
}
