using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Entities.Player;

namespace Plateformer.Entities.Player
{
    public class PlayerModePetit : PlayerMode
    {
        protected override float MoveAcceleration { get { return 17000.0f; } }//17000
        protected override float GroundDragFactor { get { return 0.58f; } }
        protected override float AirDragFactor { get { return 0.65f; } }
        protected override float MaxMoveSpeed { get { return 3000.0f; } }

        protected override float GravityAcceleration { get { return 3500.0f; } }
        protected override float MaxFallSpeed { get { return 600.0f; } }
        protected override float MaxJumpTime { get { return 0.54f; } }
        protected override float JumpLaunchVelocity { get { return -5000.0f; } }//-5000
        protected override float JumpControlPower { get { return 0.12f; } }

        public override Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(player.position.X - player.sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(player.position.Y - player.sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public PlayerModePetit(Player player)
            : base(player)
        {
            idleAnim = new Animation(Loader.PlayerNormalIdle, 0.1f, true);
            runAnim = new Animation(Loader.PlayerNormalRun, 0.1f, true);
            jumpAnim = new Animation(Loader.PlayerNormalJump, 0.1f, false);
            celebrateAnim = new Animation(Loader.PlayerNormalCelebrate, 0.1f, true);
            dieAnim = new Animation(Loader.PlayerNormalDie, 0.1f, true);

            int width = (int)(idleAnim.FrameWidth * 0.75 / 2);
            int left = (idleAnim.FrameWidth - width) / 2;
            int height = (int)(idleAnim.FrameWidth * 0.84 / 2);
            int top = idleAnim.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public override void Rebondie()
        {
            base.Rebondie();
            player.chuteRapide = false;
        }

        protected override void AdditonnalUpdate(GameTime gameTime)
        {
            if (player.IsAlive && player.IsOnGround)
            {
                if (Math.Abs(player.velocity.X) - 0.02f > 0)
                {
                    player.sprite.PlayAnimation(runAnim);
                }
                else
                {
                    player.sprite.PlayAnimation(idleAnim);
                }
            }
            if (!player.IsOnGround)
            {
                player.sprite.PlayAnimation(jumpAnim);
            }
            if (!player.IsAlive)
            {
                player.sprite.PlayAnimation(dieAnim);
            }
        }

        protected override void GetInput()
        {
            base.GetInput();

            if (player.isOnGround)
                player.desactiveChutRapide = false;
        }

        protected override void AdditionnalPhysics(GameTime gameTime)
        {
            if (player.movement != 0)
            {
                int i = 0;
                i++;
            }
        }

        protected override void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width));
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height));

            // Reset flag to search for ground collision.
            player.isOnGround = false;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    TileCollision collision = player.Level.GetCollision(x, y);
                    if (collision == TileCollision.Impassable || collision == TileCollision.Platform ||
                        (collision == TileCollision.Brissable))
                    {
                        Rectangle tileBounds = player.Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    player.isOnGround = true;
                                }
                                if (!player.isOnGround && collision == TileCollision.Platform)
                                    continue;
                                if (previousTop >= tileBounds.Bottom)
                                {
                                    player.touchTop = true;
                                }
                                else
                                    player.touchTop = false;

                                // Resolve the collision along the Y axis.
                                player.position = new Vector2(player.position.X, player.position.Y + depth.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                            else
                            {
                                if (player.IsAlive)
                                {
                                    if (absDepthX > 1)
                                    {
                                        // Resolve the collision along the X axis.
                                        player.position = new Vector2(player.position.X + depth.X, player.position.Y);
                                    }

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }

                            }
                        }
                    }

                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
            previousTop = bounds.Top;
        }

        public override void TouchEntity(Entity entity)
        {
            if (entity is Enemy && entity.IsaLive)
                player.OnKilled(entity);
            entity.onPlayerTouch(player);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            // Flip the sprite to face the way we are moving.
            if (player.velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (player.velocity.X < 0)
                flip = SpriteEffects.None;

            if (player.timeIntouchable != TimeSpan.Zero)
            {
                if (player.timeIntouchable.TotalSeconds % 0.3f < 0.15f)
                    player.sprite.Draw(gameTime, spriteBatch, player.position - posCamera, Color.White, player.rotation, 0.5f, flip, player.LayerDepth);
            }
            else
            {
                player.sprite.Draw(gameTime, spriteBatch, player.position - posCamera, Color.White, player.rotation, 0.5f, flip, player.LayerDepth);
            }
        }
    }
}
