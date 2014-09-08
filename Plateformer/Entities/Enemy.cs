using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Data;

namespace Plateformer.Entities
{
    /// <summary>
    /// Facing direction along the X axis.
    /// </summary>
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    public class Enemy : Entity
    {
        // Animations
        /*private Animation runAnimation;
        private Animation idleAnimation;
        private Animation dieAnimation;*/



        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        private float waitTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private const float MaxWaitTime = 0.5f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        private const float MoveSpeed = 128.0f;

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(Level level, EnemyData data)
            : base(level)
        {
            position = new Vector2(data.X, data.Y);

            addAnimation("Idle", new Animation(Loader.Monster1Idle, 0.15f, true));
            addAnimation("Run", new Animation(Loader.Monster1Run, 0.1f, true));
            addAnimation("Die", new Animation(Loader.Monster1Die, 0.05f, false));
            playAnimation("Idle");

            Animation anim = getAnimation("Idle");
            int width = (int)(anim.FrameWidth * 0.50);
            int left = (anim.FrameWidth - width) / 2;
            int height = (int)(anim.FrameWidth * 0.46);
            int top = anim.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }
        public override Entity Clone()
        {
            Enemy enemy = new Enemy(level, (EnemyData)getData());
            enemy.position = position;
            return enemy;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Wait for some amount of time.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Brissable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else if (Level.GetCollision((int)Position.X / Tile.Width, ((int)Position.Y + 1) / Tile.Height) == TileCollision.Passable)
                {
                    Die();
                }
                else if (isaLive)
                {
                    // Move in the current direction.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }

            }

            if (isaLive)
            {
                // Stop running when the game is paused or before turning around.
                if (level.Player != null)
                {
                    if (!Level.Player.IsAlive ||
                        Level.ReachedExit ||
                        Level.TimeRemaining == TimeSpan.Zero ||
                        waitTime > 0)
                    {
                        playAnimation("Idle");
                    }
                    else
                    {
                        playAnimation("Run");
                    }
                }
                if (waitTime > 0 && Level.GetCollision(tileX, tileY) == TileCollision.Passable)
                {
                    Die();
                }
            }
        }

        public override object getData()
        {
            EnemyData data = new EnemyData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            return data;
        }
        public override void setData(object data)
        {
            EnemyData d = (EnemyData)data;
            position.X = d.X;
            position.Y = d.Y;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position - posCamera, Color.White, 0, 1, flip, 0.2f);
        }

        public override void Die()
        {
            base.Die();
            playAnimation("Die");
        }
    }
}
