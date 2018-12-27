using System;

namespace Convertor.Json
{
    public class JsonString
    {
        /// <summary>
        /// Actual value of the string
        /// </summary>
        public string Value { get; private set; }

        public JsonString(string value)
        {
            this.Value = value;
        }
    }
}
