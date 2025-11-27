using SchoolV01.Application.Requests.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;

namespace SchoolV01.Client.Pages.Authentication
{
    public partial class Login
    {
        [Parameter]
        public string ReturnUrl { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        protected override async Task OnInitializedAsync()
        {
            //var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);

            //if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ReturnUrl", out var returnUrl))
            //{
            //    ReturnUrl = returnUrl;
            //}
           _snackBar.Add("login", Severity.Warning);
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
               _navigationManager.NavigateTo("/");
            }
            //ReturnUrl = "~/" + ReturnUrl;
            //_navigationManager.NavigateTo($"Identity/Account/Login?returnUrl={ReturnUrl}", forceLoad: true);
        }

        private async Task SubmitAsync()
        {
            var result = await _authenticationManager.Login(_tokenModel);
            if (result.Succeeded)
            {
                
                _snackBar.Add(string.Format(_localizer["Welcome {0}"], _tokenModel.EmailOrUserName), Severity.Success);
                _navigationManager.NavigateTo("/", true);
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
        {
            if(_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        //private void FillAdministratorCredentials()
        //{
        //    // _tokenModel.Email = "mukesh@blazorhero.com";
        //    _tokenModel.Email = "admin@example.com";
        //    _tokenModel.Password = "User@12345";
        //}

        //private void FillBasicUserCredentials()
        //{
        //    _tokenModel.Email = "basic@example.com";
        //    _tokenModel.Password = "User@12345";
        //}
    }
}