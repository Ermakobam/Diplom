namespace IPriziv.Server.DTOs.Income
{
    public class IncomeDispatching
    {
        public DateTime OutDate { get; set; }
        public int OrderNumber { get; set; }
        public byte FoodCount { get; set; }
        public short TeamManagerId { get; set; }
    }
}
