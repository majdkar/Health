namespace SchoolV01.Application.Requests.Hospitals
{
    public class GetAllPagedHospitalsRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}