using System;
using System.Collections.Generic;
using SharpDX.Toolkit.Graphics;

namespace Plateformer.Tiles
{
    public class TileGrass : Tile
    {
        private Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();
        private String keyTexture = null;
        private String oldKey;
        public override Texture2D Texture
        {
            get
            {
                return textures[getKeyForTexture()];
            }
        }

        public TileGrass(Level level, int x, int y)
            : base(level, x, y, null, TileCollision.Impassable)
        {
            textures.Add("", Loader.GrassNull);
            textures.Add("Up", Loader.GrassUp);
            textures.Add("Down", Loader.GrassDown);
            textures.Add("Left", Loader.GrassLeft);
            textures.Add("Right", Loader.GrassRight);
            textures.Add("UpDown", Loader.GrassUpDown);
            textures.Add("UpLeft", Loader.GrassUpLeft);
            textures.Add("UpRight", Loader.GrassUpRight);
            textures.Add("UpDownLeft", Loader.GrassUpDownLeft);
            textures.Add("UpDownRight", Loader.GrassUpDownRight);
            textures.Add("UpDownLeftRight", Loader.GrassUpDownLeftRight);
            textures.Add("UpLeftRight", Loader.GrassUpLeftRight);
            textures.Add("DownLeft", Loader.GrassDownLeft);
            textures.Add("DownRight", Loader.GrassDownRight);
            textures.Add("DownLeftRight", Loader.GrassDownLeftRight);
            textures.Add("LeftRight", Loader.GrassLeftRight);
        }

        private String getKeyForTexture()
        {
            if (keyTexture == null)
            {
                keyTexture = "";
                if (y > 0 && level.Tiles[x, y - 1].Collision != TileCollision.Impassable &&
                    !(level.Tiles[x, y - 1] is TileWater))
                    keyTexture += "Up";
                if (y < level.Height - 1 && !(level.Tiles[x, y + 1] is TileGrass))
                    keyTexture += "Down";
                if ((x > 0 && !(level.Tiles[x - 1, y] is TileGrass)) ||
                    (y > 0 && level.Tiles[x, y - 1] is TileGrass && ((TileGrass)(level.Tiles[x, y - 1])).getKeyForTexture().Contains("Left")))
                    keyTexture += "Left";
                if ((x < level.Width - 1 && !(level.Tiles[x + 1, y] is TileGrass)) ||
                    (y > 0 && level.Tiles[x, y - 1] is TileGrass && ((TileGrass)(level.Tiles[x, y - 1])).getKeyForTexture().Contains("Right")))
                    keyTexture += "Right";
                keyTexture = keyTexture != null ? keyTexture : "";
                if (!keyTexture.Equals(oldKey))
                {
                    if (x > 0)
                        level.Tiles[x - 1, y].onTileNextToChange(this);
                    if (x < level.Width - 1)
                        level.Tiles[x + 1, y].onTileNextToChange(this);
                    if (y > 0)
                        level.Tiles[x, y - 1].onTileNextToChange(this);
                    if (y < level.Height - 1)
                        level.Tiles[x, y + 1].onTileNextToChange(this);
                }
                oldKey = keyTexture;
            }
            return keyTexture;
        }

        public override Tile onTileNextToChange(Tile newTile)
        {
            keyTexture = null;
            return newTile;
        }
    }
}
