namespace SchoolV01.Application.Requests.Directorates
{
    public class GetAllPagedDirectoratesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}