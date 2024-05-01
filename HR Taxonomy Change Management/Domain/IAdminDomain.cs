using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace HR_Taxonomy_Change_Management.Domain
{
    public interface IAdminDomain
    {

        /// <summary>
        /// Gets the current change period, or the previous one if between periods
        /// </summary>
        /// <returns><![CDATA[ChangePeriodDTO]]></returns>
        Task<ChangePeriodDTO> GetCurrentChangePeriodAsync();

        /// <summary>
        /// Gets all change periods
        /// </summary>
        /// <returns></returns>
        Task<List<ChangePeriodDTO>> GetAllChangePeriodsAsync();

        /// <summary>
        /// Gets current/future change periods that are not deleted
        /// </summary>
        /// <returns></returns>
        Task<List<ChangePeriodDTO>> GetChangePeriodsAsync();

        Task<int> UpdateChangePeriodAsync(ChangePeriodDTO changePeriod);
        /// <summary>
        /// Gets the current change period, or the previous one if between periods.
        /// </summary>
        /// <param name="changePeriod"></param>
        /// <returns><![CDATA[ChangePeriodDTO]]></returns>
        Task<ChangePeriodDTO?> GetChangePeriodAsync(DateTime changePeriod);

        Task<ChangePeriodDTO?> GetChangePeriodAsync(int changePeriodId);
        Task<int> DeleteChangePeriodAsync(int changePeriodId, string UserEmail);
        Task<int> AddChangePeriodAsync(ChangePeriodDTO changePeriod);
        Task<int> CountOfOpenRequestsbyPeriod(int changePeriodId);
        /// <summary>
        /// Updates the taxononmy and marks ClosePeriod.IsClosed to true.
        /// </summary>
        /// <param name="changePeriodId"></param>
        /// <returns>Dictionary<string,string></returns>
        Task<Dictionary<string,string>> ClosePeriod(int changePeriodId);
        Task<Workbook> GetReportAsync(int changePeriodId);
    }
}
