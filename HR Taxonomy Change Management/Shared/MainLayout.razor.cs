using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Components;
using Telerik.FontIcons;

namespace HR_Taxonomy_Change_Management.Shared
{

    public class MenuItem
    {
        public string Text { get; set; } = string.Empty;
        public bool Disabled { get; set; }
        public object? Icon { get; set; }
        public string Url { get; set; } = string.Empty;
        public IEnumerable<MenuItem> Items { get; set; } = Enumerable.Empty<MenuItem>();
    }

    partial class MainLayout
    {
        [Inject] private IAuthorizationService? AuthService { get; set; }
        [Inject] private IHelperService? HelperService { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private bool ShowChangeMessage { get; set; }
        private string PeriodStartDate { get; set; }


        public MainLayout() { }

        protected async override Task OnInitializedAsync()
        {
            PeriodStartDate = (await HelperService.NextChangePeriodAsync()).StartDate.ToLongDateString();

            MenuItems = new List<MenuItem>()
            {
                new MenuItem{
                    Url = "/",
                    Icon = FontIcon.Menu,
                    Items = new List<MenuItem>(){
                    new MenuItem {Text = "Home", Url="/"},
                    new MenuItem {Text = "New Request", Url="/newrequest", Disabled = !await HelperService.InChangePeriodAsync()},
                    new MenuItem {Text = "Taxonomy", Url="/taxonomy"},
                    new MenuItem {Text = "Admin", Url="/admin", Disabled = !AuthService.IsApprover},
                    new MenuItem {Text = "Logout", Url="https://login.microsoftonline.com/common/oauth2/v2.0/logout"}
                    }
                }
            };

            ShowChangeMessage = !await HelperService.InChangePeriodAsync();
            base.OnInitialized();
        }
    }
}
