using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Domain;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using Telerik.Blazor;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using HR_Taxonomy_Change_Management.Service;

namespace HR_Taxonomy_Change_Management.Component
{
    partial class RequestRenameComponent
    {
        [Inject] IHelperService HelperService { get; set; }
        [Inject] NavigationManager? NavManager { get; set; }

        [Parameter] public List<TaxonomyDdlDTO> TaxonomyList { get; set; } = new();
        [Parameter] public RequestDTO Request { get; set; } = new();
        [Parameter] public bool IsEdit { get; set; }
        [Parameter] public EventCallback<RequestDTO> SaveEditEvent { get; set; }
        [Parameter] public EventCallback<RequestDTO> CreateRequestEvent { get; set; }

        [CascadingParameter] public DialogFactory? Dialogs { get; set; }
        private string buttonText = string.Empty;
        public TaxonomyDDLComponent CurrentTaxDDL { get; set; } = new();
        public TaxonomyDDLComponent NewTaxDDL { get; set; } = new();
        public ChangeDetailDTO? ChangeDetail { get; set; }
        public List<TaxonomyDdlDTO> NewLevels { get; set; } = new();
        public List<TaxonomyDdlDTO> CurrentLevels { get; set; } = new();
        bool IsAddEnabled { get { return (NewLevels.Count > 0); } }
        TelerikGrid<RequestDTO> ChangeGrid { get; set; } = new();
        public bool IsSaveEnabled { get; set; }

        protected override void OnInitialized()
        {
            //if (Request.Changes == null)
            //    Request.Changes = new();

            ChangeDetail = Request.Changes != null && Request.Changes.Count > 0 ? Request.Changes.SingleOrDefault() : new(); 

            if (string.IsNullOrEmpty(ChangeDetail.CurrentStatus)) 
                Request.CurrentStatus = "New";

            if (IsEdit)
                buttonText = "Save";
            else
            {
                Request.Justification = string.Empty;

                //clear the grid
                Request.Changes = new();
                ChangeGrid.Rebind();
                buttonText = "Add";
            }

            //populate the Current and New Taxonomy lists to send to the DDL component
            for (int i = 1; i <= 5; i++)
            {
                var level = ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(ChangeDetail, null);
                if (level != null)
                {
                    var tax = TaxonomyList.Where(x => x.Name == level.ToString()).FirstOrDefault();
                    CurrentLevels.Add(tax ?? new());
                }

                level = string.Empty;
                level = ChangeDetail.GetType().GetProperty(string.Concat("NewL", i)).GetValue(ChangeDetail, null);

                if (level != null)
                {
                    var tax = TaxonomyList.Where(x => x.Name == level.ToString()).FirstOrDefault();

                    if (tax == null)
                        tax = new()
                        {
                            TaxonomyId = 0.ToString(),
                            Name = level.ToString() ?? string.Empty
                        };

                    NewLevels.Add(tax ?? new());
                }
            }
        }

        protected override void OnParametersSet()
        {
            Request.RequestTypeName = "Rename";
            Request.CurrentStatus = "Open";
            Request.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Add a change to the grid for saving.  If this is an edit of an existing request, it will update the main request with the changes.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task AddToGrid(EditContext editContext)
        {
            if (!ValidateDDL())
            {
                await Dialogs.AlertAsync("This is a Rename process.  The last \"New\" entry must be a rename", "Alert");
                return;
            }

            string? currentText = string.Empty;
            string? newText = string.Empty;
            
            //The rest of this section is taking the DDL objects and creating a ChangeDetail object attached to a request
            ChangeDetail = new();

            //building out the ChangeDetail.ChangeText fields
            var i = 1;
            string? levelText;
            foreach (TaxonomyDdlDTO currentLevel in CurrentLevels)
            {
                levelText = TaxonomyList.Where(x => x.TaxonomyId == currentLevel.TaxonomyId).Select(x => x.Name).FirstOrDefault();
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i)).SetValue(ChangeDetail, levelText, null);
                currentText += i == 1 ? levelText ?? currentLevel.TaxonomyId : string.Concat(" > ", levelText ?? currentLevel.TaxonomyId);
                int.TryParse(currentLevel.TaxonomyId, out int value);
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).SetValue(ChangeDetail, value, null);
                i++;    
            }

            i = 1;
            levelText = string.Empty;
            foreach (TaxonomyDdlDTO newLevel in NewLevels)
            {
                if (!string.IsNullOrEmpty(newLevel.TaxonomyId))
                    levelText = TaxonomyList.Where(x => x.TaxonomyId == newLevel.TaxonomyId).Select(x => x.Name).FirstOrDefault();
                else levelText = newLevel.Name;

                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i)).SetValue(ChangeDetail, levelText, null);

                if (CurrentLevels[i-1].Name != NewLevels[i-1].Name)
                {
                    newText += string.Concat(i == 1 ? "" : " > ", "<span style=\"color:#e50000; \">  ", newLevel.Name ?? newLevel.TaxonomyId);
                }
                else
                {
                    newText += i == 1 ? levelText ?? newLevel.TaxonomyId : string.Concat(" > ", levelText ?? newLevel.TaxonomyId);
                }

                int.TryParse(newLevel.TaxonomyId, out int value);
                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i, "Id")).SetValue(ChangeDetail, value, null);
                i++;
            }

            ChangeDetail.Change = string.Concat(currentText, " <span style=\"color:#e50000; \"> is renamed to </span><br>", newText);

            if (!IsEdit)
            {
                if (Request.Changes == null)
                    Request.Changes = new();

                //To get the grid to refresh, you have to create a new bound object every time.  It only auto-refreshes with lists of simple types.  The .Rebind() method doesn't work the way I expected.
                //https://docs.telerik.com/blazor-ui/knowledge-base/grid-force-refresh?_gl=1*ndetl8*_ga*MjAxMDg2NzQwNi4xNjg4NTcyMDI5*_ga_9JSNBCSF54*MTY4OTc3NDE4NC4zNC4xLjE2ODk3NzQyMjAuMjQuMC4w&_ga=2.241678850.1218361352.1689629557-2010867406.1688572029
                ChangeDetail.CurrentStatus = "Add";
                var temp = Request.Changes;
                temp.Add(ChangeDetail!);
                Request.Changes = new();
                Request.Changes.AddRange(temp);
                Request.ChangePeriodId = (await HelperService.CurrentChangePeriodAsync()).ChangePeriodId;
                Request.SubmitDate = DateTime.UtcNow;
                ClearForm();
                OnParametersSet();
                IsSaveEnabled = true;
            }
            else
            {
                ChangeDetail.ModifyDate = DateTime.Now;
                ChangeDetail.ModifyUser = Request.ModifyUser;
                ChangeDetail.ChangeDetailId = Request.Changes.First().ChangeDetailId;
                Request.Changes.Clear();
                Request.Changes.Add(ChangeDetail);
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
                NewTaxDDL.ClearForm();
            }
        }

        /// <summary>
        /// Clears the grid of all changes.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task ClearGrid()
        {
            var isConfirmed = await ShowConfirm();

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

        protected void CurrentLevelSelectedEvent(List<TaxonomyDdlDTO> currentLevels)
        {
            CurrentLevels = currentLevels;
        }

        protected void NewLevelSelectedEvent(List<TaxonomyDdlDTO> newLevels)
        {
            NewLevels = newLevels;
        }

        /// <summary>
        /// Sets the parameter to trigger an update in the New Taxonomy child component
        /// </summary>
        /// <returns><![CDATA[void]]></returns>
        private async Task SendLevelsToNew()
        {
            NewLevels = new List<TaxonomyDdlDTO>(CurrentLevels);
        }

        /// <summary>
        /// Deletes the selected row from the grid
        /// </summary>
        /// <param name="args"></param>
        /// <returns><![CDATA[void]]></returns>
        private void DeleteRow(GridCommandEventArgs args)
        {
            var row = (ChangeDetailDTO)args.Item;
            Request.Changes.Remove(row);

            if(Request.Changes.Count == 0)
                IsSaveEnabled = false;
        }

        /// <summary>
        /// Create operation for new change request
        /// </summary>
        /// <returns><![CDATA[int]]> Id of new request</returns>
        private void CreateRequestAsync()
        {
            CreateRequestEvent.InvokeAsync(Request);
        }

        private bool ValidateDDL()
        {
            //there are two tests for a Rename: 1) does the count of both ddl componenets match and 2) is the last element in newLevels a custom entry.
            //the taxonomy name doesn't have to be unique across the whole taxnonomy but it does have to be unique under the parent so just checking for a Key of zero will only tell you if its custom but not unique
            if (NewLevels.Count == CurrentLevels.Count)
            {
                var i = 0;
                foreach (var level in NewLevels)
                {
                    var parent = NewLevels[i];
                    var matches = TaxonomyList.Where(x => x.ParentId == parent.ParentId);
                    if (matches.Where(x => x.TaxonomyId == NewLevels.Last().TaxonomyId).Any())
                        return false;

                    i++;
                }                    

                return true;
            }
            else
                return false;
        }

    }

}

