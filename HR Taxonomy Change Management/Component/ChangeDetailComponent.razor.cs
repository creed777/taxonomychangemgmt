using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace HR_Taxonomy_Change_Management.Component
{
    partial class ChangeDetailComponent : ComponentBase
    {
        [Inject] NavigationManager? NavManager { get; set; }
        [Inject] IHelperService? HelperService { get; set; }
        [Inject] IAuthorizationService? AuthService { get; set; }

        [Parameter] public EventCallback<RequestDTO> SaveEditRequestEvent { get; set; }
        [Parameter] public EventCallback<List<ChangeDetailDTO>> StatusChangeEvent { get; set; }
        [Parameter] public RequestDTO? Request { get; set; }
        [CascadingParameter] public DialogFactory? Dialogs { get; set; }
        [Parameter] public bool IsTaxApprover { get; set; }

        public bool showReviewDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {

        }

        protected async override Task OnParametersSetAsync()
        {
            if (Request == null)
                Request = new();

            TextIsVisible();

            if (Request.Changes == null)
                Request.Changes = new();

            foreach (ChangeDetailDTO change in Request.Changes)
            {
                if (change.Change != null)
                {
                    change.Change = change.Change.Replace("<li>", "");
                    change.Change = change.Change.Replace("</li>", "");
                }
            }

            GridRef.Rebind();

            var changePeriod = await HelperService.CurrentChangePeriodAsync();
            if (changePeriod.EndDate <= DateTime.UtcNow.Date)
                EditEnabled = false;
            else
                EditEnabled = true;
        }

        /// <summary>
        /// Called from grid by clicking pencil.  Sends Request and Change to the RequestAdd Component
        /// </summary>
        /// <param name="args"></param>
        /// <returns><![CDATA[Task]]></returns>
        private async Task EditChange(GridCommandEventArgs args)
        {
            var change = args.Item as ChangeDetailDTO;
            NavManager.NavigateTo($"newrequest/{change.ChangeDetailId}");
        }

        public async Task<string> OpenReviewModal(string reviewText, string changeText)
        {
            string userInput = await Dialogs.PromptAsync(changeText,"Enter Review Text", reviewText);

            if (userInput != null)
                reviewText = userInput;

            CloseReviewModal();
            return reviewText;
        }

        public void CloseReviewModal()
        {
            showReviewDialog = false;
        }

        private async Task StatusChange(string status)
        {
            var user = AuthService.UserEmail;

            foreach (var change in SelectedItems.ToList())
            {
                if (status == ChangeStatusEnum.PendingReview.ToString())
                {
                    string changeText = string.Empty;

                    switch (Request.RequestTypeName)
                    {
                        case nameof(RequestTypeEnum.Add):

                            for (int i = 1; i <= 5; i++)
                            {
                                var newLevel = change.GetType().GetProperty(string.Concat("NewL", i, "Id")).GetValue(change, null)??new object();
                                
                                if (int.TryParse(newLevel.ToString() ?? "", out var id) == true)
                                    if (id == 0)
                                    {
                                        changeText += string.Concat(!string.IsNullOrEmpty(changeText)? " > " : "", change.GetType().GetProperty(string.Concat("NewL", i)).GetValue(change, null).ToString() ?? "");
                                    }
                            }
                            changeText = string.Concat("Added: ", changeText);
                            break;

                        case nameof(RequestTypeEnum.Move):

                            for (int i = 1; i <= 5; i++)
                            {
                                var currentLevelId = change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) ?? new object();
                                if (int.TryParse(currentLevelId.ToString() ?? "", out var id) == false)
                                {
                                    changeText += string.Concat(change.GetType().GetProperty(string.Concat("CurrentL", i - 1)).GetValue(change, null).ToString() ?? "");
                                    changeText = string.Concat("Move: ", changeText);
                                    break;
                                }
                            }
                            break;

                        case nameof(RequestTypeEnum.Rename):

                            for (int i = 1; i <= 5; i++)
                            {
                                var currentLevelId = change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) ?? new object();
                                if (int.TryParse(currentLevelId.ToString() ?? "", out var id) == false)
                                {
                                    if (id == 0)
                                    {
                                        changeText += string.Concat(change.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(change, null).ToString() ?? "", " > ", change.GetType().GetProperty(string.Concat("CurrentL", i - 1)).GetValue(change, null).ToString() ?? "");
                                        changeText = string.Concat("Rename: ", changeText);
                                        break;
                                    }
                                }
                            }

                            break;

                        case nameof(RequestTypeEnum.Remove):
                            for (int i = 1; i <= 5; i++)
                            {
                                var currentLevelId = change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) ?? new object();
                                if (int.TryParse(currentLevelId.ToString() ?? "", out var id) == false)
                                {
                                    changeText += string.Concat(change.GetType().GetProperty(string.Concat("CurrentL", i-2)).GetValue(change, null).ToString() ?? "", " > ", change.GetType().GetProperty(string.Concat("CurrentL", i - 1)).GetValue(change, null).ToString() ?? "");
                                    changeText = string.Concat("Remove: ", changeText);
                                    break;
                                }
                            }

                            break;

                        default:
                            break;
                    }

                    change.ReviewText = await OpenReviewModal(change.ReviewText??"", changeText);
                }

                change.RequestId = Request.RequestId;
                change.CurrentStatus = status;
                change.SubmitDate = DateTime.UtcNow;
                change.SubmitUser = user ?? "";
            }

            await StatusChangeEvent.InvokeAsync(SelectedItems.ToList());
        }
    }
}
