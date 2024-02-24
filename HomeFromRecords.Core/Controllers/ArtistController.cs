using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using HomeFromRecords.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase {
        private readonly IArtist _artistRepos;

        public ArtistController(IArtist artistRepos) {
            _artistRepos = artistRepos;
        }

        [HttpGet("id")]
        public async Task<ActionResult<ArtistDto>> GetArtistById(Guid artistId) {
            var artistDto = await _artistRepos.GetArtistByIdAsync(artistId);
            return Ok(artistDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetAllArtists() {
            var artists = await _artistRepos.GetAllArtistsAsync();
            return Ok(artists);
        }

        [HttpGet("labelId")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistsByRecordLabelId(Guid labelId) {
            var artists = await _artistRepos.GetArtistsByRecordLabelIdAsync(labelId);
            return Ok(artists);
        }

        [HttpGet("format")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistsByMainFormat(MainFormat albumFormat) {
            var artists = await _artistRepos.GetArtistsByMainFormatAsync(albumFormat);
            return Ok(artists);
        }

        [HttpGet("genre")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistsByGenre(ArtistGenre artistGenre) {
            var artists = await _artistRepos.GetArtistsByGenreAsync(artistGenre);
            return Ok(artists);
        }

        [HttpGet("type")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistsByAlbumType(AlbumType albumType) {
            var artists = await _artistRepos.GetArtistsByAlbumTypeAsync(albumType);
            return Ok(artists);
        }

        // CRUD
        [HttpPost("add")]
        public async Task<IActionResult> AddArtist([FromBody] ArtistDto artistSubmit) {
            var newArtist = new Artist {
                ArtistId = Guid.NewGuid(),
                ArtistName = artistSubmit.ArtistName,
                ArtistGenre = artistSubmit.ArtistGenre
            };

            await _artistRepos.CreateArtistAsync(newArtist);
            return Ok(newArtist);
        }

        [HttpPut("update/{artistId}")]
        public async Task<IActionResult> UpdateArtist(Guid artistId, [FromBody] ArtistDto artistSubmit) {
            try {
                var updateData = new Artist {
                    ArtistName = artistSubmit.ArtistName,
                    ArtistGenre = artistSubmit.ArtistGenre
                };

                var updatedArtist = await _artistRepos.UpdateArtistAsync(artistId, updateData);
                return Ok(updatedArtist);
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete/{artistId}")]
        public async Task<IActionResult> DeleteArtist(Guid artistId) {
            var deletedArtist = await _artistRepos.DeleteArtistAsync(artistId);
            return Ok(deletedArtist);
        }
    }
}
