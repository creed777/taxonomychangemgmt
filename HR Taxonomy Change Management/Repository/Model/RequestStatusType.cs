using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
    public class RequestStatusType
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Constructor used by EF.
        /// </summary>
        public RequestStatusType() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int RequestStatusTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusTypeName { get; set; } = string.Empty;
    }
}
