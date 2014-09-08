using System;
using System.IO;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Input;
using SharpDX.Direct3D11;
using Plateformer.Data;   
using BlendState = SharpDX.Toolkit.Graphics.BlendState;  

namespace Plateformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Game
    {
        public Rectangle Viewport
        {
            get
            {
                return new Rectangle(0, 0, (int)this.GraphicsDevice.Presenter.BackBuffer.Width, (int)this.GraphicsDevice.Presenter.BackBuffer.Height);
            }
        }
        
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BlendState blendState;

        public Jeu jeu;
        public MenuPrincipal menuPrincipale;

        public KeyboardManager Keyboard;
        public MouseManager Mouse;

        private const int TargetFrameRate = 60;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;

            Keyboard = new KeyboardManager(this);
            Mouse = new MouseManager(this);
            Input.Initilize(this);

            this.IsMouseVisible=true;

            Content.RootDirectory = "Content";

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blendState = graphics.GraphicsDevice.BlendStates.AlphaBlend;
            blendState = BlendState.New(graphics.GraphicsDevice, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha, BlendOperation.Add, BlendOption.One, BlendOption.InverseSourceAlpha, BlendOperation.Add);

            Loader.Load(Content);

            menuPrincipale = new MenuPrincipal(this);
            menuPrincipale.LoadContent();
            menuPrincipale.Visible = true;
            menuPrincipale.Actif = true;
            
            jeu = new Jeu(this);
            jeu.LoadContent();

            /*MediaPlayer.IsRepeating = true;
            try { MediaPlayer.Play(Content.Load<Song>("Sounds/Music")); }
            catch (Exception) { }*/
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                Input.Update();

                menuPrincipale.Update(gameTime);

                jeu.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, blendState);

            jeu.Draw(gameTime, spriteBatch);
            
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.BackToFront, blendState);

            menuPrincipale.Draw(gameTime, spriteBatch);

            spriteBatch.End();


            base.Draw(gameTime);

        }
    }
}
