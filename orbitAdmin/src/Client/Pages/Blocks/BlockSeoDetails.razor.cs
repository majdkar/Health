using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared;
using SchoolV01.Shared.ViewModels.Blocks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SchoolV01.Shared.Constants;

using SchoolV01.Client.Shared.Components;
using System.Linq;
using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Shared.Wrapper;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace SchoolV01.Client.Pages.Blocks
{
    public partial class BlockSeoDetails
    {
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public int BlockId { get; set; } = 0;
       

        private BlockSeoUpdateModel BlockModel { get; set; } = new();



        private ClaimsPrincipal _currentUser;


    


        private LanguageSelector languageSelector { get; set; }

        private string noImageUrl = Constants.NOImagePath;

        private bool _processing = false;


        private bool _isAdmin;

        protected async override Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole("Administrator");
        }

        protected override async Task OnParametersSetAsync()
        {
            //Id = Int32.TryParse(Id, out var idInteger) ? Id : "0";
            await LoadBlock(Id);
        }

        private async Task LoadBlock(int id)
        {
            if (id != 0)
            {
                var requestUri = EndPoints.BlocksSeoSave + $"/{id}";
                var response = await _httpClient.GetFromJsonAsync<BlockSeoUpdateModel>(requestUri);
                if (response != null)
                {
                    BlockModel = response;

                    this.StateHasChanged();
                }
            }


        }



        private async Task SaveAsync()
        {

            _processing = true;


            BlockModel.BlockId = BlockId;

            var content = HelperMethods.ToJson(BlockModel);
            HttpResponseMessage response;
            if (BlockModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.BlocksSeoSave, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.BlocksSeoSave}/{BlockModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                await Cancel();
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            _processing = false;
        }

        public async Task Cancel()
        {
            await _jsRuntime.InvokeVoidAsync("history.back", -1);

        }







    }
}