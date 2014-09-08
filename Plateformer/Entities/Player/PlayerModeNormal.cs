using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Entities.Player
{
    public class PlayerModeNormal : PlayerMode
    {
        private Animation flyAnim;

        protected override float MoveAcceleration { get { return 10000.0f; } }
        protected override float GroundDragFactor { get { return 0.58f; } }
        protected override float AirDragFactor { get { return 0.65f; } }
        protected override float MaxMoveSpeed { get { return 500.0f; } }

        protected override float GravityAcceleration { get { return 3500.0f; } }
        protected override float MaxFallSpeed { get { return 600.0f; } }
        protected override float MaxJumpTime { get { return 0.35f; } }
        protected override float JumpLaunchVelocity { get { return -4000.0f; } }
        protected override float JumpControlPower { get { return 0.12f; } }
        private const float flyAcceleration = -100f;
        private const float MaxFlySpeed = -500f;
        private float coolDown;
        private const float timeCoolDown = 250f;

        public override Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(player.position.X - player.sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(player.position.Y - player.sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public PlayerModeNormal(Player player)
            : base(player)
        {
            idleAnim = new Animation(Loader.PlayerNormalIdle, 0.1f, true);
            runAnim = new Animation(Loader.PlayerNormalRun, 0.1f, true);
            jumpAnim = new Animation(Loader.PlayerNormalJump, 0.1f, false);
            celebrateAnim = new Animation(Loader.PlayerNormalCelebrate, 0.1f, true);
            dieAnim = new Animation(Loader.PlayerNormalDie, 0.1f, true);
            flyAnim = new Animation(Loader.PlayerNormalFly, 0.1f, true);

            int width = (int)(idleAnim.FrameWidth * 0.82);
            int left = (idleAnim.FrameWidth - width) / 2;
            int height = (int)(idleAnim.FrameWidth * 0.86);
            int top = idleAnim.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public override void Rebondie()
        {
            base.Rebondie();
            player.chuteRapide = false;
        }

        public override void TransformTo()
        {
            coolDown = timeCoolDown;
            base.TransformTo();
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
                player.chuteRapide = false;
            }
            if (!player.IsOnGround)
            {
                if (player.isFlying)
                    player.sprite.PlayAnimation(flyAnim);
                else
                    player.sprite.PlayAnimation(jumpAnim);
            }
            if (!player.IsAlive)
            {
                player.sprite.PlayAnimation(dieAnim);
            }
            if (coolDown > 0f)
                coolDown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                coolDown = 0f;
        }

        protected override void GetInput()
        {
            base.GetInput();

            if (player.isOnGround || player.flying)
                player.desactiveChutRapide = false;
            if (!player.flying)
            {
                if (!player.chuteRapide && !player.desactiveChutRapide && PlayerCapacities.BreakBlocInChut)
                    player.chuteRapide =
                        Input.IsKeyDown(Keys.Down) ||
                        Input.IsKeyDown(Keys.W);
                else if (Input.IsKeyDown(Keys.Up) || player.IsOnGround)
                    player.chuteRapide = false;
            }
            if (player.desactiveChutRapide)
                player.chuteRapide = false;

            if (Input.IsKeyPressed(Keys.Up) && player.jumpTime == 0.0f && player.flying &&
                !player.isOnGround && PlayerCapacities.Fly)
            {
                player.isFlying = true;
            }
            else
            {
                if (player.isFlying == true)
                    coolDown = timeCoolDown;
                player.isFlying = false;
            }
        }

        protected override void AdditionnalPhysics(GameTime gameTime)
        {
            if (PlayerCapacities.BreakBlocInChut && player.chuteRapide && !player.IsOnGround)
            {
                player.velocity.Y += 1000;
            }
            if (PlayerCapacities.Fly)
            {
                if (player.isFlying)
                {
                    if (player.isAlive)
                    {
                        player.velocity.Y = MathUtil.Clamp(player.velocity.Y + flyAcceleration, MaxFlySpeed, -MaxJumpTime);
                        player.flying = true;
                    }
                    else
                        player.flying = false;
                }
                else
                    player.flying = false;
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
            bool endChutRapide = false;

            List<Vector2> aBrisser = new List<Vector2>();

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
                                    if (PlayerCapacities.BreakBlocInChut && player.chuteRapide && !player.previousIsOnGround && collision == TileCollision.Brissable &&
                                        coolDown <= 0)
                                    {
                                        aBrisser.Add(new Vector2(x, y));
                                        endChutRapide = true;
                                        continue;
                                    }
                                    player.isOnGround = true;
                                }
                                if (!player.isOnGround && collision == TileCollision.Platform)
                                    continue;
                                if (previousTop >= tileBounds.Bottom)
                                {
                                    if (PlayerCapacities.BreakBlocWithHead && !player.flying && collision == TileCollision.Brissable &&
                                        coolDown <= 0)
                                    {
                                        aBrisser.Add(new Vector2(x, y));
                                        player.touchTop = true;
                                        continue;
                                    }
                                    player.touchTop = true;
                                }
                                else
                                    player.touchTop = false;
                                if (collision == TileCollision.Platform && depth.Y > 0)
                                    continue;

                                // Resolve the collision along the Y axis.
                                player.position = new Vector2(player.position.X, player.position.Y + depth.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                            else
                            {
                                if (player.IsAlive)
                                {
                                    if (previousBottom <= tileBounds.Top)
                                    {
                                        if (player.chuteRapide && !player.previousIsOnGround && collision == TileCollision.Brissable &&
                                        coolDown <= 0)
                                        {
                                            aBrisser.Add(new Vector2(x, y));
                                            endChutRapide = true;
                                            continue;
                                        }
                                    }

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

            if (aBrisser.Count > 0)
            {
                if (player.chuteRapide)
                {
                    Rebondie();
                    endChutRapide = true;
                }
            }
            foreach (Vector2 vector in aBrisser)
                player.Level.BriseTile((int)vector.X, (int)vector.Y);

            if (endChutRapide)
            {
                player.chuteRapide = false;
                player.desactiveChutRapide = true;
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
            previousTop = bounds.Top;
        }

        public override void TouchEntity(Entity entity)
        {
            entity.onPlayerTouch(player);
            if (entity is Enemy && entity.IsaLive)
            {
                if (player.PreviousBottom - 5 < entity.BoundingRectangle.Top)
                {
                    entity.Die();

                    player.Level.Score += 100;

                    Random r = new Random();
                    if (r.Next(100) < 50)
                    {
                        if (r.Next(100) < 20)
                        {
                            if (r.Next(100) < 10)
                                Gem.SpawnGem((int)entity.Position.X, (int)entity.Position.Y - Tile.Height, GemValues.Purple1000);
                            else
                                Gem.SpawnGem((int)entity.Position.X, (int)entity.Position.Y - Tile.Height, GemValues.Pink100);
                        }
                        else
                            Gem.SpawnGem((int)entity.Position.X, (int)entity.Position.Y - Tile.Height, GemValues.Yellow10);
                    }
                    player.rebond = true;
                }
                else
                {
                    player.OnKilled(entity);
                }
            }
        }
    }
}
