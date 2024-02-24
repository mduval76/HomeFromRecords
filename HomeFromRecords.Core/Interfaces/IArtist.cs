using HomeFromRecords.Core.Data.Entities;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Interfaces {
    public interface IArtist {
        Task<Artist> GetArtistByIdAsync(Guid artistId);
        Task<Artist> GetArtistByNameAsync(string artistName);
        Task<IEnumerable<Artist>> GetAllArtistsAsync();
        Task<IEnumerable<Artist>> GetArtistsByIdsAsync(IEnumerable<Guid> artistIds);
        Task<IEnumerable<Artist>> GetArtistsByRecordLabelIdAsync(Guid labelId);
        Task<IEnumerable<Artist>> GetArtistsByMainFormatAsync(MainFormat albumFormat);
        Task<IEnumerable<Artist>> GetArtistsByGenreAsync(ArtistGenre artistGenre);
        Task<IEnumerable<Artist>> GetArtistsByAlbumTypeAsync(AlbumType albumType);

        // CRUD
        Task CreateArtistAsync(Artist artist);
        Task<Artist> UpdateArtistAsync(Guid artistId, Artist updateData);
        Task<Artist> DeleteArtistAsync(Guid artistId);
    }
}
