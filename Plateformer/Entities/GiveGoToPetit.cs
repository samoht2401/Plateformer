using System;
using SharpDX;
using Plateformer.Entities;
using Plateformer.Entities.Player;
using Plateformer.Data;

namespace Plateformer.Tiles
{
    public class GiveGoToPetit : CollectableEntity
    {
        private GiveType type;

        public GiveGoToPetit(Level level, GiveData data)
            : base(level, Loader.GiveGoToPetit, Loader.GiveGoToPetitSound)
        {
            position = new Vector2(data.X, data.Y);
            timeToRemove = 1f;
            type = data.Type;
        }

        public override void onCollected(Player collectedBy)
        {
            base.onCollected(collectedBy);
            if (collectedSound != null)
                collectedSound.Play();
            PlayerCapacities.GoToPetit = true;
            Die();
        }

        public override object getData()
        {
            GiveData data = new GiveData();
            data.X = (int)position.X;
            data.Y = (int)position.Y;
            data.Type = type;
            return data;
        }
        public override void setData(object data)
        {
            GiveData d = (GiveData)data;
            position.X = d.X;
            position.Y = d.Y;
            type = d.Type;
        }

        public override Entity Clone()
        {
            return new GiveGoToPetit(level, (GiveData)getData());
        }
    }
}
