using HomeFromRecords.Core.Data.Entities;

namespace HomeFromRecords.Core.Interfaces {
    public interface IRecordLabel {
        Task<RecordLabel> GetRecordLabelByIdAsync(Guid recordLabelId);
        Task<RecordLabel> GetRecordLabelByNameAsync(string recordLabelName);

        Task<IEnumerable<RecordLabel>> GetAllRecordLabelsAsync();
        Task<IEnumerable<RecordLabel>> GetRecordLabelsByIdsAsync(IEnumerable<Guid> recordLabelIds);
        Task<IEnumerable<RecordLabel>> GetRecordLabelsByArtistIdAsync(Guid artistId);

        // CRUD
        Task CreateRecordLabelAsync(RecordLabel recordLabel);
        Task<RecordLabel> UpdateRecordLabelAsync(Guid recordLabelId, RecordLabel updateData);
        Task<RecordLabel> DeleteRecordLabelAsync(Guid recordLabelId);
    }
}
