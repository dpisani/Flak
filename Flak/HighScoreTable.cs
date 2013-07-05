using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Flak
{
    class HighScoreTable
    {
        private HighScoreTable()
        {
            HighScore = 0;
            LatestScore = 0;
            if (File.Exists("highscores.dat"))
            {
                using (FileStream file = new FileStream("highscores.dat", FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(file);
                    HighScore = reader.ReadInt32();
                }
            }
        }

        public int HighScore { get; private set; }

        public int LatestScore { get; set; }

        public void SubmitScore(int score)
        {
            if (score > HighScore)
            {
                HighScore = score;
                //write to file
                using (FileStream file = new FileStream("highscores.dat", FileMode.Create))
                {
                    BinaryWriter writer = new BinaryWriter(file);
                    writer.Write(HighScore);
                }
            }

            LatestScore = score;
        }

        private static HighScoreTable theInstance;

        public static HighScoreTable ScoreTable()
        {
            if (theInstance == null)
                theInstance = new HighScoreTable();

            return theInstance;
        }
    }
}
