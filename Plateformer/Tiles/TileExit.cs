using System;
using SharpDX;
using SharpDX.Toolkit;
using Plateformer.Entities.Player;

namespace Plateformer.Tiles
{
    public class TileExit : Tile
    {
        public override int WidthForDraw { get { return 64; } }
        public override int HeightForDraw { get { return 64; } }

        public override Vector2 Position { get { return base.Position + new Vector2(WidthForDraw / 2, 0); } }
        public override Vector2 Origin { get { return new Vector2(WidthForDraw / 2 + 2, HeightForDraw / 2 - 1); } }
        public override Rectangle BoundingRectangle
        {
            get
            {
                int widthDivide4 = WidthForDraw / 4;
                int heightDivide4 = HeightForDraw / 4;
                return new Rectangle(base.BoundingRectangle.X + widthDivide4, base.BoundingRectangle.Y + heightDivide4,
                    base.BoundingRectangle.Width - widthDivide4, base.BoundingRectangle.Height - heightDivide4);
            }
        }

        public TileExit(Level level, int x, int y)
            : base(level, x, y, Loader.Exit, TileCollision.Passable)
        { }

        public override void onUpdate(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotation += 10 * elapsed;

            base.onUpdate(gameTime);
        }

        public override void onPLayerTouch(Player player)
        {
            if(player.IsOnGround && player.IsAlive)
                level.OnExitReached();
        }
    }
}
