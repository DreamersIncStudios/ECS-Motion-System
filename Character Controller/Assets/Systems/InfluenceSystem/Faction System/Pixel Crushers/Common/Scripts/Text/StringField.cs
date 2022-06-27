// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// A StringField is an object that can refer to a string, StringAsset, or
    /// field in a TextTable.
    /// </summary>
    [Serializable]
    public class StringField
    {

        [Tooltip("The string that holds the value of this string field. Unused if String Asset or Text Table is assigned.")]
        [SerializeField]
        private string m_text;

        [Tooltip("The String Asset that holds the value of this string field. Unused if Text or Text Table is assigned.")]
        [SerializeField]
        private StringAsset m_stringAsset;

        [Tooltip("The Text Table that holds the value of this string field. Unused if Text or String Asset is assigned.")]
        [SerializeField]
        private TextTable m_textTable;

        [Tooltip("The field ID in the Text Table.")]
        [SerializeField]
        private int m_textTableFieldID;

        /// <summary>
        /// The string that holds the value of this string field. Unused if String Asset or Text Table is assigned.
        /// </summary>
        public string text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        /// <summary>
        /// The String Asset that holds the value of this string field. Unused if Text or Text Table is assigned.
        /// </summary>
        public StringAsset stringAsset
        {
            get { return m_stringAsset; }
            set { m_stringAsset = value; }
        }

        /// <summary>
        /// The Text Table that holds the value of this string field. Unused if Text or String Asset is assigned.
        /// </summary>
        public TextTable textTable
        {
            get { return m_textTable; }
            set { m_textTable = value; }
        }

        /// <summary>
        /// The field ID in the Text Table.
        /// </summary>
        public int textTableFieldID
        {
            get { return m_textTableFieldID; }
            set { m_textTableFieldID = value; }
        }

        /// <summary>
        /// Gets or sets the value of the String Field. If setting, you can only set the text; this property doesn't
        /// change String Assets or Text Tables.
        /// </summary>
        public string value
        {
            get
            {
                if (textTable != null)
                {
                    return Application.isPlaying
                        ? textTable.GetFieldTextForLanguage(textTableFieldID, UILocalizationManager.instance.currentLanguage)
                        : textTable.GetFieldText(textTableFieldID);
                }
                else if (stringAsset != null)
                {
                    return stringAsset.text;
                }
                else
                {
                    return text;
                }
            }
            set
            {
                if (textTable != null)
                {
                    // Do nothing. Don't change assets.
                }
                else if (stringAsset != null)
                {
                    // Do nothing. Don't change assets.
                }
                else
                {
                    text = value;
                }
            }
        }

        public override string ToString()
        {
            return value;
        }

        public StringField()
        {
            this.text = string.Empty;
            this.stringAsset = null;
            this.textTable = null;
            this.textTableFieldID = 0;
        }

        public StringField(string text)
        {
            this.text = text;
            this.stringAsset = null;
            this.textTable = null;
            this.textTableFieldID = 0;
        }

        public StringField(StringAsset stringAsset)
        {
            this.text = string.Empty;
            this.stringAsset = stringAsset;
            this.textTable = null;
            this.textTableFieldID = 0;
        }

        public StringField(TextTable textTable, int fieldID)
        {
            this.text = string.Empty;
            this.stringAsset = null;
            this.textTable = textTable;
            this.textTableFieldID = fieldID;
        }

        public StringField(StringField source)
        {
            this.text = string.Empty;
            this.stringAsset = null;
            this.textTable = null;
            this.textTableFieldID = 0;
            if (source == null) return;
            if (!string.IsNullOrEmpty(source.text))
            {
                this.text = source.text;
            }
            else if (source.stringAsset != null)
            {
                this.stringAsset = source.stringAsset;
            }
            else if (source.textTable != null)
            {
                this.textTable = source.textTable;
                this.textTableFieldID = source.textTableFieldID;
            }
        }

    public override bool Equals(object obj)
        {
            if (obj is StringField)
            {
                return string.Equals(value, (obj as StringField).value);
            }
            else if (obj is StringAsset)
            {
                return string.Equals(value, (obj as StringAsset).text);
            }
            else if (obj is string)
            {
                return string.Equals(value, (obj as string));
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// An empty StringField, similar to string.Empty.
        /// </summary>
        public static readonly StringField empty = new StringField();

        /// <summary>
        /// Similar to string.IsNullOrEmpty.
        /// </summary>
        /// <param name="stringField">The StringField to check.</param>
        /// <returns>true if the StringField is null or empty; otherwise false.</returns>
        public static bool IsNullOrEmpty(StringField stringField)
        {
            return (stringField == null || string.IsNullOrEmpty(stringField.value));
        }

        /// <summary>
        /// Returns the string value of a StringField. This function is null safe.
        /// If the StringField parameter is null, it returns an empty string.
        /// </summary>
        /// <param name="stringField">The StringField whose value to return.</param>
        /// <returns>The string value.</returns>
        public static string GetStringValue(StringField stringField)
        {
            return (stringField == null) ? string.Empty : stringField.value;
        }

    }

}
