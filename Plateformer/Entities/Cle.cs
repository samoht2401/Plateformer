using System;
using SharpDX;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Tiles
{
    public class Cle : CollectableEntity
    {
        public Cle(Level level, CleData data)
            : base(level, Loader.Cle, Loader.CleSound)
        {
            position = new Vector2(data.X, data.Y);
            timeBeforeRecoltable = 300f;
            timeToRemove = 1f;
            color = data.Color;
        }

        public override void onCollected(Player collectedBy)
        {
            base.onCollected(collectedBy);
            if (collectedSound != null)
                collectedSound.Play();
            for (int x = 0; x < level.Width; x++)
            {
                for (int y = 0; y < level.Height; y++)
                {
                    if (level.Tiles[x, y] is TileCleRemovable)
                    {
                        TileCleRemovable t = (TileCleRemovable)level.Tiles[x, y];
                        if (t.Color == color)
                            level.ChangeTile(new TileAir(level, t.X, t.Y));
                    }
                }
            }
            Die();
        }

        public override object getData()
        {
            CleData data = new CleData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            data.Color = color;
            return data;
        }
        public override void setData(object data)
        {
            CleData d = (CleData)data;
            position.X = d.X;
            position.Y = d.Y;
            color = d.Color;
        }

        public override Entity Clone()
        {
            return new Cle(level, (CleData)getData());
        }
    }
}
