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
    partial class RequestMoveComponent
    {
        [Inject] NavigationManager? NavManager { get; set; }
        [Inject] IHelperService? HelperService { get; set; }

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
        public string DialogText { get; set; } = string.Empty;
        public bool ShowMoveDialog { get; set; }
        public string? AddParent { get; set; }
        public TelerikDialog? DialogRef { get; set; }
        private int MoveCount {get; set;}
        private List<TaxonomyDdlDTO> MoveList { get; set; } = new();

        protected override void OnInitialized()
        {
            if (Request.Changes == null || Request.Changes.Count == 0)
            {
                Request.Changes = new();
                ChangeDetail = new();
            }
            else
                ChangeDetail = Request.Changes.First();

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

        protected async override Task OnParametersSetAsync()
        {

            Request.RequestTypeName = "Move";
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
                await Dialogs.AlertAsync("This is a MOVE process.  The Current and New levels cannot be the same", "Alert");
                return;
            }

            if (Request.Changes == null)
                Request.Changes = new();

            ChangeDetail = new();
            if (CurrentLevels.Count == NewLevels.Count)
            {
                MoveList = TaxonomyList.Where(x => x.ParentId == int.Parse(CurrentLevels.Last().TaxonomyId)).ToList();
                MoveCount = MoveList.Count;

                ShowMoveDialog = true;
                AddParent = null;

                while (string.IsNullOrEmpty(AddParent))
                {
                    await Task.Delay(50);
                }

                if(AddParent == "cancel")
                {
                    return;
                }
                else if(AddParent == "no")
                {
                    
                    foreach(var item in MoveList)
                    {
                        CurrentLevels.Add(item);

                        ChangeDetail = new();
                        ChangeDetail = CreateChangeText(CurrentLevels, NewLevels);
                        ChangeDetail.CurrentStatus = "Add";
                        Request.Changes.Add(ChangeDetail);

                        CurrentLevels.RemoveAt(CurrentLevels.Count() - 1);
                    }
                }
                else
                {
                    ChangeDetail = new();
                    ChangeDetail = CreateChangeText(CurrentLevels, NewLevels);
                    ChangeDetail.CurrentStatus = "Add";
                    Request.Changes.Add(ChangeDetail);

                }

                //To get the grid to refresh, you have to create a new bound object every time.  It only auto-refreshes with lists of simple types.  The .Rebind() method doesn't work the way I expected.
                //https://docs.telerik.com/blazor-ui/knowledge-base/grid-force-refresh?_gl=1*ndetl8*_ga*MjAxMDg2NzQwNi4xNjg4NTcyMDI5*_ga_9JSNBCSF54*MTY4OTc3NDE4NC4zNC4xLjE2ODk3NzQyMjAuMjQuMC4w&_ga=2.241678850.1218361352.1689629557-2010867406.1688572029

                var temp = Request.Changes;
                Request.Changes = new();
                Request.Changes.AddRange(temp);
            }


            if (!IsEdit)
            {

                Request.ChangePeriodId = (await HelperService.CurrentChangePeriodAsync()).ChangePeriodId;
                Request.SubmitDate = DateTime.UtcNow;
                ChangeDetail.CurrentStatus = ChangeStatusEnum.Add.ToString();
                ChangeDetail.SubmitDate = DateTime.UtcNow;
                ChangeDetail.SubmitUser = Request.SubmitUser;

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

        private ChangeDetailDTO CreateChangeText(List<TaxonomyDdlDTO> CurrentLevels, List<TaxonomyDdlDTO> NewLevels)
        {
            string? currentText = string.Empty;
            string? newText = string.Empty;

            var i = 1;
            string? level;
            foreach (TaxonomyDdlDTO currentLevel in CurrentLevels)
            {
                level = TaxonomyList.Where(x => x.TaxonomyId == currentLevel.TaxonomyId).Select(x => x.Name).FirstOrDefault();
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i)).SetValue(ChangeDetail, level, null);
                int.TryParse(currentLevel.TaxonomyId, out int Id);
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).SetValue(ChangeDetail, Id, null);
                currentText += i == 1 ? level ?? currentLevel.TaxonomyId : string.Concat(" > ", level ?? currentLevel.TaxonomyId);
                i++;
            }

            i = 1;
            level = string.Empty;
            foreach (TaxonomyDdlDTO newLevel in NewLevels)
            {
                level = TaxonomyList.Where(x => x.TaxonomyId == newLevel.TaxonomyId).Select(x => x.Name).FirstOrDefault();
                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i)).SetValue(ChangeDetail, level, null);
                int.TryParse(newLevel.TaxonomyId, out int Id);
                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i, "Id")).SetValue(ChangeDetail, Id, null);
                newText += i == 1 ? level ?? newLevel.TaxonomyId : string.Concat(" > ", level ?? newLevel.TaxonomyId);
                i++;
            }

            ChangeDetail.Change = string.Concat(currentText, "<span style=\"color:#e50000; \"> is moved to <br/></span>", newText, "> <span style=\"color:#e50000; \" > ", CurrentLevels.Last().Name, "</span>");
            return ChangeDetail;
        }

        private void DialogConfirmation(string dialogConfirmed)
        {
            AddParent = dialogConfirmed;
            ShowMoveDialog = false;
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

        public bool ValidateDDL()
        {
            if (CurrentLevels.Count == NewLevels.Count)
            {
                for (int i = 0; i <= CurrentLevels.Count-1; i++)
                {
                    if (CurrentLevels[i] != NewLevels[i])
                        return true;
                }

                return false;
            }
            return true;
        }

        public async Task CancelChanges()
        {
            var isConfirmed = await ShowConfirm();

            if (isConfirmed)
                NavManager.NavigateTo("/");
            else
                ClearGrid();
        }

        /// <summary>
        /// Clears the grid of all changes.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public void ClearGrid()
        {
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

        public void CreateRequest()
        {
            CreateRequestEvent.InvokeAsync(Request);
        }

        /// <summary>
        /// Sets the parameter to trigger an update in the New Taxonomy child component
        /// </summary>
        /// <returns><![CDATA[void]]></returns>
        private void SendLevelsToNew()
        {
            NewLevels = new(CurrentLevels);
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

            if (Request.Changes.Count == 0)
                IsSaveEnabled = false;
        }
    }
}
