using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Telerik.Blazor;

namespace HR_Taxonomy_Change_Management.Pages
{
    partial class RequestDetail : BasePage
    {
        [Inject] private NavigationManager? NavManager { get; set; }
        [Inject] private IRequestDomain? RequestDomain { get; set; }
        [Inject] private IChangeDomain? ChangeDomain { get; set; }
        [Inject] IAuthorizationService? AuthService { get; set; }

        [Parameter] public string? RequestIdParam { get; set; }
        [CascadingParameter] public DialogFactory? Dialogs { get; set; }
        [Parameter] public bool IsTaxApprover { get; set; } = false;

        private RequestDTO? _request;
        private string? _justification;
        public RequestDTO Request
        {
            get { return _request!; }
            set
            {
                _request = value;
                _justification = value.Justification;
            }
        }
            
        protected async override Task OnInitializedAsync()
        {
            if (Request == null)
                Request = new();

            if(string.IsNullOrEmpty(RequestIdParam))
                return;

            int.TryParse(RequestIdParam, out var id);
            Request = await RequestDomain.GetRequestAsync(id).ConfigureAwait(false);

            if (Request == null)
                 Request = new RequestDTO();
        }

        protected override void OnParametersSet()
        {
            IsTaxApprover = AuthService.IsApprover;
        }

        /// <summary>
        /// The save from the ChangeDetails component that sends it to the database
        /// </summary>
        /// <param name="Request"></param>
        /// <returns><![CDATA[Task]]></returns>
        protected async Task SaveEditRequestEvent(RequestDTO Request)
        {
            await RequestDomain.UpdateRequestAsync(Request);
            NavManager.NavigateTo("/");
        }

        protected async Task ApproveRequestEvent(List<ChangeDetailDTO> changeList)
        {
            int result = 0;
            switch(changeList.Select(x => x.CurrentStatus).First())
            {
                case "Approve":
                    result = await ChangeDomain.StatusChangeAsync(changeList, ChangeStatusEnum.Approved).ConfigureAwait(false);
                    break;
                case "Denied":
                    result = await ChangeDomain.StatusChangeAsync(changeList, ChangeStatusEnum.Denied).ConfigureAwait(false);
                    break;
                case "PendingReview":
                    result = await ChangeDomain.StatusChangeAsync(changeList, ChangeStatusEnum.PendingReview).ConfigureAwait(false);
                    break;
            }

            if (result <= 0)
            {
                //TODO: get support contact
                var message = "There was an error saving your changes.  Please contact ....";
                await ShowAlert(message, "Error");
            }
            else
            {
                ShowConfirmDialog = true;
            }
        }

        public async Task SaveEdit(EditContext editContext)
        {
            if (editContext == null || editContext.Model == null)
                return;

            var request = editContext.Model as RequestDTO;
            request.Changes = new ();
            await RequestDomain.UpdateRequestAsync(request!).ConfigureAwait(false);
        }

        public void CancelEdit()
        {
            Request.Justification = _justification ?? string.Empty;
        }

        public async Task ShowAlert(string message, string headerText)
        {
            await Dialogs.AlertAsync(message, headerText);
        }
    }
}
