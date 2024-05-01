using System.ComponentModel.DataAnnotations;

namespace HR_Taxonomy_Change_Management.Domain.Model
{
    public class ChangePeriodDTO
    {
        public int ChangePeriodId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string CreateUser { get; set; } = string.Empty;
        [Required]
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
    }
}
