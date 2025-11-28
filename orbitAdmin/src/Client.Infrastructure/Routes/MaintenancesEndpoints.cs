using System;
using System.Linq;

namespace SchoolV01.Client.Infrastructure.Routes
{
    public static class MaintenancesEndpoints
    {

        public static string Endpoints = "api/v1/Maintenances";
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy,int DeviceId)
        {
            var url = $"{Endpoints}?DeviceId={DeviceId}&pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            if (orderBy?.Any() == true)
            {
                foreach (var orderByPart in orderBy)
                {
                    url += $"{orderByPart},";
                }
                url = url[..^1]; // loose training ,
            }
            return url;
        }

        public static string GetAll = $"{Endpoints}/GetAll";
    }
}