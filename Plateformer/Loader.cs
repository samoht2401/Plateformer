using System;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Content;
using Plateformer.Tiles;
using Plateformer.Entities;

namespace Plateformer
{
    public static class Loader
    {
        #region Tiles


        //Herbe et Terre
        public static Texture2D GrassNull { get; private set; }
        public static Texture2D GrassUp { get; private set; }
        public static Texture2D GrassDown { get; private set; }
        public static Texture2D GrassLeft { get; private set; }
        public static Texture2D GrassRight { get; private set; }
        public static Texture2D GrassUpDown { get; private set; }
        public static Texture2D GrassUpLeft { get; private set; }
        public static Texture2D GrassUpRight { get; private set; }
        public static Texture2D GrassUpDownLeft { get; private set; }
        public static Texture2D GrassUpDownRight { get; private set; }
        public static Texture2D GrassUpDownLeftRight { get; private set; }
        public static Texture2D GrassUpLeftRight { get; private set; }
        public static Texture2D GrassDownLeft { get; private set; }
        public static Texture2D GrassDownRight { get; private set; }
        public static Texture2D GrassDownLeftRight { get; private set; }
        public static Texture2D GrassLeftRight { get; private set; }

        //Eau
        public static Texture2D WaterNull { get; private set; }
        public static Texture2D WaterUp { get; private set; }

        //Exit
        public static Texture2D Exit { get; private set; }

        //Gem
        public static Texture2D Gem { get; private set; }
        public static SoundEffect GemSound { get; private set; }

        //CheckPoint
        public static Texture2D CheckPoint { get; private set; }

        //Life
        public static Texture2D Life { get; private set; }
        public static SoundEffect LifeSound { get; private set; }

        //Plateform
        public static Texture2D Plateform { get; private set; }

        //Breakable
        public static Texture2D Breakable { get; private set; }

        //CleRemovable
        public static Texture2D CleRemovable { get; private set; }


        #endregion

        #region Entity


        //Player
        public static Texture2D PlayerNormalIdle { get; private set; }
        public static Texture2D PlayerNormalRun { get; private set; }
        public static Texture2D PlayerNormalJump { get; private set; }
        public static Texture2D PlayerNormalCelebrate { get; private set; }
        public static Texture2D PlayerNormalDie { get; private set; }
        public static Texture2D PlayerNormalFly { get; private set; }

        public static Texture2D PlayerPetitIdle { get; private set; }
        public static Texture2D PlayerPetitRun { get; private set; }
        public static Texture2D PlayerPetitJump { get; private set; }
        public static Texture2D PlayerPetitCelebrate { get; private set; }
        public static Texture2D PlayerPetitDie { get; private set; }

        //Monster1
        public static Texture2D Monster1Idle { get; private set; }
        public static Texture2D Monster1Run { get; private set; }
        public static Texture2D Monster1Die { get; private set; }

        //Teleport
        public static Texture2D Teleport { get; private set; }

        //GiveBreakWithHead
        public static Texture2D GiveBreakWithHead { get; private set; }
        public static SoundEffect GiveBreakWithHeadSound { get; private set; }

        //GiveGoToPetit
        public static Texture2D GiveGoToPetit { get; private set; }
        public static SoundEffect GiveGoToPetitSound { get; private set; }

        //Cle
        public static Texture2D Cle { get; private set; }
        public static SoundEffect CleSound { get; private set; }


        #endregion

        public static void Load(ContentManager content)
        {
            #region Tiles


            //Herbe et Terre
            GrassNull = content.Load<Texture2D>("Tiles/terre.png");
            GrassUp = content.Load<Texture2D>("Tiles/herbe.png");
            GrassDown = content.Load<Texture2D>("Tiles/terre.png");
            GrassLeft = content.Load<Texture2D>("Tiles/terreG.png");
            GrassRight = content.Load<Texture2D>("Tiles/terreD.png");
            GrassUpDown = content.Load<Texture2D>("Tiles/herbe.png");
            GrassUpLeft = content.Load<Texture2D>("Tiles/herbeG.png");
            GrassUpRight = content.Load<Texture2D>("Tiles/herbeD.png");
            GrassUpDownLeft = content.Load<Texture2D>("Tiles/herbeG.png");
            GrassUpDownRight = content.Load<Texture2D>("Tiles/herbeD.png");
            GrassUpDownLeftRight = content.Load<Texture2D>("Tiles/herbe.png");
            GrassUpLeftRight = content.Load<Texture2D>("Tiles/herbe.png");
            GrassDownLeft = content.Load<Texture2D>("Tiles/terreG.png");
            GrassDownRight = content.Load<Texture2D>("Tiles/terreD.png");
            GrassDownLeftRight = content.Load<Texture2D>("Tiles/terre.png");
            GrassLeftRight = content.Load<Texture2D>("Tiles/terre.png");

            //Eau
            WaterNull = content.Load<Texture2D>("Tiles/eau.png");
            WaterUp = content.Load<Texture2D>("Tiles/eauSurface.png");

            //Exit
            Exit = content.Load<Texture2D>("Tiles/exit.png");

            //Gem
            Gem = content.Load<Texture2D>("Sprites/Gem.png");
            try
            {
                GemSound = content.Load<SoundEffect>("Sounds/GemCollected.wma");
            }
            catch (Exception) { }

            //CheckPoint
            CheckPoint = content.Load<Texture2D>("Tiles/checkPoint.png");

            //Life
            Life = content.Load<Texture2D>("Sprites/Vie.png");
            try
            {
                LifeSound = content.Load<SoundEffect>("Sounds/GemCollected.wma");
            }
            catch (Exception) { }

            //Plateform
            Plateform = content.Load<Texture2D>("Tiles/plateform.png");

            //Breakable
            Breakable = content.Load<Texture2D>("Tiles/case.png");

            //Cleremovable
            CleRemovable = content.Load<Texture2D>("Tiles/cleRemovable.png");


            #endregion

            #region Entity


            //Player
            PlayerNormalIdle = content.Load<Texture2D>("Sprites/Player/IdleN.png");
            PlayerNormalRun = content.Load<Texture2D>("Sprites/Player/RunN.png");
            PlayerNormalJump = content.Load<Texture2D>("Sprites/Player/JumpN.png");
            PlayerNormalCelebrate = content.Load<Texture2D>("Sprites/Player/CelebrateN.png");
            PlayerNormalDie = content.Load<Texture2D>("Sprites/Player/DieN.png");
            PlayerNormalFly = content.Load<Texture2D>("Sprites/Player/FlyN.png");

            PlayerPetitIdle = content.Load<Texture2D>("Sprites/Player/IdleN.png");
            PlayerPetitRun = content.Load<Texture2D>("Sprites/Player/RunN.png");
            PlayerPetitJump = content.Load<Texture2D>("Sprites/Player/JumpN.png");
            PlayerPetitCelebrate = content.Load<Texture2D>("Sprites/Player/CelebrateN.png");
            PlayerPetitDie = content.Load<Texture2D>("Sprites/Player/DieN.png");

            //Monster1
            Monster1Idle = content.Load<Texture2D>("Sprites/Monster1/Idle.png");
            Monster1Run = content.Load<Texture2D>("Sprites/Monster1/Run.png");
            Monster1Die = content.Load<Texture2D>("Sprites/Monster1/Die.png");

            //Teleport
            Teleport = content.Load<Texture2D>("Tiles/teleporteur.png");

            //GiveBreakWithHead
            GiveBreakWithHead = content.Load<Texture2D>("Tiles/teteroche.png");
            try
            {
                GiveBreakWithHeadSound = content.Load<SoundEffect>("Sounds/GemCollected.png");
            }
            catch (Exception) { }

            //GiveGoToPetit
            GiveGoToPetit = content.Load<Texture2D>("Tiles/goToPetit.png");
            try
            {
                GiveGoToPetitSound = content.Load<SoundEffect>("Sounds/GemCollected.png");
            }
            catch (Exception) { }

            //Cle
            Cle = content.Load<Texture2D>("Tiles/cle.png");
            try
            {
                CleSound = content.Load<SoundEffect>("Sounds/GemCollected.png");
            }
            catch (Exception) { }


            #endregion
        }
    }
}
