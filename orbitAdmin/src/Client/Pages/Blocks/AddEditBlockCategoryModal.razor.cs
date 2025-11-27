using SchoolV01.Shared.ViewModels.Blocks;
using SchoolV01.Client.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolV01.Client.Extensions;
using System.Collections.Generic;

namespace SchoolV01.Client.Pages.Blocks
{
    public partial class AddEditBlockCategoryModal
    {

        [Parameter]
        public BlockCategoryUpdateModel BlockCategoryModel { get; set; } = new();

        private bool _processing=false;
        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }

        List<string> BlockTypes = new List<string> {
            "Blog" ,
            "Link" ,
            "Photo Gallery" ,
            "Video Gallery" ,
            "Home Slider" ,
        };

        public void Cancel()
        {
            MudDialog.Cancel();

        }

        private async Task SaveAsync()
        {
            _processing = true;
            if (string.IsNullOrEmpty(BlockCategoryModel.DescriptionAr))
            {
                BlockCategoryModel.DescriptionAr = "";
            }
            if (string.IsNullOrEmpty(BlockCategoryModel.DescriptionEn))
            {
                BlockCategoryModel.DescriptionEn = "";
            }
            var content = HelperMethods.ToJson(BlockCategoryModel);
                HttpResponseMessage response;
                if (BlockCategoryModel.Id == 0)
                {
                    response = await _httpClient.PostAsync(EndPoints.BlockCategories, content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{EndPoints.BlockCategories}/{BlockCategoryModel.Id}", content);
                }
                if (response.IsSuccessStatusCode)
                {
                    _snackBar.Add("Completed Successful!", Severity.Success);
                    MudDialog.Close();
                }
                else
                {
                    _snackBar.Add("Something went wrong!", Severity.Error);
                }
            _processing = false;
        }

    }
}

