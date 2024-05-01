using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Request = HR_Taxonomy_Change_Management.Repository.Model.Request;

namespace HR_Taxonomy_Change_Management.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly ILogger<RequestRepository>? Logger;
        private readonly TaxonomyContext Context;

        public RequestRepository(TaxonomyContext context, ILogger<RequestRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<List<Request>> GetAllRequestsAsync()
        {
            try
            {
                List<Request> result = new();
                result = await Context.Request
               .Include(request => request.RequestType)
               .Include(request => request.RequestStatuses)
                   .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .AsNoTracking()
               .ToListAsync()
               .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                Logger!.LogCritical(ex, "GetAllRequests failed");
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<List<Request>> GetAllRequestsAsync(int changePeriodId)
        {
            try
            {
                List<Request> result = new();
                result = await Context.Request
               .Include(request => request.RequestType)
               .Include(request => request.RequestStatuses)
                   .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Where(x => x.ChangePeriodId == changePeriodId)
                .AsNoTracking()
               .ToListAsync()
               .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex) 
            {
                Logger!.LogCritical(ex, "GetAllRequests failed for ChangePeriod {id}", changePeriodId);
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <summary>
        /// Gets requests by SubmitUser
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<List<Request>> GetRequestsAsync(string submitUser)
        {
            if (string.IsNullOrEmpty(submitUser))
                throw new Exception("user cannot be null");

            try
            {
                List<Request> result = new();
                result = await Context.Request
                .Where(x => x.SubmitUser == submitUser)
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.RequestType)
                .Include(request => request.RequestStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                if(Logger != null)
                    Logger.LogCritical(ex, "GetRequests failed for {submitUser}", submitUser);
 
                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<Request> GetRequestAsync(int requestId)
        {
            try
            {
                Request result = new();
                result = await Context.Request
                .Where(x => x.RequestId == requestId)
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.RequestType)
                .Include(request => request.RequestStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .AsNoTracking()
                .SingleAsync()
                .ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetRequestAsync failed for {requestId}", requestId);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<Request> GetRequestByChangeAsync(int changeId)
        {
            try
            {
                 Request result = new();
                result = await Context.Request
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.RequestType)
                .Include(request => request.RequestStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Where(x => x.ChangeDetail.Select(x => x.ChangeDetailId).Contains(changeId))
                .AsNoTracking()
                .FirstAsync()
                .ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetRequestAsync failed for {changeId}", changeId);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<List<RequestType>> GetAllRequestTypesAsync()
        {
            try
            {
                List<RequestType> result = new();
                result = await Context.RequestType
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetAllRequestTypesAsync failed");

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<List<RequestStatusType>> GetAllStatusTypesAsync()
        {
            try
            { 
                var result = await Context.RequestStatusType
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetAllStatusTypesAsync failed");

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<int> CreateRequestAsync(Request request)
        {
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.Entry(request).State = EntityState.Added;

                    var existingRequestType = Context.RequestType.Local
                        .FirstOrDefault(x => x.RequestTypeId == request.RequestTypeId);
                    if (existingRequestType != null)
                    {
                        request.RequestType = existingRequestType;
                    }
                    else
                        Context.Attach(request.RequestType);

                    foreach (var requestStatus in request.RequestStatuses)
                    {
                        var existingStatusType = Context.RequestStatusType.Local
                            .FirstOrDefault(x => x.RequestStatusTypeId == requestStatus.StatusTypes.RequestStatusTypeId);
                        if (existingStatusType != null)
                        {
                            request.RequestStatusId = existingStatusType.RequestStatusTypeId;
                            requestStatus.StatusTypes = existingStatusType;
                        }
                        else
                            Context.Attach(requestStatus.StatusTypes);

                        Context.AttachRange(request.RequestStatuses);
                    }

                    List<ChangeDetail> addedChanges = new();
                    addedChanges = request.ChangeDetail;
                    request.ChangeDetail = new List<ChangeDetail>();

                    Context.Add(request);
                    
                    foreach (var change in addedChanges)
                    {
                        foreach (var changeStatus in change.ChangeStatuses)
                        {
                            var existingStatusType = Context.ChangeStatusType.Local.FirstOrDefault(x => x.ChangeStatusTypeId == changeStatus.StatusTypes.ChangeStatusTypeId);
                            if (existingStatusType != null)
                                changeStatus.StatusTypes = existingStatusType;

                            Context.Attach(changeStatus);
                            Context.ChangeDetail.Add(change);
                        }
                    }

                    request.ChangeDetail.AddRange(addedChanges);
                    var result = await Context.SaveChangesAsync().ConfigureAwait(false);
                        Context.AddRange(addedChanges);
                    dbContextTransaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogCritical(ex, "CreateRequestAsync failed for {Request}", request);

                    throw new Exception(ex.Message, innerException: ex);
                }
            }
        }

        public async Task<bool> UpdateRequestAsync(Request editedRequest)
        {
            try
            {
                var tracked = Context.Request.Local.Where(x => x.RequestId == editedRequest.RequestId).SingleOrDefault();

                if (tracked == null)
                {
                    tracked = await Context.Request
                    .Where(x => x.RequestId == editedRequest.RequestId)
                    .Include(request => request.RequestType)
                    .Include(request => request.RequestStatuses)
                       .ThenInclude(statuses => statuses.StatusTypes)
                    .Include(request => request.ChangeDetail)
                    .SingleAsync();
                }

                tracked.Justification = editedRequest.Justification;
                tracked.Change = editedRequest.Change;
                tracked.ModifyDate = editedRequest.ModifyDate;
                tracked.ModifyUser = editedRequest.ModifyUser;

                for(int i = 0; i < tracked.ChangeDetail.Count; i++)
                {
                    var tc = tracked.ChangeDetail[i];
                    var ec = editedRequest.ChangeDetail.Where(cd => cd.ChangeDetailId == tc.ChangeDetailId).SingleOrDefault();

                    tc.ChangeText = ec.ChangeText;
                    tc.ModifyDate = ec.ModifyDate;
                    tc.ModifyUser = ec.ModifyUser;
                    tc.CurrentL1 = ec.CurrentL1;
                    tc.CurrentL2 = ec.CurrentL2;
                    tc.CurrentL3 = ec.CurrentL3;
                    tc.CurrentL4 = ec.CurrentL4;
                    tc.CurrentL5 = ec.CurrentL5;
                    tc.NewL1 = ec.NewL1;
                    tc.NewL2 = ec.NewL2;
                    tc.NewL3 = ec.NewL3;
                    tc.NewL4 = ec.NewL4;
                    tc.NewL5 = ec.NewL5;

                }

                var result = await Context.SaveChangesAsync().ConfigureAwait(false);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "UpdateRequestAsync failed for {editedRequest}", editedRequest);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

       
        public async Task<int> CloseRequestAsync(RequestStatus status)
        {
            if(status.RequestStatusTypeId != (int)RequestStatusEnum.Closed)
                throw new Exception("Status type is not Closed");

            try
            {
                var request = Context.Request
                    .Include(x => x.RequestStatuses)
                        .ThenInclude(x => x.StatusTypes)
                    .Where(x => x.RequestId == status.RequestId)
                    .Single();

                Context.Request.Attach(request);
                var existingRequestStatusType = Context.RequestStatusType.Local
                    .FirstOrDefault(x => x.RequestStatusTypeId == status.RequestStatusTypeId);
                if (existingRequestStatusType != null)
                {
                    status.StatusTypes = existingRequestStatusType;
                }

                request.RequestStatuses.Add(status);
                //Context.RequestStatusType.AttachRange(request.RequestStatuses.First().StatusTypes);
                request.RequestStatusId = (int)status.RequestStatusTypeId!;

                return await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "CloseRequestAsync failed for {status}", status);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<int> CountOfOpenRequestsbyPeriod(int changePeriodId)
        {
            try
            {
                var result = await Context.Request
                    .Include(x => x.RequestStatuses)
                    .Where(x => x.ChangePeriodId == changePeriodId && x.RequestStatusId != (int)RequestStatusEnum.Closed)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return result.Count;
            }
            catch(Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "CloseRequestACountOfOpenRequestsbyPeriodsync failed for period {period}", changePeriodId);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        public async Task<List<Request>> GetRequestsAsyncByChangePeriod(int changePeriodId)
        {
            try
            {
                var result = await Context.Request
                .Where(x => x.ChangePeriodId == changePeriodId)
                .Include(request => request.ChangeDetail)
                    .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .Include(request => request.RequestType)
                .Include(request => request.RequestStatuses)
                    .ThenInclude(statuses => statuses.StatusTypes)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

                return result;

            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetRequestsAsyncByChangePeriod failed for period {period}", changePeriodId);

                throw new Exception(ex.Message, innerException: ex);
            }
        }

        /// <summary>
        /// This was created for use by the test data extrack program
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetAllRequestIds()
        {
            try
            {
                var result = await Context.Request
                    .Select(x => x.RequestId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return result;
            }
            catch(Exception ex)
            {
                if (Logger != null)
                    Logger.LogCritical(ex, "GetAllRequestId failed");

                throw new Exception(ex.Message, innerException: ex);
            }
        }
    }
}
