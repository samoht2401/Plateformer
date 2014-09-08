using System;

namespace Plateformer.Tiles
{
    public class TileBreakable : Tile
    {
        public TileBreakable(Level level, int x, int y)
            : base(level, x, y, Loader.Breakable, TileCollision.Brissable)
        { }


    }
}
