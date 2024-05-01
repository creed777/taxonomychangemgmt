using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
    public class ChangeStatus
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Constructor used by EF.
        /// </summary>
        public ChangeStatus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int? ChangeStatusId { get; set; }
        [ForeignKey(nameof(ChangeDetail.ChangeDetailId))]
        public int? ChangeDetailId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(150)")] 
        public string SubmitUser { get; set; } = string.Empty;
        [Required]
        public DateTime StatusDate { get; set; }
        [ForeignKey(nameof(ChangeStatusType.ChangeStatusTypeId))]
        public int ChangeStatusTypeId { get; set; }
        public virtual ChangeStatusType StatusTypes { get; set; }
        public virtual List<ChangeDetail> Changes { get; set; }
        public string? ReviewText { get; set; }
    }
}
