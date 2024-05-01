using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Identity.ConditionalAccess.AuthenticationContextClassReferences.Item;
using System.Data.Common;

namespace HR_Taxonomy_Change_Management.Repository
{
    public class ChangeRepository : IChangeRepository
    {
        private readonly TaxonomyContext? Context;
        private readonly ILogger<ChangeRepository> Logger;

        public ChangeRepository(TaxonomyContext context, ILogger<ChangeRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<List<ChangeDetail>> GetRequestChangesAsync(int RequestId)
        {
            if (RequestId == 0) throw new ArgumentNullException(nameof(ChangeDetail));

            var result = await Context.ChangeDetail
            .Where(x => x.RequestId == RequestId)
            .Include(change => change.ChangeStatuses)
                .ThenInclude(statuses => statuses.StatusTypes)
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

            if (result == null) return new List<ChangeDetail>();
            else return result;
        }

        public async Task<int> AddChangeStatusAsync(List<ChangeStatus> statuses)
        {
            if (statuses == null) throw new ArgumentNullException();

            var updateCount = 0;

            using (var transction = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var status in statuses)
                    {
                        var change = await Context.ChangeDetail
                        .Where(x => x.ChangeDetailId == status.ChangeDetailId)
                        .Include(change => change.ChangeStatuses)
                            .ThenInclude(statuses => statuses.StatusTypes)
                        .SingleAsync()
                        .ConfigureAwait(false);

                        Context.Entry(change).State = EntityState.Unchanged;
                        change.ChangeStatuses.Add(status);

                        foreach (ChangeStatus changeStatus in change.ChangeStatuses)
                        {
                            if (changeStatus.ChangeStatusId == 0 || changeStatus.ChangeStatusId == null)
                                Context.Entry(changeStatus).State = EntityState.Added;
                            else
                                Context.Entry(changeStatus).State = EntityState.Unchanged;
                        }

                        var result = await Context.SaveChangesAsync().ConfigureAwait(false);
                        updateCount++;
                    }

                    await transction.CommitAsync().ConfigureAwait(false);
                    return updateCount;
                }
                catch (DbException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    transction.Rollback();
                    return -1;
                }
            }
        }

        public async Task<List<ChangeStatusType>> GetAllStatusTypesAsync()
        {
            List<ChangeStatusType> result = new();
            result = await Context.ChangeStatusType
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

            if (result == null)
                return new List<ChangeStatusType>();

            else
                return result;

        }

        /// <summary>
        /// This was created for use by the unit tests
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetAllChangeIds()
        {
            var result = await Context.ChangeDetail
                .Select(x => x.ChangeDetailId)
                .ToListAsync() 
                .ConfigureAwait(false);

            return result;
        }

    }
}
