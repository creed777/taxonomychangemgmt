using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Domain;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using Telerik.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.AspNetCore.Identity;
using HR_Taxonomy_Change_Management.Service;

namespace HR_Taxonomy_Change_Management.Component
{
    partial class RequestRemoveComponent
    {
        [Inject] IHelperService HelperService { get; set; }
        [Inject] NavigationManager? NavManager { get; set; }

        [Parameter] public List<TaxonomyDdlDTO> TaxonomyList { get; set; } = new();
        [Parameter] public RequestDTO? Request { get; set; } = new();
        [Parameter] public bool IsEdit { get; set; }
        [Parameter] public EventCallback<RequestDTO> SaveEditEvent { get; set; }
        [Parameter] public EventCallback<RequestDTO> CreateRequestEvent { get; set; }
        [CascadingParameter] public DialogFactory? Dialogs { get; set; }

        private string buttonText = string.Empty;
        public TaxonomyDDLComponent CurrentTaxDDL { get; set; } = new();
        public TaxonomyDDLComponent NewTaxDDL { get; set; } = new();
        public ChangeDetailDTO? ChangeDetail { get; set; }
        List<TaxonomyDdlDTO> CurrentLevels { get; set; } = new();
        List<TaxonomyDdlDTO> NewLevels { get; set; } = new();
        private bool IsAddEnabled { get { return (CurrentLevels.Count > 0); } }
        TelerikGrid<RequestDTO> ChangeGrid { get; set; } = new();
        private List<TaxonomyDdlDTO> RemoveList { get; set; } = new();
        public bool ShowRemoveDialog { get; set; }
        public string? AddParent { get; set; }
        public TelerikDialog? DialogRef { get; set; }

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

        protected override void OnParametersSet()
        {
            Request.RequestTypeName = RequestTypeEnum.Remove.ToString();
            Request.CurrentStatus = "Open";
        }

        protected void NewLevelSelectedEvent(List<TaxonomyDdlDTO> newLevels)
        {
            if (NewLevels != null && NewLevels.Last().TaxonomyId != null)
                CurrentLevels = new(newLevels);

            NewLevels = newLevels;
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
                CurrentTaxDDL.ClearForm();
                NewTaxDDL.ClearForm();
            }
        }

        /// <summary>
        /// Add a change to the grid for saving.  If this is an edit of an existing request, it will update the main request with the changes.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task AddToGrid(EditContext editContext)
        {
            if (Request.Changes == null)
                Request.Changes = new();

            ChangeDetail = new();

            RemoveList = TaxonomyList.Where(x => x.ParentId == int.Parse(CurrentLevels.Last().TaxonomyId)).ToList();

            ShowRemoveDialog = true;
            AddParent = null;

            while (string.IsNullOrEmpty(AddParent))
            {
                await Task.Delay(50);
            }

            if (AddParent == "cancel")
            {
                return;
            }
            else if (AddParent == "no")
            {

                foreach (var item in RemoveList)
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
                NewLevels.RemoveAt(NewLevels.Count() - 1);

                ChangeDetail = CreateChangeText(CurrentLevels, NewLevels);
                ChangeDetail.CurrentStatus = "Add";
                Request.Changes.Add(ChangeDetail);

            }

                //To get the grid to refresh, you have to create a new bound object every time.  It only auto-refreshes with lists of simple types.  The .Rebind() method doesn't work the way I expected.
                //https://docs.telerik.com/blazor-ui/knowledge-base/grid-force-refresh?_gl=1*ndetl8*_ga*MjAxMDg2NzQwNi4xNjg4NTcyMDI5*_ga_9JSNBCSF54*MTY4OTc3NDE4NC4zNC4xLjE2ODk3NzQyMjAuMjQuMC4w&_ga=2.241678850.1218361352.1689629557-2010867406.1688572029

                var temp = Request.Changes;
                Request.Changes = new();
                Request.Changes.AddRange(temp);
            
            if (!IsEdit)
            {
                //To get the grid to refresh, you have to create a new bound object every time.  It only auto-refreshes with lists of simple types.  The .Rebind() method doesn't work the way I expected.
                //https://docs.telerik.com/blazor-ui/knowledge-base/grid-force-refresh?_gl=1*ndetl8*_ga*MjAxMDg2NzQwNi4xNjg4NTcyMDI5*_ga_9JSNBCSF54*MTY4OTc3NDE4NC4zNC4xLjE2ODk3NzQyMjAuMjQuMC4w&_ga=2.241678850.1218361352.1689629557-2010867406.1688572029

                Request.ChangePeriodId = (await HelperService.CurrentChangePeriodAsync()).ChangePeriodId;
                Request.SubmitDate = DateTime.UtcNow;
                ChangeDetail.CurrentStatus = ChangeStatusEnum.Add.ToString();
                ChangeDetail.SubmitDate = DateTime.UtcNow;
                ChangeDetail.SubmitUser = Request.SubmitUser;

                ClearForm();
                OnParametersSet();
            }
            else
            {
                ChangeDetail.ModifyDate = DateTime.Now;
                ChangeDetail.ModifyUser = Request.ModifyUser;
                ChangeDetail.ChangeDetailId = Request.Changes!.First().ChangeDetailId;
                Request.Changes.Clear();
                Request.Changes.Add(ChangeDetail);
                await SaveEditEvent.InvokeAsync(Request);
            }

        }

        private ChangeDetailDTO CreateChangeText(List<TaxonomyDdlDTO> currentLevels, List<TaxonomyDdlDTO> newLevels)
        {
            string? currentText = string.Empty;
            string? newText = string.Empty;

            var i = 1;
            string? level;
            foreach (TaxonomyDdlDTO currentLevel in CurrentLevels)
            {
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i)).SetValue(ChangeDetail, currentLevel.Name, null);
                int.TryParse(currentLevel.TaxonomyId, out int Id);
                ChangeDetail.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).SetValue(ChangeDetail, Id, null);

                i++;
            }

            i = 1;
            level = string.Empty;
            foreach (TaxonomyDdlDTO newLevel in NewLevels)
            {
                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i)).SetValue(ChangeDetail, newLevel.Name, null);
                int.TryParse(newLevel.TaxonomyId, out int Id);
                ChangeDetail.GetType().GetProperty(string.Concat("NewL", i, "Id")).SetValue(ChangeDetail, Id, null);
                newText += string.Concat(i == 1 ? "" : " > ", newLevel.Name ?? newLevel.TaxonomyId);

                i++;
            }

            ChangeDetail.Change = string.Concat(newText, " > " , "<span style=\"color:#e50000; \">", CurrentLevels.Last().Name," is removed </span>");

            return ChangeDetail;
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
        }

        /// <summary>
        /// Clears the grid of all changes.
        /// </summary>
        /// <returns><![CDATA[Task]]></returns>
        public async Task ClearGrid(bool showConfirm)
        {
            bool isConfirmed = false;
            if (showConfirm)
            {
                isConfirmed = await ShowConfirm();
                if (isConfirmed)
                    NavManager.NavigateTo("/");
            }

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

        private void CreateRequest()
        {
            LoaderVisible = true;
            CreateRequestEvent.InvokeAsync(Request);
            LoaderVisible = false;
        }

        private void DialogConfirmation(string dialogConfirmed)
        {
            AddParent = dialogConfirmed;
            ShowRemoveDialog = false;
        }
    }
}

