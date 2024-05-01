namespace HR_Taxonomy_Change_Management.Domain.Model
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Constructor used by EF.
    /// </summary>   
    public class TaxonomyDTO
    {
        public TaxonomyDTO() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int TaxonomyId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public virtual TaxonomyDTO? ParentTaxonomy { get; set; }
        public virtual List<TaxonomyDTO>? ChildTaxonomy { get; set; }
        public int? OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
