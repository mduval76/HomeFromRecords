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

        // COLLECTION QUERIES
        public async Task<IEnumerable<Album>> GetAlbumsPagedAsync(int page, int itemsPerPage) {
            try {
                return await _context.Albums
                    .OrderBy(a => a.Artist!.ArtistName)
                    .ThenBy(a => a.Title)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving paged albums");
                throw new Exception("An error occurred while retrieving paged albums.");
            }
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync() {
            try {
                return await _context.Albums
                    .OrderBy(a => a.Artist!.ArtistName)
                    .ThenBy(a => a.Title)
                    .ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving all albums");
                throw new Exception("An error occurred while retrieving all albums.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(Guid artistId, int? albumFormat = null) {
            try {
                var query = _context.Albums.Where(a => a.ArtistId == artistId);
                if (albumFormat.HasValue) query = query.Where(a => a.Format == (MainFormat)albumFormat.Value);
                return await query.ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by artist ID {ArtistId}", artistId);
                throw new Exception("An error occurred while retrieving albums by artist.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByCountryAsync(string country) {
            try {
                return await _context.Albums.Where(a => a.Country == country).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by country {Country}", country);
                throw new Exception("An error occurred while retrieving albums by country.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByRecordLabelIdAsync(Guid labelId) {
            try {
                return await _context.Albums.Where(a => a.RecordLabelId == labelId).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by record label ID {LabelId}", labelId);
                throw new Exception("An error occurred while retrieving albums by record label.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByMainFormatAsync(MainFormat mainFormat) {
            try {
                return await _context.Albums.Where(a => a.Format == mainFormat).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by main format {Format}", mainFormat);
                throw new Exception("An error occurred while retrieving albums by main format.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsBySubFormatAsync(SubFormat subFormat) {
            try {
                return await _context.Albums.Where(a => a.SubFormat == subFormat).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by sub format {SubFormat}", subFormat);
                throw new Exception("An error occurred while retrieving albums by sub format.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByGradeAsync(Grade grade) {
            try {
                return await _context.Albums.Where(a => a.MediaGrade == grade || a.SleeveGrade == grade).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by grade {Grade}", grade);
                throw new Exception("An error occurred while retrieving albums by grade.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre) {
            try {
                var artistIds = await _context.Artists
                    .Where(a => a.ArtistGenre == artistGenre)
                    .Select(a => a.ArtistId)
                    .ToListAsync();
                return await _context.Albums.Where(a => artistIds.Contains(a.ArtistId)).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by artist genre {ArtistGenre}", artistGenre);
                throw new Exception("An error occurred while retrieving albums by artist genre.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre) {
            try {
                return await _context.Albums.Where(a => a.AlbumGenre == albumGenre).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by album genre {AlbumGenre}", albumGenre);
                throw new Exception("An error occurred while retrieving albums by album genre.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength) {
            try {
                return await _context.Albums.Where(a => a.AlbumLength == albumLength).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by album length {AlbumLength}", albumLength);
                throw new Exception("An error occurred while retrieving albums by album length.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumTypeAsync(AlbumType albumType) {
            try {
                return await _context.Albums.Where(a => a.AlbumType == albumType).ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by album type {AlbumType}", albumType);
                throw new Exception("An error occurred while retrieving albums by album type.");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade) {
            try {
                return await _context.Albums
                    .Where(a => a.Format == mainFormat && a.MediaGrade == grade)
                    .ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving albums by format {Format} and grade {Grade}", mainFormat, grade);
                throw new Exception("An error occurred while retrieving albums by format and grade.");
            }
        }

        public async Task<IEnumerable<Album>> GetSearchAlbumsAsync(string query, int? albumFormat = null) {
            try {
                var albumsQuery = _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.RecordLabel)
                    .Where(a =>
                        EF.Functions.Like(a.Title, $"%{query}%") ||
                        EF.Functions.Like(a.Details, $"%{query}%") ||
                        (a.Artist != null && EF.Functions.Like(a.Artist.ArtistName, $"%{query}%")) ||
                        (a.RecordLabel != null && EF.Functions.Like(a.RecordLabel.RecordLabelName, $"%{query}%"))
                    );

                if (albumFormat.HasValue)
                    albumsQuery = albumsQuery.Where(a => a.Format == (MainFormat)albumFormat.Value);

                return await albumsQuery.ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error searching albums for query {Query}", query);
                throw new Exception("An error occurred while searching albums.");
            }
        }

        public async Task<IEnumerable<Album>> GetRandomAlbumsAsync(int count) {
            try {
                return await _context.Albums
                    .OrderBy(a => Guid.NewGuid())
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving random albums");
                throw new Exception("An error occurred while retrieving random albums.");
            }
        }

        public async Task<int> GetAlbumCountAsync() {
            try {
                return await _context.Albums.CountAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error counting albums");
                throw new Exception("An error occurred while counting albums.");
            }
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
