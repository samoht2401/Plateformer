using System;
using System.IO;
using System.Xml.Serialization;

namespace Plateformer.Data
{
    

    public class GestionScore
    {        
        private int meilleurScore = 0;
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

        private bool isFinish = false;
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                isFinish = value;
            }
        }

        public static GestionScore Load(string path, string nameOfFile)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + "\\" + nameOfFile))
            {
                File.Create(path + "\\" + nameOfFile).Close();
            }
            try
            {
                using (FileStream s = new FileStream(path + "\\" + nameOfFile, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(GestionScore));
                    GestionScore newInstance = (GestionScore)xs.Deserialize(s);
                    return newInstance;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static void Save(string path,string nameOfFile, GestionScore existingInstance)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            XmlSerializer xs = new XmlSerializer(typeof(GestionScore));
            using (FileStream configFile = File.Open(path + "\\" + nameOfFile, FileMode.Create, FileAccess.Write))
            {
                xs.Serialize(configFile, existingInstance);
            }
        }
    }
}
