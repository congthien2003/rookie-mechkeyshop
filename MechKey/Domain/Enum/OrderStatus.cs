namespace Domain.Enum
{
    public enum OrderStatus
    {
        Pending = 0,       // Đơn hàng vừa được tạo, chưa xử lý
        Accepted = 1,    // Đang xử lý (đóng gói, chuẩn bị)
        Canceled = 2,       // Đã giao cho đơn vị vận chuyển 
    }
}