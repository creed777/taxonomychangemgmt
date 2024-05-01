using System.ComponentModel.DataAnnotations;
using System.Net;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ChangePeriod
    {
        public ChangePeriod() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }
        public virtual List<Request> Requests { get; set; }
    }
}
