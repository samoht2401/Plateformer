using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Plateformer.Data;
using Plateformer.Tiles;
using Plateformer.Entities;
using Plateformer.Entities.Player;

namespace Plateformer
{
    public class Jeu
    {
        public PlatformerGame Game;
        public bool Visible = false;
        public bool Actif = false;

        public Vector2 posCamera;

        private Texture2D texGem;
        private Texture2D texMonstre;
        private Texture2D texVie;

        public enum Block
        {
            Null,
            Suprimer = '.',
            Case = ')',
            Terre = '#',
            Eau = '~',
            Platform = '=',
            Player = '1',
            Sortit = 'X',
            CheckPoint = 'P',
            CleRemovableRed = '+',
            CleRemovableBlue = '-',
            CleRemovableYellow = '*',
            CleRemovableGreen = '/',
        }
#if DEBUG
        Block block = Block.Null;
        Entity entity = null;
        Texture2D texBlock = null;
        String teleportDest = "";
        bool wantPLaceTeleport = false;
        Vector2? baseFill;
        Vector2? finalFill;
        bool wantFill = false;
#endif

        private const String PlayerSavePath = "Saved/player.xml";
        private PlayerData playerData;
        // Global content.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        public int ViePlayer;
        public bool[] nivFini;

        public Jeu(PlatformerGame game)
        {
            Game = game;
            Game.Exiting += new EventHandler<EventArgs>(Game_Exiting);
            if (File.Exists(PlayerSavePath))
                playerData = PlayerData.Deserialize(PlayerSavePath);
            else
            {
                playerData = new PlayerData();
                playerData.LevelName = "base";
                playerData.LevelIndex = 0;
            }
            PlayerCapacities.Update(playerData);
        }

        void Game_Exiting(object sender, EventArgs e)
        {
            playerData.LevelName = level.name;
            playerData.LevelIndex = levelIndex;
            PlayerData.Serialize(PlayerSavePath, playerData);
        }

        public void LoadContent()
        {
            // Load fonts
            hudFont = Game.Content.Load<SpriteFont>("Fonts/Hud");

            //Load sprites
            texGem = Game.Content.Load<Texture2D>("Sprites/Gem.png");
            texMonstre = Game.Content.Load<Texture2D>("Sprites/Monster1/Idle.png");
            texVie = Game.Content.Load<Texture2D>("Sprites/Vie.png");

            // Load overlay textures
            winOverlay = Game.Content.Load<Texture2D>("Overlays/you_win.png");
            loseOverlay = Game.Content.Load<Texture2D>("Overlays/you_lose.png");
            diedOverlay = Game.Content.Load<Texture2D>("Overlays/you_died.png");

            int index = -1;
            while (true)
            {
                index++;
                string file = "Content/Levels/" + index + ".txt";
                if (!File.Exists(file))
                {
                    nivFini = new bool[index + 1];
                    break;
                }
            }
            /*string path = "Content/" + "Saved/General.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                ViePlayer = ToInt(reader.ReadLine());
                for (int i = 0; i < nivFini.Length - 1; i++)
                {
                    nivFini[i] = reader.ReadLine() != "0";
                }
            }*/

            LoadNextLevel(playerData.LevelName, playerData.LevelIndex);
        }

        public int ToInt(string chaine)
        {
            int number = 0;
            for (int i = 0; i < chaine.Length; ++i)
            {
                char c = chaine.ToCharArray()[i];
                number += ((int)c - 48) * (int)Math.Pow(10, chaine.Length - i + 1) / 100;
            }
            return number;
        }

        public void Update(GameTime gameTime)
        {
            if (Actif)
            {

                HandleInput();
#if !DEBUG
                level.Update(gameTime);

                if (level.Player.Position.X - posCamera.X < Game.Viewport.Width / 3)
                    posCamera.X -= (Game.Viewport.Width / 3) - (level.Player.Position.X - posCamera.X);

                else if (level.Player.Position.X - posCamera.X + level.Player.BoundingRectangle.Width > Game.Viewport.Width / 3 * 2)
                    posCamera.X += (level.Player.Position.X - posCamera.X + level.Player.BoundingRectangle.Width) - (Game.Viewport.Width / 3 * 2);

                if (level.Player.Position.Y - posCamera.Y < Game.Viewport.Height / 3)
                    posCamera.Y -= (Game.Viewport.Height / 3) - (level.Player.Position.Y - posCamera.Y);

                else if (level.Player.Position.Y - posCamera.Y + level.Player.BoundingRectangle.Height > Game.Viewport.Height / 3 * 2)
                    posCamera.Y += (level.Player.Position.Y - posCamera.Y + level.Player.BoundingRectangle.Height) - (Game.Viewport.Height / 3 * 2);

                if (level.Width * Tile.Width > Game.Viewport.Width)
                    posCamera.X = MathUtil.Clamp(posCamera.X, 0.0f, level.Width * Tile.Width - Game.Viewport.Width);
                else
                    posCamera.X = MathUtil.Clamp(posCamera.X, level.Width * Tile.Width - Game.Viewport.Width, 0.0f);
                if (level.Height * Tile.Height > Game.Viewport.Height)
                    posCamera.Y = MathUtil.Clamp(posCamera.Y, 0.0f, level.Height * Tile.Height - Game.Viewport.Height);
                else
                    posCamera.Y = MathUtil.Clamp(posCamera.Y, level.Height * Tile.Height - Game.Viewport.Height, 0.0f);

#endif
#if DEBUG
                Editeur(gameTime);
#endif
            }
        }

        private void Editeur(GameTime gameTime)
        {
#if DEBUG
            int x = -1;
            int y = -1;

            if (Input.IsKeyDown(Keys.Left))
            {
                posCamera = new Vector2(posCamera.X - 10, posCamera.Y);
                posCamera = new Vector2(Math.Max(0.0f, posCamera.X), Math.Max(0.0f, posCamera.Y));
            }
            else if (Input.IsKeyDown(Keys.Right))
            {
                posCamera = new Vector2(posCamera.X + 10, posCamera.Y);
                posCamera = new Vector2(Math.Max(0.0f, posCamera.X), Math.Max(0.0f, posCamera.Y));
            }
            if (Input.IsKeyDown(Keys.Up))
            {
                posCamera = new Vector2(posCamera.X, posCamera.Y - 10);
                posCamera = new Vector2(Math.Max(0.0f, posCamera.X), Math.Max(0.0f, posCamera.Y));
            }
            else if (Input.IsKeyDown(Keys.Down))
            {
                posCamera = new Vector2(posCamera.X, posCamera.Y + 10);
                posCamera = new Vector2(Math.Max(0.0f, posCamera.X), Math.Max(0.0f, posCamera.Y));
            }
            if (!wantPLaceTeleport && !wantFill)
            {
                
                if (Input.IsKeyDown(Keys.NumPad0))
                {
                    block = Block.Suprimer;
                    entity = null;
                    texBlock = null;
                }

                else if (Input.IsKeyDown(Keys.NumPad1))
                {
                    block = Block.Terre;
                    entity = null;
                    texBlock = Loader.GrassNull;
                }
                else if (Input.IsKeyDown(Keys.NumPad2))
                {
                    block = Block.Case;
                    entity = null;
                    texBlock = Loader.Breakable;
                }
                else if (Input.IsKeyDown(Keys.NumPad3))
                {
                    block = Block.Eau;
                    entity = null;
                    texBlock = Loader.WaterNull;
                }
                else if (Input.IsKeyDown(Keys.NumPad4))
                {
                    block = Block.CheckPoint;
                    entity = null;
                    texBlock = Loader.CheckPoint;
                }
                else if (Input.IsKeyDown(Keys.NumPad5))
                {
                    block = Block.Platform;
                    entity = null;
                    texBlock = Loader.Plateform;
                }
                else if (Input.IsKeyDown(Keys.NumPad6))
                {
                    EnemyData data = new EnemyData();
                    data.X = 0;
                    data.Y = 0;
                    entity = new Enemy(level, data);
                    block = Block.Null;
                    texBlock = Loader.Monster1Idle;
                }
                else if (Input.IsKeyDown(Keys.NumPad7))
                {
                    GemData data = new GemData();
                    data.X = 0;
                    data.Y = 0;
                    data.Value = GemValues.Yellow10;
                    entity = new Gem(level, data);
                    block = Block.Null;
                    texBlock = Loader.Gem;
                }
                else if (Input.IsKeyDown(Keys.NumPad8))
                {
                    block = Block.Sortit;
                    entity = null;
                    texBlock = Loader.Exit;
                }
                else if (Input.IsKeyDown(Keys.NumPad9))
                {
                    block = Block.Player;
                    entity = null;
                    texBlock = Game.Content.Load<Texture2D>("Sprites/Player/I.png");
                }
                else if (Input.IsKeyDown(Keys.V))
                {
                    LiveData data = new LiveData();
                    data.X = 0;
                    data.Y = 0;
                    entity = new Live(level, data);
                    block = Block.Null;
                    texBlock = Loader.Life;
                }
                else if (Input.IsKeyDown(Keys.C))
                {
                    CleData data = new CleData();
                    data.X = 0;
                    data.Y = 0;
                    data.Color = CleColor.Green;
                    entity = new Cle(level, data);
                    block = Block.Null;
                    texBlock = Loader.Cle;
                }
                else if (Input.IsKeyDown(Keys.T))
                {
                    TeleportData data = new TeleportData();
                    data.X = 0;
                    data.Y = 0;
                    entity = new Teleport(level, data);
                    block = Block.Null;
                    texBlock = Loader.Teleport;
                }
                else if (Input.IsKeyDown(Keys.G))
                {
                    GiveData data = new GiveData();
                    data.X = 0;
                    data.Y = 0;
                    entity = new GiveBreakWithHead(level, data);
                    block = Block.Null;
                    texBlock = Loader.GiveBreakWithHead;
                }
                else if (Input.IsKeyDown(Keys.X))
                {
                    block = Block.CleRemovableGreen;
                    entity = null;
                    texBlock = Loader.CleRemovable;
                }
                if (entity is Gem)
                {
                    if (Input.IsKeyDown(Keys.Add))
                    {
                        GemData data = new GemData();
                        data.X = 0;
                        data.Y = 0;
                        data.Value = GemValues.Purple1000;
                        entity = new Gem(level, data);
                    }
                    if (Input.IsKeyDown(Keys.Subtract))
                    {
                        GemData data = new GemData();
                        data.X = 0;
                        data.Y = 0;
                        data.Value = GemValues.Pink100;
                        entity = new Gem(level, data);
                    }
                }
                if (entity is Cle)
                {
                    if (Input.IsKeyDown(Keys.Add))
                    {
                        CleData data = new CleData();
                        data.X = 0;
                        data.Y = 0;
                        data.Color = CleColor.Red;
                        entity = new Cle(level, data);
                    }
                    if (Input.IsKeyDown(Keys.Subtract))
                    {
                        CleData data = new CleData();
                        data.X = 0;
                        data.Y = 0;
                        data.Color = CleColor.Blue;
                        entity = new Cle(level, data);
                    }
                    if (Input.IsKeyDown(Keys.Multiply))
                    {
                        CleData data = new CleData();
                        data.X = 0;
                        data.Y = 0;
                        data.Color = CleColor.Yellow;
                        entity = new Cle(level, data);
                    }
                }
                if (entity is GiveBreakWithHead || entity is GiveGoToPetit)
                {
                    if (Input.IsKeyDown(Keys.Add))
                    {
                        GiveData data = new GiveData();
                        data.X = 0;
                        data.Y = 0;
                        data.Type = GiveType.BreakWithHead;
                        entity = new GiveBreakWithHead(level, data);
                        block = Block.Null;
                        texBlock = Loader.GiveBreakWithHead;
                    }
                    if (Input.IsKeyDown(Keys.Subtract))
                    {
                        GiveData data = new GiveData();
                        data.X = 0;
                        data.Y = 0;
                        data.Type = GiveType.GoToPetit;
                        entity = new GiveGoToPetit(level, data);
                        block = Block.Null;
                        texBlock = Loader.GiveGoToPetit;
                    }
                }
                if (block == Block.CleRemovableGreen || block == Block.CleRemovableRed ||
                    block == Block.CleRemovableBlue || block == Block.CleRemovableYellow)
                {
                    if (Input.IsKeyDown(Keys.Add))
                    {
                        block = Block.CleRemovableRed;
                        entity = null;
                        texBlock = Loader.CleRemovable;
                    }
                    if (Input.IsKeyDown(Keys.Subtract))
                    {
                        block = Block.CleRemovableBlue;
                        entity = null;
                        texBlock = Loader.CleRemovable;
                    }
                    if (Input.IsKeyDown(Keys.Multiply))
                    {
                        block = Block.CleRemovableYellow;
                        entity = null;
                        texBlock = Loader.CleRemovable;
                    }
                }

                if (Input.IsKeyPressed(Keys.F2))
                {
                    LoadNextLevel(level.name);
                }
                else if (Input.IsKeyPressed(Keys.F1))
                {
                    levelIndex -= 2;
                    LoadNextLevel(level.name);
                }
                if (Input.IsKeyPressed(Keys.S))
                {
                    string levelPath = String.Format("{0}.xml", DateTime.Now.ToString().Replace('/', '_').Replace(':', '_'));
                    level.SaveTiles(levelPath);
                }
                if (Input.IsKeyDown(Keys.F))
                {
                    wantFill = true;
                }
            }

            if (Input.LeftButton.Down)
            {
                x = (Input.X + (int)posCamera.X) / Tile.Width;
                y = (Input.Y + (int)posCamera.Y) / Tile.Height;
            }
            if (x >= 0 && y >= 0)
            {
                if (entity != null && entity is Teleport)
                    wantPLaceTeleport = true;
                else
                    wantPLaceTeleport = false;
                if (!wantFill)
                {
                    bool haveRemoveOne = false;
                    if (block == Block.Suprimer)
                    {
                        List<Entity> toRemove = new List<Entity>();
                        foreach (Entity e in level.entities)
                        {
                            if (e.Intersects(new Rectangle(Input.X + (int)posCamera.X, Input.Y + (int)posCamera.Y, 1, 1)))
                            {
                                toRemove.Add(e);
                            }
                        }
                        foreach (Entity e in toRemove)
                        {
                            level.entities.Remove(e);
                            haveRemoveOne = true;
                        }
                    }
                    if (!haveRemoveOne && block != Block.Null)
                        level.ChangeTile(level.LoadTile((char)block, x, y));
                    if (entity != null && Input.LeftButton.Pressed)
                    {
                        if (entity is Enemy)
                            entity.Position = RectangleExtensions.GetBottomCenter(level.GetBounds(x, y));
                        else
                            entity.Position = new Vector2(x * Tile.Width, y * Tile.Height);
                        if (!(entity is Teleport))
                            level.entities.Add(entity.Clone());
                    }
                }
                else if (Input.LeftButton.Pressed)
                {
                    if (baseFill == null)
                        baseFill = new Vector2(x, y);
                    else
                    {
                        finalFill = new Vector2(x, y);
                        for (int i = (int)baseFill.Value.X; i <= (int)finalFill.Value.X; i++)
                        {
                            for (int j = (int)baseFill.Value.Y; j <= (int)finalFill.Value.Y; j++)
                            {
                                bool haveRemoveOne = false;
                                if (block == Block.Suprimer)
                                {
                                    List<Entity> toRemove = new List<Entity>();
                                    foreach (Entity e in level.entities)
                                    {
                                        if (e.Intersects(new Rectangle(i * Tile.Width + Tile.Width / 2 + (int)posCamera.X, j * Tile.Height + Tile.Height / 2 + (int)posCamera.Y, 1, 1)))
                                        {
                                            toRemove.Add(e);
                                        }
                                    }
                                    foreach (Entity e in toRemove)
                                    {
                                        level.entities.Remove(e);
                                        haveRemoveOne = true;
                                    }
                                }
                                if (!haveRemoveOne && block != Block.Null)
                                    level.ChangeTile(level.LoadTile((char)block, i, j));
                                if (entity != null)
                                {
                                    if (entity is Enemy)
                                        entity.Position = RectangleExtensions.GetBottomCenter(level.GetBounds(i, j));
                                    else
                                        entity.Position = new Vector2(i * Tile.Width, j * Tile.Height);
                                    if (!(entity is Teleport))
                                        level.entities.Add(entity.Clone());
                                }
                            }
                        }
                        baseFill = null;
                        finalFill = null;
                        wantFill = false;
                    }
                }
            }
            if (entity != null && entity is Teleport && wantPLaceTeleport)
            {
                Keys[] keys = Input.GetDownKeys();
                TeleportData data = (TeleportData)((Teleport)entity).getData();
                if (keys.Contains(Keys.Enter))
                {
                    if (data.Destination == null)
                    {
                        data.Destination = teleportDest;
                        teleportDest = "";
                    }
                    else
                    {
                        data.Index = level.ToInt(teleportDest);
                        data.Y += Tile.Height;
                        level.entities.Add(new Teleport(level, data));
                        wantPLaceTeleport = false;
                        teleportDest = "";
                        data.Destination = null;
                    }
                    entity.setData(data);
                }
                foreach (Keys key in keys)
                {
                    switch (key)
                    {
                        case Keys.NumPad0: { teleportDest += "0"; break; }
                        case Keys.NumPad1: { teleportDest += "1"; break; }
                        case Keys.NumPad2: { teleportDest += "2"; break; }
                        case Keys.NumPad3: { teleportDest += "3"; break; }
                        case Keys.NumPad4: { teleportDest += "4"; break; }
                        case Keys.NumPad5: { teleportDest += "5"; break; }
                        case Keys.NumPad6: { teleportDest += "6"; break; }
                        case Keys.NumPad7: { teleportDest += "7"; break; }
                        case Keys.NumPad8: { teleportDest += "8"; break; }
                        case Keys.NumPad9: { teleportDest += "9"; break; }
                    }
                    if (data.Destination == null)
                    {
                        if (key == Keys.A || key == Keys.Z || key == Keys.E || key == Keys.R || key == Keys.T || key == Keys.Y ||
                            key == Keys.U || key == Keys.I || key == Keys.O || key == Keys.P || key == Keys.Q || key == Keys.S ||
                            key == Keys.D || key == Keys.F || key == Keys.G || key == Keys.H || key == Keys.J || key == Keys.K ||
                            key == Keys.L || key == Keys.M || key == Keys.W || key == Keys.X || key == Keys.C || key == Keys.V ||
                            key == Keys.B || key == Keys.N)
                        {
                            teleportDest += key.ToString().ToLower();
                        }
                    }

                }
            }

#endif
        }

        private void HandleInput()
        {
            if (Input.IsKeyDown(Keys.Escape))
                Game.Exit();

            bool continuePressed = Input.IsKeyDown(Keys.Space);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Player.IsAlive && ViePlayer < 0)
                {
                    level.StartNewLife();
                    ReloadCurrentLevel();
                    ViePlayer = 3;
                }
                else if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }

                if (level.ReachedExit)
                {
                    /*string levelPath = "Content/Levels/" + levelIndex + ".xml";
                    level.SaveScore("scoreOfLevel" + levelIndex);
                    levelPath = "Content/" + "Saved/General.txt";
                    using (StreamWriter writer = new StreamWriter(levelPath))
                    {
                        writer.WriteLine(ViePlayer.ToString());
                        for (int i = 0; i < nivFini.Length - 1; i++)
                        {
                            if (nivFini[i])
                            {
                                writer.WriteLine(1);
                            }
                            else
                            {
                                writer.WriteLine(0);
                            }
                        }
                    }*/
                    LoadNextLevel(level.nameNextLevel, level.indexNextLevel);
                }
            }

            wasContinuePressed = continuePressed;
        }

        public void LoadNextLevel(String name, int index)
        {
            // Find the path of the next level.
            string levelPath;

            levelIndex = index - 1;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Levels/{0}{1}.xml", name, ++levelIndex);
                levelPath = "Content/" + levelPath;
                if (File.Exists(levelPath))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            level = new Level(Game.Services, this, LevelData.Deserialize(levelPath), name, levelIndex);
        }
        public void LoadNextLevel(String name)
        {
            LoadNextLevel(name, ++levelIndex);
        }
        public void LoadNextLevel(String name, TimeSpan timeOfLevel)
        {
            LoadNextLevel(name);
            level.TimeRemaining = timeOfLevel;
        }
        public void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel(level.name, level.TimeRemaining);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                level.Draw(gameTime, spriteBatch, posCamera);
#if DEBUG
                if (texBlock != null)
                {
                    spriteBatch.Draw(texBlock, new Vector2(((int)Input.X - posCamera.X) / Tile.Width * Tile.Width, ((int)Input.Y - posCamera.Y) / Tile.Height * Tile.Height) + posCamera, Color.White);
                }
#endif
#if !DEBUG
                DrawHud(spriteBatch);
#endif
            }

        }
        private void DrawHud(SpriteBatch spriteBatch)
        {
            Rectangle titleSafeArea = Game.Viewport;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TEMPS: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor = Color.Yellow;
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor, spriteBatch);
            
            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow, spriteBatch);

            //Draw vie
            DrawShadowedString(hudFont, "VIES: " + Math.Max(0, ViePlayer).ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 2), Color.Yellow, spriteBatch);

            // Determine the status overlay message to show.
            Texture2D status = null;

            if (level.ReachedExit)
            {
                DrawScoreOfCurrentLevel(hudFont, spriteBatch);
            }
            if (!level.Player.IsAlive)
            {
                status = diedOverlay;
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }
        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(2.0f, 1.0f), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(font, value, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        private void DrawScoreOfCurrentLevel(SpriteFont font, SpriteBatch spriteBatch)
        {
            Rectangle fenetre = new Rectangle((Game.Viewport.Width - 300) / 2, 200, 300, 300);
            DrawShadowedString(font, "SCORE : " + level.Score, new Vector2(fenetre.X + fenetre.Width * 0.1f, fenetre.Y + fenetre.Height * 0.1f), Color.Yellow, spriteBatch);
            spriteBatch.Draw(texGem, new Vector2(fenetre.X + fenetre.Width * 0.2f, fenetre.Y + fenetre.Height * 0.2f), Color.Yellow);
            DrawShadowedString(font, level.nombreGem10Recolte + "/" + level.nombreGem10, new Vector2(fenetre.X + fenetre.Width * 0.2f + texGem.Width, fenetre.Y + fenetre.Height * 0.2f), Color.Yellow, spriteBatch);
            spriteBatch.Draw(texGem, new Vector2(fenetre.X + fenetre.Width * 0.2f, fenetre.Y + fenetre.Height * 0.3f), new Color(255, 83, 145));
            DrawShadowedString(font, level.nombreGem100Recolte + "/" + level.nombreGem100, new Vector2(fenetre.X + fenetre.Width * 0.2f + texGem.Width, fenetre.Y + fenetre.Height * 0.3f), Color.Yellow, spriteBatch);
            spriteBatch.Draw(texGem, new Vector2(fenetre.X + fenetre.Width * 0.2f, fenetre.Y + fenetre.Height * 0.4f), new Color(180, 20, 180));
            DrawShadowedString(font, level.nombreGem1000Recolte + "/" + level.nombreGem1000, new Vector2(fenetre.X + fenetre.Width * 0.2f + texGem.Width, fenetre.Y + fenetre.Height * 0.4f), Color.Yellow, spriteBatch);
            spriteBatch.Draw(texVie, new Vector2(fenetre.X + fenetre.Width * 0.2f, fenetre.Y + fenetre.Height * 0.6f), Color.White);
            DrawShadowedString(font, level.nombreVieRecolte + "/" + level.nombreVie, new Vector2(fenetre.X + fenetre.Width * 0.2f + texVie.Width, fenetre.Y + fenetre.Height * 0.6f), Color.Yellow, spriteBatch);
            spriteBatch.Draw(texMonstre, new Vector2(fenetre.X + fenetre.Width * 0.2f - 16, fenetre.Y + fenetre.Height * 0.8f - 32), Color.White);
            DrawShadowedString(font, level.nombreEnemyRecolte + "/" + level.nombreEnemy, new Vector2(fenetre.X + fenetre.Width * 0.2f + texMonstre.Width / 2, fenetre.Y + fenetre.Height * 0.8f), Color.Yellow, spriteBatch);
        }
    }
}
