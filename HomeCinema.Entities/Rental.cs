using System;

namespace HomeCinema.Entities
{
    public class Rental : IEntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int StockId { get; set; }
        public virtual Stock Stock { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }
    }
}
