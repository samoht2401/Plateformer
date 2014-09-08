using System;
using SharpDX;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Tiles
{
    public class Gem : CollectableEntity
    {
        private int pointValue;
        public int PointValue { get { return pointValue; } }

        private GemValues typeGem;
        public GemValues TypeGem { get { return typeGem; } }

        public Gem(Level level, GemData data)
            : base(level, Loader.Gem, Loader.GemSound)
        {
            position = new Vector2(data.X, data.Y);
            typeGem = data.Value;
            timeBeforeRecoltable = 300f;
            timeToRemove = 1f;
            switch (typeGem)
            {
                case GemValues.Yellow10:
                    {
                        pointValue = 10;
                        color = Color.Yellow;
                        break;
                    }
                case GemValues.Pink100:
                    {
                        pointValue = 100;
                        color = new Color(255, 83, 145);
                        break;
                    }
                case GemValues.Purple1000:
                    {
                        pointValue = 1000;
                        color = new Color(180, 20, 180);
                        break;
                    }
            }
        }

        public override void onCollected(Player collectedBy)
        {
            base.onCollected(collectedBy);
            if (collectedSound != null)
                collectedSound.Play();
            level.Score += PointValue;
            Die();
        }

        public override object getData()
        {
            GemData data = new GemData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            data.Value = typeGem;
            return data;
        }
        public override void setData(object data)
        {
            GemData d = (GemData)data;
            position.X = d.X;
            position.Y = d.Y;
            typeGem = d.Value;
            switch (typeGem)
            {
                case GemValues.Yellow10:
                    {
                        pointValue = 10;
                        color = Color.Yellow;
                        break;
                    }
                case GemValues.Pink100:
                    {
                        pointValue = 100;
                        color = new Color(255, 83, 145);
                        break;
                    }
                case GemValues.Purple1000:
                    {
                        pointValue = 1000;
                        color = new Color(180, 20, 180);
                        break;
                    }
            }
        }

        public override Entity Clone()
        {
            return new Gem(level, (GemData)getData());
        }

        public static void SpawnGem(Vector2 position, GemValues value)
        {
            SpawnGem((int)position.X, (int)position.Y, value);
        }
        public static void SpawnGem(int x, int y, GemValues value)
        {
            GemData data = new GemData();
            data.X = x;
            data.Y = y;
            data.Value = value;
            SpawnGem(data);
        }
        public static void SpawnGem(GemData data)
        {
            Level.Instance.entities.Add(new Gem(Level.Instance, data));
        }
    }
}
