using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using HomeFromRecords.Core.Interfaces;
using HomeFromRecords.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AlbumController : ControllerBase {
        private readonly IAlbum _albumRepos;
        private readonly IArtist _artistRepos;
        private readonly IRecordLabel _recordLabelRepos;
        private readonly IConfiguration _config;
        private readonly string _targetFolderPath;
        private readonly string _baseUrl;

        public AlbumController(IAlbum albumRepos, IArtist artistRepos, IRecordLabel recordLabelRepos, IWebHostEnvironment env, IConfiguration config) {
            _albumRepos = albumRepos;
            _targetFolderPath = Path.Combine(env.ContentRootPath, "Uploads", "Images", "AlbumCovers");
            _artistRepos = artistRepos;
            _recordLabelRepos = recordLabelRepos;
            _config = config;
            _baseUrl = _config.GetValue<string>("AppSettings:BaseUrl");
        }

        [HttpGet("id")]
        public async Task<ActionResult<AlbumDto>> GetAlbumById(Guid albumId) {
            var albumDto = await _albumRepos.GetAlbumByIdAsync(albumId);
            return Ok(albumDto);
        }

        [HttpGet("title")]
        public async Task<ActionResult<AlbumDto>> GetAlbumByTitle(string title) {
            var albumDto = await _albumRepos.GetAlbumByTitleAsync(title);
            return Ok(albumDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAllAlbums() {
            var albums = await _albumRepos.GetAllAlbumsAsync();
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("artistId")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByArtistId(Guid artistId, int? albumFormat = null) {
            var albums = await _albumRepos.GetAlbumsByArtistIdAsync(artistId, albumFormat);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("country")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByCountry(string country) {
            var albums = await _albumRepos.GetAlbumsByCountryAsync(country);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("labelId")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByRecordLabelId(Guid labelId) {
            var albums = await _albumRepos.GetAlbumsByRecordLabelIdAsync(labelId);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("format")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByMainFormat(MainFormat albumFormat) {
            var albums = await _albumRepos.GetAlbumsByMainFormatAsync(albumFormat);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("artistGenre")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByArtistGenre(ArtistGenre artistGenre) {
            var albums = await _albumRepos.GetAlbumsByArtistGenreAsync(artistGenre);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("albumGenre")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByAlbumGenre(AlbumGenre albumGenre) {
            var albums = await _albumRepos.GetAlbumsByAlbumGenreAsync(albumGenre);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("length")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByAlbumLength(AlbumLength albumLength) {
            var albums = await _albumRepos.GetAlbumsByAlbumLengthAsync(albumLength);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("type")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByAlbumType(AlbumType albumType) {
            var albums = await _albumRepos.GetAlbumsByAlbumTypeAsync(albumType);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("formatGrade")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByFormatAndGrade(MainFormat mainFormat, Grade grade) {
            var albums = await _albumRepos.GetAlbumsByFormatAndGradeAsync(mainFormat, grade);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetSearchAlbums(string query, int? albumFormat = null) {
            var albums = await _albumRepos.GetSearchAlbumsAsync(query, albumFormat);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        [HttpGet("random")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetRandomAlbums(int count) {
            var albums = await _albumRepos.GetRandomAlbumsAsync(count);
            var albumDtos = await AssignPropertiesInCollection(albums);
            return Ok(albumDtos);
        }

        // CRUD
        [HttpPost("add")]
        public async Task<IActionResult> AddAlbum([FromForm] AlbumDto albumSubmit, IFormFile file) {
            if (file == null || file.Length == 0) {
                return BadRequest("No file was uploaded.");
            }

            try {
                var artist = await CheckExistingArtist(albumSubmit);
                var recordLabel = await CheckExistingRecordLabel(albumSubmit.RecordLabelName);

                var existingAlbum = await _albumRepos.CheckForDoubles(artist.ArtistName, albumSubmit.Title, recordLabel.RecordLabelName, albumSubmit.Format, albumSubmit.Country);
                if (existingAlbum != null) {
                    return BadRequest("Album already exists.");
                }

                var newAlbum = new Album {
                    AlbumId = Guid.NewGuid(),
                    Title = albumSubmit.Title,
                    Price = albumSubmit.Price,
                    Quantity = albumSubmit.Quantity,
                    Country = albumSubmit.Country,
                    ReleaseYear = albumSubmit.ReleaseYear,
                    CatalogNumber = albumSubmit.CatalogNumber,
                    MatrixNumber = albumSubmit.MatrixNumber,
                    Details = albumSubmit.Details,
                    ArtistId = artist.ArtistId,
                    RecordLabelId = recordLabel.RecordLabelId,
                    MediaGrade = albumSubmit.MediaGrade,
                    SleeveGrade = albumSubmit.SleeveGrade,
                    Format = albumSubmit.Format,
                    SubFormat = albumSubmit.SubFormat,
                    PackageType = albumSubmit.PackageType,
                    VinylSpeed = albumSubmit.VinylSpeed,
                    AlbumGenre = albumSubmit.AlbumGenre,
                    AlbumLength = albumSubmit.AlbumLength,
                    AlbumType = albumSubmit.AlbumType
                };

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_targetFolderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                newAlbum.ImgFileExt = fileName;

                await _albumRepos.CreateAlbumAsync(newAlbum);
                return Ok((new { message = "Album and image uploaded successfully" }));
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding album");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAlbum(Guid albumId, [FromForm] AlbumUpdateDto albumUpdateDto, IFormFile? file) {
            var existingAlbum = await _albumRepos.GetAlbumByIdAsync(albumId);
            if (existingAlbum == null) {
                return NotFound($"Album with ID {albumId} not found.");
            }

            string? fileName = null;
            if (file != null && file.Length > 0) {
                fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_targetFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
            }

            if (!string.IsNullOrEmpty(albumUpdateDto.ImgFileExt) && existingAlbum.ImgFileExt != albumUpdateDto.ImgFileExt) {
                var oldFilePath = Path.Combine(_targetFolderPath, existingAlbum.ImgFileExt);
                var newFilePath = Path.Combine(_targetFolderPath, albumUpdateDto.ImgFileExt);

                if (System.IO.File.Exists(oldFilePath)) {
                    System.IO.File.Move(oldFilePath, newFilePath);
                }
            }

            var updatedDto = albumUpdateDto with { ImgFileExt = fileName ?? albumUpdateDto.ImgFileExt };
            var updatedAlbum = await _albumRepos.UpdateAlbumAsync(albumId, updatedDto);
            return Ok(updatedAlbum);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAlbum(Guid albumId) {
            var deletedAlbum = await _albumRepos.DeleteAlbumAsync(albumId);
            return Ok(deletedAlbum);
        }

        // Helper methods
        private async Task<IEnumerable<AlbumDto>> AssignPropertiesInCollection(IEnumerable<Album> albums) {
            var artistIds = albums.Select(a => a.ArtistId).Distinct();
            var recordLabelIds = albums.Select(a => a.RecordLabelId).Distinct();

            var artists = await _artistRepos.GetArtistsByIdsAsync(artistIds);
            var recordLabels = await _recordLabelRepos.GetRecordLabelsByIdsAsync(recordLabelIds);

            var artistDict = artists.ToDictionary(a => a.ArtistId, a => a);
            var recordLabelDict = recordLabels.ToDictionary(r => r.RecordLabelId, r => r);

            var albumDtos = albums.Select(album => {
                var artist = artistDict[album.ArtistId];
                var recordLabel = recordLabelDict[album.RecordLabelId];
                return new AlbumDto(
                    album.AlbumId,
                    album.Title,
                    album.Price,
                    album.Quantity,
                    album.Country,
                    album.ReleaseYear,
                    album.CatalogNumber,
                    album.MatrixNumber,
                    GetImgUrl(album.ImgFileExt),
                    album.Details,
                    artist.ArtistName,
                    recordLabel.RecordLabelName,
                    album.MediaGrade,
                    album.SleeveGrade,
                    album.Format,
                    album.SubFormat,
                    album.PackageType,
                    album.VinylSpeed,
                    artist.ArtistGenre,
                    album.AlbumGenre,
                    album.AlbumLength,
                    album.AlbumType
                );
            }).ToList();

            return albumDtos;
        }

        private async Task<Artist> CheckExistingArtist(AlbumDto albumSubmit) {
            var artist = await _artistRepos.GetArtistByNameAsync(albumSubmit.ArtistName);
            if (artist == null) {
                artist = new Artist {
                    ArtistId = Guid.NewGuid(),
                    ArtistName = albumSubmit.ArtistName,
                    ArtistGenre = albumSubmit.ArtistGenre
                };
                await _artistRepos.CreateArtistAsync(artist);
            }
            return artist;
        }

        private async Task<RecordLabel> CheckExistingRecordLabel(string recordLabelName) {
            var recordLabel = await _recordLabelRepos.GetRecordLabelByNameAsync(recordLabelName);
            if (recordLabel == null) {
                recordLabel = new RecordLabel {
                    RecordLabelId = Guid.NewGuid(),
                    RecordLabelName = recordLabelName
                };
                await _recordLabelRepos.CreateRecordLabelAsync(recordLabel);
            }
            return recordLabel;
        }

        private string GetImgUrl(string fileName) {
            return $"{_baseUrl}/Images/{fileName}";
        }
    }
}