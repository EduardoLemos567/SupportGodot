using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Support.Extensions;

public static class StringExtensions
{
    public static string TrimStart(this string value, string trim) => value.StartsWith(trim) ? value.Remove(0, trim.Length) : value;
    public static string TrimEnd(this string value, string trim) => value.EndsWith(trim) ? value.Remove(value.Length - trim.Length, trim.Length) : value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FirstLetterUpper(this string value) => char.ToUpper(value[0]) + value[1..].ToLower();
    public static string RemoveAccents(this string word)
    {
        var normalizedString = word.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(word.Length);
        for (var i = 0; i < normalizedString.Length; i++)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                _ = stringBuilder.Append(normalizedString[i]);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}