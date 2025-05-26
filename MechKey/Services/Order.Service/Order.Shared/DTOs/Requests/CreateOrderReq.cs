using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Shared.DTOs.Requests
{
    public record class CreateOrderReq
    {
        public decimal TotalAmount { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItemReq> orderItems { get; set; } = new List<OrderItemReq>();
    }
}
