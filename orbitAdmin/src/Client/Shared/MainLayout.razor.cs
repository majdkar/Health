using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Settings;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SchoolV01.Client.Infrastructure.Managers.Identity.Roles;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using SchoolV01.Client.Infrastructure.Managers.Dashboard;
using SchoolV01.Client.Helpers;
using System.Collections.Generic;
using System.Net.Http.Json;
using SchoolV01.Application.Responses.Identity;
using System.Net.Http;
using SchoolV01.Shared.Constants.Role;

namespace SchoolV01.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }
        [Inject] private IDashboardManager DashboardManager { get; set; }

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private string ReturnUrl { get; set; }
        private char FirstLetterOfName { get; set; }

        private ClaimsPrincipal _authenticationStateProviderUser;

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;


         

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                FirstName = user.GetFirstName();
                if (FirstName.Length > 0)
                {
                    FirstLetterOfName = FirstName[0];
                }
                SecondName = user.GetLastName();
                Email = user.GetEmail();
                var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
                if (imageResponse.Succeeded)
                {
                    ImageDataUrl = imageResponse.Data;
                }

                var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                {
                    _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted."], Severity.Error);
                    await _authenticationManager.Logout();
                }

               //await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);
            }
        }

        bool _isDarkMode;
        MudTheme _currentTheme = BlazorHeroTheme.DefaultTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft = true;
        private int _notificationCount = 0;
        private List<NotificationResponse> _notifications = new();
        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;
            _drawerOpen = false;
        }
        private async Task SelectAsync(NotificationResponse notification)
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.DeleteConfirmation.ContentText), notification.Body},
                {nameof(Dialogs.DeleteConfirmation.Title), notification.Title},
                {nameof(Dialogs.DeleteConfirmation.ButtonText),"Set As Seen"}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<Dialogs.DeleteConfirmation>(notification.Title, parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var content = HelperMethods.ToJson(true);
                HttpResponseMessage response = await _httpClient.PostAsync($"{EndPoints.Notifications}/SetAsSeen/{notification.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    _snackBar.Add("Completed Successful!", Severity.Success);
                    await LoadNotificationsAsync();

                }
                else
                {
                    _snackBar.Add("Something went wrong!", Severity.Error);
                }
            }
        }
        private async Task ChangeLanguageAsync(string languageCode)
        {
            var result = await _clientPreferenceManager.ChangeLanguageAsync(languageCode);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
                if (!_rightToLeft && languageCode.Contains("ar"))
                {
                    var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
                    _rightToLeft = isRtl;
                    _drawerOpen = false;
                }
                if (_rightToLeft && languageCode.Contains("en"))
                {
                    var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
                    _rightToLeft = isRtl;
                    _drawerOpen = false;
                }
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }



        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAny(LoadNotificationsAsync());

            _rightToLeft = await _clientPreferenceManager.IsRTL();
            _interceptor.RegisterEvent();
            hubConnection = hubConnection.TryInitialize(_navigationManager);
            await hubConnection.StartAsync();
            hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId == receiverUserId)
                {
                    _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                    _snackBar.Add(message, Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 10000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.Action = localizer["Chat?"];
                        config.ActionColor = Color.Primary;
                        config.OnClick = snackbar =>
                        {
                            _navigationManager.NavigateTo($"chat/{senderUserId}");
                            return Task.CompletedTask;
                        };
                    });
                }
            });
            hubConnection.On(ApplicationConstants.SignalR.ReceiveNotification, async () =>
            {
                try
                {
                    LoadQ();
                    await LoadNotificationsAsync();
                    // await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
                StateHasChanged();

            });
            hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = await _authenticationManager.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(localizer["Refreshed Token."], Severity.Success);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(localizer["You are Logged Out."], Severity.Error);
                    await _authenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            });
            hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResponse = await RoleManager.GetRolesAsync();
                    if (rolesResponse.Succeeded)
                    {
                        var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                            if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                _snackBar.Add(localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
                                await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                                await _authenticationManager.Logout();
                                _navigationManager.NavigateTo("/login");
                            }
                        }
                    }
                }
            });

            _authenticationStateProviderUser = await _stateProvider.GetAuthenticationStateProviderUserAsync();
            LoadQ();

        }

        private void LoadQ()
        {
            //BackgroundJob.Enqueue(() => Console.WriteLine("helloooooooooo"));
        }


        private async Task LoadNotificationsAsync()
        {
            var notifications = await _httpClient.GetFromJsonAsync<List<NotificationResponse>>(EndPoints.Notifications + "/GetUserUnSeenNotification");

            if (notifications.Count() > 0)
            {
                _snackBar.Add(localizer["Receive Notification"], Severity.Success);
                _notifications = notifications;
                _notificationCount = notifications.Count;
            }
        }
    
        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.ShowAsync<Dialogs.Logout>(localizer["Logout"], parameters, options);
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task DarkMode()
        {
            _isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}