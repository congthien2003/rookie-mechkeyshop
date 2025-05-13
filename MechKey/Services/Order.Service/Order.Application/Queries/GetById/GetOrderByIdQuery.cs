using MediatR;
using Order.Application.Interfaces;
using Order.Application.Queries.Models;

namespace Order.Application.Queries.GetById
{
    public class GetOrderByIdQuery : IRequest<OrderReadModel>
    {
        public Guid OrderId { get; set; }

        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
    {
        private readonly IReadOrderService _readOrderService;

        public GetOrderByIdQueryHandler(IReadOrderService readOrderService)
        {
            _readOrderService = readOrderService;
        }

        public async Task<OrderReadModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _readOrderService.GetByIdAsync(request.OrderId.ToString());

            return result;
        }
    }


}
