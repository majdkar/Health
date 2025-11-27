using SchoolV01.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SchoolV01.Client.Shared.Components
{
    public partial class CultureSelector
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public IConfiguration Configuration  { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private Dictionary<string, string> cultures;

        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)JSRuntime;
                    js.InvokeVoid("blazorCulture.set", value.Name);
                    NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
                }
            }
        }

        public void SetCulture(string cultureKey)
        {
            Culture = new CultureInfo(cultureKey);
        }

        protected override void OnInitialized()
        {
            this.cultures = Configuration.GetCulturesSection();
        }
    }
}
