using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Interfaces {
    public interface IAlbum {
        // SINGLE ITEM QUERIES
        Task<Album?> GetAlbumByIdAsync(Guid albumId);
        Task<Album?> GetAlbumByTitleAsync(string title);
        Task<Album?> CheckForDoubles(
            string artistName,
            string title,
            string labelName,
            MainFormat mainFormat,
            string country
        );

        // GENERAL PAGED COLLECTION QU
        Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsPagedAsync(
            int page,
            int itemsPerPage,
            string? searchQuery = null,
            Guid? artistId = null,
            Guid? labelId = null,
            string? country = null,
            MainFormat? mainFormat = null,
            SubFormat? subFormat = null,
            Grade? grade = null,
            ArtistGenre? artistGenre = null,
            AlbumGenre? albumGenre = null,
            AlbumLength? albumLength = null,
            AlbumType? albumType = null,
            string? sortBy = null, // e.g., "Artist", "Title", "Price"
            bool ascending = true
        );

        // CRUD OPERATIONS
        Task CreateAlbumAsync(Album album);
        Task<Album?> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData);
        Task<Album?> DeleteAlbumAsync(Guid albumId);
    }
}
