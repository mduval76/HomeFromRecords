using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Data.Entities {
    public class Album {
        public Guid AlbumId { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string Country { get; set; }
        public string ReleaseYear { get; set; } = "Unknown";
        public string CatalogNumber { get; set; } = string.Empty;
        public string MatrixNumber { get; set; } = string.Empty;
        public string ImgFileExt { get; set; } = string.Empty;  
        public string Details { get; set; } = string.Empty;

        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }

        public Guid RecordLabelId { get; set; }
        public RecordLabel RecordLabel { get; set; }

        public Grade MediaGrade { get; set; } = Grade.UNSPECIFIED;
        public Grade SleeveGrade { get; set; } = Grade.UNSPECIFIED;
        public MainFormat Format { get; set; } // Vinyl, CD, Cassette...
        public SubFormat SubFormat { get; set; } // Format precisions
        public PackageType PackageType { get; set; } = PackageType.UNSPECIFIED; // Gatefold, Digipak...
        public VinylSpeed VinylSpeed { get; set; } = VinylSpeed.INNAPLICABLE; // 33, 45, 78...
        public AlbumGenre AlbumGenre { get; set; } // Genre precisions
        public AlbumLength AlbumLength { get; set; } // Single, EP, LP...
        public AlbumType AlbumType { get; set; } = AlbumType.UNSPECIFIED; // Studio, Live, Compilation...
    }
}
