using HomeFromRecords.Core.Data;
using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using HomeFromRecords.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Repositories {
    public class AlbumRepos : IAlbum {
        private readonly HomeFromRecordsContext _context;
        private readonly ILogger<AlbumRepos> _logger;
        private readonly Random _random = new Random();

        public AlbumRepos(HomeFromRecordsContext context, ILogger<AlbumRepos> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<Album?> GetAlbumByIdAsync(Guid albumId) {
            try {
                return await _context.Albums
                    .Where(a => a.AlbumId == albumId).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving album by id");
            }
        }

        public async Task<Album?> GetAlbumByTitleAsync(string title) {
            try {
                return await _context.Albums
                    .Where(a => a.Title.ToLower() == title.ToLower()).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving album by title");
            }
        }

        public async Task<Album> CheckForDoubles(string artistName, string title, string labelName) {
            try {
                var artist = await _context.Artists
                    .Where(a => a.ArtistName.ToLower() == artistName.ToLower())
                    .FirstOrDefaultAsync();

                var label = await _context.RecordLabels
                    .Where(r => r.RecordLabelName.ToLower() == labelName.ToLower())
                    .FirstOrDefaultAsync();

                if (artist != null) {
                    var album = await _context.Albums
                        .Where(a => a.ArtistId == artist.ArtistId && a.Title.ToLower() == title.ToLower() && a.RecordLabelId == label.RecordLabelId)
                        .FirstOrDefaultAsync();

                    return album;
                }

                return null;
            }
            catch (Exception ex) {
                throw new Exception("An error occurred while retrieving the album by artist name and title", ex);
            }
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync() {
            try {
                return await _context.Albums.ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving all albums");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(Guid artistId, int? albumFormat = null) {
            try {
                var albums = _context.Albums
                    .Where(a => a.ArtistId == artistId);

                if (albumFormat.HasValue) {
                    albums = albums.Where(a => a.Format == (MainFormat)albumFormat.Value);
                }

                return await albums.ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by artist");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByCountryAsync(string country) {
            try {
                return await _context.Albums
                    .Where(a => a.Country.ToLower() == country.ToLower()).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by country");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByRecordLabelIdAsync(Guid labelId) {
            try {
                return await _context.Albums
                    .Where(a => a.RecordLabelId == labelId).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by record label");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByMainFormatAsync(MainFormat mainFormat) {
            try {
                return await _context.Albums
                    .Where(a => a.Format == mainFormat).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by main format");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsBySubFormatAsync(SubFormat subFormat) {
            try {
                return await _context.Albums
                    .Where(a => a.SubFormat == subFormat).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by sub format");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByGradeAsync(Grade grade) {
            try {
                return await _context.Albums
                    .Where(a => a.MediaGrade == grade || a.SleeveGrade == grade).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by grade");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByArtistGenreAsync(ArtistGenre artistGenre) {
            try {
                var artistIdsInGenre = await _context.Artists
                    .Where(a => a.ArtistGenre == artistGenre)
                    .Select(a => a.ArtistId)
                    .Distinct()
                    .ToListAsync();

                var albumsInGenre = await _context.Albums
                    .Where(a => artistIdsInGenre.Contains(a.ArtistId))
                    .ToListAsync();

                return albumsInGenre;
            }
            catch (Exception) {
                throw new Exception("An error occurred while retrieving albums by artist genre");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumGenreAsync(AlbumGenre albumGenre) {
            try {
                return await _context.Albums
                    .Where(a => a.AlbumGenre == albumGenre).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by album genre");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumLengthAsync(AlbumLength albumLength) {
            try {
                return await _context.Albums
                    .Where(a => a.AlbumLength == albumLength).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by album length");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByAlbumTypeAsync(AlbumType albumType) {
            try {
                return await _context.Albums
                    .Where(a => a.AlbumType == albumType).ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by album type");
            }
        }

        public async Task<IEnumerable<Album>> GetAlbumsByFormatAndGradeAsync(MainFormat mainFormat, Grade grade) {
            try {
                return await _context.Albums
                    .Where(a => a.Format == mainFormat && (a.MediaGrade == grade))
                    .ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving albums by format and grade");
            }
        }

        public async Task<IEnumerable<Album>> GetSearchAlbumsAsync(string query, int? albumFormat = null) {
            try {
                var lowerQuery = query.ToLower();
                var normalizedQuery = NormalizeQuery(query);

                var albumsQuery = _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.RecordLabel)
                    .Where(a => a.Title.ToLower().Contains(lowerQuery) ||
                                a.Details.ToLower().Contains(lowerQuery) ||
                                (a.Artist != null &&
                                 (a.Artist.ArtistName.ToLower().Contains(lowerQuery) ||
                                  a.Artist.ArtistName.ToLower().Contains(normalizedQuery))) ||
                                (a.RecordLabel != null && a.RecordLabel.RecordLabelName.ToLower().Contains(lowerQuery)));

                if (albumFormat.HasValue) {
                    albumsQuery = albumsQuery.Where(a => a.Format == (MainFormat)albumFormat.Value);
                }

                return await albumsQuery.ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occurred while retrieving albums by search query");
            }
        }

        public async Task<IEnumerable<Album>> GetRandomAlbumsAsync(int count) {
            try {
                var albums = await _context.Albums.ToListAsync();
                var randomAlbums = new List<Album>();

                for (int i = 0; i < count; i++) {
                    var randomIndex = _random.Next(0, albums.Count);
                    randomAlbums.Add(albums[randomIndex]);
                }

                return randomAlbums;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving random albums");
            }
        }

        // CRUD
        public async Task CreateAlbumAsync(Album album) {
            try {
                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while creating a new album");
            }
        }

        public async Task<Album> UpdateAlbumAsync(Guid albumId, AlbumUpdateDto updateData) {
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
            catch (Exception) {
                throw new Exception("An error occurred while updating an album");
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
            catch (Exception) {
                throw new Exception("An error occured while deleting an album");
            }
        }

        // Helper methods
        private string NormalizeQuery(string query) {
            var parts = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) {
                return $"{parts[1]}, {parts[0]}".ToLower();
            }

            return query.ToLower();

        }
    }
}
