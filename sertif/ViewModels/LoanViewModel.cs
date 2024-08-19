namespace sertif.ViewModels
{
    public class LoanViewModel
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuantityAvailable { get; set; }
        public string Reason { get; set; }
        public int QuantityBorrowed { get; set; }
        public DateTime LoanDate { get; set; }
    }
}
