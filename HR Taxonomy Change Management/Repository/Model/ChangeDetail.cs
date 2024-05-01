using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
    public class ChangeDetail
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Constructor used by EF.
        /// </summary>
        public ChangeDetail() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int ChangeDetailId { get; set; }
        [Required]
        public int RequestId { get; set; }
        [Column (TypeName ="text")]
        public string? ChangeText { get; set; }
        public int? CurrentL1Id { get; set; }
        [Column (TypeName="nvarchar(150)")]
        public string? CurrentL1 { get; set; }
        public int? CurrentL2Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? CurrentL2 { get; set; }
        public int? CurrentL3Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? CurrentL3 { get; set;}
        public int? CurrentL4Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? CurrentL4 { get; set; }
        public int? CurrentL5Id { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? CurrentL5 { get; set; }
        public int? NewL1Id { get; set; }
        [Column(TypeName = "nvarchar(150)")] 
        public string? NewL1 { get; set; }
        public int? NewL2Id { get; set; }
        [Column(TypeName = "nvarchar(150)")] 
        public string? NewL2 { get; set; }
        public int? NewL3Id { get; set; }
        [Column(TypeName = "nvarchar(150)")] 
        public string? NewL3 { get; set; }
        public int? NewL4Id { get; set; }
        [Column(TypeName = "nvarchar(150)")] 
        public string? NewL4 { get; set;}
        public int? NewL5Id { get; set; }
        [Column(TypeName = "nvarchar(150)")] 
        public string? NewL5 { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string SubmitUser { get; set; } = string.Empty;
        [Required]
        public DateTime SubmitDate { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? ModifyUser { get; set; } = string.Empty;
        public DateTime? ModifyDate { get; set; }
        public virtual Request Request { get; set; }
        public int? LegacyId { get; set; }
        [ForeignKey(nameof(ChangeStatus.ChangeDetailId))]
        public virtual List<ChangeStatus> ChangeStatuses { get; set; } = new();
    }
}
