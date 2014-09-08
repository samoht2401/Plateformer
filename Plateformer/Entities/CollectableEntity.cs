using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using Plateformer.Animations;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Tiles
{
    public abstract class CollectableEntity : Entity
    {
        protected SoundEffect collectedSound;
        protected bool collected;
        protected float bounce;

        public override Vector2 Position
        {
            get { return base.Position + new Vector2(0.0f, bounce) + sprite.Origin; }
        }

        protected Color color = Color.White;
        public virtual Color Color { get { return color; } }

        public float LayerDepth { get { return 0.2f; } }

        public CollectableEntity(Level level, Texture2D texture, SoundEffect sound)
            : base(level)
        {
            collectedSound = sound;
            addAnimation("", new Animation(texture, 0.1f, false));
            playAnimation("");

            localBounds = new Rectangle((int)position.X, (int)position.Y, Tile.Width, Tile.Height);
        }

        protected float timeBeforeRecoltable;

        public override void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.09f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * sprite.Animation.FrameHeight;

            if (timeBeforeRecoltable > 0f)
                timeBeforeRecoltable -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            sprite.Draw(gameTime, spriteBatch, Position - posCamera, color, SpriteEffects.None);
        }

        public override void onPlayerTouch(Player player)
        {
            base.onPlayerTouch(player);

            if (timeBeforeRecoltable <= 0 && !collected)
                onCollected(player);
        }

        public virtual void onCollected(Player collectedBy)
        {
            collected = true;
        }
    }
}
