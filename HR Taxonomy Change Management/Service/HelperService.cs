using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Service
{
    public class HelperService : IHelperService
    {
        async Task<bool> IHelperService.InChangePeriodAsync()
        {
            return (await _dbOperation.Value).InChangePeriod;
        }

        async Task<ChangePeriodDTO> IHelperService.CurrentChangePeriodAsync()
        {
            return (await _dbOperation.Value).CurrentChangePeriod;
        }

        async Task<ChangePeriodDTO> IHelperService.NextChangePeriodAsync()
        {
            return (await _dbOperation.Value).NextChangePeriod;
        }

        private readonly Lazy<Task<(ChangePeriodDTO CurrentChangePeriod, bool InChangePeriod, ChangePeriodDTO NextChangePeriod)>> _dbOperation;

        public HelperService(IAdminDomain adminDomain)
        {
            _dbOperation = new Lazy<Task<(ChangePeriodDTO CurrentChangePeriod, bool InChangePeriod, ChangePeriodDTO NextChangePeriod)>>(async () =>
            {
                var currentChangePeriod = await adminDomain.GetCurrentChangePeriodAsync();
                var inChangePeriod = currentChangePeriod.EndDate >= DateTime.UtcNow.Date ? true : false;
                var changePeriods = await adminDomain.GetAllChangePeriodsAsync();
                var nextChangePeriod = changePeriods.Where(c => c.StartDate > DateTime.UtcNow.Date).OrderBy(x => x.StartDate)
                    .DefaultIfEmpty(new ChangePeriodDTO())
                    .First();

                    return (currentChangePeriod, inChangePeriod, nextChangePeriod);

            });
        }
    }
}
