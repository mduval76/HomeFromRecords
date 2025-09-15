using HomeFromRecords.Core.Data;
using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using HomeFromRecords.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Repositories {
    public class AlbumRepos(HomeFromRecordsContext context, ILogger<AlbumRepos> logger) : IAlbum {
        private readonly HomeFromRecordsContext _context = context;
        private readonly ILogger<AlbumRepos> _logger = logger;
        private static readonly char[] separator = [' '];

        // SINGLE ITEM QUERIES
        public async Task<Album?> GetAlbumByIdAsync(Guid albumId) {
            try {
                return await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == albumId);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums for AlbumId : {AlbumId}", albumId);
                throw new Exception("An error occurred while retrieving album by ID.");
            }
        }

        public async Task<Album?> GetAlbumByTitleAsync(string title) {
            try {
                return await _context.Albums.FirstOrDefaultAsync(a => a.Title == title);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving album by Title : {Title}", title);
                throw new Exception("An error occurred while retrieving album by title.");
            }
        }

        public async Task<Album?> CheckForDoubles(string artistName, string title, string labelName, MainFormat mainFormat, string country) {
            try {
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistName == artistName);
                var label = await _context.RecordLabels.FirstOrDefaultAsync(r => r.RecordLabelName == labelName);

                if (artist != null && label != null) {
                    return await _context.Albums.FirstOrDefaultAsync(a =>
                         a.ArtistId == artist.ArtistId &&
                         a.Title == title &&
                         a.RecordLabelId == label.RecordLabelId &&
                         a.Format == mainFormat &&
                         a.Country == country);
                }
                return null;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occured while checking for doubles.");
                throw new Exception("An error occurred while checking for doubles.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsPagedAsync(int page, int itemsPerPage) {
            return await _context.Albums
                .OrderBy(a => a.Artist!.ArtistName)
                .ThenBy(a => a.Title)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
        }

        public async Task<IEnumerable<Album>> GetAlbumsPagedAsync(int page, int itemsPerPage) {
            return await _context.Albums
                .OrderBy(a => a.Artist!.ArtistName)
                .ThenBy(a => a.Title)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
        }

        // PAGINATED COLLECTION QUERIES
        private async Task<PaginatedResult<Album>> PaginateAsync(IQueryable<Album> query, int pageNumber, int pageSize) {
            try {
                var totalItems = await query.CountAsync();
                var ordered = query.OrderBy(a => a.Artist.ArtistName).ThenBy(a => a.Title);
                var items = await ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return new PaginatedResult<Album>(items, totalItems, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during pagination.");
                throw new Exception("An error occurred while paginating results.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAllAlbumsAsync(int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.AsQueryable();
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving store catalog.");
                throw new Exception("An error occurred retrieving store catalog.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByArtistIdAsync(Guid artistId, int? albumFormat, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.ArtistId == artistId);

                if (albumFormat.HasValue) {
                    query = query.Where(a => a.Format == (MainFormat)albumFormat.Value);
                }

                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums for ArtistId : {ArtistId}", artistId);
                throw new Exception("An error occurred retrieving albums by artist ID.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByCountryAsync(string country, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.Country == country);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by Country : {Country}", country);
                throw new Exception("An error occurred retrieving albums by country.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByRecordLabelIdAsync(Guid labelId, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.RecordLabelId == labelId);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by RecordLabelId : {RecordLabelId}", labelId);
                throw new Exception("An error occurred retrieving albums by record label ID.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByMainFormatAsync(MainFormat mainFormat, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.Format == mainFormat);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by MainFormat : {Format}", mainFormat);
                throw new Exception("An error occurred retrieving albums by main format.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsBySubFormatAsync(SubFormat subFormat, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.SubFormat == subFormat);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by SubFormat : {SubFormat}", subFormat);
                throw new Exception("An error occurred retrieving albums by sub format.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByGradeAsync(Grade grade, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.MediaGrade == grade || a.SleeveGrade == grade);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by Grade : {Grade}", grade);
                throw new Exception("An error occurred retrieving albums by media grade.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre, int pageNumber, int pageSize = 12) {
            try {
                var artistIds = await _context.Artists
                    .Where(a => a.ArtistGenre == artistGenre)
                    .Select(a => a.ArtistId)
                    .ToListAsync();

                var query = _context.Albums.Where(a => artistIds.Contains(a.ArtistId));
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by ArtistGenre : {ArtistGenre}", artistGenre);
                throw new Exception("An error occurred retrieving albums by artist genre.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.AlbumGenre == albumGenre);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by AlbumGenre : {AlbumGenre}", albumGenre);
                throw new Exception("An error occurred retrieving albums by album genre.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.AlbumLength == albumLength);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by AlbumLength : {AlbumLength}", albumLength);
                throw new Exception("An error occurred retrieving albums by album length.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByAlbumTypeAsync(AlbumType albumType, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.AlbumType == albumType);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by AlbumType : {AlbumType}", albumType);
                throw new Exception("An error occurred retrieving albums by album type.");
            }
        }

        public async Task<PaginatedResult<Album>> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade, int pageNumber, int pageSize = 12) {
            try {
                var query = _context.Albums.Where(a => a.Format == mainFormat && a.MediaGrade == grade);
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving albums by MainFormat : {Format} and Grade : {Grade}", mainFormat, grade);
                throw new Exception("An error occurred retrieving albums by main format and media grade.");
            }
        }

        public async Task<PaginatedResult<Album>> GetSearchAlbumsAsync(string query, int? albumFormat, int pageNumber, int pageSize = 12) {
            try {
                var normalizedQuery = NormalizeQuery(query);

                var albumsQuery = _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.RecordLabel)
                    .Where(a =>
                        EF.Functions.Like(a.Title, $"%{query}%") ||
                        EF.Functions.Like(a.Details, $"%{query}%") || (a.Artist != null && (EF.Functions.Like(a.Artist.ArtistName, $"%{query}%") ||
                        EF.Functions.Like(a.Artist.ArtistName, $"%{normalizedQuery}%"))) || (a.RecordLabel != null && EF.Functions.Like(a.RecordLabel.RecordLabelName, $"%{query}%"))
                    );

                if (albumFormat.HasValue) {
                    albumsQuery = albumsQuery.Where(a => a.Format == (MainFormat)albumFormat.Value);
                }

                return await PaginateAsync(albumsQuery, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error during album search for Query: '{Query}', Format: '{Format}'", query, albumFormat?.ToString() ?? "Any");
                throw new Exception("An error occurred while retrieving albums by search query.");
            }
        }

        public async Task<PaginatedResult<Album>> GetRandomAlbumsAsync(int count, int pageNumber, int pageSize = 12) {
            try {
                var albums = await _context.Albums.OrderBy(a => Guid.NewGuid()).Take(count).ToListAsync();
                var query = albums.AsQueryable();
                return await PaginateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error retrieving random Album(s).");
                throw new Exception("An error occurred retrieving random album(s).");
            }
        }

        public async Task<int> GetAlbumCountAsync() {
            return await _context.Albums.CountAsync();
        }

        // CRUD
        public async Task CreateAlbumAsync(Album album) {
            try {
                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error creating new entry for Album : {Album} by Artist : {Artist}", album.Title, album.Artist?.ArtistName);
                throw new Exception("An error occurred while creating a new album entry.");
            }
        }

        public async Task<Album?> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData) {
            try {
                var album = await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == albumId);

                if (album != null) {
                    album.CatalogNumber = updateData.CatalogNumber ?? album.CatalogNumber;
                    album.MatrixNumber = updateData.MatrixNumber ?? album.MatrixNumber;
                    album.ImgFileExt = updateData.ImgFileExt ?? album.ImgFileExt;
                    album.Country = updateData.Country ?? album.Country;
                    album.Details = updateData.Details ?? album.Details;
                    album.Title = updateData.Title ?? album.Title;
                    album.ReleaseYear = updateData.ReleaseYear ?? album.ReleaseYear;

                    if (updateData.Price.HasValue)
                        album.Price = updateData.Price.Value;

                    if (updateData.Quantity.HasValue)
                        album.Quantity = updateData.Quantity.Value;

                    if (!string.IsNullOrEmpty(updateData.ArtistName)) {
                        var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistName == updateData.ArtistName);
                        if (artist == null) {
                            artist = new Artist { ArtistName = updateData.ArtistName };
                            _context.Artists.Add(artist);
                            await _context.SaveChangesAsync();
                        }
                        album.ArtistId = artist.ArtistId;
                    }

                    if (!string.IsNullOrEmpty(updateData.RecordLabelName)) {
                        var recordLabel = await _context.RecordLabels.FirstOrDefaultAsync(r => r.RecordLabelName == updateData.RecordLabelName);
                        if (recordLabel == null) {
                            recordLabel = new RecordLabel { RecordLabelName = updateData.RecordLabelName };
                            _context.RecordLabels.Add(recordLabel);
                            await _context.SaveChangesAsync();
                        }
                        album.RecordLabelId = recordLabel.RecordLabelId;
                    }

                    if (updateData.ArtistGenre.HasValue) {
                        var artistId = await _context.Albums
                            .Where(a => a.AlbumId == albumId)
                            .Select(a => a.ArtistId)
                            .FirstOrDefaultAsync();

                        var artist = await _context.Artists
                            .FirstOrDefaultAsync(a => a.ArtistId == artistId);

                        if (artist != null) {
                            artist.ArtistGenre = updateData.ArtistGenre.Value;
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (updateData.MediaGrade.HasValue)
                        album.MediaGrade = updateData.MediaGrade.Value;
                    if (updateData.SleeveGrade.HasValue)
                        album.SleeveGrade = updateData.SleeveGrade.Value;
                    if (updateData.Format.HasValue)
                        album.Format = updateData.Format.Value;
                    if (updateData.SubFormat.HasValue)
                        album.SubFormat = updateData.SubFormat.Value;
                    if (updateData.PackageType.HasValue)
                        album.PackageType = updateData.PackageType.Value;
                    if (updateData.VinylSpeed.HasValue)
                        album.VinylSpeed = updateData.VinylSpeed.Value;
                    if (updateData.AlbumGenre.HasValue)
                        album.AlbumGenre = updateData.AlbumGenre.Value;
                    if (updateData.AlbumLength.HasValue)
                        album.AlbumLength = updateData.AlbumLength.Value;
                    if (updateData.AlbumType.HasValue)
                        album.AlbumType = updateData.AlbumType.Value;

                    await _context.SaveChangesAsync();
                }

                return album;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error updating AlbumId : {AlbumID}", albumId);
                throw new Exception("An error occurred updating an album.");
            }
        }

        public async Task<Album?> DeleteAlbumAsync(Guid albumId) {
            try {
                var album = await _context.Albums
                    .Where(a => a.AlbumId == albumId).FirstOrDefaultAsync();

                if (album != null) {
                    _context.Albums.Remove(album);
                    await _context.SaveChangesAsync();
                    return album;
                }
                else {
                    return null;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error deleting AlbumId : {AlbumID}", albumId);
                throw new Exception("An error occurred deleting an album.");
            }
        }

        // HELPER
        private static string NormalizeQuery(string query) {
            var parts = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) {
                return $"{parts[1]}, {parts[0]}".ToLower();
            }

            return query.ToLower();

        }
    }
}
