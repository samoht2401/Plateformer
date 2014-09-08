using System;
using SharpDX;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Tiles
{
    public class Live : CollectableEntity
    {
        public Live(Level level, LiveData data)
            : base(level, Loader.Life, Loader.LifeSound)
        {
            position = new Vector2(data.X, data.Y);
            timeToRemove = 1f;
        }

        public override void onCollected(Player collectedBy)
        {
            base.onCollected(collectedBy);
            if (collectedSound != null)
                collectedSound.Play();
            level.jeu.ViePlayer++;
            Die();
        }

        public override object getData()
        {
            LiveData data = new LiveData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            return data;
        }
        public override void setData(object data)
        {
            LiveData d = (LiveData)data;
            position.X = d.X;
            position.Y = d.Y;
        }

        public override Entity Clone()
        {
            return new Live(level, (LiveData)getData());
        }
    }
}
