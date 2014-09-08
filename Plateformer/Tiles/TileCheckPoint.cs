using System;
using SharpDX;
using SharpDX.Toolkit;
using Plateformer.Entities;

namespace Plateformer.Tiles
{
    public class TileCheckPoint : Tile
    {
        private bool activer;

        public override int WidthForDraw { get { return 64; } }
        public override int HeightForDraw { get { return 64; } }

        public override Vector2 Origin { get { return new Vector2(WidthForDraw / 2 - 1, HeightForDraw / 2 + 2); } }

        public TileCheckPoint(Level level, int x, int y)
            : base(level, x, y, Loader.CheckPoint, TileCollision.Passable)
        {
            rotation = 0.0f;
            activer = false;
        }

        public override void onUpdate(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (activer)
            {
                rotation += MathUtil.Pi * elapsed;
            }
            if (!activer)
            {
                if (level.Player.BoundingRectangle.Intersects(BoundingRectangle))
                {
                    activer = true;
                    level.Player.Level.lastCheckPoint = Position;
                }
            }
        }
    }
}
