namespace SchoolV01.Application.Requests.Clinics
{
    public class GetAllPagedClinicsRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}