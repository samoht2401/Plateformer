using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Entities;
using Plateformer.Entities.Player;

namespace Plateformer.Tiles
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,

        Brissable = 3,
        Mort = 4,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    public abstract class Tile
    {
        public const int Width = 32;
        public const int Height = 32;
        public static readonly Vector2 Size = new Vector2(Width, Height);

        protected Level level;
        protected int x;
        public int X { get { return x; } }
        protected int y;
        public int Y { get { return y; } }

        //public Boolean NeedUpdate { get; protected set; }

        protected Texture2D texture;
        public virtual Texture2D Texture { get { return texture; } }
        protected Color color;
        public virtual Color Color { get { return color; } }
        protected float rotation;
        public virtual float Rotation { get { return rotation; } }
        public virtual Vector2 Origin { get { return new Vector2(0, 0); } }

        protected TileCollision collision;
        public virtual TileCollision Collision { get { return collision; } }
        public virtual Rectangle BoundingRectangle { get { return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), WidthForDraw, HeightForDraw); } }

        public virtual Vector2 Position { get { return new Vector2(x, y) * Size; } }

        public virtual int WidthForDraw { get { return 32; } }
        public virtual int HeightForDraw { get { return 32; } }
        public virtual Rectangle? SourceRectangle { get { return null; } }
        public virtual float LayerDepth { get { return 0.9f; } }

        public Tile(Level level, int x, int y, Texture2D texture, TileCollision collision)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.texture = texture;
            this.collision = collision;

            this.color = Color.White;
        }

        public virtual void onUpdate(GameTime gameTime)
        {
            if (this is TileAir)
                return;
            if (level.Player.BoundingRectangle.Intersects(BoundingRectangle) ||
                level.Player.BoundingRectangle.Contains(BoundingRectangle) ||
                BoundingRectangle.Contains(level.Player.BoundingRectangle))
                onPLayerTouch(level.Player);

            /*foreach (Entity entity in level.entities)
            {
                if (Math.Abs(entity.Position.X - Position.X) < 64 &&
                    Math.Abs(entity.Position.Y - Position.Y) < 64)
                    if (BoundingRectangle.Intersects(entity.BoundingRectangle))
                        onEntityTouch(entity);
            }*/
        }

        public virtual void onPLayerTouch(Player player)
        {

        }

        public virtual void onEntityTouch(Entity entity)
        {

        }

        public virtual Tile onTileNextToChange(Tile newTile)
        {
            return newTile;
        }
    }
}
