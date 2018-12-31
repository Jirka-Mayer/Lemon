﻿using System;
using System.IO;

namespace Convertor.Xml
{
    public struct StringifyOptions
    {
        /// <summary>
        /// Options for default pretty printing
        /// </summary>
        public static StringifyOptions PrettyPrint = new StringifyOptions {
            collapseEmptyTags = true,
            trimStrings = true,
            indentSize = 4,
            currentIndent = 0,
            indentCharacter = " "
        };

        /// <summary>
        /// Should empty tags be collapsed into a non-pair tags
        /// </summary>
        public bool collapseEmptyTags;

        /// <summary>
        /// Should empty tags be collapsed into a non-pair tags
        /// </summary>
        public bool trimStrings;

        /// <summary>
        /// How many characters contains a single indent
        /// </summary>
        public int indentSize;

        /// <summary>
        /// How many indents are piled-up already (how many to add to each line)
        /// </summary>
        public int currentIndent;

        /// <summary>
        /// Which character is used to indent
        /// </summary>
        public string indentCharacter;

        /// <summary>
        /// Do not format the XML in multiline fashion?
        /// </summary>
        public bool Inlined => indentCharacter == null;

        /// <summary>
        /// Prints a newline character and indets properly
        /// </summary>
        public void BreakLine(StreamWriter writer)
        {
            if (Inlined)
                return;

            writer.Write("\n");
            
            for (int i = 0; i < currentIndent; i++)
                for (int j = 0; j < indentSize; j++)
                    writer.Write(indentCharacter);
        }
    }
}
