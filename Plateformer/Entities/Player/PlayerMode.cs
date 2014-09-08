using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Input;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Entities.Player;

namespace Plateformer.Entities.Player
{
    public abstract class PlayerMode
    {
        protected Player player;

        protected SpriteEffects flip = SpriteEffects.None;
        protected Animation idleAnim;
        public virtual Animation IdleAnim { get { return idleAnim; } }
        protected Animation runAnim;
        public virtual Animation RunAnim { get { return runAnim; } }
        protected Animation jumpAnim;
        public virtual Animation JumpAnim { get { return jumpAnim; } }
        protected Animation celebrateAnim;
        public virtual Animation CelebrateAnim { get { return celebrateAnim; } }
        protected Animation dieAnim;
        public virtual Animation DieAnim { get { return dieAnim; } }

        protected SoundEffect killedSound;
        public virtual SoundEffect KilledSound { get { return killedSound; } }
        protected SoundEffect jumpSound;
        public virtual SoundEffect JumpSound { get { return jumpSound; } }
        protected SoundEffect fallSound;
        public virtual SoundEffect FallSound { get { return fallSound; } }

        protected virtual float MoveAcceleration { get { return 10000.0f; } }
        protected virtual float GroundDragFactor { get { return 0.58f; } }
        protected virtual float AirDragFactor { get { return 0.65f; } }
        protected virtual float MaxMoveSpeed { get { return 500.0f; } }

        protected virtual float GravityAcceleration { get { return 3500.0f; } }
        protected virtual float MaxFallSpeed { get { return 600.0f; } }
        protected virtual float MaxJumpTime { get { return 0.35f; } }
        protected virtual float JumpLaunchVelocity { get { return -4000.0f; } }
        protected virtual float JumpControlPower { get { return 0.12f; } }

        protected int previousBottom;
        protected int previousTop;

        protected Rectangle localBounds;
        public virtual Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(player.position.X - player.sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(player.position.Y - player.sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public PlayerMode(Player player)
        {
            this.player = player;
        }

        public static void Init(Player player)
        {
            ModeNormal = new PlayerModeNormal(player);
            ModePetit = new PlayerModePetit(player);
        }

        public virtual void Rebondie()
        {
            if (player.isAlive)
                player.velocity.Y = -player.velocity.Y;
        }

        public virtual void Update(GameTime gameTime)
        {
            player.PreviousBottom = BoundingRectangle.Bottom;

            GetInput();

            ApplyPhysics(gameTime);

            for (int i = 0; i < player.Level.entities.Count; i++)
            {
                Entity entity = player.Level.entities[i];
                if (player.BoundingRectangle.Intersects(entity.BoundingRectangle) ||
                    player.BoundingRectangle.Contains(entity.BoundingRectangle) ||
                    entity.BoundingRectangle.Contains(player.BoundingRectangle))
                    TouchEntity(entity);
            }

            AdditonnalUpdate(gameTime);

            // Clear input.
            player.movement = 0.0f;
            player.previousIsOnGround = player.isOnGround;
            player.isJumping = false;
        }

        protected virtual void AdditonnalUpdate(GameTime gameTime)
        {

        }

        protected virtual void GetInput()
        {
            // If any digital horizontal movement input is found, override the analog movement.
            if (Input.IsKeyDown(Keys.Left) ||
                Input.IsKeyDown(Keys.A))
            {
                player.movement = -1.0f;
            }
            else if (Input.IsKeyDown(Keys.Right) ||
                     Input.IsKeyDown(Keys.D))
            {
                player.movement = 1.0f;
            }

            // Check if the player wants to jump.
            player.isJumping =
                Input.IsKeyDown(Keys.Up) ||
                Input.IsKeyDown(Keys.W);
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.previousPosition = player.Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            player.velocity.X += player.movement * MoveAcceleration * elapsed;
            player.velocity.Y = MathUtil.Clamp(player.velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            player.velocity.Y = DoJump(player.velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (player.isOnGround)
                player.velocity.X *= GroundDragFactor;
            else
                player.velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            player.velocity.X = MathUtil.Clamp(player.velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            if (player.touchTop)
            {
                player.velocity.Y = 0;
                player.touchTop = false;
            }
            if (player.rebond)
            {
                Rebondie();
                player.rebond = false;
            }

            AdditionnalPhysics(gameTime);

            // Apply velocity.
            player.position += player.velocity * elapsed;
            player.position = new Vector2((float)Math.Round(player.position.X), (float)Math.Round(player.position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (player.position.X == player.previousPosition.X)
                player.velocity.X = 0;

            if (player.position.Y == player.previousPosition.Y)
                player.velocity.Y = 0;
        }

        protected virtual void AdditionnalPhysics(GameTime gameTime)
        {

        }

        protected virtual float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (player.isJumping)
            {
                // Begin or continue a jump
                if ((!player.wasJumping && player.isOnGround) || player.jumpTime > 0.0f)
                {
                    /*if (player.jumpTime == 0.0f)
                         jumpSound.Play();*/

                    player.jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    player.sprite.PlayAnimation(jumpAnim);
                }

                // If we are in the ascent of the jump
                if (0.0f < player.jumpTime && player.jumpTime <= MaxJumpTime && !player.touchTop)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(player.jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    player.jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                player.jumpTime = 0.0f;
            }
            player.wasJumping = player.isJumping;

            return velocityY;
        }

        protected abstract void HandleCollisions();

        public virtual void TouchEntity(Entity entity)
        {
            //HandleCollisions();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            // Flip the sprite to face the way we are moving.
            if (player.velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (player.velocity.X < 0)
                flip = SpriteEffects.None;

            if (player.timeIntouchable != TimeSpan.Zero)
            {
                if (player.timeIntouchable.TotalSeconds % 0.3f < 0.15f)
                    player.sprite.Draw(gameTime, spriteBatch, player.position - posCamera, Color.White, flip);
            }
            else
            {
                player.sprite.Draw(gameTime, spriteBatch, player.position - posCamera, Color.White, player.rotation, 1.0f, flip, player.LayerDepth);
            }
        }

        public virtual void TransformTo()
        {

        }

        public static PlayerModeNormal ModeNormal;
        public static PlayerModePetit ModePetit;
    }
}
