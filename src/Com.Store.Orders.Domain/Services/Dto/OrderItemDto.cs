namespace Com.Store.Orders.Domain.Services.Dto
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
