using Blazored.FluentValidation;
using SchoolV01.Application.Features.ProjectTypes.Commands;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class AddEditProjectTypeModal
    {
        [Inject] private IProjectTypeManager ProjectTypeManager { get; set; }

        [Parameter] public AddEditProjectTypeCommand AddEditProjectTypeModel { get; set; } = new();

        [Parameter] public bool _isShowingSubProjects { get; set; } = false;
        [Parameter] public int ParentId { get; set; }


        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;

        private List<GetAllProjectTypesResponse> _projectTypes;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private bool _processing = false;

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            _processing = true;
            var response = await ProjectTypeManager.SaveAsync(AddEditProjectTypeModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            _processing = false;
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }
        private async Task LoadDataAsync()
        {
            var result = _isShowingSubProjects
                ? await ProjectTypeManager.GetAllLevelsAsync()
                : await ProjectTypeManager.GetAllAsync();

            if (result.Succeeded)
            {
                _projectTypes = result.Data;

                // إذا هذا Modal للفئة الفرعية أو تعديل، اجعل ParentId مضبوط
                if (_isShowingSubProjects)
                {
                    if (AddEditProjectTypeModel.Id == 0)
                    {
                        // إنشاء فئة فرعية جديدة
                        AddEditProjectTypeModel.ParentId = ParentId;
                    }
                    else
                    {
                        // تعديل فئة موجودة، إذا ParentId فارغ ضعها تلقائيًا
                        if (AddEditProjectTypeModel.ParentId == null)
                            AddEditProjectTypeModel.ParentId = ParentId;
                    }

                }

                StateHasChanged(); // تحديث UI بعد تحميل البيانات
            }
        }

    }
}