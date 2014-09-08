using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Animations;
using Plateformer.Tiles;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Entities
{
    public abstract class Entity
    {
        public Level Level { get { return level; } }
        protected Level level;

        public virtual Vector2 Position { get { return position; } set { position = value; } }
        protected Vector2 position;

        protected bool mustBeRemove;
        public virtual bool MustBeRemove { get { return mustBeRemove; } }

        protected Rectangle localBounds;
        public virtual Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        protected AnimationPlayer sprite;
        public AnimationPlayer Sprite { get { return sprite; } }
        private Dictionary<String, Animation> animations;

        protected bool isaLive = true;
        public bool IsaLive { get { return isaLive; } }

        protected float timeIsDie;
        public float TimeIsDie { get { return timeIsDie; } }
        protected float timeToRemove = 3000f;

        public Entity(Level level)
        {
            this.level = level;

            animations = new Dictionary<String, Animation>();
            sprite = new AnimationPlayer();
        }
        public abstract Entity Clone();

        public virtual void Update(GameTime gameTime)
        {
            if (timeIsDie > 0)
            {
                timeIsDie -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeIsDie <= 0f)
                    mustBeRemove = true;
            }

            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) + 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) + 1;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    Rectangle tileBounds = level.Player.Level.GetBounds(x, y);
                    if (tileBounds.Intersects(BoundingRectangle))
                        level.Tiles[x, y].onEntityTouch(this);
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {

        }

        public virtual void onPlayerTouch(Player.Player player)
        {

        }

        public abstract object getData();
        public abstract void setData(object data);

        protected void addAnimation(String key, Animation anim)
        {
            animations.Add(key, anim);
        }
        protected Animation getAnimation(String key)
        {
            return animations[key];
        }
        protected void playAnimation(String key)
        {
            sprite.PlayAnimation(animations[key]);
        }

        public virtual void Die()
        {
            if (isaLive)
                timeIsDie = timeToRemove;
            isaLive = false;
        }

        public virtual bool Intersects(Rectangle rect)
        {
            return BoundingRectangle.Intersects(rect);
        }
    }
}
