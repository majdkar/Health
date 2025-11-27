using System;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SchoolV01.Application.Features.ProjectTypes.Commands;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class ProjectType
    {
        [Inject] private IProjectTypeManager ProjectTypeManager { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllProjectTypesResponse> _PositionList = new();
        private GetAllProjectTypesResponse _Position = new();

        private List<GetAllProjectTypesResponse> _subProjects = new();
        private bool _isShowingSubProjects = false;
        private int _currentProjectTypeId = 0;
        private Stack<int> _parentStack = new(); // استخدم int بدل int? لتسهيل المقارنة

        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreatePositions;
        private bool _canEditPositions;
        private bool _canDeletePositions;
        private bool _canExportPositions;
        private bool _canSearchPositions;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreatePositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ProjectTypes.Create)).Succeeded;
            _canEditPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ProjectTypes.Edit)).Succeeded;
            _canDeletePositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ProjectTypes.Delete)).Succeeded;
            _canExportPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ProjectTypes.Export)).Succeeded;
            _canSearchPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ProjectTypes.Search)).Succeeded;

            await GetPositionsAsync();
            _loaded = true;

            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        // رجوع مباشرة للفئات الرئيسية
        private async Task BackToMainTable()
        {
            _isShowingSubProjects = false;
            _currentProjectTypeId = 0;
            _parentStack.Clear();
            await GetPositionsAsync();
        }

        // رجوع خطوة للأعلى
        private async Task BackOneLevel()
        {
            if (_parentStack.Count > 0)
            {
                var parentId = _parentStack.Pop();

                if (parentId == 0)
                {
                    // رجوع للفئات الرئيسية
                    await BackToMainTable();
                }
                else
                {
                    // عرض المستوى الأعلى
                    await ShowSubProjects(parentId);
                }
            }
            else
            {
                // إذا لم يكن هناك عناصر في الستاك، اعتبرنا الفئات الرئيسية
                await BackToMainTable();
            }
        }

        private async Task GetPositionsAsync()
        {
            var response = await ProjectTypeManager.GetAllAsync();
            if (response.Succeeded)
            {
                _PositionList = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await ProjectTypeManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                        _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task ShowSubProjects(int projectTypeId)
        {
            // حفظ المستوى الحالي قبل تغييره
            _parentStack.Push(_currentProjectTypeId);

            var response = await ProjectTypeManager.GetAsync(projectTypeId);
            if (response.Succeeded)
            {
                _subProjects = response.Data.SubProjectTypes.ToList();

                _PositionList = _subProjects.Select(x => new GetAllProjectTypesResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    EnglishName = x.EnglishName,
                    ParentId = projectTypeId
                }).ToList();

                _isShowingSubProjects = true;
                _currentProjectTypeId = projectTypeId;
            }
            else
            {
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            AddEditProjectTypeCommand modalModel;

            if (id != 0)
            {
                _Position = _PositionList.FirstOrDefault(c => c.Id == id);
                modalModel = _Position != null
                    ? new AddEditProjectTypeCommand
                    {
                        Id = _Position.Id,
                        Name = _Position.Name,
                        EnglishName = _Position.EnglishName,
                        ParentId = _Position.ParentId
                    }
                    : new AddEditProjectTypeCommand();
            }
            else
            {
                modalModel = new AddEditProjectTypeCommand();

                // إذا كنا نعرض المشاريع الفرعية، ضع ParentId تلقائيًا
                if (_isShowingSubProjects)
                    modalModel.ParentId = _currentProjectTypeId;
            }

            parameters.Add(nameof(AddEditProjectTypeModal.AddEditProjectTypeModel), modalModel);
            parameters.Add(nameof(AddEditProjectTypeModal._isShowingSubProjects), _isShowingSubProjects);
            parameters.Add(nameof(AddEditProjectTypeModal.ParentId), _currentProjectTypeId);

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<AddEditProjectTypeModal>(
                id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
                await Reset();
        }

        private async Task Reset()
        {
            _Position = new GetAllProjectTypesResponse();
            await GetPositionsAsync();
        }

        private bool Search(GetAllProjectTypesResponse Position)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (Position.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) return true;
            if (Position.EnglishName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) return true;
            return false;
        }
    }
}
