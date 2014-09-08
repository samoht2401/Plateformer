using System;

namespace Plateformer.Data
{
    public class ScoreData
    {
        private int meilleurScore;
        private int scoreForA;
        private int scoreForB;
        private int scoreForC;
        private int scoreForD;
        private int scoreForE;
        private char lastLetter;

        public ScoreData()
        {
            meilleurScore = 0;
            scoreForA = 0;
            scoreForB = 0;
            scoreForC = 0;
            scoreForD = 0;
            scoreForE = 0;
            lastLetter = 'E';
        }

        public int MeilleurScore
        {
            get
            {
                return meilleurScore;
            }
            set
            {
                meilleurScore = value;
            }
        }

        public int ScoreForA
        {
            get
            {
                return scoreForA;
            }
            set
            {
                scoreForA = value;
            }
        }
        public int ScoreForB
        {
            get
            {
                return scoreForB;
            }
            set
            {
                scoreForB = value;
            }
        }
        public int ScoreForC
        {
            get
            {
                return scoreForC;
            }
            set
            {
                scoreForC = value;
            }
        }
        public int ScoreForD
        {
            get
            {
                return scoreForD;
            }
            set
            {
                scoreForD = value;
            }

        }
        public int ScoreForE
        {
            get
            {
                return scoreForE;
            }
            set
            {
                scoreForE = value;
            }
        }

        public char LastLetter
        {
            get
            {
                return lastLetter;
            }
            set
            {
                lastLetter = value;
            }
        }
    }
}
