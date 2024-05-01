using HR_Taxonomy_Change_Management.Repository.Model;

namespace HR_Taxonomy_Change_Management.Repository
{
    public interface ITaxonomyRepository
    {
        Task<List<Taxonomy>> GetAllTaxonomyAsync();
        Task<int> ApplyAddChangesAsync(List<Taxonomy> taxonomy);
        Task<int> ApplyRenameChangesAsync(List<Taxonomy[,]> renameTaxonomy, int requestId);
        Task<int> ApplyMoveChangesAsync(List<Taxonomy[,]> renameTaxonomy, int requestId);
        void BackupTaxonomyDbForClose();
    }
}
