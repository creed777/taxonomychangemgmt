using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;


namespace HR_Taxonomy_Change_Management.Pages
{
    [Authorize(Roles = "TaxonomyApprover, TaxonomyOwner")]

    public partial class BasePage : ComponentBase { }
}
