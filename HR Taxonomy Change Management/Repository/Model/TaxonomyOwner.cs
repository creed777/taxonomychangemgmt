using System.ComponentModel.DataAnnotations;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Constructor used by EF.
    /// </summary>
    public class TaxonomyOwner
    {
        public TaxonomyOwner() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Key]
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public virtual List<Taxonomy> Taxonomies { get; set; }
    }
}
