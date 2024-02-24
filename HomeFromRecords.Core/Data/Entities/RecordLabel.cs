namespace HomeFromRecords.Core.Data.Entities {
    public class RecordLabel {
        public Guid RecordLabelId { get; set; }
        public string RecordLabelName { get; set; } = "Self-released";
        public List<Artist> Artists { get; set; }
    }
}
