using Azure.Core;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace HR_Taxonomy_Change_Management.Domain.Model
{
    public class RequestDetailDTO
    {
        public int RequestId { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string SubmitUser { get; set; } = string.Empty;
        public DateTime SubmitDate { get; set; }
        public string? ModifyUser { get; set; } = string.Empty;
        public DateTime? ModifyDate { get; set; }
        [JsonIgnore]
        public string CurrentStatus 
        {
            get
            {
                if (Statuses.Any())
                {
                    return Statuses.MaxBy(x => x.StatusTypes.RequestStatusTypeId).StatusTypes.StatusTypeName;
                }
                return string.Empty;
            }
            set => CurrentStatus = value; 
        }
        public virtual List<ChangeDetailDTO> ChangeDetail { get; set; } = new List<ChangeDetailDTO>();
        public virtual List<RequestStatus> Statuses { get; set; } = new List<RequestStatus>();
        public int RequestTypeId { get; set; }
        public virtual RequestType RequestType { get; set; }

    }
}
