using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Dtos {
    public record AlbumDto(
        Guid AlbumId,
        string Title,
        float Price,
        int Quantity,
        string Country,
        string ReleaseYear,
        string CatalogNumber,
        string MatrixNumber,
        string ImgFileExt,
        string Details,
        string ArtistName,
        string RecordLabelName,
        Grade MediaGrade,
        Grade SleeveGrade,
        MainFormat Format,
        SubFormat SubFormat,
        PackageType PackageType,
        VinylSpeed VinylSpeed,
        ArtistGenre ArtistGenre,
        AlbumGenre AlbumGenre,
        AlbumLength AlbumLength,
        AlbumType AlbumType
    );

    public record AlbumUpdateDto(
        string? Title,
        float? Price,
        int? Quantity,
        string? Country,
        string? ReleaseYear,
        string? CatalogNumber,
        string? MatrixNumber,
        string? ImgFileExt,
        string? Details,
        string? ArtistName,
        string? RecordLabelName,
        Grade? MediaGrade,
        Grade? SleeveGrade,
        MainFormat? Format,
        SubFormat? SubFormat,
        PackageType? PackageType,
        VinylSpeed? VinylSpeed,
        ArtistGenre? ArtistGenre,
        AlbumGenre? AlbumGenre,
        AlbumLength? AlbumLength,
        AlbumType? AlbumType
    );

    public record PaginatedResult<T>(
        IEnumerable<T> Items,
        int TotalItems,
        int CurrentPage,
        int ItemsPerPage
    );
}
