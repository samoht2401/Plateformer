using System;

namespace Plateformer.Tiles
{
    public class TileDepart : Tile
    {
        public TileDepart(Level level, int x, int y)
            : base(level, x, y, null, TileCollision.Passable)
        { }
    }
}
