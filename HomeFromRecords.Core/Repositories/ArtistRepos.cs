using HomeFromRecords.Core.Data;
using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Repositories {
    public class ArtistRepos : IArtist {
        private readonly HomeFromRecordsContext _context;

        public ArtistRepos(HomeFromRecordsContext context) {
            _context = context;
        }

        public async Task<Artist> GetArtistByIdAsync(Guid artistId) {
            try {
                return await _context.Artists
                    .Where(a => a.ArtistId == artistId).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artist by id");
            }
        }

        public async Task<Artist> GetArtistByNameAsync(string artistName) {
            try {
                return await _context.Artists
                    .Where(a => a.ArtistName == artistName).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artist by name");
            }
        }

        public async Task<IEnumerable<Artist>> GetAllArtistsAsync() {
            try {
                return await _context.Artists.ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving all artists");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtistsByIdsAsync(IEnumerable<Guid> artistIds) {
            return await _context.Artists.Where(a => artistIds.Contains(a.ArtistId)).ToListAsync();
        }

        public async Task<IEnumerable<Artist>> GetArtistsByRecordLabelIdAsync(Guid labelId) {
            try {
                var artistIdsInLabel = await _context.Artists
                    .Where(a => a.RecordLabels.Any(l => l.RecordLabelId == labelId))
                    .Select(a => a.ArtistId)
                    .Distinct()
                    .ToListAsync();

                var artistsInLabel = await _context.Artists
                    .Where(a => artistIdsInLabel.Contains(a.ArtistId))
                    .ToListAsync();

                return artistsInLabel;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artists by record label");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtistsByMainFormatAsync(MainFormat albumFormat) {
            try {
                var artistIdsInFormat = await _context.Albums
                    .Where(a => a.Format == albumFormat)
                    .Select(a => a.ArtistId)
                    .Distinct()
                    .ToListAsync();

                var artistsInFormat = await _context.Artists
                    .Where(a => artistIdsInFormat.Contains(a.ArtistId))
                    .ToListAsync();

                return artistsInFormat;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artists by main format");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtistsByGenreAsync(ArtistGenre artistGenre) {
            try {
                var artistIdsInGenre = await _context.Artists
                    .Where(a => a.ArtistGenre == artistGenre)
                    .Select(a => a.ArtistId)
                    .Distinct()
                    .ToListAsync();

                var artistsInGenre = await _context.Artists
                    .Where(a => artistIdsInGenre.Contains(a.ArtistId))
                    .ToListAsync();

                return artistsInGenre;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artists by genre");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtistsByAlbumTypeAsync(AlbumType albumType) {
            try {
                var artistIdsInType = await _context.Albums
                   .Where(a => a.AlbumType == albumType)
                   .Select(a => a.ArtistId)
                   .Distinct()
                   .ToListAsync();

                var artistsInType = await _context.Artists
                    .Where(a => artistIdsInType.Contains(a.ArtistId))
                    .ToListAsync();

                return artistsInType;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving artists by album type");
            }
        }

        // CRUD
        public async Task CreateArtistAsync(Artist artist) {
            try {
                await _context.Artists.AddAsync(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while creating a new artist");
            }
        }

        public async Task<Artist> UpdateArtistAsync(Guid artistId, Artist updateData) {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistId == artistId);

            if (artist == null) {
                throw new KeyNotFoundException($"Artist with ID {artistId} not found.");
            }

            artist.ArtistName = updateData.ArtistName;
            artist.ArtistGenre = updateData.ArtistGenre;

            try {
                await _context.SaveChangesAsync();
                return artist;
            }
            catch (Exception ex) {
                throw new Exception("An error occured while updating artist", ex);
            }
        }

        public async Task<Artist> DeleteArtistAsync(Guid artistId) {
            try {
                var artist = await _context.Artists
                    .Where(a => a.ArtistId == artistId).FirstOrDefaultAsync();

                if (artist != null) {
                    _context.Artists.Remove(artist);
                    await _context.SaveChangesAsync();
                    return artist;
                }
                else {
                    return null;
                }
            }
            catch (Exception) {
                return null;
            }
            
        }
    }
}
