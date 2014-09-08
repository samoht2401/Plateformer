using System;

namespace Plateformer.Tiles
{
    public class TileAir : Tile
    {
        public TileAir(Level level, int x, int y)
            : base(level, x, y, null, TileCollision.Passable)
        { }
    }
}
