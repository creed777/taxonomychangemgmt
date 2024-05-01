using HR_Taxonomy_Change_Management.Component;
using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.AspNetCore.Components;

namespace HR_Taxonomy_Change_Management.Pages
{
    partial class NewRequest : BasePage
    {
        [Inject] private NavigationManager? NavManager { get; set; }
        [Inject] private ITaxonomyDomain? TaxonomyDomain { get; set; }
        [Inject] private IChangeDomain? ChangeDomain { get; set; }
        [Inject] private IRequestDomain? RequestDomain { get; set; }
        [Inject] private IAuthorizationService? AuthService { get; set; }
        [Parameter] public string? ChangeIdParam { get; set; }

        private RequestDTO Request { get; set; } = new();
        public List<TaxonomyDTO> TaxonomyList { get; set; } = new();
        public List<TaxonomyDdlDTO> TaxonomyDdl { get; set; } = new();
        public List<RequestType> RequestTypeList { get; set; } = new();
        public int SelectedRequestTypeId { get; set; }
        private bool IsEdit { get; set; }
        private NotificationComponent? Notice { get; set; }
        private bool ShowConfirmDialog { get; set; }
        private string ConfirmTitle { get; set; } = string.Empty;
        private RequestMoveComponent? MoveComponent { get; set; }
        private RequestRenameComponent? RenameComponent { get; set; }
        private RequestAddComponent? AddComponent { get; set; }
        private RequestRemoveComponent? RemoveComponent { get; set; }
        private MarkupString helpCardText { get; set; }

        protected async override Task OnInitializedAsync()
        {
            TaxonomyList = await TaxonomyDomain.GetAllTaxonomyAsync().ConfigureAwait(false);

            foreach (TaxonomyDTO t in TaxonomyList)
            {
                TaxonomyDdlDTO listItem = new()
                {
                    ParentId = t.ParentId,
                    Name = t.Name,
                    TaxonomyId = t.TaxonomyId.ToString()
                };

                TaxonomyDdl.Add(listItem);
            }

            RequestTypeList = await ChangeDomain.GetAllRequestTypesAsync();

            if (!string.IsNullOrEmpty(ChangeIdParam))
            {
                IsEdit = true;
                int.TryParse(ChangeIdParam, out var changeId);
                Request = await RequestDomain.GetRequestByChangeAsync(changeId);
                Request.Changes = Request.Changes.Where(x => x.ChangeDetailId == changeId).ToList();

                RequestTypeEnum requestTypeParam = new();
                Enum.TryParse(Request.RequestTypeName, out requestTypeParam);
                SelectedRequestTypeId = (int)requestTypeParam;
            }
            else
                IsEdit = false;
            Request.SubmitDate = DateTime.Now;
            Request.SubmitUser = AuthService.UserEmail;
        }

        /// <summary>
        /// Event raised from RequestAdd component
        /// </summary>
        /// <param name="editRequest"> <![CDATA[RequestDTO]]></param>
        /// <returns><![CDATA[Task]]></returns>
        private async Task SaveEditEvent(RequestDTO request)
        {
            var result = await RequestDomain.UpdateRequestAsync(request);
            SaveNotice.SaveSuccess();
            NavManager.NavigateTo($"requestdetail/{request.RequestId}");
        }

        /// <summary>
        /// Event raised from RequestAdd component
        /// </summary>
        /// <param name="editRequest"> <![CDATA[RequestDTO]]></param>
        /// <returns><![CDATA[Task]]></returns>
        private async Task CreateRequestEvent(RequestDTO request)
        {
            Request = request;
            var result = await RequestDomain.CreateRequestAsync(request).ConfigureAwait(false);

            if (result > 0)
            {
                SaveNotice.SaveSuccess();
                ShowConfirmDialog = true;
            }
            else
                SaveNotice.SaveFailure();
        }

        private async Task StayOnPage(bool answer)
        {
            if (answer)
            {
                switch (Request.RequestTypeName)
                {
                    case "Move":
                        MoveComponent.ClearForm();
                        MoveComponent.ClearGrid();
                        break;
                    case "Rename":
                        RenameComponent.ClearForm();
                        await RenameComponent.ClearGrid();
                        break;
                    case "Add":
                        AddComponent.ClearForm();
                        await AddComponent.ClearGrid();
                        break;
                    case "Remove":
                        RemoveComponent.ClearForm();
                        await RemoveComponent.ClearGrid(false);
                        break;
                    default: break;
                }

                ShowConfirmDialog = false;
                Request = new RequestDTO();

                SelectedRequestTypeId = 0; 
            }
            else
            {
                ShowConfirmDialog = false;
                NavManager.NavigateTo("/");
            }
        }

        private void displayHelpCard(object value)
        {
            switch((int)value)
            {
                case (int)RequestTypeEnum.Add:
                helpCardText = new MarkupString("To add a new Taxononmy level: <ul><li>Select the existing Taxonomy levels.</li>" +
                    "<li>Type in the name of the new Taxonomy Level</li>" +
                    "<li>Click \"Add\" and repeat for as many new levels as you need.</li> </ul>" +
                    "Once all changes are created, click \"Save\" and the request is saved.  You can either create a new Request or return to the Home screen.");
                break;

                case (int)RequestTypeEnum.Move:
                helpCardText = new MarkupString("To move Taxononmy level: <ul><li>Select the levels in Current Taxonomy.</li>" +
                    "<li>Select the levels in New Taxononmy that you want to move to.  If you want to move to the same level as Current, then don't select that level in New.<br />" +
                    "Making a New selection at the same level will move the Current <b>under</b> the New</li>" +
                    "<li>Click \"Add\" and repeat for as many new levels as you need.</li> </ul>" +
                    "Once all changes are created, click \"Save\" and the request is saved.  You can either create a new Request or return to the Home screen.");
                break;

                case (int)RequestTypeEnum.Rename:
                helpCardText = new MarkupString("To rename Taxononmy level: <ul><li>Select the levels in Current Taxonomy.</li>" +
                    "<li>In the New Taxonomy type in the new name at the same level." +
                    "<li>Click \"Add\" and repeat for as many new levels as you need.</li> </ul>" +
                    "Once all changes are created, click \"Save\" and the request is saved.  You can either create a new Request or return to the Home screen.");
                break;

                case (int)RequestTypeEnum.Remove:
                helpCardText = new MarkupString("To remove a Taxononmy level: <ul><li>Select the existing Taxonomy level you want to remove.</li>" +
                    "<li>By default it will remove all levels <b>below</b> the selected level but it will give you the opportunity to include the selected level in your remove when you click \"Add\"</li>" +
                    "<li>Click \"Add\" and repeat for as many new levels as you need.</li> </ul>" +
                    "Once all changes are created, click \"Save\" and the request is saved.  You can either create a new Request or return to the Home screen.");
                break;
            }
        }
    }
}
