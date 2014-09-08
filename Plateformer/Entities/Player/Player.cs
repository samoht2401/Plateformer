using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Plateformer.Animations;
using Plateformer.Tiles;

namespace Plateformer.Entities.Player
{
    public class Player
    {
        internal SpriteEffects flip = SpriteEffects.None;
        internal AnimationPlayer sprite;

        internal TimeSpan timeIntouchable = TimeSpan.Zero;

        public PlayerMode Mode;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        internal bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        internal Vector2 position;
        internal Vector2 previousPosition;

        internal bool touchTop;
        internal bool rebond;
        public float PreviousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        internal Vector2 velocity;

        public float LayerDepth { get { return 0.5f; } }

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        internal bool isOnGround;
        internal bool previousIsOnGround;

        internal float movement;

        // Jumping state
        internal bool isJumping;
        internal bool wasJumping;
        internal float jumpTime;
        internal bool chuteRapide;
        internal bool desactiveChutRapide;
        internal bool isFlying;
        internal bool flying;
        internal Jeu jeu;
        internal float rotation;
        internal bool tourne = false;

        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle { get { return Mode.BoundingRectangle; } }
        public Rectangle OldBoundingRectangle;

        public Player(Level level, Jeu jeu, Vector2 position)
        {
            this.level = level;
            this.jeu = jeu;

            PlayerMode.Init(this);

            Reset(position);
        }

        public void Reset(Vector2 position)
        {
            Position = position - new Vector2(0, 2);
            previousPosition = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            Mode = PlayerMode.ModeNormal;
            sprite.PlayAnimation(Mode.IdleAnim);
            OldBoundingRectangle = BoundingRectangle;
            PreviousBottom = BoundingRectangle.Bottom;
        }

        public void ToPetit()
        {
            if (!IsPetit() && PlayerCapacities.GoToPetit)
            {
                position.Y--;
                position.X--;
                Mode = PlayerMode.ModePetit;
                Mode.TransformTo();
            }
        }
        public bool IsPetit()
        {
            if (Mode is PlayerModePetit)
                return true;
            return false;
        }
        public void ToNormal()
        {
            if (!IsNormal())
            {
                position.Y--;
                position.X--;
                Mode = PlayerMode.ModeNormal;
                Mode.TransformTo();
                VerifiePlace();
            }
        }
        public bool IsNormal()
        {
            if (Mode is PlayerModeNormal)
                return true;
            return false;
        }
        private void VerifiePlace()
        {
            int posX = (int)Position.X / Tile.Width;
            int posY = (int)Position.Y / Tile.Height;

            if ((Level.Tiles[posX - 1, posY].Collision == TileCollision.Impassable || Level.Tiles[posX - 1, posY].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX + 1, posY].Collision == TileCollision.Impassable || Level.Tiles[posX + 1, posY].Collision == TileCollision.Brissable))
            {
                if (Level.Tiles[posX - 1, posY].Collision != TileCollision.Brissable &&
                    Level.Tiles[posX + 1, posY].Collision != TileCollision.Brissable)
                {
                    OnKilled(null);
                }
                if (Level.Tiles[posX - 1, posY].Collision == TileCollision.Brissable)
                {
                    Level.BriseTile(posX - 1, posY);
                    if (Level.Tiles[posX - 1, posY - 1].Collision == TileCollision.Brissable)
                    {
                        Level.BriseTile(posX - 1, posY - 1);
                    }
                }
                if (Level.Tiles[posX + 1, posY].Collision == TileCollision.Brissable)
                {
                    Level.BriseTile(posX + 1, posY);
                    if (Level.Tiles[posX + 1, posY - 1].Collision == TileCollision.Brissable)
                    {
                        Level.BriseTile(posX + 1, posY - 1);
                    }
                }
            }

            if ((Level.Tiles[posX, posY - 1].Collision == TileCollision.Impassable || Level.Tiles[posX, posY - 1].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX, posY + 1].Collision == TileCollision.Impassable || Level.Tiles[posX, posY + 1].Collision == TileCollision.Brissable))
            {
                if (Level.Tiles[posX, posY - 1].Collision != TileCollision.Brissable &&
                    Level.Tiles[posX, posY + 1].Collision != TileCollision.Brissable)
                {
                    OnKilled(null);
                }
                if (Level.Tiles[posX, posY - 1].Collision == TileCollision.Brissable)
                {
                    Level.BriseTile(posX, posY - 1);
                    if (Level.Tiles[posX - 1, posY - 1].Collision == TileCollision.Brissable)
                    {
                        Level.BriseTile(posX - 1, posY - 1);
                    }
                    if (Level.Tiles[posX + 1, posY - 1].Collision == TileCollision.Brissable)
                    {
                        Level.BriseTile(posX + 1, posY - 1);
                    }
                }
            }

            if ((Level.Tiles[posX - 1, posY].Collision == TileCollision.Impassable || Level.Tiles[posX - 1, posY].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX, posY + 1].Collision == TileCollision.Impassable || Level.Tiles[posX, posY + 1].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX + 1, posY - 1].Collision == TileCollision.Impassable || Level.Tiles[posX + 1, posY - 1].Collision == TileCollision.Brissable))
            {
                if (Level.Tiles[posX + 1, posY - 1].Collision == TileCollision.Brissable)
                {
                    Level.BriseTile(posX + 1, posY - 1);
                }
                else
                {
                    OnKilled(null);
                }
            }
            if ((Level.Tiles[posX + 1, posY].Collision == TileCollision.Impassable || Level.Tiles[posX + 1, posY].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX, posY + 1].Collision == TileCollision.Impassable || Level.Tiles[posX, posY + 1].Collision == TileCollision.Brissable) &&
                (Level.Tiles[posX - 1, posY - 1].Collision == TileCollision.Impassable || Level.Tiles[posX - 1, posY - 1].Collision == TileCollision.Brissable))
            {
                if (Level.Tiles[posX - 1, posY - 1].Collision == TileCollision.Brissable)
                {
                    Level.BriseTile(posX - 1, posY - 1);
                }
                else
                {
                    OnKilled(null);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Mode.Update(gameTime);

            if (Input.IsKeyDown(Keys.LeftShift))
            {
                ToPetit();
            }
            else
            {
                ToNormal();
            }
            if (timeIntouchable > TimeSpan.Zero)
            {
                float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeIntouchable -= TimeSpan.FromSeconds(seconds);
            }
            else
                timeIntouchable = TimeSpan.Zero;
            if (tourne)
                rotation += MathUtil.Pi * elapsed;

            OldBoundingRectangle = BoundingRectangle;
        }

        public void OnKilled(Entity killedBy)
        {
            if (IsAlive)
                jeu.ViePlayer--;
            isAlive = false;

            if (killedBy != null)
            {
                if (Mode.KilledSound != null)
                    Mode.KilledSound.Play();
            }
            else
            {
                if (Mode.FallSound != null)
                    Mode.FallSound.Play();
            }

            sprite.PlayAnimation(Mode.DieAnim);
        }

        public void OnReachedExit()
        {
            sprite.PlayAnimation(Mode.CelebrateAnim);
            tourne = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            Mode.Draw(gameTime, spriteBatch, posCamera);
        }
    }
}