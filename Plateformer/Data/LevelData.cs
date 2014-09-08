using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Plateformer.Data
{
    public class LevelData
    {
        private List<string> tiles;
        private int heigth;
        private int width;
        private List<EnemyData> enemies;
        private List<GemData> gems;
        private List<LiveData> lives;
        private List<CleData> cles;
        private List<TeleportData> teleports;
        private List<GiveData> gives;
        private ScoreData score;

        public LevelData()
        {
            score = new ScoreData();
            enemies = new List<EnemyData>();
            gems = new List<GemData>();
            lives = new List<LiveData>();
            cles = new List<CleData>();
            teleports = new List<TeleportData>();
            gives = new List<GiveData>();
            tiles = new List<string>();
        }

        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return heigth; } set { heigth = value; } }

        public List<string> Tiles { get { return tiles; } set { tiles = value; } }

        public List<EnemyData> Enemies { get { return enemies; } set { enemies = value; } }
        public List<GemData> Gems { get { return gems; } set { gems = value; } }
        public List<LiveData> Lives { get { return lives; } set { lives = value; } }
        public List<CleData> Cles { get { return cles; } set { cles = value; } }
        public List<TeleportData> Teleports { get { return teleports; } set { teleports = value; } }
        public List<GiveData> Gives { get { return gives; } set { gives = value; } }

        /*public char[,] GetTiles
        {
            get
            {
                char[,] tabChar = new char[Width, Height];
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        tabChar[i, j] = Tiles[j][i];
                    }
                }
                return tabChar;
            }
            set
            {
                tiles = new List<string>();
                string line = "";
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    line = "";
                    for (int i = 0; i < value.GetLength(0); i++)
                    {
                        line = line + value[i, j];
                    }
                    tiles.Add(line);
                }
            }
        }*/

        public ScoreData Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public static void Serialize(String path, LevelData instance)
        {
            XmlSerializer xs = new XmlSerializer(instance.GetType());
            using (FileStream configFile = File.Open(path, FileMode.Create))
            {
                xs.Serialize(configFile, instance);
            }
        }

        public static LevelData Deserialize(String path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(LevelData));
            using (FileStream configFile = File.Open(path, FileMode.Open))
            {
                return (LevelData)xs.Deserialize(configFile);
            }
        }
    }
}
