using HomeFromRecords.Core.Data;
using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeFromRecords.Core.Repositories {
    public class RecordLabelRepos : IRecordLabel {
        private readonly HomeFromRecordsContext _context;

        public RecordLabelRepos(HomeFromRecordsContext context) {
            _context = context;
        }

        public async Task<RecordLabel> GetRecordLabelByIdAsync(Guid recordLabelId) {
            try {
                return await _context.RecordLabels
                    .Where(r => r.RecordLabelId == recordLabelId).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving record label by id");
            }
        }

        public Task<RecordLabel> GetRecordLabelByNameAsync(string recordLabelName) {
            try {
                return _context.RecordLabels
                    .Where(r => r.RecordLabelName == recordLabelName).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving record label by name");
            }
        }

        public async Task<IEnumerable<RecordLabel>> GetAllRecordLabelsAsync() {
            try {
                return await _context.RecordLabels.ToListAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving all record labels");
            }
        }

        public async Task<IEnumerable<RecordLabel>> GetRecordLabelsByIdsAsync(IEnumerable<Guid> recordLabelIds) {
            return await _context.RecordLabels.Where(r => recordLabelIds.Contains(r.RecordLabelId)).ToListAsync();
        }

        public async Task<IEnumerable<RecordLabel>> GetRecordLabelsByArtistIdAsync(Guid artistId) {
            try {
                var labelIdsForArtist = await _context.Artists
                    .Where(a => a.ArtistId == artistId)
                    .SelectMany(a => a.RecordLabels)
                    .Select(l => l.RecordLabelId)
                    .Distinct()
                    .ToListAsync();

                var labelsForArtist = await _context.RecordLabels
                    .Where(l => labelIdsForArtist.Contains(l.RecordLabelId))
                    .ToListAsync();

                return labelsForArtist;
            }
            catch (Exception) {
                throw new Exception("An error occured while retrieving record labels by artist");
            }
        }

        // CRUD
        public async Task CreateRecordLabelAsync(RecordLabel recordLabel) {
            try {
                await _context.RecordLabels.AddAsync(recordLabel);
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                throw new Exception("An error occured while creating record label");
            }
        }

        public async Task<RecordLabel> UpdateRecordLabelAsync(Guid recordLabelId, RecordLabel updateData) {
            var recordLabel = await _context.RecordLabels.FirstOrDefaultAsync(r => r.RecordLabelId == recordLabelId);

            if (recordLabel == null) {
                throw new KeyNotFoundException($"RecordLabel with ID {recordLabelId} not found.");
            }

            recordLabel.RecordLabelName = updateData.RecordLabelName;

            try {
                await _context.SaveChangesAsync();
                return recordLabel;
            }
            catch (Exception ex) {
                throw new Exception("An error occured while updating record label", ex);
            }
        }

        public async Task<RecordLabel> DeleteRecordLabelAsync(Guid recordLabelId) {
            try {
                var recordLabel = await _context.RecordLabels
                    .Where(r => r.RecordLabelId == recordLabelId).FirstOrDefaultAsync();

                if (recordLabel != null) {
                    _context.RecordLabels.Remove(recordLabel);
                    await _context.SaveChangesAsync();
                    return recordLabel;
                }
                else {
                    return null;
                }
            }
            catch (Exception) {
                throw new Exception("An error occured while deleting record label");
            }
        }
    }
}
