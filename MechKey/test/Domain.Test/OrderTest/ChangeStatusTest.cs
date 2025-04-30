using Domain.Entity;
using Domain.Enum;

namespace Domain.Test.OrderTest
{
    public class ChangeStatusTest
    {
        [Theory]
        [InlineData(OrderStatus.Accepted)]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.Completed)]
        public void Change_Status_Order_Test(OrderStatus status)
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                TotalAmount = 200,
                Status = OrderStatus.Pending,
            };

            // Act
            order.ChangeStatus(((int)status));

            // Assert
            Assert.Equal(status, order.Status);

        }
    }
}
