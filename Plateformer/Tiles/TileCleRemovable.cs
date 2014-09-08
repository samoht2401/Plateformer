using System;
using SharpDX;

namespace Plateformer.Tiles
{
    public class TileCleRemovable : Tile
    {
        public TileCleRemovable(Level level, int x, int y, Color color)
            : base(level, x, y, Loader.CleRemovable, TileCollision.Impassable)
        {
            this.color = color;
        }
    }
}
