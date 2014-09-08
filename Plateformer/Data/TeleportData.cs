using System;

namespace Plateformer.Data
{
    public class TeleportData
    {
        private int x;
        public int X { get { return x; } set { x = value; } }
        private int y;
        public int Y { get { return y; } set { y = value; } }
        private string destination;
        public string Destination { get { return destination; } set { destination = value; } }
        private int index;
        public int Index { get { return index; } set { index = value; } }
    }
}
