using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using SchoolV01.Application.Requests.Identity;

namespace SchoolV01.Client.Pages.Identity
{
    public partial class ResetUserPassword
    {

        [CascadingParameter] private IMudDialogInstance  MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        [Parameter] public ResetPasswordUserRequest _resetPasswordModel { get; set; } = new();
        protected override void OnInitialized()
        {


        }
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            var result = await _userManager.ResetPasswordAsync(_resetPasswordModel);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                MudDialog.Close();
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
      
        private bool _confirmPasswordVisibility;
        private InputType _confirmPasswordInput = InputType.Password;
        private string _confirmPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
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
        private void ToggleConfirmPasswordVisibility()
        {
            if (_confirmPasswordVisibility)
            {
                _confirmPasswordVisibility = false;
                _confirmPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _confirmPasswordInput = InputType.Password;
            }
            else
            {
                _confirmPasswordVisibility = true;
                _confirmPasswordInputIcon = Icons.Material.Filled.Visibility;
                _confirmPasswordInput = InputType.Text;
            }
        }








    }
}