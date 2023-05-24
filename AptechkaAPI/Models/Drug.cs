using System.ComponentModel.DataAnnotations.Schema;

namespace AptechkaAPI
{

    public partial class Drug
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int? ProducerId { get; set; }

        public DateTime? DateOfManufacture { get; set; }

        public DateTime? BestBeforeDate { get; set; }

        [NotMapped]
        public string? ProducerName { get; set; }

    }

}