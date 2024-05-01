using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Taxonomy_Change_Management.Domain.Model
{
    [NotMapped]
    public class RequestDTO
    {
        public RequestDTO() { }
        public int RequestId { get; set; }
        [Required]
        [StringLength(1000,MinimumLength = 50, ErrorMessage ="The Jusitification is a 50 char minimum")]
        public string Justification { get; set; } = string.Empty;
        public string SubmitUser { get; set; } = string.Empty;
        public DateTime SubmitDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string RequestTypeName { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string Change { get; set; }  = string.Empty;
        public int ChangePeriodId { get; set; }
        public List<ChangeDetailDTO>? Changes { get; set; }
    }

}
