using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Domain
{
    public interface IChangeDomain
    {
        Task<List<ChangeDetailDTO>> GetRequestChangesAsync(int requestId);
        Task<List<RequestType>> GetAllRequestTypesAsync();
        Task<int> StatusChangeAsync(List<ChangeDetailDTO> changeList, ChangeStatusEnum changeStatus);
    }
}
