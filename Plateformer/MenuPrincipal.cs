using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using SharpDX.Toolkit.Graphics;

namespace Plateformer
{
    public class MenuPrincipal
    {
        enum ActionIndex
        {
            Jouer = 0,
            Option = 1,
            Quitter = 2,
        }
        
        PlatformerGame Game;
        public bool Visible = false;
        public bool Actif = false;

        private Vector2 posEcran;
        private Vector2 velocity;
        private bool IsActeleration = false;
        private int index;
        private const int posibiliteIndex = 3;
        private Texture2D carreB;
        private Texture2D texFont;
        private Texture2D texJouerA;
        private Texture2D texOptionA;
        private Texture2D texQuitterA;
        private Texture2D texJouerI;
        private Texture2D texOptionI;
        private Texture2D texQuitterI;
        private Vector2 positionJouer;
        private Vector2 positionOption;
        private Vector2 positionQuitter;

        public MenuPrincipal(PlatformerGame game)
        {
            Game = game;
        }

        public void LoadContent()
        {
            carreB = Game.Content.Load<Texture2D>("Tiles/carreB.png");
            texFont = Game.Content.Load<Texture2D>("Menu/font.png");
            texJouerA = Game.Content.Load<Texture2D>("Menu/JouerA.png");
            texOptionA = Game.Content.Load<Texture2D>("Menu/OptionA.png");
            texQuitterA = Game.Content.Load<Texture2D>("Menu/QuitterA.png");
            texJouerI = Game.Content.Load<Texture2D>("Menu/JouerI.png");
            texOptionI = Game.Content.Load<Texture2D>("Menu/OptionI.png");
            texQuitterI = Game.Content.Load<Texture2D>("Menu/QuitterI.png");
            positionJouer = new Vector2(Game.Viewport.Width / 2 - texJouerI.Width / 2, Game.Viewport.Height * 0.35f);
            positionOption = new Vector2(Game.Viewport.Width / 2 - texOptionI.Width / 2, Game.Viewport.Height * 0.5f);
            positionQuitter = new Vector2(Game.Viewport.Width / 2 - texQuitterI.Width / 2, Game.Viewport.Height * 0.65f);
        }

        public void Update(GameTime gameTime)
        {
            if (Actif)
            {
                if (Input.IsKeyPressed(Keys.Down))
                {
                    index++;
                    if (index > posibiliteIndex - 1)
                        index = 0;
                }
                else if (Input.IsKeyPressed(Keys.Up))
                {
                    index--;
                    if (index < 0)
                        index = posibiliteIndex - 1;
                }
                if (Input.IsKeyPressed(Keys.Space))
                {
                    if (index == (int)ActionIndex.Jouer)
                    {
                        IsActeleration = true;
                    }
                    if (index == (int)ActionIndex.Quitter)
                    {
                        Game.Exit();
                    }
                }
                if (IsActeleration)
                {
                    velocity.Y += 0.5f;
                    Game.jeu.Visible = true;
                }
                if (posEcran.Y > Game.GraphicsDevice.Viewport.Height)
                {
                    velocity = Vector2.Zero;
                    posEcran = Vector2.Zero;
                    IsActeleration = false;
                    Visible = false;
                    Actif = false;
                    Game.jeu.Actif = true;
                }
                posEcran += velocity;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(carreB, new Rectangle(Game.Viewport.X, Game.Viewport.Y, Game.Viewport.Width - (int)posEcran.X, Game.Viewport.Height - (int)posEcran.Y), new Color(64, 176, 0));
                spriteBatch.Draw(texFont, new Rectangle(Game.Viewport.Width / 2 - Math.Min(Game.Viewport.Width, Game.Viewport.Height) / 2 - (int)posEcran.X,
                                                        Game.Viewport.Height / 2 - Math.Min(Game.Viewport.Width, Game.Viewport.Height) / 2 - (int)posEcran.Y,
                                                        Math.Min(Game.Viewport.Width, Game.Viewport.Height),
                                                        Math.Min(Game.Viewport.Width, Game.Viewport.Height)), Color.White);
                switch (index)
                {
                    case 0:
                        spriteBatch.Draw(texJouerA, positionJouer - posEcran, Color.White);
                        spriteBatch.Draw(texOptionI, positionOption - posEcran, Color.White);
                        spriteBatch.Draw(texQuitterI, positionQuitter - posEcran, Color.White);
                        break;
                    case 1:
                        spriteBatch.Draw(texJouerI, positionJouer - posEcran, Color.White);
                        spriteBatch.Draw(texOptionA, positionOption - posEcran, Color.White);
                        spriteBatch.Draw(texQuitterI, positionQuitter - posEcran, Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(texJouerI, positionJouer - posEcran, Color.White);
                        spriteBatch.Draw(texOptionI, positionOption - posEcran, Color.White);
                        spriteBatch.Draw(texQuitterA, positionQuitter - posEcran, Color.White);
                        break;
                }
            }
        }
    }
}
