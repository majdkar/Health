namespace SchoolV01.Application.Requests.Devices
{
    public class GetAllPagedDevicesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}