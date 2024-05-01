using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Repository.Model;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using Telerik.Blazor.Components;

namespace HR_Taxonomy_Change_Management.Pages
{

    partial class Index : BasePage
    {
        [Inject] private ProtectedSessionStorage? Storage { get; set; }
        [Inject] private IAuthorizationService? AuthService {get; set;}
        [Inject] private IRequestDomain? RequestDomain { get; set; }
        [Inject] NavigationManager? NavManager { get; set; }
        [Inject] private IAdminDomain AdminDomain { get; set; }
        [Inject] private IHelperService? HelperService { get; set; }

        public List<RequestDTO> RequestList { get; set; } = new();
        public List<RequestStatusType> RequestStatusList { get; set; } = new();
        List<string> CurrentStatuses = new ();
        private ChangePeriodDTO ChangePeriod { get; set; } = new();
        public bool FilterForPeriod { get; set; } = false;
        public string UserEmail { get; set; } = string.Empty;

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            List<RequestDTO> result = new();
            LoaderVisible = true;
            UserEmail = AuthService.UserEmail;
            ChangePeriod = await HelperService.CurrentChangePeriodAsync();
            FilterForPeriod = (await Storage.GetAsync<bool>("ViewAllCheckbox")).Value;

            if (AuthService.IsApprover == true)
                result = await RequestDomain.GetAllRequestsAsync(ChangePeriod.ChangePeriodId).ConfigureAwait(false);
            else
                result = await RequestDomain.GetRequestsAsync(UserEmail ?? "").ConfigureAwait(false);

            if (FilterForPeriod)
            {
                object args = new();
                args = false;
                await GetAllRequets(args);
            }
            else 
                RequestList = result.OrderByDescending(x => x.RequestId).ToList();

            RequestStatusList = await RequestDomain.GetAllRequestStatusTypeAsync();
            CurrentStatuses = RequestStatusList.Select(x => x.StatusTypeName).ToList();
            LoaderVisible = false;
        }

        private async Task InitGridState(GridStateEventArgs<RequestDTO> args)
        {
             var gridState = (await Storage.GetAsync<GridState<RequestDTO>>("gridState"));
            if(gridState.Success)
            {
                args.GridState = gridState.Value;
            }
        }


        public async Task GetAllRequets(object args)
        {
            List<RequestDTO> result = new();
            if (args.ToString() == "False")
            {
                if (AuthService.IsApprover)
                {
                    result = new List<RequestDTO>(await RequestDomain.GetAllRequestsAsync(ChangePeriod.ChangePeriodId));
                }
                else
                {
                    result = new List<RequestDTO>(await RequestDomain.GetRequestsAsync(UserEmail ?? ""));
                }
            }
            else
            {
                if (AuthService.IsApprover)
                {
                    result = new List<RequestDTO>(await RequestDomain.GetAllRequestsAsync());
                }
                else
                {
                    result = new List<RequestDTO>(await RequestDomain.GetRequestsAsync(UserEmail ?? ""));
                }
            }

            RequestList = result.OrderByDescending(x => x.RequestId).ToList();
        }

        public async Task OpenRequestDetailAsync(GridRowClickEventArgs args)
        {
            if (args.Item is RequestDTO request)
            {
                await Storage.SetAsync("ViewAllCheckbox", FilterForPeriod);
                NavManager.NavigateTo($"requestdetail/{request.RequestId}");
            }
        }

        public async Task GridStateChanged(GridStateEventArgs<RequestDTO> args)
        {
            await Storage.SetAsync("gridState", args.GridState).ConfigureAwait(false);
        }
    }
}
