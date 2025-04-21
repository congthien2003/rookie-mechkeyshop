namespace Domain.Enum
{
    public enum OrderStatus
    {
        Pending = 0,       // Đơn hàng vừa được tạo, chưa xử lý
        Accepted = 1,    // Đang xử lý (đóng gói, chuẩn bị)
        Cancelled = 2,
        Completed = 3,// Đơn hàng đã hủy
    }
}