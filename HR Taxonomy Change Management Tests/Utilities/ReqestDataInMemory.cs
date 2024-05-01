using AutoFixture;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HR_Taxonomy_Change_Management_Tests.Utilities
{
    public class ReqestDataInMemory
    {
        /// <summary>
        /// Gets the request data from the JSON file
        /// </summary>
        /// <returns>List<Request></returns>
        public List<Request> AllRepositoryRequests()
        {
            var requestJson = Properties.Resources.RequestJsonData;
            var mockRequestList = JsonConvert.DeserializeObject<List<Request>>(requestJson);

            return mockRequestList ?? new List<Request>();
        }

        /// <summary>
        /// Get the Change Status Type data from JSON file
        /// </summary>
        /// <returns>List<ChangeStatusType></returns>
        public List<ChangeStatusType> AllChangeStatusTypes()
        {
            var statusTypeJson = Properties.Resources.ChangeStatusTypeJson;
            var mockStatusTypeList = JsonConvert.DeserializeObject<List<ChangeStatusType>>(statusTypeJson);

            return mockStatusTypeList ?? new List<ChangeStatusType>().DistinctBy(x => x.ChangeStatusTypeId).ToList();
        }

        /// <summary>
        /// Get the Request Type data from JSON file
        /// </summary>
        /// <returns>List<RequestType></returns>
        public List<RequestType> AllRequestTypes()
        {
            var requestTypeJson = Properties.Resources.RequestTypeJsonData;
            var mockRequestTypeList = JsonConvert.DeserializeObject<List<RequestType>>(requestTypeJson);

            return mockRequestTypeList ?? new List<RequestType>();
        }

        /// <summary>
        /// Get the Change Period data from JSON file
        /// </summary>
        /// <returns>List<ChangePeriod></returns>
        public List<ChangePeriod> AllChangePeriods()
        {
            var changePeriodJson = Properties.Resources.ChangePeriodJson;
            var mockChangePeriodList = JsonConvert.DeserializeObject<List<ChangePeriod>>(changePeriodJson);

            return mockChangePeriodList?? new List<ChangePeriod>();
        }

        /// <summary>
        /// Get the Request Status Type data from JSON file
        /// </summary>
        /// <returns>List<RequestStatusType></returns>
        public List<RequestStatusType> AllRequestStatusTypes()
        {
            var requestStatusTypeJson = Properties.Resources.RequestStatusTypeJson;
            var mockStatusTypeList = JsonConvert.DeserializeObject<List<RequestStatusType>>(requestStatusTypeJson);

            return mockStatusTypeList ?? new List<RequestStatusType>();
        }

        /// <summary>
        /// Get the Taxonomy data from JSON file
        /// </summary>
        /// <returns>List<Taxonomy></returns>
        public List<Taxonomy> AllTaxonomies()
        {
            var taxonomyJson = Properties.Resources.TaxonomyJson;
            var mockTaxonomyList = JsonConvert.DeserializeObject<List<Taxonomy>>(taxonomyJson);

            return mockTaxonomyList ?? new List<Taxonomy>();
        }

        /// <summary>
        /// Get the Taxonomy data from JSON file
        /// </summary>
        /// <returns>List<Taxonomy></returns>
        public List<TaxonomyOwner> AllTaxonomyOwners()
        {
            var ownerJson = Properties.Resources.TaxonomyOwnerJson;
            var mockOwnerList = JsonConvert.DeserializeObject<List<TaxonomyOwner>>(ownerJson);

            return mockOwnerList ?? new List<TaxonomyOwner>();
        }

        /// <summary>
        /// Creates a new Request using random data generator.  All child objects are updated to the same RequestId
        /// </summary>
        /// <returns>Request</returns>
        public async Task<Request> GenerateRequestAsync(TaxonomyContext mockContext)
        {
            var fixture = new Fixture();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var request =  fixture.Create<Request>();
            var index = 0;

            var requestTypes = await mockContext.RequestType.ToListAsync();
            index = new Random().Next(0, requestTypes.Count() - 1);
            request.RequestId = 0;
            request.RequestType = requestTypes.ElementAt(index);
            request.RequestTypeId = requestTypes.ElementAt(index).RequestTypeId;
            request.RequestStatuses = new();
            request.RequestStatusId = 0;
            request.RequestStatuses.Add(new RequestStatus
            {
                StatusDate = DateTime.UtcNow,
                SubmitUser = "test.user@testme.com",
                StatusTypes = await mockContext.RequestStatusType.FirstAsync()
            });
            request.RequestStatuses.First().RequestStatusTypeId = request.RequestStatuses.First().StatusTypes.RequestStatusTypeId;

            var changePeriods = await mockContext.ChangePeriod.ToListAsync();
            index = new Random().Next(0, changePeriods.Count() - 1);
            request.ChangePeriod = changePeriods.ElementAt(index);
            request.ChangePeriodId = changePeriods.ElementAt(index).ChangePeriodId;

            var changeStatusTypes = await mockContext.ChangeStatusType.ToListAsync();
            index = new Random().Next(0, changeStatusTypes.Count() - 1);
            foreach(ChangeDetail change in request.ChangeDetail)
            {
                change.RequestId = 0;
                change.ChangeDetailId = 0;
                change.ChangeStatuses = new List<ChangeStatus>();
                change.ChangeStatuses.Add(new ChangeStatus
                {
                    ChangeDetailId = change.ChangeDetailId,
                    ChangeStatusId = null,
                    ChangeStatusTypeId = changeStatusTypes.ElementAt(index).ChangeStatusTypeId,
                    StatusTypes = changeStatusTypes.ElementAt(index),
                    StatusDate = DateTime.UtcNow,
                    SubmitUser = "test.user@testme.com"
                });
            }

            return request;
        }

        /// <summary>
        /// Creates a new Change Period using random data generator.
        /// </summary>
        /// <returns>Request</returns>
        public ChangePeriod GenerateChangePeriod(TaxonomyContext mockContext)
        {
            var fixture = new Fixture();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var request = fixture.Create<ChangePeriod>();
            return request;
        }

    }
}
