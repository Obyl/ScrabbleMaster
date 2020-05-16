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
    class ScrabbleGame
    {
        //Variables
        private ScrabbleLetter[] m_scrabbleLetters;
        private int m_tilesLeft;
        private int m_totalTiles;
        private System.Random r = new Random(/*1*/);
        /// <summary>
        /// Set the tiles based on the Scrabble Game
        /// </summary>
        public ScrabbleGame()
        {
            int startingLetter = (char)'A';
            int total = 0;
            //Loop through the letters and determine how many tiles per letter
            for (int i = startingLetter; i < (startingLetter + 26); i++)
            {
                total += ScrabbleLetter.HowManyTiles((char)i);
            }
            //Set variable values and instantiate the array
            m_tilesLeft = total + 2;
            m_totalTiles = total + 2;
            m_scrabbleLetters = new ScrabbleLetter[total + 2];//2 blank tiles!
            int counter = 0;//use to control the index of the array
            //Loop through the letters to create the tiles
            for (int i = startingLetter; i < (startingLetter + 26); i++)
            {
                //Loop through the number of tiles for that letter
                for (int j = 0; j < ScrabbleLetter.HowManyTiles((char)i); j++)
                {
                    m_scrabbleLetters[counter] = new ScrabbleLetter((char)i);
                    counter++;//always increase the counter!
                }
            }
            //add the two blanks
            m_scrabbleLetters[counter] = new ScrabbleLetter(' ');
            counter++;
            m_scrabbleLetters[counter] = new ScrabbleLetter(' ');
        }

        /// <summary>
        /// Gets the next tile
        /// </summary>
        /// <returns>A letter that would be on the next tile</returns>
        public char drawTile()
        {
            char t = '\0';//null value
            //only run if there are tiles to draw
            if (m_tilesLeft > 0)
            {
                //when tiles are used they are set to null
                //randomly pick a tile until you get a valid one
                int index = r.Next(m_totalTiles);

                while (m_scrabbleLetters[index] == null)
                {
                    index = r.Next(m_totalTiles);
                }
                //set the tile to return and remove the tile from the game
                t = m_scrabbleLetters[index].Letter;
                m_scrabbleLetters[index] = null;
            }
            return t;
        }

        /// <summary>
        /// Draws seven tiles at random
        /// </summary>
        /// <returns>A string made up of the letters from the tiles</returns>
        public string drawInitialTiles()
        {
            string temp = "";
            for (int i = 0; i < 7; i++)
            {
                temp += drawTile().ToString();
            }
            return temp;
        }
    }
}