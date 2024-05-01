using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Constructor used by EF.
    /// </summary>

    public class Taxonomy
    {
        public Taxonomy() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int TaxonomyId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public virtual Taxonomy ParentTaxonomy { get; set; }
        public virtual List<Taxonomy> ChildTaxonomy { get; set; }
        public int? OwnerId { get; set; } = null;
        public virtual TaxonomyOwner? Owner { get; set; }
    }
}
