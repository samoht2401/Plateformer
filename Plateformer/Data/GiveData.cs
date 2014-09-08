using System;

namespace Plateformer.Data
{
    public enum GiveType
    {
        BreakWithHead,
        GoToPetit,
    }

    public class GiveData
    {
        private int x;
        public int X { get { return x; } set { x = value; } }
        private int y;
        public int Y { get { return y; } set { y = value; } }
        private GiveType type;
        public GiveType Type { get { return type; } set { type = value; } }
    }
}
