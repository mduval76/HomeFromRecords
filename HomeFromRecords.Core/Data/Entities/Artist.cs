using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Data.Entities {
    public class Artist {
        public Guid ArtistId { get; set; }
        public string ArtistName { get; set; } = "-";
        public ArtistGenre ArtistGenre { get; set; } // Main genre

        public List<RecordLabel> RecordLabels { get; set; }
    }
}
