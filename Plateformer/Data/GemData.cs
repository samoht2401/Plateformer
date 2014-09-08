using System;

namespace Plateformer.Data
{
    public enum GemValues
    {
        Yellow10,
        Pink100,
        Purple1000,
    }

    public class GemData
    {
        private int x;
        public int X { get { return x; } set { x = value; } }
        private int y;
        public int Y { get { return y; } set { y = value; } }
        private GemValues value;
        public GemValues Value { get { return value; } set { this.value = value; } }
    }
}
