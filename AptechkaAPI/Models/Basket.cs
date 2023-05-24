namespace AptechkaAPI
{

    public sealed class Basket
    {
        public int Id { get; set; }

        public int DrugstoreId { get; set; }

        public List<BasketRow> Rows { get; set; } = null!;
    }

    public sealed class BasketRow
    {
        public int Id { get; set; }

        public int DrugId { get; set; }

        public int Count { get; set; }
    }
}