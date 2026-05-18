namespace Presentation;

public class PagedResponse<T>:ApiResponse<T>
{
    
    public int TotalCount;
    public int PageNumber;
    public int PageSize;
    public int TotalPages;
    public bool HasNextPage;
    public bool HasPreviousPage;
}