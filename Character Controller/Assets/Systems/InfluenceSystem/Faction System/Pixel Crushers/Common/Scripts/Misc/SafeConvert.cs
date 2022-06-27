// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers
{

    /// <summary>
    /// Conversion methods that return default values instead of throwing exceptions.
    /// </summary>
    public static class SafeConvert
    {

        /// <summary>
        /// Converts a string to an int.
        /// </summary>
        /// <param name="s">Source string.</param>
        /// <returns>int value, or 0 on conversion failure.</returns>
		public static int ToInt(string s)
        {
            int result;
            return int.TryParse(s, out result) ? result : 0;
        }

        /// <summary>
        /// Converts a string to a float.
        /// </summary>
        /// <param name="s">Source string.</param>
        /// <returns>float value, or 0 on conversion failure.</returns>
		public static float ToFloat(string s)
        {
            float result;
            return float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result) ? result : 0;
        }

        private const string CommaTag = "%COMMA%";
        private const string DoubleQuoteTag = "%QUOTE%";
        private const string NewlineTag = "%NEWLINE%";

        /// <summary>
        /// Sanitizes a string so it can be serialized to string-based serialization systems.
        /// </summary>
        /// <param name="s">Source string.</param>
        /// <returns>Sanitized version.</returns>
        public static string ToSerializedElement(string s)
        {
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
            {
                return s.Replace(",", CommaTag).Replace("\"", DoubleQuoteTag).Replace("\n", NewlineTag);
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// Desanitizes a string that was previously sanitized for use in string-based serialization systems.
        /// </summary>
        /// <param name="s">Sanitized version.</param>
        /// <returns>Original source string.</returns>
        public static string FromSerializedElement(string s)
        {
            if (s.Contains(CommaTag) || s.Contains(DoubleQuoteTag) || s.Contains(NewlineTag))
            {
                return s.Replace(CommaTag, ",").Replace(DoubleQuoteTag, "\"").Replace(NewlineTag, "\n");
            }
            else
            {
                return s;
            }
        }

    }

}
