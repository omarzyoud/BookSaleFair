namespace BookSaleFair.api.Domain.DTOs
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
        public string Subject { get; set; }
        public int QuantityAvailable { get; set; }
        public byte[] Cover { get; set; }
    }
}
