using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace HR_Taxonomy_Change_Management.Component
{
    partial class RequestAddComponent
    {
        [Inject] IHelperService? HelperService { get; set; }
        [Inject] NavigationManager? NavManager { get; set; }
        
        [Parameter]
        public List<TaxonomyDdlDTO> TaxonomyList { get; set; } = new ();
        [Parameter]
        public RequestDTO Request { get; set; } = new();
        [Parameter]
        public bool IsEdit { get; set; } = false;
        [Parameter]
        public EventCallback<RequestDTO> SaveEditEvent { get; set; }
        [Parameter]
        public EventCallback<RequestDTO> CreateRequestEvent { get; set; }

        public List<TaxonomyDdlDTO> NewLevels { get; set; } = new();
        public List<TaxonomyDdlDTO> CurrentLevels { get; set; } = new();
        public TaxonomyDDLComponent CurrentTaxDDL { get; set; } = new();
        public string? ChangeDetailId { get; set; }
        public ChangeDetailDTO? ChangeDetails { get; set; }
        private TelerikGrid<ChangeDetailDTO> ChangeGrid { get; set; }  = new ();
        [CascadingParameter]
        public DialogFactory? Dialogs { get; set; }
        private bool EnableSaveButton { get { return ChangeGrid.Data.Any(); } }

        protected override void OnInitialized()
        {
            if (Request.Changes == null || Request.Changes.Count == 0)
            {
                Request.Changes = new();
                ChangeDetails = new();
            }
            else
                ChangeDetails = Request.Changes.First();

            if (Request.RequestId != 0)
            {
                buttonText = "Save";
                IsEdit = true;
            }
            else
            {
                Request.Justification = string.Empty;

                //clear the grid
                Request.Changes = new();
                ChangeGrid.Rebind();

                buttonText = "Add";
            }

            for (int i = 1; i <= 5; i++)
            {
                var level = ChangeDetails.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(ChangeDetails, null);
                if (level != null)
                {
                    var tax = TaxonomyList.Where(x => x.Name == level.ToString()).FirstOrDefault();
                    CurrentLevels.Add(tax ?? new());
                }

                level = string.Empty;
                level = ChangeDetails.GetType().GetProperty(string.Concat("NewL", i)).GetValue(ChangeDetails, null);

                if (level != null)
                {
                    var tax = TaxonomyList.Where(x => x.Name == level.ToString()).FirstOrDefault();

                    if (tax == null)
                        tax = new()
                        {
                            TaxonomyId = 0.ToString(),
                            Name = level.ToString()??string.Empty
                        };

                    NewLevels.Add(tax ?? new());
                }
            }
        }

        protected override void OnParametersSet()
        {
            Request.RequestTypeName = "Add";
            Request.CurrentStatus = "Open";

            Request.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Add a change to the grid for saving.  If this is an edit of an existing request, it will bypass the grid and update the main request with the change.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task AddToGrid(EditContext editContext)
        {

            if (!NewLevels.Select(x => x.TaxonomyId).Contains(null))
            {
                await Dialogs.AlertAsync("This is adding a new level.  At least one level must be a new entry", "Alert");
                return;
            }

            string? newText = string.Empty;
            ChangeDetails = new();

            var i = 1;
            foreach (TaxonomyDdlDTO currentLevel in CurrentLevels)
            {
                ChangeDetails.GetType().GetProperty(string.Concat("CurrentL", i)).SetValue(ChangeDetails, currentLevel.Name, null);
                int.TryParse(currentLevel.TaxonomyId, out int Id);
                ChangeDetails.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).SetValue(ChangeDetails, Id, null);

                i++;
            }

            //building out the ChangeDetails.ChangeText fields
            i = 1;
            foreach (TaxonomyDdlDTO newLevel in NewLevels)
            {
                ChangeDetails.GetType().GetProperty(string.Concat("NewL", i)).SetValue(ChangeDetails, newLevel.Name, null);
                int.TryParse(newLevel.TaxonomyId, out int Id);
                ChangeDetails.GetType().GetProperty(string.Concat("NewL", i, "Id")).SetValue(ChangeDetails, Id, null);

                if(newLevel.TaxonomyId == null)
                    newText += string.Concat(i==1? "" : " > ", "<span style=\"color:#e50000; \">  ", newLevel.Name ?? newLevel.TaxonomyId);
                else 
                    newText += string.Concat(i == 1 ? "" : " > ", newLevel.Name ?? newLevel.TaxonomyId);

                i++;
            }

            ChangeDetails.Change = string.Concat(newText, " is added </span>");


            if (Request.Changes == null)
                return;

            if (!IsEdit)
            {
                
                //To get the grid to refresh, you have to create a new bound object every time.  It only auto-refreshes with lists of simple types.  The .Rebind() method doesn't work the way I expected.
                //https://docs.telerik.com/blazor-ui/knowledge-base/grid-force-refresh?_gl=1*ndetl8*_ga*MjAxMDg2NzQwNi4xNjg4NTcyMDI5*_ga_9JSNBCSF54*MTY4OTc3NDE4NC4zNC4xLjE2ODk3NzQyMjAuMjQuMC4w&_ga=2.241678850.1218361352.1689629557-2010867406.1688572029
                
                ChangeDetails.CurrentStatus = "Add";
                ChangeDetails.SubmitUser = Request.SubmitUser;
                var temp = Request.Changes;
                temp.Add(ChangeDetails!);
                Request.Changes = new();
                Request.Changes.AddRange(temp);
                Request.ChangePeriodId = (await HelperService.CurrentChangePeriodAsync()).ChangePeriodId;
                Request.SubmitDate = DateTime.UtcNow;
                ClearForm();
                OnParametersSet();
            }
            else
            {
                ChangeDetails.ModifyDate = DateTime.Now;
                ChangeDetails.ModifyUser = Request.ModifyUser;
                ChangeDetails.ChangeDetailId = Request.Changes.First().ChangeDetailId;
                ChangeDetails.CurrentStatus = Request.Changes.First().CurrentStatus;
                Request.Changes.Clear();
                Request.Changes.Add(ChangeDetails);
                await SaveEditEvent.InvokeAsync(Request);
            }
        }

        /// <summary>
        /// Clear the request data and ddls
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public void ClearForm()
        {
            if (IsEdit)
            {
                NavManager.NavigateTo($"requestdetail/{Request.RequestId}", replace: false);
            }
            else
            {
                CurrentLevels = new();
                NewLevels = new();
                CurrentTaxDDL.ClearForm();
            }
        }

        /// <summary>
        /// Clears the grid of all changes.
        /// </summary>
        public async Task ClearGrid()
        {
            var isConfirmed = await ShowConfirm().ConfigureAwait(false);

            if (isConfirmed)
                NavManager.NavigateTo("/");

                Request.Changes = new();
                ChangeGrid.Rebind();
            }

        /// <summary>
        /// confirm the ClearGrid() method
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task<bool> ShowConfirm()
        {
            bool isConfirmed = await Dialogs.ConfirmAsync("This action will delete ALL changes.  Are you sure?", "Confirm");
            return isConfirmed;
        }

        /// <summary>
        /// Deletes the selected row from the grid
        /// </summary>
        /// <param name="args"></param>
        private void DeleteRow(GridCommandEventArgs args)
        {
            var row = (ChangeDetailDTO)args.Item;
            Request.Changes.Remove(row);
        }

        /// <summary>
        /// Create operation for new change request
        /// </summary>
        /// <returns><![CDATA[int]]> Id of new request</returns>
        private async Task CreateRequestAsync()
        {
            LoaderVisible = true;
            await CreateRequestEvent.InvokeAsync(Request);
            LoaderVisible = false;
        }

        protected void NewLevelSelectedEvent(List<TaxonomyDdlDTO> newLevels)
        {
            if(newLevels.Last().TaxonomyId != null)
                CurrentLevels = new(newLevels);

            NewLevels = newLevels;
        }

    }
}
