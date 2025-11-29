namespace SchoolV01.Application.Requests.DeviceStatuss
{
    public class GetAllPagedDeviceStatussRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int DeviceId { get; set; }
    }
}