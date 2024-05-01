using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Repository.Model
{
    public class RequestStatus
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Constructor used by EF.
        /// </summary>
        public RequestStatus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int? RequestStatusId { get; set; }
        [ForeignKey(nameof(Request.RequestId))]
        public int? RequestId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(150)")] 
        public string SubmitUser { get; set; } = string.Empty;
        [Required]
        public DateTime StatusDate { get; set; }
        public virtual List<Request> Requests { get; set; }
        [ForeignKey(nameof(RequestStatusType.RequestStatusTypeId))]
        public int RequestStatusTypeId { get; set; }
        public virtual RequestStatusType StatusTypes { get; set; }
    }
}
