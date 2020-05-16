/*
 * I McTavish
 * April 12, 2018
 * Code to generate a collection of letters that represent Scrabble tiles
 * Used for the ICS4U unit 3 culminating project
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class ScrabbleLetter
    {
        //Global variables - private
        private char m_letter;
        private int m_numberOfLetters;
        private int m_points;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="L">Character for the Scrabble Tile</param>
        public ScrabbleLetter(char L)
        {
            m_letter = L;
            switch (m_letter)
            {
                case 'J':
                case 'K':
                case 'Q':
                case 'X':
                case 'Z':
                    m_numberOfLetters = 1;
                    break;
                case 'B':
                case 'C':
                case 'F':
                case 'H':
                case 'M':
                case 'P':
                case 'V':
                case 'W':
                case 'Y':
                case ' ':
                    m_numberOfLetters = 2;
                    break;
                case 'G':
                    m_numberOfLetters = 3;
                    break;
                case 'D':
                case 'L':
                case 'S':
                case 'U':
                    m_numberOfLetters = 4;
                    break;
                case 'N':
                case 'R':
                case 'T':
                    m_numberOfLetters = 6;
                    break;
                case 'O':
                    m_numberOfLetters = 8;
                    break;
                case 'A':
                case 'I':
                    m_numberOfLetters = 9;
                    break;
                case 'E':
                    m_numberOfLetters = 12;
                    break;
                default:
                    throw new Exception("This character is not used in Scrabble");
            }
            switch (m_letter)
            {


                case 'Q':
                case 'Z':
                    m_points = 10;
                    break;
                case 'J':
                case 'X':
                    m_points = 8;
                    break;
                case 'K':
                    m_points = 5;
                    break;
                case 'F':
                case 'H':
                case 'V':
                case 'W':
                case 'Y':
                    m_points = 4;
                    break;
                case 'B':
                case 'C':
                case 'M':
                case 'P':
                    m_points = 3;
                    break;
                case 'G':
                case 'D':
                    m_points = 2;
                    break;
                case 'L':
                case 'S':
                case 'U':
                case 'N':
                case 'R':
                case 'T':
                case 'O':
                case 'A':
                case 'I':
                case 'E':
                    m_points = 1;
                    break;
                case ' ':
                    m_points = 0;
                    break;
                default:
                    throw new Exception("This character is not used in Scrabble");
            }
        }
        /// <summary>
        /// Returns the letter on the tile.
        /// </summary>
        public char Letter { get { return m_letter; } }
        /// <summary>
        /// Returns how many tiles in a scrabble board with this letter.
        /// </summary>
        public int NumberOfLetters { get { return m_numberOfLetters; } }
        /// <summary>
        /// How many points the letter is worth
        /// </summary>
        public int Points { get { return m_points; } }
        /// <summary>
        /// Returns how many tiles in scrabble have the letter.
        /// </summary>
        /// <param name="L">The letter.</param>
        /// <returns></returns>
        public static int HowManyTiles(char L)
        {
            ScrabbleLetter s = new ScrabbleLetter(L);
            return s.NumberOfLetters;
        }
    }
}