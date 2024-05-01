using HR_Taxonomy_Change_Management.Migrations;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HR_Taxonomy_Change_Management.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository>? Logger;
        private readonly TaxonomyContext? Context;

        public AdminRepository(TaxonomyContext context, ILogger<AdminRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<ChangePeriod>> GetAllChangePeriodsAsync()
        {
            return await Context.ChangePeriod
                .Where(x => x.IsDeleted != true)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ChangePeriod> GetCurrentChangePeriodAsync()
        {
            try
            {
                var result = await Context.ChangePeriod
                    .Where(x => x.StartDate <= DateTime.UtcNow && x.EndDate >= DateTime.UtcNow)
                    .AsNoTracking()
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (result == null)
                {
                    result = await Context.ChangePeriod
                        .AsNoTracking()
                        .Where(x => x.EndDate < DateTime.UtcNow && x.IsDeleted == false)
                        .OrderByDescending(x => x.StartDate)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }

                return result!;
            }
            catch(DbException ex)
            {
                Logger!.LogCritical(ex, "GetCurrentChangePeriod failed");
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <inheritdoc />
        public async Task<List<ChangePeriod>> GetChangePeriodsAsync()
        {
            try
            {

                return await Context.ChangePeriod
                    .AsNoTracking()
                    .Where(x => (x.StartDate >=  DateTime.UtcNow.Date || (x.StartDate <= DateTime.UtcNow.Date && x.EndDate >= DateTime.UtcNow.Date)) && x.IsDeleted == false)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "GetChangePeriodsAsync failed");
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <inheritdoc />
        public async Task<ChangePeriod?> GetChangePeriodAsync(DateTime changeDate)
        {
            try
            {
                    var changePeriod = await Context.ChangePeriod
                        .Where(x => x.StartDate <= changeDate && x.EndDate >= changeDate && x.IsDeleted == false)
                        .AsNoTracking()
                        .SingleOrDefaultAsync()
                        .ConfigureAwait(false);

                    if (changePeriod == null)
                    {
                        changePeriod = await Context.ChangePeriod
                            .Where(x => x.StartDate >= changeDate && x.IsDeleted == false)
                            .OrderBy(x => x.StartDate)
                            .AsNoTracking()
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);
                    }

                    return changePeriod;
                
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "GetChangePeriod failed for {changeDate}", changeDate);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <inheritdoc />
        public async Task<ChangePeriod?> GetChangePeriodAsync(int changePeriodId)
        {
            try
            {
                return await Context.ChangePeriod
                    .AsNoTracking()
                    .Where(x => x.ChangePeriodId == changePeriodId)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait (false);
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "GetChangePeriod failed for {changePeriodId}", changePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<int> UpdateChangePeriodAsync(ChangePeriod changePeriod)
        {
            try
            {
                var result = await Context.ChangePeriod
                    .Where(x => x.ChangePeriodId == changePeriod.ChangePeriodId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                
                //I tried just setting result=changePeriod and then saving but it wouldn't update the record.  No errors, just returned zero.
                result.StartDate = changePeriod.StartDate;
                result.EndDate = changePeriod.EndDate;
                result.ModifyDate = changePeriod.ModifyDate;
                result.ModifyUser = changePeriod.ModifyUser;
                result.IsDeleted = changePeriod.IsDeleted;
                return await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "UpdateChangePeriod failed for {changePeriodId}", changePeriod.ChangePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<int> AddChangePeriodAsync(ChangePeriod changePeriod)
        {
            try
            {
                await Context.AddAsync(changePeriod);
                return await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "AddChangePeriod failed for {changePeriodId}", changePeriod.ChangePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CloseChangePeriodAsync(int changePeriodId)
        {
            try
            {
                var period = await Context.ChangePeriod.Where(x => x.ChangePeriodId == changePeriodId).SingleAsync();
                period.IsClosed = true;
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "CloseChangePeriod failed for {changePeriodId}", changePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
            catch (Exception ex)
            {
                Logger!.LogCritical(ex, "CloseChangePeriod failed for {changePeriodId}", changePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        ///<inheritdoc/>
        public async Task<List<int>> GetChangePeriodIdsAsync()
        {
            return await Context.ChangePeriod
                .Where(x => x.IsDeleted == false)
                .Select(x => x.ChangePeriodId)
                .ToListAsync();
        }
    }
}
