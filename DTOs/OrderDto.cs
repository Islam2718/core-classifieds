namespace SimpleEcoms.DTOs
{
    public class OrderCreateDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
    }
    
    public class OrderItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
    
    public class OrderItemResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}