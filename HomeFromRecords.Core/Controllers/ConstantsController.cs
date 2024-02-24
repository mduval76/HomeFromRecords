using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Azure.Core.HttpHeader;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConstantsController : ControllerBase {
        [HttpGet("enums")]
        public IActionResult GetEnums() {
            var formattedEnums = new {
                Grades = GetEnumValues<Grade>(),
                MainFormats = GetEnumValues<MainFormat>(),
                SubFormats = GetEnumValues<SubFormat>(),
                PackageTypes = GetEnumValues<PackageType>(),
                VinylSpeeds = GetEnumValues<VinylSpeed>(),
                AlbumLengths = GetEnumValues<AlbumLength>(),
                AlbumTypes = GetEnumValues<AlbumType>(),
                AlbumGenres = GetEnumValues<AlbumGenre>(),
                ArtistGenres = GetEnumValues<ArtistGenre>()
            };

            return Ok(formattedEnums);
        }

        // Helper methods
        private object[] GetEnumValues<T>() where T : Enum {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new {
                           name = GetTransformedName(e.ToString()),
                           value = Convert.ToInt32(e)
                       })
                       .ToArray();
        }

        private string GetTransformedName(string name) {
            var transformations = new Dictionary<string, string> {
                { "TWELVE_INCH", "12\"" },
                { "TEN_INCH", "10\"" },
                { "SEVEN_INCH", "7\"" },
                { "THREE_INCH", "3\"" },
                { "CD", "CD" },
                { "CD_R", "CD-R" },
                { "DVD", "DVD" },
                { "BLURAY", "Blu-ray" },
                { "EIGHT_TRACK", "8-track" },
                { "PICTURE_DISC", "Picture-Disc"},
                { "NM", "NM" },
                { "VG_PLUS", "VG+" },
                { "VG", "VG" },
                { "G_PLUS", "G+" },
                { "EP", "EP" },
                { "LP", "LP" },
                { "PVC", "PVC" },
                { "THIRTY_THREE", "33rpm" },
                { "FORTY_FIVE", "45rpm" },
                { "SEVENTY_EIGHT", "78rpm"}
            };

            if (transformations.TryGetValue(name, out var transformedName)) {
                return transformedName;
            }
            else {
                return CapitalizeWords(name.Replace("_", " "));
            }
        }

        private string CapitalizeWords(string value) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }
    }
}
