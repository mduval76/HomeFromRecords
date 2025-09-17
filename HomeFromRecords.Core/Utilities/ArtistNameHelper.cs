using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeFromRecords.Core.Utilities {
    public static class ArtistNameHelper {

        // DB => UI
        public static string FormatForUI(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var rawArtists = Regex.Split(input, @"\s-\s")
                                  .Select(a => a.Trim())
                                  .Where(a => a.Length > 0)
                                  .ToList();

            var formattedArtists = rawArtists.Select(FormatSingleArtistForUI).ToList();

            if (formattedArtists.Count == 1)
                return formattedArtists[0];
            if (formattedArtists.Count == 2)
                return $"{formattedArtists[0]} & {formattedArtists[1]}";

            return string.Join(", ", formattedArtists.Take(formattedArtists.Count - 1))
                   + " & " + formattedArtists.Last();
        }

        private static string FormatSingleArtistForUI(string artist) {
            if (string.IsNullOrWhiteSpace(artist))
                return artist;

            if (Regex.IsMatch(artist, @"\bAnd\b", RegexOptions.IgnoreCase))
                return artist;

            var partsComma = artist.Split(',')
                                   .Select(p => p.Trim())
                                   .Where(p => p.Length > 0)
                                   .ToList();

            if (partsComma.Count == 1)
                return artist;

            if (partsComma.Count == 2) {
                var left = partsComma[0]; 
                var right = partsComma[1];

                if (Regex.IsMatch(right, @"^(The|Le|La|El|Los|Les|De|Von)$", RegexOptions.IgnoreCase))
                    return $"{right} {left}";

                if (Regex.IsMatch(left, @"('s|’s)$", RegexOptions.IgnoreCase)) {
                    var rightTokens = right.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (rightTokens.Length == 0)
                        return $"{right} {left}";
                    var first = rightTokens[0];
                    var rest = rightTokens.Length > 1 ? " " + string.Join(" ", rightTokens.Skip(1)) : string.Empty;
                    return $"{first} {left}{rest}";
                }

                return $"{right} {left}";
            }

            if (partsComma.Count >= 3) {
                var lastPart = partsComma.Last();

                if (Regex.IsMatch(lastPart, @"^(The|Le|La|El|Los|Les|De|Von)$", RegexOptions.IgnoreCase)) {
                    var lastName = partsComma[0];
                    var middleParts = partsComma.Skip(1).Take(partsComma.Count - 2).ToList();
                    var firstMiddle = middleParts.Count > 0 ? middleParts[0] : string.Empty;
                    var fmTokens = firstMiddle.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                    string constructedFirstMiddle;
                    if (fmTokens.Count == 0) {
                        constructedFirstMiddle = lastName;
                    }
                    else {
                        var fmFirst = fmTokens[0];
                        var fmRest = fmTokens.Count > 1 ? " " + string.Join(" ", fmTokens.Skip(1)) : string.Empty;
                        constructedFirstMiddle = $"{fmFirst} {lastName}{fmRest}";
                    }

                    if (middleParts.Count > 1) {
                        constructedFirstMiddle += " " + string.Join(" ", middleParts.Skip(1));
                    }

                    return $"{lastPart} {constructedFirstMiddle}";
                }

                var middleCombined = string.Join(" ", partsComma.Skip(1));
                var midTokens = middleCombined.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (midTokens.Count == 0)
                    return artist;

                var firstTok = midTokens[0];
                var restTok = midTokens.Count > 1 ? " " + string.Join(" ", midTokens.Skip(1)) : string.Empty;
                return $"{firstTok} {partsComma[0]}{restTok}";
            }

            return artist;
        }

        // UI => DB
        public static string FormatForDB(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var parts = SplitArtists(input);

            var dbArtists = parts.Select(FormatSingleArtistForDB).ToList();

            if (dbArtists.Count == 1)
                return dbArtists[0];

            return string.Join(" - ", dbArtists);
        }

        private static string FormatSingleArtistForDB(string artist) {
            if (string.IsNullOrWhiteSpace(artist))
                return artist;

            if (Regex.IsMatch(artist, @"\bAnd\b", RegexOptions.IgnoreCase))
                return artist;

            var tokens = artist.Split(' ')
                               .Where(t => !string.IsNullOrWhiteSpace(t))
                               .ToList();

            if (tokens.Count == 1)
                return artist;

            if (Regex.IsMatch(tokens[0], @"^(The|Le|La|El|Los|Les|De|Von)$", RegexOptions.IgnoreCase)) {
                string article = tokens[0];
                string rest = string.Join(" ", tokens.Skip(1));
                return $"{rest}, {article}";
            }

            var possIndex = tokens.FindIndex(t => Regex.IsMatch(t, @"('s|’s)$", RegexOptions.IgnoreCase));
            if (possIndex > 0) {
                var last = tokens[possIndex];
                var first = string.Join(" ", tokens.Take(possIndex));
                var rest = string.Join(" ", tokens.Skip(possIndex + 1));
                var right = string.IsNullOrWhiteSpace(rest) ? first : $"{first} {rest}";
                return $"{last}, {right}";
            }

            string lastToken = tokens.Last();
            string firstTokens = string.Join(" ", tokens.Take(tokens.Count - 1));
            return $"{lastToken}, {firstTokens}";
        }

        private static List<string> SplitArtists(string input) {
            var result = new List<string>();

            var temp = input.Split('&')
                            .Select(s => s.Trim())
                            .ToList();

            if (temp.Count == 1) {
                result = input.Split(',')
                              .Select(s => s.Trim())
                              .Where(s => s.Length > 0)
                              .ToList();
            }
            else {
                var beforeAnd = temp[0].Split(',')
                                       .Select(s => s.Trim())
                                       .Where(s => s.Length > 0);
                var afterAnd = temp[1].Trim();

                result = beforeAnd.ToList();
                result.Add(afterAnd);
            }

            return result;
        }
    }
}
