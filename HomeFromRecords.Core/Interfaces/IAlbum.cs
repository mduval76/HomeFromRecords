using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using Microsoft.SqlServer.Server;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Interfaces {
    public interface IAlbum {
        Task<Album> GetAlbumByIdAsync(Guid albumId);
        Task<Album> GetAlbumByTitleAsync(string title);
        Task<Album> GetAlbumsByArtistNameAndAlbumTitleAsync(string artistName, string title);

        Task<IEnumerable<Album>> GetAllAlbumsAsync();
        Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(Guid artistId, int? albumFormat = null);
        Task<IEnumerable<Album>> GetAlbumsByCountryAsync(string country);
        Task<IEnumerable<Album>> GetAlbumsByRecordLabelIdAsync(Guid labelId);
        Task<IEnumerable<Album>> GetAlbumsByMainFormatAsync(MainFormat mainFormat);
        Task<IEnumerable<Album>> GetAlbumsBySubFormatAsync(SubFormat subFormat);
        Task<IEnumerable<Album>> GetAlbumsByGradeAsync(Grade grade);
        Task<IEnumerable<Album>> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre);
        Task<IEnumerable<Album>> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre);
        Task<IEnumerable<Album>> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength);
        Task<IEnumerable<Album>> GetAlbumsByAlbumTypeAsync(AlbumType albumType);
        Task<IEnumerable<Album>> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade);
        Task<IEnumerable<Album>> GetSearchAlbumsAsync(string query, int? albumFormat = null);
        Task<IEnumerable<Album>> GetRandomAlbumsAsync(int count);

        // CRUD
        Task CreateAlbumAsync(Album album);
        Task<Album> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData);
        Task<Album> DeleteAlbumAsync(Guid albumId);
    }
}
