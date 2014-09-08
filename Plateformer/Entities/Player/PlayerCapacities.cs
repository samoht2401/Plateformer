using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plateformer.Data;

namespace Plateformer.Entities.Player
{
    public static class PlayerCapacities
    {
        private static PlayerData data;
        public static bool BreakBlocWithHead { get { return data.BreakBlocWithHead; } set { data.BreakBlocWithHead = value; } }
        public static bool BreakBlocInChut { get { return data.BreakBlocInChut; } set { data.BreakBlocInChut = value; } }
        public static bool GoToPetit { get { return data.GoToPetit; } set { data.GoToPetit = value; } }
        public static bool Fly { get { return data.Fly; } 
            set { data.Fly = value; } }

        public static void Update(PlayerData playerData)
        {
            data = playerData;
        }
    }
}
