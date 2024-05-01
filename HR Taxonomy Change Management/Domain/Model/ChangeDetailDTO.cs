
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Domain.Model
{
    [NotMapped]
    public class ChangeDetailDTO
    {
        public int ChangeDetailId { get; set; }
        public int RequestId { get; set; }
        public int? CurrentL1Id { get; set; }
        public string? CurrentL1 { get; set; }
        public int? CurrentL2Id { get; set; }
        public string? CurrentL2 { get; set; }
        public int? CurrentL3Id { get; set; }
        public string? CurrentL3 { get; set; }
        public int? CurrentL4Id { get; set; }
        public string? CurrentL4 { get; set; }
        public int? CurrentL5Id { get; set; }
        public string? CurrentL5 { get; set; }
        public int? NewL1Id { get; set; }
        public string? NewL1 { get; set; }
        public int? NewL2Id { get; set; }
        public string? NewL2 { get; set; }
        public int? NewL3Id { get; set; }
        public string? NewL3 { get; set; }
        public int? NewL4Id { get; set; }
        public string? NewL4 { get; set; }
        public int? NewL5Id { get; set; }
        public string? NewL5 { get; set; }
        public string Change { get; set; } = string.Empty;
        public string SubmitUser { get; set; } = string.Empty;
        public DateTime SubmitDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public string? ReviewText { get; set; }
    }
}
