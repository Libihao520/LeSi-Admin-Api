namespace LeSi.Admin.Contracts.Models.Common;

public class Dtos
{
    /// <summary>
    /// 分页结果DTO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}