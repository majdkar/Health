using System;
using System.Linq;

namespace SchoolV01.Client.Infrastructure.Routes
{
    public static class ClinicsEndpoints
    {

        public static string Endpoints = "api/v1/Clinics";
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{Endpoints}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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
        public static string GetAllPagedByDirectorateId(int pageNumber, int pageSize, string searchString, string[] orderBy,int supplierId)
        {
            var url = $"{Endpoints}/ByDirectorateId?supplierId={supplierId}&pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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