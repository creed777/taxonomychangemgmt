using HR_Taxonomy_Change_Management.Component;
using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;

namespace HR_Taxonomy_Change_Management.Pages
{
    partial class Administration
    {
        [Inject] private IAdminDomain? AdminDomain { get; set; }
        [Inject] private ITaxonomyDomain? TaxonomyDomain { get; set; }
        [Inject] private IHelperService? HelperService { get; set; }
        [Inject] private IAuthorizationService? AuthService { get; set; }

        public List<ChangePeriodDTO>? AllChangePeriods { get; set; }
        public List<ChangePeriodDTO>? ChangePeriods { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public NotificationComponent Notification { get; set; } = new();
        [CascadingParameter]
        public DialogFactory? Dialogs { get; set; }
        private ChangePeriodDTO? CurrentChangePeriod { get; set; }
        private bool EnableCloseButton { get; set; }
        private string DialogText { get; set; } = string.Empty;
        private bool ShowCloseDialog { get; set; } = false;
        private string AddText { get; set; } = string.Empty;
        private string RenameText { get; set; } = string.Empty;
        private string MoveText { get; set; } = string.Empty;
        private TelerikGrid<ChangePeriodDTO>? AdminGrid { get; set; }
        private bool LoaderVisible { get; set; }
        private bool includePast { get; set; } = false;
        private int selectedChangeId { get; set; }

        protected async override Task OnInitializedAsync()
        {
            UserEmail = AuthService.UserEmail;
            CurrentChangePeriod = await HelperService.CurrentChangePeriodAsync();           
            ChangePeriods = await AdminDomain.GetChangePeriodsAsync();
            AllChangePeriods = (ChangePeriods).Where(x => x.StartDate <= DateTime.UtcNow.Date).OrderByDescending(x => x.EndDate).ToList();
            EnableCloseButton = await AdminDomain.CountOfOpenRequestsbyPeriod(CurrentChangePeriod.ChangePeriodId).ConfigureAwait(false) == 0 && !CurrentChangePeriod.IsClosed;
            var result = await TaxonomyDomain.GetAllTaxonomyAsync();
        }

        public async Task UpdateHandler(GridCommandEventArgs args)
        {
            ChangePeriodDTO item = (ChangePeriodDTO)args.Item;
            item.ModifyDate = DateTime.UtcNow;
            item.ModifyUser = UserEmail;
            var result = await AdminDomain.UpdateChangePeriodAsync(item);
            if (result > 0)
            {
                Notification.SaveSuccess();
                await GetAllChangePeriods();
            }
            else
                Notification.SaveFailure();
        }

        public async Task DeleteHandler(GridCommandEventArgs args)
        {
            ChangePeriodDTO item = (ChangePeriodDTO)args.Item;
            var result = await AdminDomain.DeleteChangePeriodAsync(item.ChangePeriodId, UserEmail);
            if (result > 0)
            {
                Notification.SaveSuccess();
                ChangePeriods = await AdminDomain.GetChangePeriodsAsync();
            }
            else
                Notification.SaveFailure();
        }

        public async Task GetAllChangePeriods()
        {
            if (includePast == true)
            {
                ChangePeriods = await AdminDomain.GetAllChangePeriodsAsync();
                ChangePeriods.OrderByDescending(x => x.EndDate).ToList();
            }
            else
                ChangePeriods = await AdminDomain.GetChangePeriodsAsync();

            AdminGrid.Rebind();
        }

        public void AddHandler(GridCommandEventArgs args)
        {
            ChangePeriodDTO item = (ChangePeriodDTO)args.Item;
            item.CreateDate = DateTime.UtcNow;
            item.CreateUser = UserEmail;
            StateHasChanged();
        }

        public async Task CreateHandler(GridCommandEventArgs args)
        {
            ChangePeriodDTO item = (ChangePeriodDTO)args.Item;
            var result = await AdminDomain.AddChangePeriodAsync(item).ConfigureAwait(false);
            if (result > 0)
            {
                Notification.SaveSuccess();
                ChangePeriods = await AdminDomain.GetChangePeriodsAsync();
            }
            else
                Notification.SaveFailure();
        }

        public void InitGridState(GridStateEventArgs<ChangePeriodDTO> args)
        {
            GridState<ChangePeriodDTO> state = new GridState<ChangePeriodDTO>()
            {
                SortDescriptors = new List<SortDescriptor>()
                {
                     new SortDescriptor { Member = "EndDate", SortDirection = ListSortDirection.Descending }
                }

            };

            args.GridState = state;
        }

        public async Task ClosePeriod()
        {
            var result = await AdminDomain.ClosePeriod(CurrentChangePeriod.ChangePeriodId).ConfigureAwait(false);
            result.TryGetValue("isClosed", out string? closed);

            if (closed == "True")
            {
                CurrentChangePeriod.IsClosed = true;
                var changePeriod = await AdminDomain.GetChangePeriodAsync(CurrentChangePeriod.ChangePeriodId);
                var changePeriodToJson = JsonConvert.SerializeObject(changePeriod, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            };

            //modal to display results of close
            result.TryGetValue("add", out string? addResult);
            result.TryGetValue("rename", out string? renameResult);
            result.TryGetValue("move", out string? moveResult);
            result.TryGetValue("isClosed", out string? periodClosed);

            AddText += "Add: " + addResult;
            RenameText += "Rename: " + renameResult;
            MoveText += "Move: " + moveResult;
            DialogText += "Change Period is Closed: " + periodClosed;
            ShowCloseDialog = true;

            var updateChnagePeriod = ChangePeriods!.Where(x => x.ChangePeriodId == CurrentChangePeriod.ChangePeriodId).SingleOrDefault();
            updateChnagePeriod.IsClosed = true;
        }

        private async Task GetSpreadsheet(int changePeriodId)
        {
            LoaderVisible = true;

            IBinaryWorkbookFormatProvider formatProvider = new Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx.XlsxFormatProvider();

            var docService = await AdminDomain.GetReportAsync(changePeriodId).ConfigureAwait(false);
            byte[] bytesFromWorkbook = formatProvider.Export(docService);

            var reportName = "TaxonomyChanges" + DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            await JSRuntime.InvokeAsync<object>("saveFile", reportName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", bytesFromWorkbook);
            LoaderVisible = false;
        }
    }
}

