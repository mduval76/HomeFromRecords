using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using HomeFromRecords.Core.Interfaces;
using HomeFromRecords.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeFromRecords.Core.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class RecordLabelController : ControllerBase {
        private readonly IRecordLabel _recordLabelRepos;

        public RecordLabelController(IRecordLabel recordLabelRepos) {
            _recordLabelRepos = recordLabelRepos;
        }

        [HttpGet("id")]
        public async Task<ActionResult<RecordLabelDto>> GetRecordLabelById(Guid labelId) {
            var recordLabelDto = await _recordLabelRepos.GetRecordLabelByIdAsync(labelId);
            return Ok(recordLabelDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<RecordLabelDto>>> GetAllRecordLabels() {
            var recordLabels = await _recordLabelRepos.GetAllRecordLabelsAsync();
            return Ok(recordLabels);
        }

        [HttpGet("artistId")]
        public async Task<ActionResult<IEnumerable<RecordLabelDto>>> GetRecordLabelsByArtistId(Guid artistId) {
            var recordLabels = await _recordLabelRepos.GetRecordLabelsByArtistIdAsync(artistId);
            return Ok(recordLabels);
        }

        // CRUD
        [HttpPost("add")]
        public async Task<IActionResult> AddRecordLabel([FromForm] RecordLabelDto recordLabelSubmit) {
            var newRecordLabel = new RecordLabel {
                RecordLabelId = Guid.NewGuid(),
                RecordLabelName = recordLabelSubmit.RecordLabelName
            };

            await _recordLabelRepos.CreateRecordLabelAsync(newRecordLabel);
            return Ok();
        }

        [HttpPut("update/{recordLabelId}")]
        public async Task<IActionResult> UpdateRecordLabel(Guid recordLabelId, [FromForm] RecordLabelDto recordLabelSubmit) {
            try {
                var updateData = new RecordLabel {
                    RecordLabelName = recordLabelSubmit.RecordLabelName
                };

                var updatedRecordLabel = await _recordLabelRepos.UpdateRecordLabelAsync(recordLabelId, updateData);
                return Ok(updatedRecordLabel);
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete/{recordLabelId}")]
        public async Task<IActionResult> DeleteRecordLabel(Guid recordLabelId) {
            var deletedLabel = await _recordLabelRepos.DeleteRecordLabelAsync(recordLabelId);
            return Ok(deletedLabel);
        }
    }
}
