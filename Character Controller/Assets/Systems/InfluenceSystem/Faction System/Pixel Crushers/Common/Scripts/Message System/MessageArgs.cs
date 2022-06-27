// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// This struct is passed to listeners of the MessageSystem when a message is sent.
    /// </summary>
    [Serializable]
    public struct MessageArgs
    {
        /// <summary>
        /// Reference to the message sender (e.g., GameObject or possibly custom-defined string ID).
        /// </summary>
        public object sender;

        /// <summary>
        /// Reference to the message target (e.g., GameObject or possibly custom-defined string ID).
        /// Typically null or blank string is interpreted as broadcast to all targets.
        /// </summary>
        public object target;

        public string message;

        public string parameter;

        public object[] values;

        /// <summary>
        /// If true, the message arguments specify a target.
        /// </summary>
        public bool hasTarget {
            get
            {
                return !(target == null || string.IsNullOrEmpty(targetString));
            }
        }

        /// <summary>
        /// True if the target value is a string or StringField.
        /// </summary>
        public bool isTargetString {
            get
            {
                var type = (target != null) ? target.GetType() : null;
                return target != null && (type == typeof(string) || type == typeof(StringField));
            }
        }

        /// <summary>
        /// If the target is a string or StringField, its value.
        /// </summary>
        public string targetString {
            get
            {
                if (target == null) return string.Empty;
                var type = target.GetType();
                return (type == typeof(string)) ? (string)target : 
                    ((type == typeof(StringField)) ? StringField.GetStringValue((StringField)target) : string.Empty);
            }
        }

        public MessageArgs(object sender, object target, string message, string parameter, object[] values = null)
        {
            this.sender = sender;
            this.target = target;
            this.message = message;
            this.parameter = parameter;
            this.values = values;
        }

        public MessageArgs(object sender, string message, string parameter, object[] values = null)
        {
            this.sender = sender;
            this.target = null;
            this.message = message;
            this.parameter = parameter;
            this.values = values;
        }

        public bool Matches(string message, string parameter)
        {
            return string.Equals(message, this.message) && (string.IsNullOrEmpty(parameter) || string.Equals(parameter, this.parameter));
        }

        public bool Matches(StringField message, StringField parameter)
        {
            return string.Equals(message.value, this.message) && (StringField.IsNullOrEmpty(parameter) || string.Equals(parameter.value, this.parameter));
        }

        public bool Matches(StringField message, string parameter)
        {
            return string.Equals(message.value, this.message) && (string.IsNullOrEmpty(parameter) || string.Equals(parameter, this.parameter));
        }

        public bool Matches(string message, StringField parameter)
        {
            return string.Equals(message, this.message) && (StringField.IsNullOrEmpty(parameter) || string.Equals(parameter.value, this.parameter));
        }

        /// <summary>
        /// Returns true if the args' sender matches a required sender.
        /// </summary>
        public bool IsRequiredSender(string requiredSender)
        {
            return string.IsNullOrEmpty(requiredSender) || string.Equals(requiredSender, GetSenderString());

        }

        /// <summary>
        /// Returns true if the args' target matches a required target.
        /// </summary>
        public bool IsRequiredTarget(string requiredTarget)
        {
            return string.IsNullOrEmpty(requiredTarget) || string.Equals(requiredTarget, GetTargetString());
        }

        /// <summary>
        /// Returns the string name of the sender.
        /// </summary>
        public string GetSenderString()
        {
            return GetObjectString(sender);
        }

        /// <summary>
        /// Returns the string name of the target.
        /// </summary>
        public string GetTargetString()
        {
            return GetObjectString(target);
        }

        private string GetObjectString(object obj)
        {
            if (obj == null) return string.Empty;
            var type = obj.GetType();
            if (type == typeof(string)) return (string)obj;
            if (type == typeof(StringField)) return StringField.GetStringValue((StringField)obj);
            if (type == typeof(GameObject)) return (obj as GameObject).name;
            if (type == typeof(Component)) return (obj as Component).name;
            return obj.ToString();
        }

        public object firstValue
        {
            get
            {
                return (values != null && values.Length > 0) ? values[0] : null;
            }
        }

        public int intValue
        {
            get
            {
                try
                {
                    return (int)firstValue;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }

}