using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Repository
{
    public interface IAdminRepository
    {
        /// <summary>
        /// Gets all change periods regarless of date
        /// </summary>
        /// <returns><![CDATA[List<ChangePeriod>]]></returns>
        Task<List<ChangePeriod>> GetAllChangePeriodsAsync();

        /// <summary>
        /// Gets the current change period or the previous one if between periods.
        /// </summary>
        /// <returns><![CDATA[ChangePeriod]]></returns>
        Task<ChangePeriod> GetCurrentChangePeriodAsync();
        /// <summary>
        /// Get all current and future change periods that are not deleted
        /// </summary>
        /// <returns>List<ChangePeriod></ChangePeriod></returns>
        Task<List<ChangePeriod>> GetChangePeriodsAsync();
        /// <summary>
        /// Get a change period by Id
        /// </summary>
        /// <param name="changePeriodId"></param>
        /// <returns>ChangePeriod</returns>
        Task<ChangePeriod?> GetChangePeriodAsync(int changePeriodId);
        /// <summary>
        /// Get a change period based on the date.  
        /// </summary>
        /// <param name="changePeriod"></param>
        /// <returns>ChangePeriod</returns>
        Task<ChangePeriod?> GetChangePeriodAsync(DateTime changePeriod);
        Task<int> UpdateChangePeriodAsync(ChangePeriod changePeriod);
        Task<int> AddChangePeriodAsync(ChangePeriod changePeriod);
        /// <summary>
        /// Sets the IsClosed flag to true
        /// </summary>
        /// <param name="changePeriodId"></param>
        /// <returns></returns>
        Task<bool> CloseChangePeriodAsync(int changePeriodId);

        /// <summary>
        /// This is used by unit tests.
        /// </summary>
        /// <returns></returns>
        Task<List<int>> GetChangePeriodIdsAsync();
    }
}
