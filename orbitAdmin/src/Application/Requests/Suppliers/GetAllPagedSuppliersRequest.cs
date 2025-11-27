namespace SchoolV01.Application.Requests.Suppliers
{
    public class GetAllPagedSuppliersRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}