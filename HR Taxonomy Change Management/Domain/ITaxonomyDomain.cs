using HR_Taxonomy_Change_Management.Domain.Model;

namespace HR_Taxonomy_Change_Management.Domain
{
    public interface ITaxonomyDomain
    {
        Task<List<TaxonomyDTO>> GetAllTaxonomyAsync();
        Task<List<TaxonomyDTO>> GetAllTaxonomyOwnersAsync();
    }
}
