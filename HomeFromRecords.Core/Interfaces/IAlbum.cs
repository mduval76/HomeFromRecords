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

        // PAGINATED COLLECTIONS QUERIES
        Task<PaginatedResult<Album>> GetAllAlbumsAsync(int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByArtistIdAsync(Guid artistId, int? albumFormat, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByCountryAsync(string country, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByRecordLabelIdAsync(Guid labelId, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByMainFormatAsync(MainFormat mainFormat, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsBySubFormatAsync(SubFormat subFormat, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByGradeAsync(Grade grade, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByAlbumTypeAsync(AlbumType albumType, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetSearchAlbumsAsync(string query, int? albumFormat, int pageNumber, int pageSize = 12);
        Task<PaginatedResult<Album>> GetRandomAlbumsAsync(int count, int pageNumber, int pageSize = 12);

        // CRUD OPERATIONS
        Task CreateAlbumAsync(Album album);
        Task<Album?> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData);
        Task<Album?> DeleteAlbumAsync(Guid albumId);
    }
}
