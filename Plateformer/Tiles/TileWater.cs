using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Plateformer.Entities;

namespace Plateformer.Tiles
{
    public class TileWater : Tile
    {
        private Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();
        private String keyTexture = null;
        private String oldKey;
        public override Vector2 Position { get { return base.Position + new Vector2(0, base.HeightForDraw - HeightForDraw); } }
        public override float LayerDepth { get { return 0.1f; } }
        public override int HeightForDraw
        {
            get
            {
                if (y > 0 && level.Tiles[x, y - 1] is TileWater)
                    return base.HeightForDraw;
                return (int)(base.HeightForDraw * levelWater);
            }
        }
        public override Rectangle? SourceRectangle
        {
            get
            {
                return new Rectangle(0, 0, textures[""].Width, HeightForDraw);
            }
        }
        public override Texture2D Texture
        {
            get
            {
                return textures[getKeyForTexture()];
            }
        }
        public override Color Color
        {
            get
            {
                Color c = Color.CornflowerBlue * pressionInf1;
                c.A = 255;
                return c;
            }
        }

        private float timeBeforeMove;
        private const float timePerMove = 1000 / 20f;

        private float l;
        private float levelWater { get { return l; } set { l = value; recalculPression = true; } }

        private bool recalculPression = true;
        private float p = 0;
        private float pression
        {
            get
            {
                if (recalculPression)
                {
                    float oldPression = p;
                    if (y > 0 && level.Tiles[x, y - 1] is TileWater)
                    {
                        p = levelWater + ((TileWater)level.Tiles[x, y - 1]).pression;
                    }
                    else
                        p = levelWater;

                    float pDiv3 = p / 3;
                    if (x > 0 && level.Tiles[x - 1, y] is TileWater &&
                        x < level.Width - 1 && level.Tiles[x + 1, y] is TileWater)
                    {
                        TileWater l = (TileWater)level.Tiles[x - 1, y];
                        TileWater r = (TileWater)level.Tiles[x + 1, y];
                        p = (p + l.p + r.p) / 3;
                    }
                    else if (x > 0 && level.Tiles[x - 1, y] is TileWater)
                    {
                        TileWater l = (TileWater)level.Tiles[x - 1, y];
                        p = (p + l.p) / 2;
                    }
                    else if (x < level.Width - 1 && level.Tiles[x + 1, y] is TileWater)
                    {
                        TileWater r = (TileWater)level.Tiles[x + 1, y];
                        p = (p + r.p) / 2;
                    }

                    if (oldPression != p)
                        if (y < level.Height - 1 && level.Tiles[x, y + 1] is TileWater)
                            ((TileWater)level.Tiles[x, y + 1]).recalculPression = true;
                    recalculPression = false;
                }
                return p;
            }
        }
        private float pressionInf1 { get { return 1 - MathUtil.Clamp(pression / 20, 0, 1); } }

        public TileWater(Level level, int x, int y)
            : base(level, x, y, null, TileCollision.Passable)
        {
            textures.Add("", Loader.WaterNull);
            textures.Add("Up", Loader.WaterUp);
            levelWater = 1f;
        }

        private TileWater(TileWater tileBase)
            : base(tileBase.level, tileBase.x, tileBase.y, null, TileCollision.Passable)
        {
            textures.Add("", Loader.WaterNull);
            textures.Add("Up", Loader.WaterUp);
            levelWater = tileBase.levelWater;
        }

        public override void onUpdate(GameTime gameTime)
        {
            base.onUpdate(gameTime);

            if (levelWater < 1f / 64)
            {
                level.ChangeTile(new TileAir(level, x, y));
                return;
            }

            if (timeBeforeMove > 0f)
                timeBeforeMove -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            else
            {
                Tile underTile = y < level.Height - 1 ? level.Tiles[x, y + 1] : null;
                Tile leftTile = x > 0 ? level.Tiles[x - 1, y] : null;
                Tile rightTile = x < level.Width - 1 ? level.Tiles[x + 1, y] : null;
                if (underTile != null && underTile is TileAir)
                {
                    y++;
                    level.ChangeTile(this);
                    level.ChangeTile(new TileAir(level, x, y - 1));
                    timeBeforeMove = timePerMove;
                    keyTexture = null;
                }
                else if (underTile != null && underTile is TileWater && ((TileWater)underTile).levelWater < 0.9f)
                {
                    TileWater t = (TileWater)underTile;
                    t.levelWater += levelWater;
                    if (t.levelWater > 1)
                    {
                        levelWater = t.levelWater - 1;
                        t.levelWater = 1;
                    }
                    else
                        level.ChangeTile(new TileAir(level, x, y));
                }
                else if (leftTile != null && leftTile is TileAir &&
                    rightTile != null && rightTile is TileAir)
                {
                    levelWater /= 3;
                    x--;
                    level.ChangeTile(new TileWater(this));
                    x += 2;
                    level.ChangeTile(new TileWater(this));
                    x--;
                }
                else if (leftTile != null && leftTile is TileWater &&
                    rightTile != null && rightTile is TileWater)
                {
                    TileWater l = (TileWater)leftTile;
                    TileWater r = (TileWater)rightTile;
                    float moyenne = (levelWater + l.levelWater + r.levelWater) / 3;
                    levelWater = moyenne;
                    l.levelWater = moyenne;
                    r.levelWater = moyenne;
                }
                else if (leftTile != null && leftTile is TileAir &&
                    rightTile != null && rightTile is TileWater)
                {
                    TileWater r = (TileWater)rightTile;
                    float moyenne = (levelWater + r.levelWater) / 3;
                    levelWater = moyenne;
                    r.levelWater = moyenne;
                    x--;
                    level.ChangeTile(new TileWater(this));
                    x++;
                }
                else if (rightTile != null && rightTile is TileAir &&
                leftTile != null && leftTile is TileWater)
                {
                    TileWater l = (TileWater)leftTile;
                    float moyenne = (levelWater + l.levelWater) / 3;
                    levelWater = moyenne;
                    l.levelWater = moyenne;
                    x++;
                    level.ChangeTile(new TileWater(this));
                    x--;
                }
                else if (underTile != null && underTile is TileWater && ((TileWater)underTile).levelWater < 1f)
                {
                    TileWater t = (TileWater)underTile;
                    t.levelWater += levelWater;
                    if (t.levelWater > 1)
                    {
                        levelWater = t.levelWater - 1;
                        t.levelWater = 1;
                    }
                    else
                        level.ChangeTile(new TileAir(level, x, y));
                }
                else if (leftTile != null && leftTile is TileWater)
                {
                    TileWater l = (TileWater)leftTile;
                    float moyenne = (levelWater + l.levelWater) / 2;
                    levelWater = moyenne;
                    l.levelWater = moyenne;
                }
                else if (rightTile != null && rightTile is TileWater)
                {
                    TileWater r = (TileWater)rightTile;
                    float moyenne = (levelWater + r.levelWater) / 2;
                    levelWater = moyenne;
                    r.levelWater = moyenne;
                }
                else if (leftTile != null && leftTile is TileAir)
                {
                    levelWater /= 2;
                    x--;
                    level.ChangeTile(new TileWater(this));
                    x++;
                    timeBeforeMove = timePerMove;
                    keyTexture = null;
                }
                else if (rightTile != null && rightTile is TileAir)
                {
                    levelWater /= 2;
                    x++;
                    level.ChangeTile(new TileWater(this));
                    x--;
                    timeBeforeMove = timePerMove;
                    keyTexture = null;

                }
            }
        }

        private String getKeyForTexture()
        {
            if (keyTexture == null)
            {
                keyTexture = "";
                if (levelWater < 0.99f ||
                    (y > 0 && level.Tiles[x, y - 1].Collision == TileCollision.Passable) ||
                    (x > 0 && level.Tiles[x - 1, y] is TileWater && ((TileWater)level.Tiles[x - 1, y]).getKeyForTexture().Contains("Up")) ||
                    (x < level.Width - 1 && level.Tiles[x + 1, y] is TileWater && ((TileWater)level.Tiles[x + 1, y]).getKeyForTexture().Contains("Up")))
                    keyTexture += "Up";
                if (y > 0 && level.Tiles[x, y - 1] is TileWater)
                    keyTexture = "";
                keyTexture = keyTexture != null ? keyTexture : "";
                if (!keyTexture.Equals(oldKey))
                {
                    level.onTileChange(this);
                }
                keyTexture = keyTexture != null ? keyTexture : "";
                oldKey = keyTexture;
            }
            return keyTexture;
        }

        public override Tile onTileNextToChange(Tile newTile)
        {
            if (newTile != this)
                keyTexture = null;
            return newTile;
        }

        public override void onEntityTouch(Entity entity)
        {
            if (entity.IsaLive && !(entity is Teleport) && !(entity is Cle))
                entity.Die();
        }
    }
}
