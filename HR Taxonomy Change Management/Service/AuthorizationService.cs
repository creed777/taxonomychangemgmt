using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
namespace HR_Taxonomy_Change_Management.Misc
{
    public class AuthorizationService : IAuthorizationService
    {   
        private AuthenticationState AuthState { get; set; }
        bool _isOwner;
        bool IAuthorizationService.IsOwner 
        { 
            get => _isOwner; 
        }
        bool _isApprover;
        bool IAuthorizationService.IsApprover 
        { 
            get => _isApprover; 
        }
        string _userEmail;
        string IAuthorizationService.UserEmail
        {
            get => _userEmail;
        }
        string _userName;
        string IAuthorizationService.UserName
        {
            get => _userName;
        }

        public AuthorizationService(AuthenticationStateProvider authState) {

            AuthState = authState.GetAuthenticationStateAsync().Result;
            var user = AuthState.User;
            _isOwner = user.IsInRole("TaxonomyOwner");
            _isApprover = user.IsInRole("TaxonomyApprover");
            _userEmail = user.Identity.Name ?? string.Empty;
            _userName = user.Claims.First(c => c.Type == "name").Value ?? string.Empty;
        }

    }
}
