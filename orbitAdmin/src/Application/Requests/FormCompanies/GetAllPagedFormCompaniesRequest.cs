namespace SchoolV01.Application.Requests.FormCompanies
{
    public class GetAllPagedFormCompaniesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}