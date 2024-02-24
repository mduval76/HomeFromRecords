using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Dtos {
    public record ArtistDto(
        Guid ArtistId,
        string ArtistName,
        ArtistGenre ArtistGenre,
        List<Guid> RecordLabelIds
    );
}
