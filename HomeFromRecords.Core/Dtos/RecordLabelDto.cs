namespace HomeFromRecords.Core.Dtos {
    public record RecordLabelDto(
        Guid RecordLabelId,
        string RecordLabelName,
        List<Guid> ArtistIds
    );
}
