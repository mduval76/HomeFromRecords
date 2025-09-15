using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using Microsoft.SqlServer.Server;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Interfaces {
    public interface IAlbum {
        // SINGLE ITEM QUERIES
        Task<Album?> GetAlbumByIdAsync(Guid albumId);
        Task<Album?> GetAlbumByTitleAsync(string title);
        Task<Album?> CheckForDoubles(string artistName, string title, string labelName, MainFormat mainFormat, string country);

        // PAGED COLLECTION QUERIES
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsPagedAsync(int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByArtistIdAsync(Guid artistId, int page, int itemsPerPage, int? albumFormat = null);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByCountryAsync(string country, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByRecordLabelIdAsync(Guid labelId, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByMainFormatAsync(MainFormat mainFormat, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsBySubFormatAsync(SubFormat subFormat, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByGradeAsync(Grade grade, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByAlbumTypeAsync(AlbumType albumType, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade, int page, int itemsPerPage);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetSearchAlbumsAsync(string query, int page, int itemsPerPage, int? albumFormat = null);
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetRandomAlbumsAsync(int count, int page, int itemsPerPage);
        Task<int> GetAlbumCountAsync();

        // CRUD OPERATIONS
        Task CreateAlbumAsync(Album album);
        Task<Album?> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData);
        Task<Album?> DeleteAlbumAsync(Guid albumId);
    }
}
