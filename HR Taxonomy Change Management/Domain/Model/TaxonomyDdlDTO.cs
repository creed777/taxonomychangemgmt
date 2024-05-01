namespace HR_Taxonomy_Change_Management.Domain.Model
{
    public class TaxonomyDdlDTO
    {
        public string TaxonomyId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
