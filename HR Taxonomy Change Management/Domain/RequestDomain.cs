using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using Request = HR_Taxonomy_Change_Management.Repository.Model.Request;

namespace HR_Taxonomy_Change_Management.Domain
{
    public class RequestDomain : IRequestDomain
    {
        private readonly IRequestRepository Repository;
        private readonly IChangeRepository ChangeRepository;

        public RequestDomain(IRequestRepository requestRepository, IChangeRepository changeRepository) {
            Repository = requestRepository;
            ChangeRepository = changeRepository;
        }
            
        public async Task<List<RequestDTO>> GetAllRequestsAsync()
        {
            var result = await Repository.GetAllRequestsAsync().ConfigureAwait(false);
            var requestList = new List<RequestDTO>();
            requestList = MapRequestToRequestDTO(result).OrderByDescending(x => x.RequestId).ToList();

            return requestList;
        }

        public async Task<List<RequestDTO>> GetAllRequestsAsync(int changePeriodId)
        {
            var result = await Repository.GetAllRequestsAsync(changePeriodId).ConfigureAwait(false);
            var requestList = new List<RequestDTO>();
            requestList = MapRequestToRequestDTO(result).OrderByDescending(x => x.RequestId).ToList();

            return requestList;
        }

        /// <summary>
        /// Gets users by SubmitUser
        /// </summary>
        /// <param name="submitUser"></param>
        /// <returns></returns>
        public async Task<List<RequestDTO>> GetRequestsAsync(string submitUser)
        {
            var result = await Repository.GetRequestsAsync(submitUser).ConfigureAwait(false);
            var requestList = new List<RequestDTO>();
            requestList = MapRequestToRequestDTO(result).OrderByDescending(x => x.RequestId).ToList();

            return requestList;
        }

        public async Task<RequestDTO> GetRequestByChangeAsync(int changeId)
        {
            var result = await Repository.GetRequestByChangeAsync(changeId).ConfigureAwait(false);
            var resultList = new List<Request>();
            resultList.Add(result);

            var request = MapRequestToRequestDTO(resultList).FirstOrDefault() ?? new();
            return request;
        }

        public async Task<RequestDTO> GetRequestAsync(int requestId)
        {
            var result = await Repository.GetRequestAsync(requestId).ConfigureAwait(false);
            var resultList = new List<Request>();
            resultList.Add(result);

            var request = MapRequestToRequestDTO(resultList).FirstOrDefault() ?? new();
            return request;
        }

        public async Task<bool> UpdateRequestAsync(RequestDTO requestDTO)
        {
            string changeString = string.Empty;
            for (int i = 0; i < requestDTO.Changes.Count; i++)
            {
                if (i == 0)
                    changeString = "<li>" + requestDTO.Changes[i].Change + "</li>";
                else
                    changeString += "<li>" + requestDTO.Changes[i].Change + "</li>";
            }

            requestDTO.Change = "<ul>" + changeString + "</ul>";

            var request = await MapRequestDTOToRequest(requestDTO).ConfigureAwait(false);
            return await Repository.UpdateRequestAsync(request).ConfigureAwait(false);
        }

        public List<RequestDTO> MapRequestToRequestDTO(List<Request> requestList)
        {
            List<RequestDTO> requestDtoList = new();

            foreach (var request in requestList)
            {
                var status = request.RequestStatuses!.MaxBy(x => x.StatusDate);
                var item = new RequestDTO()
                {
                    RequestId = request.RequestId,
                    Justification = request.Justification,
                    SubmitDate = request.SubmitDate,
                    SubmitUser = request.SubmitUser,
                    RequestTypeName = request.RequestType.RequestTypeName,
                    CurrentStatus = status != null ? status.StatusTypes.StatusTypeName : string.Empty,
                    Change = request.Change,
                    ChangePeriodId = request.ChangePeriodId ?? 0
                };

                foreach (var changeItem in request.ChangeDetail)
                {
                    var chngStatus = changeItem.ChangeStatuses.MaxBy(x => x.StatusDate);
                    var change = new ChangeDetailDTO()
                    {
                        ChangeDetailId = changeItem.ChangeDetailId,
                        Change = changeItem.ChangeText ?? string.Empty,
                        SubmitDate = changeItem.SubmitDate,
                        SubmitUser = changeItem.SubmitUser,
                        CurrentL1Id = changeItem.CurrentL1Id,
                        CurrentL1 = changeItem.CurrentL1,
                        CurrentL2Id = changeItem.CurrentL2Id,
                        CurrentL2 = changeItem.CurrentL2,
                        CurrentL3Id = changeItem.CurrentL3Id,
                        CurrentL3 = changeItem.CurrentL3,
                        CurrentL4Id = changeItem.CurrentL4Id,
                        CurrentL4 = changeItem.CurrentL4,
                        CurrentL5Id = changeItem.CurrentL5Id,
                        CurrentL5 = changeItem.CurrentL5,
                        NewL1Id = changeItem.NewL1Id,
                        NewL1 = changeItem.NewL1,
                        NewL2Id = changeItem.NewL2Id,
                        NewL2 = changeItem.NewL2,
                        NewL3Id = changeItem.NewL3Id,
                        NewL3 = changeItem.NewL3,
                        NewL4Id = changeItem.NewL4Id,
                        NewL4 = changeItem.NewL4,
                        NewL5Id = changeItem.NewL5Id,
                        NewL5 = changeItem.NewL5,
                        ModifyDate = changeItem.ModifyDate,
                        ModifyUser = changeItem.ModifyUser,
                        ReviewText = chngStatus.ReviewText,
                        CurrentStatus = changeItem.ChangeStatuses.MaxBy(x => x.StatusDate).StatusTypes.StatusTypeName
                    };

                    if (item.Changes == null)
                        item.Changes = new();

                    item.Changes.Add(change);
                }
                requestDtoList.Add(item);
            }

            return requestDtoList;
        }

        public async Task<Request> MapRequestDTOToRequest(RequestDTO requestDTO)
        {
            var request = new Request();
            request.RequestId = requestDTO.RequestId;
            request.Justification = requestDTO.Justification;
            request.SubmitDate = requestDTO.SubmitDate;
            request.SubmitUser = requestDTO.SubmitUser;
            request.ModifyDate = requestDTO.ModifyDate;
            request.ModifyUser = requestDTO.ModifyUser;
            request.ChangePeriodId = requestDTO.ChangePeriodId;
            request.Change = requestDTO.Change;
  
            //TODO: don't depend on the RequestTypeId in the enum matching the db.
            if (request.RequestType == null)
            {
                request.RequestType = new RequestType
                {
                    RequestTypeId = (int)Enum.Parse(typeof(RequestTypeEnum), requestDTO.RequestTypeName),
                    RequestTypeName = requestDTO.RequestTypeName
                };
            }

            var requestStatusTypes = await Repository.GetAllStatusTypesAsync().ConfigureAwait(false);
            var statusTypeName = Enum.Parse(typeof(RequestStatusEnum), requestDTO.CurrentStatus).ToString();
            var statusTypeId = requestStatusTypes.Where(x => x.StatusTypeName == statusTypeName).Select(x => x.RequestStatusTypeId).Single();

            RequestStatus status = new()
            {
                RequestStatusTypeId = statusTypeId,
                StatusDate = DateTime.UtcNow,
                SubmitUser = request.SubmitUser,
                StatusTypes = new()
                {
                    StatusTypeName = statusTypeName ?? "",
                    RequestStatusTypeId = statusTypeId
                }
            };

            request.RequestStatuses = new();
            request.RequestStatuses.Add(status);
            request.ChangeDetail = new();

            foreach (ChangeDetailDTO changeItem in requestDTO.Changes)
            {
                ChangeDetail change = new()
                {
                    ChangeDetailId = changeItem.ChangeDetailId,
                    ChangeText = changeItem.Change,
                    SubmitDate = changeItem.SubmitDate,
                    SubmitUser = changeItem.SubmitUser,
                    CurrentL1Id = changeItem.CurrentL1Id,
                    CurrentL1 = changeItem.CurrentL1,
                    CurrentL2Id = changeItem.CurrentL2Id,
                    CurrentL2 = changeItem.CurrentL2,
                    CurrentL3Id = changeItem.CurrentL3Id,
                    CurrentL3 = changeItem.CurrentL3,
                    CurrentL4Id = changeItem.CurrentL4Id,
                    CurrentL4 = changeItem.CurrentL4,
                    CurrentL5Id = changeItem.CurrentL5Id,
                    CurrentL5 = changeItem.CurrentL5,
                    NewL1Id = changeItem.NewL1Id,
                    NewL1 = changeItem.NewL1,
                    NewL2Id = changeItem.NewL2Id,
                    NewL2 = changeItem.NewL2,
                    NewL3Id = changeItem.NewL3Id,
                    NewL3 = changeItem.NewL3,
                    NewL4Id = changeItem.NewL4Id,
                    NewL4 = changeItem.NewL4,
                    NewL5Id = changeItem.NewL5Id,
                    NewL5 = changeItem.NewL5,
                    ModifyDate = changeItem.ModifyDate,
                    ModifyUser = changeItem.ModifyUser,
                };

                var changeStatusTypes = await ChangeRepository.GetAllStatusTypesAsync().ConfigureAwait(false);
                statusTypeName = Enum.Parse(typeof(ChangeStatusEnum), changeItem.CurrentStatus).ToString();
                statusTypeId = changeStatusTypes.Where(x => x.StatusTypeName == statusTypeName).Select(x => x.ChangeStatusTypeId).Single();

                change.ChangeStatuses = new();
                change.ChangeStatuses.Add(new()
                {
                    StatusDate = requestDTO.SubmitDate,
                    ChangeStatusTypeId = statusTypeId,
                    StatusTypes = new ChangeStatusType()
                    {
                        ChangeStatusTypeId = statusTypeId,
                        StatusTypeName = statusTypeName ?? ""
                    }
                });

                request.ChangeDetail.Add( change );
            }
            return request;
        }

        public async Task<int> CreateRequestAsync(RequestDTO requestDTO)
        {
            string changeString = string.Empty;

            for(int i = 0; i < requestDTO.Changes.Count; i++)
            {
                if (i == 0)
                    changeString = "<li>" + requestDTO.Changes[i].Change + "</li>";
                else
                    changeString += "<li>" + requestDTO.Changes[i].Change + "</li>";
            }

            requestDTO.Change = "<ul>" + changeString + "</ul>";
            var request = await MapRequestDTOToRequest(requestDTO).ConfigureAwait(false);
            return await Repository.CreateRequestAsync(request).ConfigureAwait(false);
        }

        public async Task<List<RequestStatusType>> GetAllRequestStatusTypeAsync()
        {
            return await Repository.GetAllStatusTypesAsync();
        }
    }
}
