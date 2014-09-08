using System;
using SharpDX;

namespace Plateformer.Data
{
    public static class CleColor
    {
        public static Color Red = new Color(255, 0, 0);
        public static Color Blue = new Color(0, 108, 255);
        public static Color Yellow = new Color(255, 255, 0);
        public static Color Green = new Color(8, 144, 24);
    }

    public class CleData
    {
        private int x;
        public int X { get { return x; } set { x = value; } }
        private int y;
        public int Y { get { return y; } set { y = value; } }
        private Color color;
        public Color Color { get { return color; } set { color = value; } }
    }
}
