using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Repository
{
    public interface IChangeRepository
    {
        Task<List<ChangeDetail>> GetRequestChangesAsync(int RequestId);
        Task<int> AddChangeStatusAsync(List<ChangeStatus> statuses);
        Task<List<ChangeStatusType>> GetAllStatusTypesAsync();
        Task<List<int>> GetAllChangeIds();
    }
}
