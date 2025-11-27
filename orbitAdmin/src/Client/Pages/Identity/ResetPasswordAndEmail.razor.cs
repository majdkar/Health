using SchoolV01.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using SchoolV01.Client.Extensions;
using Microsoft.AspNetCore.Components;

namespace SchoolV01.Client.Pages.Identity
{
    public partial class ResetPasswordAndEmail
    {
        [Parameter] public string OldEmail { get; set; }
        [CascadingParameter] private IMudDialogInstance  MudDialog { get; set; }

        private readonly ResetPasswordAndEmailRequest _passwordModel = new();
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            _passwordModel.NewEmail = user.GetEmail();
            if (!string.IsNullOrEmpty(OldEmail))
            {
                _passwordModel.NewEmail = OldEmail;
                _passwordModel.OldEmail = OldEmail;
            }
        }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        private async Task ChangePasswordAsync()
        {
            var response = await _accountManager.ResetPasswordAndEmailAsync(_passwordModel);
            if (response.Succeeded)
            {
                _snackBar.Add(_localizer["Change Completed!"], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private bool _currentPasswordVisibility;
        private InputType _currentPasswordInput = InputType.Password;
        private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private bool _newPasswordVisibility;
        private InputType _newPasswordInput = InputType.Password;
        private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility(bool newPassword)
        {
            if (newPassword)
            {
                if (_newPasswordVisibility)
                {
                    _newPasswordVisibility = false;
                    _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _newPasswordInput = InputType.Password;
                }
                else
                {
                    _newPasswordVisibility = true;
                    _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _newPasswordInput = InputType.Text;
                }
            }
            else
            {
                if (_currentPasswordVisibility)
                {
                    _currentPasswordVisibility = false;
                    _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _currentPasswordInput = InputType.Password;
                }
                else
                {
                    _currentPasswordVisibility = true;
                    _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _currentPasswordInput = InputType.Text;
                }
            }
        }
    }
}