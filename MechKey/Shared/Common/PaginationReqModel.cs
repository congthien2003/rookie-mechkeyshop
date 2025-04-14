namespace Shared.Common
{
    public abstract class PaginationReqModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }

    }
}
