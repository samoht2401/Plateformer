using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Data;

namespace Plateformer.Entities
{
    public class Teleport : Entity
    {
        private String destination;
        private int index;

        public Teleport(Level level, TeleportData data)
            : base(level)
        {
            position = new Vector2(data.X, data.Y);
            destination = data.Destination;
            index = data.Index;
            timeToRemove = 1f;

            addAnimation("", new Animation(Loader.Teleport, 0.1f, false));
            playAnimation("");

            int width = (int)(Tile.Width * 2);
            int left = Tile.Width * 2 - width;
            int height = (int)(Tile.Width * 1.9);
            int top = Tile.Height * 2 - height;
            localBounds = new Rectangle(left, top, width, height);
        }
        public override Entity Clone()
        {
            return new Teleport(level, (TeleportData)getData());
        }

        public override void Update(GameTime gameTime)
        {
            if (level.Player.IsOnGround && BoundingRectangle.Intersects(level.Player.BoundingRectangle))
                level.OnExitReached(destination, index);

            base.Update(gameTime);
        }

        public override object getData()
        {
            TeleportData data = new TeleportData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            data.Destination = destination;
            data.Index = index;
            return data;
        }
        public override void setData(object data)
        {
            TeleportData d = (TeleportData)data;
            position.X = d.X;
            position.Y = d.Y;
            destination = d.Destination;
            index = d.Index;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            sprite.Draw(gameTime, spriteBatch, Position - posCamera, Color.White, 0, 1, SpriteEffects.None, 0.2f);
        }
    }
}
