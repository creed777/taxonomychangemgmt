using HR_Taxonomy_Change_Management.Domain.Model;

namespace HR_Taxonomy_Change_Management.Service
{
    public interface IHelperService
    {
        Task<bool> InChangePeriodAsync();
        Task<ChangePeriodDTO> CurrentChangePeriodAsync();
        Task<ChangePeriodDTO> NextChangePeriodAsync();
    }
}
