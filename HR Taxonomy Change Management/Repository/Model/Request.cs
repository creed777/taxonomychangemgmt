using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
    public class Request
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Constructor used by EF.
        /// </summary>
        public Request() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int RequestId { get; set; }
        /// <summary>changed from "navarchar(max)" to "text" for sqlite testing db </summary>
        [Required]
        [MinLength(50)]
        [Column(TypeName = "text")]
        public string Justification { get; set; } = string.Empty;
        [Column(TypeName = "text")]
        public string Change { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string SubmitUser { get; set; } = string.Empty;
        [Required]
        public DateTime SubmitDate { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string? ModifyUser { get; set; } = string.Empty;
        public DateTime? ModifyDate { get; set; }
        public virtual List<ChangeDetail> ChangeDetail { get; set; } = new List<ChangeDetail>();
        [ForeignKey(nameof(RequestStatus.RequestId))]
        public int RequestStatusId { get; set; }    
        public virtual List<RequestStatus> RequestStatuses { get; set; }
        public int RequestTypeId { get; set; }
        public virtual RequestType RequestType { get; set; }
        public int LegacyId { get; set; }
        public int? ChangePeriodId { get; set; }
        public virtual ChangePeriod ChangePeriod { get; set; }
    }
}
