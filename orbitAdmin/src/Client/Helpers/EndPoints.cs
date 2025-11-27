using System.Linq;

namespace SchoolV01.Client.Helpers
{
    public static class EndPoints
    {
        public static string GetAllPaged(string mainRoute, int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}";
            if (searchString != "" && orderBy != null)
            {
                url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            }
            else if (searchString == "" && orderBy != null)
            {
                url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}&orderBy=";
            }
            else if (searchString != "" && orderBy == null)
            {
                url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}";
                return url;
            }
            else
            {
                url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}";
            }
            if (orderBy != null)
            {
                //var url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
                if (orderBy?.Any() == true)
                {
                    foreach (var orderByPart in orderBy)
                    {
                        url += $"{orderByPart},";
                    }
                    url = url[..^1];
                }


            }
            return url;

        }


        public static string GetAllPagedByCategoryID(string mainRoute, int pageNumber, int pageSize, string searchString, string[] orderBy, int categoryId)
        {
            var url = $"{mainRoute}?categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}";
            if (searchString != "" && orderBy != null)
            {
                url = $"{mainRoute}?categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            }
            else if (searchString == "" && orderBy != null)
            {
                url = $"{mainRoute}?categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}&orderBy=";
            }
            else if (searchString != "" && orderBy == null)
            {
                url = $"{mainRoute}?categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}";
                return url;
            }
            else
            {
                url = $"{mainRoute}?categoryId={categoryId}&pageNumber={pageNumber}&pageSize={pageSize}";
            }
            if (orderBy != null)
            {
                //var url = $"{mainRoute}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
                if (orderBy?.Any() == true)
                {
                    foreach (var orderByPart in orderBy)
                    {
                        url += $"{orderByPart},";
                    }
                    url = url[..^1];
                }


            }
            return url;

        }

        public static string GetAll(string mainRoute, string searchString, string[] orderBy) //without paging
        {
            var url = $"{mainRoute}";
            if (searchString != "" && orderBy != null)
            {
                url = $"{mainRoute}?searchString={searchString}&orderBy=";
            }
            else if (searchString == "" && orderBy != null)
            {
                url = $"{mainRoute}?orderBy=";
            }
            else if (searchString != "" && orderBy == null)
            {
                url = $"{mainRoute}?searchString={searchString}";
                return url;
            }
            else
            {
                url = $"{mainRoute}";
            }

            if (orderBy != null)
            {
                if (orderBy?.Any() == true)
                {
                    foreach (var orderByPart in orderBy)
                    {
                        url += $"{orderByPart},";
                    }
                    url = url[..^1];
                }
            }

            return url;
        }

        public static string GetAllWithParameter(string mainRoute, string searchString, string[] orderBy) //without paging
        {
            var url = $"{mainRoute}";
            if (searchString != "" && orderBy != null)
            {
                url = $"{mainRoute}&searchString={searchString}&orderBy=";
            }
            else if (searchString == "" && orderBy != null)
            {
                url = $"{mainRoute}&orderBy=";
            }
            else if (searchString != "" && orderBy == null)
            {
                url = $"{mainRoute}&searchString={searchString}";
                return url;
            }
            else
            {
                url = $"{mainRoute}";
            }

            if (orderBy != null)
            {
                if (orderBy?.Any() ?? false)
                {
                    foreach (var orderByPart in orderBy)
                    {
                        url += $"{orderByPart},";
                    }
                    url = url[..^1];
                }
            }

            return url;
        }


        public static string BlockCategories = "api/blockcategories";
        public static string BlockCategoriesSelect = "api/blockcategories/all";
        public static string Blocks = "api/blocks";
        public static string BlocksPhoto = "api/BlockPhoto";
        public static string BlocksSeo = "api/BlockSeo/all";
        public static string BlocksSeoSave = "api/BlockSeo";
        public static string BlocksAttachement = "api/BlockAttachement";

        public static string MenuCategories = "api/v1/menucategories";
        public static string MenuCategoriesSelect = "api/v1/menucategories/all";
        public static string Menus = "api/v1/menus";
        public static string MenusNoCategory = "api/v1/menus/NoCategory";
        public static string MenuSelect = "api/v1/menus/all";




        public static string Pages = "api/pages";
        public static string PagesPhoto = "api/PagePhoto";
        public static string PagesAttachement = "api/PageAttachement";
        public static string PagesSeo = "api/PageSeo/all";
        public static string PagesSeoSave = "api/PageSeo";


        public static string EventCategories = "api/eventcategories";
        public static string EventCategoriesSelect = "api/eventcategories/all";
        public static string Events = "api/events";
        public static string EventsPhoto = "api/EventPhoto";
        public static string EventsAttachement = "api/EventAttachement";

        public static string ArticleCategories = "api/ArticleCategories";
        public static string ArticleCategoriesSelect = "api/articlecategories/all";
        public static string Articles = "api/Articles";
        public static string ArticlesSelect = "api/Articles/all";

        public static string FileUpload = "api/fileupload";

        public static string ResetPassword = "api/resetpassword";

        public static string Statistics = "api/statistics";

        public static string Languages = "api/languages";
        public static string Countries = "api/Countries";
        public static string Classifications = "api/Classifications";
        public static string Cities = "api/Cities";
        public static string Currencies = "api/Currencies";
        public static string Units = "api/Units";
        
        public static string Notifications = "api/v1/Notifications";


        public static string HeadChampionshipId = "api/v1/Championships/GetCupoftheHeadofStateId";
    }
}
