using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;
using HR_Taxonomy_Change_Management.Repository;
using Azure.Core;
using Telerik.DataSource;

namespace HR_Taxonomy_Change_Management.Domain
{
    public class ChangeDomain : IChangeDomain
    {
        private readonly IRequestRepository RequestRepository;
        private readonly IChangeRepository ChangeRepository;

        public ChangeDomain(IChangeRepository changeRepository, IRequestRepository requestRepository)
        {
            ChangeRepository = changeRepository;
            RequestRepository = requestRepository;
        }

        public async Task<List<ChangeDetailDTO>> GetRequestChangesAsync(int RequestId)
        {
            var result = await ChangeRepository.GetRequestChangesAsync(RequestId).ConfigureAwait(false);
            List<ChangeDetailDTO> changeList = new();

            foreach (ChangeDetail item in result)
            {
                ChangeDetailDTO change = new ()
                {
                    RequestId = RequestId,
                    ChangeDetailId = item.ChangeDetailId,
                    Change = item.ChangeText,
                    CurrentL1Id = item.CurrentL1Id,
                    CurrentL1 = item.CurrentL1,
                    CurrentL2Id = item.CurrentL2Id,
                    CurrentL2 = item.CurrentL2,
                    CurrentL3Id = item.CurrentL3Id,
                    CurrentL3 = item.CurrentL3,
                    CurrentL4Id = item.CurrentL4Id,
                    CurrentL4 = item.CurrentL4,
                    CurrentL5Id = item.CurrentL5Id,
                    CurrentL5 = item.CurrentL5,
                    NewL1Id = item.NewL1Id,
                    NewL1 = item.NewL1,
                    NewL2Id = item.NewL2Id,
                    NewL2 = item.NewL2,
                    NewL3Id = item.NewL3Id,
                    NewL3 = item.NewL3,
                    NewL4Id = item.NewL4Id,
                    NewL4 = item.NewL4,
                    NewL5Id = item.NewL5Id,
                    NewL5 = item.NewL5,
                    SubmitUser = item.SubmitUser,
                    SubmitDate = item.SubmitDate,
                    ModifyUser = item.ModifyUser,
                    ModifyDate = item.ModifyDate,
                    CurrentStatus = item.ChangeStatuses.MaxBy(x => x.StatusDate).StatusTypes.StatusTypeName,
                    ReviewText = item.ChangeStatuses.MaxBy(x => x.StatusTypes.ChangeStatusTypeId).ReviewText
                };

                string currentChangeString = string.Empty;
                string newChangeString = string.Empty;

                newChangeString = string.Concat(
                    change.NewL1,
                    !string.IsNullOrEmpty(change.NewL2) ? " > " : "",
                    change.NewL2,
                    !string.IsNullOrEmpty(change.NewL3) ? " > " : "",
                    change.NewL3,
                    !string.IsNullOrEmpty(change.NewL4) ? " > " : "",
                    change.NewL4,
                    !string.IsNullOrEmpty(change.NewL5) ? " > " : "",
                    change.NewL5);

                changeList.Add(change);   
            }

            return changeList;
        }

        public async Task<List<RequestType>> GetAllRequestTypesAsync()
        {
            return await RequestRepository.GetAllRequestTypesAsync().ConfigureAwait(false);
        }

        public async Task<int> StatusChangeAsync(List<ChangeDetailDTO>changeList, ChangeStatusEnum changeStatus)
        {
            var statusTypes = await ChangeRepository.GetAllStatusTypesAsync().ConfigureAwait(false);
            List<ChangeStatus> statuses = new();
            var requestId = changeList.Select(x => x.RequestId).First();

            foreach (ChangeDetailDTO change in changeList)
            {
                ChangeStatus status = new()
                {
                    ChangeDetailId = change.ChangeDetailId,
                    SubmitUser = change.SubmitUser,
                    StatusDate = change.SubmitDate,
                    ChangeStatusTypeId = (int)changeStatus
                };

                if (status.ChangeStatusTypeId == (int)ChangeStatusEnum.PendingReview)
                    status.ReviewText = change.ReviewText;

                statuses.Add(status);
            }

            var result = await ChangeRepository.AddChangeStatusAsync(statuses).ConfigureAwait(false);
            if(result >= 0)
            {
                bool closeRequest = false;
                var request = await RequestRepository.GetRequestAsync(requestId).ConfigureAwait(false);
                foreach (var change in request.ChangeDetail)
                {
                    if (change.ChangeStatuses.MaxBy(x => x.StatusDate).ChangeStatusTypeId != (int)ChangeStatusEnum.Denied && change.ChangeStatuses.MaxBy(x => x.StatusDate).ChangeStatusTypeId != (int)ChangeStatusEnum.Approved)
                        closeRequest = false;
                    else 
                        closeRequest = true;
                }

                RequestStatus status = new()
                {
                    RequestId = requestId,
                    StatusDate = DateTime.Now,
                    SubmitUser = ""
                };

                if (closeRequest == true)
                {
                    status.RequestStatusTypeId = (int)RequestStatusEnum.Closed;
                    await RequestRepository.CloseRequestAsync(status);
                }
                else
                {
                    status.RequestStatusTypeId = (int)RequestStatusEnum.Open;
                }
            }

            return result;
        }
    }
}
