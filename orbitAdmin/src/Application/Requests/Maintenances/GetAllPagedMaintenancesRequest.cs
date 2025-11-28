namespace SchoolV01.Application.Requests.Maintenances
{
    public class GetAllPagedMaintenancesRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int DeviceId { get; set; }
    }
}