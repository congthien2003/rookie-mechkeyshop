using Domain.Common;

namespace Domain.Entity
{
    public class ProductImage : BaseEntity
    {
        public string FilePath { get; set; }
        public string Url { get; set; }
        public Guid ProductId { get; set; }
    }
}
