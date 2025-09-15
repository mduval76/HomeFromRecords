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
        //private static readonly char[] separator = [' '];

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

        // PAGED COLLECTION QUERIES
        public async Task<(IEnumerable<Album> Albums, int TotalCount)> GetAlbumsPagedAsync(
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
            string? sortBy = null,
            bool ascending = true
        ) {
            try {
                var query = _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.RecordLabel)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchQuery)) {
                    query = query.Where(a =>
                        EF.Functions.Like(a.Title, $"%{searchQuery}%") ||
                        EF.Functions.Like(a.Details, $"%{searchQuery}%") ||
                        (a.Artist != null && EF.Functions.Like(a.Artist.ArtistName, $"%{searchQuery}%")) ||
                        (a.RecordLabel != null && EF.Functions.Like(a.RecordLabel.RecordLabelName, $"%{searchQuery}%"))
                    );
                }

                if (artistId.HasValue)
                    query = query.Where(a => a.ArtistId == artistId.Value);

                if (labelId.HasValue)
                    query = query.Where(a => a.RecordLabelId == labelId.Value);

                if (!string.IsNullOrEmpty(country))
                    query = query.Where(a => a.Country == country);

                if (mainFormat.HasValue)
                    query = query.Where(a => a.Format == mainFormat.Value);

                if (subFormat.HasValue)
                    query = query.Where(a => a.SubFormat == subFormat.Value);

                if (grade.HasValue)
                    query = query.Where(a => a.MediaGrade == grade.Value || a.SleeveGrade == grade.Value);

                if (artistGenre.HasValue)
                    query = query.Where(a => a.Artist != null && a.Artist.ArtistGenre == artistGenre.Value);

                if (albumGenre.HasValue)
                    query = query.Where(a => a.AlbumGenre == albumGenre.Value);

                if (albumLength.HasValue)
                    query = query.Where(a => a.AlbumLength == albumLength.Value);

                if (albumType.HasValue)
                    query = query.Where(a => a.AlbumType == albumType.Value);

                var totalCount = await query.CountAsync();

                query = sortBy?.ToLower() switch {
                    "artist" => ascending ? query.OrderBy(a => a.Artist!.ArtistName) : query.OrderByDescending(a => a.Artist!.ArtistName),
                    "title" => ascending ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title),
                    "price" => ascending ? query.OrderBy(a => a.Price) : query.OrderByDescending(a => a.Price),
                    "year" => ascending ? query.OrderBy(a => a.ReleaseYear) : query.OrderByDescending(a => a.ReleaseYear),
                    _ => query.OrderBy(a => a.Artist!.ArtistName).ThenBy(a => a.Title) // default sort
                };

                var albums = await query
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToListAsync();

                return (albums, totalCount);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving paged albums");
                throw new Exception("An error occurred while retrieving paged albums.");
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
        //private static string NormalizeQuery(string query) {
        //    var parts = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        //    if (parts.Length == 2) {
        //        return $"{parts[1]}, {parts[0]}".ToLower();
        //    }

        //    return query.ToLower();
        //}
    }
}
