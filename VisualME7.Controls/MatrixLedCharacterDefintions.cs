//-----------------------------------------------------------------------
// <copyright file="MatrixLedCharacterDefintions.cs" company="David Black">
//      Copyright 2008 David Black
//  
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at
//     
//          http://www.apache.org/licenses/LICENSE-2.0
//    
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace VisualME7.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Definies characters for the MatrixLedCharacter amoungst other things. Definitions are
    /// a character to 5 bytes of data each byte represents a column of 7 leds the MSB is ignored for
    /// now.
    /// </summary>
    public static class MatrixLedCharacterDefinitions
    {
        /// <summary>
        /// Empty square, shows an unknown character
        /// </summary>
        private static readonly byte[] unknownCharacter = { 0x3e, 0x22, 0x22, 0x22, 0x3e, 0 };

        /// <summary>
        /// Datastructure containing the mapping from character to leds
        /// </summary>
        private static Dictionary<string, byte[]> characterDefinitions = new Dictionary<string, byte[]>
        {
            { " ", new byte[] { 0, 0, 0, 0, 0, 0 } }, 
            { "A", new byte[] { 63, 68, 68, 68, 63, 0 } },
            { "B", new byte[] { 127, 73, 73, 73, 54, 0 } },
            { "C", new byte[] { 62, 65, 65, 65, 34, 0 } },
            { "D", new byte[] { 127, 65, 65, 34, 28, 0 } },
            { "E", new byte[] { 127, 73, 73, 73, 65, 0 } },
            { "F", new byte[] { 127, 72, 72, 72, 64, 0 } },
            { "G", new byte[] { 62, 65, 73, 73, 47, 0 } },
            { "H", new byte[] { 127, 8, 8, 8, 127, 0 } }, 
            { "I", new byte[] { 0, 65, 127, 65, 0, 0 } }, 
            { "J", new byte[] { 0, 2, 65, 126, 64, 0 } }, 
            { "K", new byte[] { 127, 8, 20, 34, 65, 0 } },
            { "L", new byte[] { 127, 1, 1, 1, 1, 0 } }, 
            { "M", new byte[] { 127, 32, 16, 32, 127, 0 } },
            { "N", new byte[] { 127, 16, 8, 4, 127, 0 } },
            { "O", new byte[] { 62, 65, 65, 65, 62, 0 } },
            { "P", new byte[] { 127, 72, 72, 72, 48, 0 } },
            { "Q", new byte[] { 62, 65, 69, 66, 61, 0 } },
            { "R", new byte[] { 127, 72, 76, 74, 49, 0 } },
            { "S", new byte[] { 49, 73, 73, 73, 102, 0 } },
            { "T", new byte[] { 64, 64, 127, 64, 64, 0 } },
            { "U", new byte[] { 126, 1, 1, 1, 126, 0 } }, 
            { "V", new byte[] { 124, 2, 1, 2, 124, 0 } }, 
            { "W", new byte[] { 126, 1, 6, 1, 126, 0 } }, 
            { "X", new byte[] { 99, 20, 8, 20, 99, 0 } }, 
            { "Y", new byte[] { 112, 8, 7, 8, 112, 0 } }, 
            { "Z", new byte[] { 67, 69, 73, 81, 97, 0 } },
            { "a", new byte[] { 2, 21, 21, 21, 15, 0 } }, 
            { "b", new byte[] { 127, 9, 17, 17, 14, 0 } },
            { "c", new byte[] { 14, 17, 17, 17, 2, 0 } }, 
            { "d", new byte[] { 14, 17, 17, 9, 127, 0 } },
            { "e", new byte[] { 14, 21, 21, 21, 12, 0 } },
            { "f", new byte[] { 8, 63, 72, 64, 32, 0 } }, 
            { "g", new byte[] { 8, 21, 21, 21, 30, 0 } },
            { "h", new byte[] { 127, 8, 16, 16, 15, 0 } },
            { "i", new byte[] { 0, 17, 95, 1, 0, 0 } }, 
            { "j", new byte[] { 2, 1, 17, 94, 0, 0 } }, 
            { "k", new byte[] { 127, 4, 10, 17, 0, 0 } }, 
            { "l", new byte[] { 0, 65, 127, 1, 0, 0 } }, 
            { "m", new byte[] { 15, 16, 12, 16, 15, 0 } },
            { "n", new byte[] { 31, 8, 16, 16, 15, 0 } }, 
            { "o", new byte[] { 14, 17, 17, 17, 14, 0 } },
            { "p", new byte[] { 31, 20, 20, 20, 8, 0 } }, 
            { "q", new byte[] { 8, 20, 20, 12, 31, 0 } }, 
            { "r", new byte[] { 31, 8, 16, 16, 8, 0 } }, 
            { "s", new byte[] { 8, 21, 21, 21, 2, 0 } }, 
            { "t", new byte[] { 16, 126, 17, 1, 2, 0 } }, 
            { "u", new byte[] { 30, 1, 1, 2, 31, 0 } }, 
            { "v", new byte[] { 28, 2, 1, 2, 28, 0 } }, 
            { "w", new byte[] { 30, 1, 6, 1, 30, 0 } }, 
            { "x", new byte[] { 17, 10, 4, 10, 17, 0 } }, 
            { "y", new byte[] { 0, 25, 5, 5, 30, 0 } }, 
            { "z", new byte[] { 17, 19, 21, 25, 17, 0 } },
            { "!", new byte[] { 0, 0, 121, 0, 0, 0 } }, 
            { "\"", new byte[] { 0, 112, 0, 112, 0, 0 } },
            { "#", new byte[] { 20, 127, 20, 127, 20, 0 } },
            { "$", new byte[] { 18, 42, 127, 42, 36, 0 } },
            { "%", new byte[] { 98, 100, 8, 19, 35, 0 } },
            { "&", new byte[] { 54, 73, 85, 34, 5, 0 } }, 
            { "'", new byte[] { 0, 80, 96, 0, 0, 0 } }, 
            { "(", new byte[] { 0, 28, 34, 65, 0, 0 } }, 
            { ")", new byte[] { 0, 65, 34, 28, 0, 0 } }, 
            { "*", new byte[] { 20, 8, 62, 8, 20, 0 } }, 
            { "+", new byte[] { 8, 8, 62, 8, 8, 0 } }, 
            { ",", new byte[] { 0, 5, 6, 0, 0, 0 } }, 
            { "-", new byte[] { 8, 8, 8, 8, 8, 0 } }, 
            { ".", new byte[] { 0, 3, 3, 0, 0, 0 } }, 
            { "/", new byte[] { 2, 4, 8, 16, 32, 0 } }, 
            { "0", new byte[] { 62, 69, 73, 81, 62, 0 } },
            { "1", new byte[] { 0, 33, 127, 1, 0, 0 } }, 
            { "2", new byte[] { 33, 67, 69, 73, 49, 0 } },
            { "3", new byte[] { 66, 65, 81, 105, 70, 0 } },
            { "4", new byte[] { 8, 24, 40, 127, 8, 0 } }, 
            { "5", new byte[] { 114, 81, 81, 81, 78, 0 } },
            { "6", new byte[] { 30, 41, 73, 73, 6, 0 } }, 
            { "7", new byte[] { 64, 71, 72, 80, 96, 0 } },
            { "8", new byte[] { 54, 73, 73, 73, 54, 0 } },
            { "9", new byte[] { 48, 73, 73, 74, 60, 0 } },
            { ":", new byte[] { 0, 54, 54, 0, 0, 0 } }, 
            { ";", new byte[] { 0, 53, 54, 0, 0, 0 } }, 
            { "<", new byte[] { 8, 20, 34, 65, 0, 0 } }, 
            { "=", new byte[] { 20, 20, 20, 20, 20, 0 } },
            { ">", new byte[] { 65, 34, 20, 8, 0, 0 } }, 
            { "?", new byte[] { 32, 64, 69, 72, 48, 0 } },
            { "@", new byte[] { 38, 73, 79, 65, 62, 0 } },
            { "[", new byte[] { 0, 127, 65, 65, 0, 0 } }, 
            { "]", new byte[] { 0, 65, 65, 127, 0, 0 } }, 
            { "£", new byte[] { 9, 63, 73, 33, 0, 0 } }, 
            { "^", new byte[] { 16, 32, 64, 32, 16, 0 } },
            { "_", new byte[] { 1, 1, 1, 1, 1, 0 } }, 
            { "`", new byte[] { 64, 32, 16, 0, 0, 0 } }, 
            { "{", new byte[] { 0, 8, 54, 65, 0, 0 } }, 
            { "|", new byte[] { 0, 0, 119, 0, 0, 0 } }, 
            { "}", new byte[] { 0, 65, 54, 8, 0, 0 } }, 
        };

        /// <summary>
        /// Gets the data structure containing the mapping from character to leds
        /// </summary>
        public static Dictionary<string, byte[]> CharacterDefinitions
        {
            get { return characterDefinitions; }
        }

        /// <summary>
        /// Retrieves the defintion for a character. unreconized characters are 
        /// rendered as an open rectangle
        /// </summary>
        /// <param name="character">the character to render</param>
        /// <returns>The array of bytes representing vertical sets of on/off setting for LEDS</returns>
        public static byte[] GetDefinition(string character)
        {
            if (CharacterDefinitions.ContainsKey(character))
            {
                return CharacterDefinitions[character];
            }

            return unknownCharacter;
        }
    }
}
