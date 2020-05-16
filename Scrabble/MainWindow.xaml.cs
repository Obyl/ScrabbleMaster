using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrabble
{
    public partial class MainWindow : Window
    {
        // Testing config.
        private const bool TEST_MODE = false;
        private const int TEST_TRIALS = 1000;

        // Instance of provided ScrabbleGame class.
        private ScrabbleGame game;

        // Letters of 7 initial scrabble tiles.
        private string letters;

        // Variables used to calculate words and word points.
        private int[] charCodes = new int[26];
        private int[] charPoints = new int[26];
        private byte blanks = 0;

        // Control of word output.
        private Label output;
        private int rowCounter;
        private int maxRows = 10;

        /// <summary>
        /// Program initialization.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (TEST_MODE)
                RunPerformanceTest();
            else
                NewGame();
        }

        /// <summary>
        /// Reset all and create a new game.
        /// </summary>
        private void NewGame()
        {
            // Setup game.
            game = new ScrabbleGame();
            letters = game.drawInitialTiles();

            // Setup GUI.
            CreateTileGraphics();
            words.Children.Clear();
            rowCounter = 0;
            RefreshOutput();

            // Execute program.
            GenerateFile();
            GenerateDictionaries();
            CalculateWords();
        }

        /// <summary>
        /// Runs a speed test of the NewGame() function.
        /// </summary>
        private void RunPerformanceTest()
        {
            StreamWriter writer = new StreamWriter("test_data.txt");

            Stopwatch sw = new Stopwatch();

            for(int i = 0; i < TEST_TRIALS; i++)
            {
                sw.Reset();
                sw.Start();
                NewGame();
                sw.Stop();

                writer.WriteLine(sw.Elapsed.ToString());
            }

            writer.Close();
            MessageBox.Show("Performance test complete.");
        }

        /// <summary>
        /// Displays the random generated Scrabble tiles.
        /// </summary>
        private void CreateTileGraphics()
        {
            tileCanvas.Children.Clear();

            for(int i = 0; i < letters.Length; i++)
            {
                Label tileChar = new Label();
                tileChar.Width = 70;
                tileChar.Height = 70;
                tileChar.Content = letters[i];
                tileChar.FontSize = 40;
                tileChar.BorderBrush = Brushes.Black;
                tileChar.BorderThickness = new Thickness(3);
                
                Canvas.SetLeft(tileChar, (i * 105) + 40);
                Canvas.SetTop(tileChar, 15);
                
                tileCanvas.Children.Add(tileChar);
            }
        }

        /// <summary>
        /// Adds a new column of words to the output.
        /// </summary>
        private void RefreshOutput()
        {
            output = new Label();
            output.FontSize = 14;

            words.Children.Add(output);
        }

        /// <summary>
        /// Generates the file containing the words for the program to search for.
        /// Original file obtained online before unnecessary words are eliminated.
        /// Makes words all uppercase.
        /// </summary>
        private void GenerateFile()
        {
            // No need to create the file if it already exists.
            if (File.Exists("words.txt"))
                return;

            try
            {
                StreamReader reader = new StreamReader(new WebClient().OpenRead("http://darcy.rsgc.on.ca/ACES/ICS4U/SourceCode/Words.txt"));
                StreamWriter writer = new StreamWriter("words.txt");

                string lastLine = "";
                while (!reader.EndOfStream)
                {
                    // .ToUpper() makes sure everything is uppercase.
                    string line = reader.ReadLine().ToUpper();

                    /*
                     * Only include line in new file if: 
                     *     a) it is less than 8 characters long (only seven tiles to work with) 
                     *     b) was not included in the last pass (to exclude words identical excluding capitalization)
                     */
                    if (line.Length > 1 && line.Length < 8 && lastLine != line)
                        writer.WriteLine(line);

                    lastLine = line;
                }

                reader.Close();
                writer.Close();
            }catch(Exception e)
            {
                resultBest.Content = "Failed to load content from the internet.";
                output.Content = "Failed with error message: " + e.Message;
            }
        }

        /// <summary>
        /// Generates the dictionaries needed to calculate the words.
        /// 
        /// charCodes assigns one prime number to each letter in the tileset.
        /// charPoints associates each letter with its number of points (obtained from ScrabbleLetter)
        /// </summary>
        private void GenerateDictionaries()
        {
            // Clear previous values.
            for(int i = 0; i < charCodes.Length; i++)
            {
                charCodes[i] = 0;
                charPoints[i] = 0;
            }
            blanks = 0;

            // First seven prime numbers. No more than these will ever be needed.
            int[] primes = { 2, 3, 5, 7, 11, 13, 17 };
            for (int i = 0; i < letters.Length; i++)
            {
                // If the letter is a blank, ignore it and simply add to the blanks counter.
                if (letters[i] == ' ')
                {
                    blanks++;
                }
                else
                {
                    charCodes[letters[i] - 65] = primes[i];
                    charPoints[letters[i] - 65] = new ScrabbleLetter(letters[i]).Points;
                }
            }
        }

        /// <summary>
        /// Calculate all the words that can be made with the tileset.
        /// Also calculate word with highest points.
        /// </summary>
        private void CalculateWords()
        {
            // Word ID of the tileset.
            int lettersId = GetWordData(letters) >> 6;

            int mostPoints = 0;
            string mostPointsWord = "";

            StreamReader reader = new StreamReader("words.txt");
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                // Get and decompress word ID and points for current line.
                int data = GetWordData(line);
                int id = data >> 6;

                // If ID <= 0 there is a 0% chance the tileset can contain the word.
                if(id > 0)
                {
                    /*
                     * If the tileset's ID can be divided by the line's ID and leave
                     * no remainder, than the line is contained within the tileset.
                     * 
                     * Note that at this point most words have been discarded due to their
                     * ID being 0 (aka they contain a letter not in the tileset).
                     * This operation is only to prune out words with letters all in the tileset
                     * but that cannot be created. For example, if the word has two As but the
                     * tileset only has one.
                     */
                    if(((double)lettersId / (double)id) % 1 == 0)
                    {
                        output.Content += line + Environment.NewLine;
                        rowCounter++;
                        if(rowCounter >= maxRows)
                        {
                            rowCounter = 0;
                            RefreshOutput();
                        }

                        // Keep track of word with most points.
                        int points = data & 0b111111;
                        if (points > mostPoints)
                        {
                            mostPoints = points;
                            mostPointsWord = line;
                        }
                    }
                }
            }
            reader.Close();

            if (mostPointsWord == "")
                resultBest.Content = "No words could be made with the tiles.";
            else
                resultBest.Content = "Best word is " + mostPointsWord + " with " + mostPoints + " points.";
        }

        /// <summary>
        /// Calculates data surrounding a given word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Word ID + points</returns>
        private int GetWordData(string word)
        {
            int id = 1;
            int points = 0;

            byte blanksUsed = 0;
            foreach(char c in word)
            {
                if(c != ' ' && charCodes[c - 65] > 0)
                {
                    id *= charCodes[c - 65];
                    points += charPoints[c - 65];
                }
                else
                {
                    if(blanksUsed < blanks)
                        blanksUsed++;
                    else
                        return 0;
                }
            }

            // Compress ID and points and then return.
            return (id << 6) | points;
        }

        /// <summary>
        /// Runs when "Refresh" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
    }
}
