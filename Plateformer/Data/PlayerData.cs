using System;
using System.IO;
using System.Xml.Serialization;

namespace Plateformer.Data
{
    public class PlayerData
    {
        private bool breakBlocWithHead;
        public bool BreakBlocWithHead { get { return breakBlocWithHead; } set { breakBlocWithHead = value; } }
        private bool breakBlocInChut;
        public bool BreakBlocInChut { get { return breakBlocInChut; } set { breakBlocInChut = value; } }
        private bool goToPetit;
        public bool GoToPetit { get { return goToPetit; } set { goToPetit = value; } }
        private bool fly;
        public bool Fly { get { return fly; } set { fly = value; } }

        private String levelName;
        public String LevelName { get { return levelName; } set { levelName = value; } }
        private int levelIndex;
        public int LevelIndex { get { return levelIndex; } set { levelIndex = value; } }

        public static void Serialize(String path, PlayerData instance)
        {
            XmlSerializer xs = new XmlSerializer(instance.GetType());
            using (FileStream configFile = File.Open(path, FileMode.Create))
            {
                xs.Serialize(configFile, instance);
            }
        }

        public static PlayerData Deserialize(String path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PlayerData));
            using (FileStream configFile = File.Open(path, FileMode.Open))
            {
                return (PlayerData)xs.Deserialize(configFile);
            }
        }
    }
}
