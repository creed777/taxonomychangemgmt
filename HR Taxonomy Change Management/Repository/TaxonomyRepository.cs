using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HR_Taxonomy_Change_Management.Repository
{
    public class TaxonomyRepository : ITaxonomyRepository
    {
        private readonly TaxonomyContext Context;
        private readonly ILogger<TaxonomyRepository> Logger;

        public TaxonomyRepository(TaxonomyContext context, ILogger<TaxonomyRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<List<Taxonomy>> GetAllTaxonomyAsync()
        {
            try
            {
                var all = await Context.Taxonomy
                    .Include(x => x.ParentTaxonomy)
                    .Include(x => x.ChildTaxonomy)
                    .Include(x => x.Owner)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return all;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> ApplyAddChangesAsync(List<Taxonomy> newTaxonomy)
        {
            try
            {
             
                foreach (var taxonomy in newTaxonomy)
                {
                    await Context.Taxonomy.AddAsync(taxonomy);
                }
                
                var result = await Context.SaveChangesAsync();
                return result;
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "ApplyAddChangesAsync failed for RequestId");
                throw new Exception(ex.Message, innerException: ex);
            }
            catch (Exception ex)
            {
                Logger!.LogCritical(ex, "ApplyAddChangesAsync failed for RequestId");
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<int> ApplyRenameChangesAsync(List<Taxonomy[,]> renameTaxonomy, int requestId)
        {
            try
            {
                var oldTaxonomy = renameTaxonomy.First()[0,0];
                var newTaxonomy = renameTaxonomy.First()[0,1];

                var count = 0;
                using (var transaction = Context.Database.BeginTransaction())
                {
                    foreach (var taxonomy in renameTaxonomy)
                    {
                        var result = await Context.Taxonomy
                            .Where(x => x.TaxonomyId == oldTaxonomy.TaxonomyId)
                            .SingleAsync()
                            .ConfigureAwait(false);

                        result.Name = newTaxonomy.Name;
                        await Context.SaveChangesAsync().ConfigureAwait(false);

                        count++;
                    }

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return count;
                }
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "ApplyRenameChangesAsync failed for request {1}", requestId);
                throw new Exception(ex.Message, innerException: ex);
            }
            catch (Exception ex)
            {
                Logger!.LogCritical(ex, "ApplyRenameChangesAsync failed for request {1}", requestId);
                throw new Exception(ex.Message, innerException: ex);
            }

        }

        public async Task<int> ApplyMoveChangesAsync(List<Taxonomy[,]> moveTaxonomy, int requestId)
        {
            try
            {
                var oldTaxonomy = moveTaxonomy.First()[0, 0];
                var newTaxonomy = moveTaxonomy.First()[0, 1];

                var count = 0;
                using (var transaction = Context.Database.BeginTransaction())
                {
                    foreach (var taxonomy in moveTaxonomy)
                    {
                        var result = await Context.Taxonomy
                            .Where(x => x.TaxonomyId == oldTaxonomy.TaxonomyId)
                            .SingleAsync()
                            .ConfigureAwait(false);

                        result.ParentId = newTaxonomy.TaxonomyId;
                        await Context.SaveChangesAsync().ConfigureAwait(false);

                        count++;
                    }

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return count;
                }
            }
            catch (DbException ex)
            {
                Logger!.LogCritical(ex, "ApplyMoveChangesAsync failed for request {1}", requestId);
                throw new Exception(ex.Message, innerException: ex);
            }
            catch (Exception ex)
            {
                Logger!.LogCritical(ex, "ApplyMoveChangesAsync failed for request {1}", requestId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public void BackupTaxonomyDbForClose()
        {
            string sql = "EXEC Taxonomy_Table_Backup";
            var result = Context.Database.ExecuteSqlRaw(sql);
        }
    }
}
