using System;

namespace Plateformer.Tiles
{
    public class TilePlateform : Tile
    {
        public TilePlateform(Level level, int x, int y)
            : base(level, x, y, Loader.Plateform, TileCollision.Platform)
        { }
    }
}
