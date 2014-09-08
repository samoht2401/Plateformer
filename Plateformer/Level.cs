using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using Plateformer.Data;
using Plateformer.Tiles;
using Plateformer.Entities;
using Plateformer.Entities.Player;

namespace Plateformer
{
    public class Level : IDisposable
    {
        // Physical structure of the level.
        private static Level instance;
        public static Level Instance { get { return instance; } }

        private Tile[,] tiles;
        public Tile[,] Tiles
        {
            get { return tiles; }
        }

        private Texture2D[] layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }
        Player player;

        public List<Entity> entities = new List<Entity>();

        // Key locations in the level.        
        private Vector2 start;
        private Point exit = InvalidPosition;
        public Vector2 lastCheckPoint;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        public Jeu jeu;
        public String name;

        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        int score;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;
        public string nameNextLevel;
        public int indexNextLevel;

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
            set
            {
                timeRemaining = value;
            }
        }
        TimeSpan timeRemaining;

        private const int PointsPerSecond = 5;

        private LevelData dataOfLevel;

        private SoundEffect exitReachedSound;

        public int nombreEnemy;
        public int nombreVie;
        public int nombreGem10;
        public int nombreGem100;
        public int nombreGem1000;
        public int nombreEnemyRecolte;
        public int nombreVieRecolte;
        public int nombreGem10Recolte;
        public int nombreGem100Recolte;
        public int nombreGem1000Recolte;

        #region Loading

        public Level(IServiceProvider serviceProvider, Jeu jeu, LevelData levelData, String name, int index)
        {
            instance = this;
            this.jeu = jeu;
            this.name = name;
            this.nameNextLevel = name;
            this.indexNextLevel = index + 1;

            timeRemaining = TimeSpan.FromMinutes(0.0);

            dataOfLevel = levelData;
            LoadTiles(dataOfLevel);
            LoadEntities(dataOfLevel);

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            layers = new Texture2D[3];
            for (int i = 0; i < layers.Length; ++i)
            {
                // Choose a random segment if each background layer for level variety.
                int segmentIndex = random.Next(3);
                layers[i] = jeu.Game.Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex + ".png");
            }

            // Load sounds.
            try
            {
                exitReachedSound = jeu.Game.Content.Load<SoundEffect>("Sounds/ExitReached.wma");
            }
            catch (Exception) { }
        }

        public void SaveTiles(string name)
        {
            LevelData level = new LevelData();
            char[,] tab = new char[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (tiles[i, j] is TileAir)
                        tab[i, j] = (char)Jeu.Block.Suprimer;

                    if (tiles[i, j] is TileGrass)
                        tab[i, j] = (char)Jeu.Block.Terre;
                    else if (tiles[i, j] is TileBreakable)
                        tab[i, j] = (char)Jeu.Block.Case;
                    else if (tiles[i, j] is TilePlateform)
                        tab[i, j] = (char)Jeu.Block.Platform;
                    else if (tiles[i, j] is TileWater)
                        tab[i, j] = (char)Jeu.Block.Eau;
                    else if (tiles[i, j] is TileCheckPoint)
                        tab[i, j] = (char)Jeu.Block.CheckPoint;
                    else if (tiles[i, j] is TileDepart)
                        tab[i, j] = (char)Jeu.Block.Player;
                    else if (tiles[i, j] is TileExit)
                        tab[i, j] = (char)Jeu.Block.Sortit;
                    else if (tiles[i, j] is TileCleRemovable)
                    {
                        if (tiles[i, j].Color == CleColor.Red)
                            tab[i, j] = (char)Jeu.Block.CleRemovableRed;
                        if (tiles[i, j].Color == CleColor.Blue)
                            tab[i, j] = (char)Jeu.Block.CleRemovableBlue;
                        if (tiles[i, j].Color == CleColor.Yellow)
                            tab[i, j] = (char)Jeu.Block.CleRemovableYellow;
                        if (tiles[i, j].Color == CleColor.Green)
                            tab[i, j] = (char)Jeu.Block.CleRemovableGreen;
                    }
                }
            }
            foreach (Entity entity in entities)
            {
                if (entity is Enemy)
                    level.Enemies.Add((EnemyData)entity.getData());
                else if (entity is Gem)
                    level.Gems.Add((GemData)entity.getData());
                else if (entity is Cle)
                    level.Cles.Add((CleData)entity.getData());
                else if (entity is Live)
                    level.Lives.Add((LiveData)entity.getData());
                else if (entity is Teleport)
                    level.Teleports.Add((TeleportData)entity.getData());
                else if (entity is GiveBreakWithHead)
                    level.Gives.Add((GiveData)entity.getData());
                else if (entity is GiveGoToPetit)
                    level.Gives.Add((GiveData)entity.getData());
            }
            level.Width = Width;
            level.Height = Height;
            level.Score = new ScoreData();
            level.Score.MeilleurScore = 8365;
            level.Score.ScoreForA = 6000;
            level.Score.ScoreForB = 5000;
            level.Score.ScoreForC = 4000;
            level.Score.ScoreForD = 3000;
            level.Score.ScoreForE = 1000;
            level.Score.LastLetter = 'E';
            level.Tiles = new List<string>();
            string line = "";
            for (int j = 0; j < tab.GetLength(1); j++)
            {
                line = "";
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    line = line + tab[i, j];
                }
                level.Tiles.Add(line);
            }

            LevelData.Serialize(name, level);
        }
        public void SaveScore(string nameoffile)
        {
            nameoffile += ".sav";

            GestionScore gestionScore = new GestionScore();
            GestionScore presedentScore = GestionScore.Load("Saved/Level", nameoffile);

            if (presedentScore == null || presedentScore.MeilleurScore < Score)
            {
                gestionScore.MeilleurScore = Score;
            }
            else
            {
                gestionScore.MeilleurScore = presedentScore.MeilleurScore;
            }
            gestionScore.IsFinish = true;

            GestionScore.Save("Saved/Level", nameoffile, gestionScore);
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

        private void LoadTiles(LevelData levelData)
        {
            // Allocate the tile grid.
            tiles = new Tile[levelData.Width, levelData.Height];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    char tileType = levelData.Tiles[y][x];//tabChar[x, y];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
#if!DEBUG
            if (Player.Position.X - jeu.posCamera.X < jeu.Game.GraphicsDevice.Viewport.Width / 3)
                jeu.posCamera.X -= (jeu.Game.GraphicsDevice.Viewport.Width / 3) - (Player.Position.X - jeu.posCamera.X);

            else if (Player.Position.X - jeu.posCamera.X + Player.BoundingRectangle.Width > jeu.Game.GraphicsDevice.Viewport.Width / 3 * 2)
                jeu.posCamera.X += (Player.Position.X - jeu.posCamera.X + Player.BoundingRectangle.Width) - (jeu.Game.GraphicsDevice.Viewport.Width / 3 * 2);

            if (Player.Position.Y - jeu.posCamera.Y < jeu.Game.GraphicsDevice.Viewport.Height / 3)
                jeu.posCamera.Y -= (jeu.Game.GraphicsDevice.Viewport.Height / 3) - (Player.Position.Y - jeu.posCamera.Y);

            else if (Player.Position.Y - jeu.posCamera.Y + Player.BoundingRectangle.Height > jeu.Game.GraphicsDevice.Viewport.Height / 3 * 2)
                jeu.posCamera.Y += (Player.Position.Y - jeu.posCamera.Y + Player.BoundingRectangle.Height) - (jeu.Game.GraphicsDevice.Viewport.Height / 3 * 2);

            jeu.posCamera.X = MathUtil.Clamp(jeu.posCamera.X, 0.0f, Width * Tile.Width - jeu.Game.GraphicsDevice.Viewport.Width);
            jeu.posCamera.Y = MathUtil.Clamp(jeu.posCamera.Y, 0.0f, Height * Tile.Height - jeu.Game.GraphicsDevice.Viewport.Height);

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");
#endif
        }

        private void LoadEntities(LevelData levelData)
        {
            foreach (EnemyData entityData in levelData.Enemies)
            {
                entities.Add(new Enemy(this, entityData));
            }
            foreach (GemData entityData in levelData.Gems)
            {
                entities.Add(new Gem(this, entityData));
            }
            foreach (LiveData entityData in levelData.Lives)
            {
                entities.Add(new Live(this, entityData));
            }
            foreach (CleData entityData in levelData.Cles)
            {
                entities.Add(new Cle(this, entityData));
            }
            foreach (TeleportData entityData in levelData.Teleports)
            {
                entities.Add(new Teleport(this, entityData));
            }
            foreach (GiveData entityData in levelData.Gives)
            {
                switch (((GiveData)entityData).Type)
                {
                    case GiveType.BreakWithHead:
                        {
                            entities.Add(new GiveBreakWithHead(this, entityData));
                            break;
                        }
                    case GiveType.GoToPetit:
                        {
                            entities.Add(new GiveGoToPetit(this, entityData));
                            break;
                        }
                }
            }
        }

        public Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '#':
                    return new TileGrass(this, x, y);
                case '~':
                    return new TileWater(this, x, y);
                case '1':
                    return LoadTileDepart(x, y);
                case 'X':
                    return LoadExitTile(x, y);
                case 'P':
                    return new TileCheckPoint(this, x, y);
                case '=':
                    return new TilePlateform(this, x, y);
                case ')':
                    return new TileBreakable(this, x, y);
                case '+':
                    return new TileCleRemovable(this, x, y, CleColor.Red);
                case '-':
                    return new TileCleRemovable(this, x, y, CleColor.Blue);
                case '*':
                    return new TileCleRemovable(this, x, y, CleColor.Yellow);
                case '/':
                    return new TileCleRemovable(this, x, y, CleColor.Green);
                default:
                    return new TileAir(this, x, y);
            }
        }
        private Tile LoadTileDepart(int x, int y)
        {
#if!DEBUG
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");
#endif
            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            lastCheckPoint = start;
            player = new Player(this, jeu, start);

            return new TileDepart(this, x, y);
        }
        private Tile LoadExitTile(int x, int y)
        {
#if!DEBUG
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");
#endif
            exit = new Point(GetBounds(x, y).Right, GetBounds(x, y).Bottom - GetBounds(x, y).Height / 2);

            return new TileExit(this, x, y);
        }

        public void BriseTile(int x, int y)
        {
            ChangeTile(new TileAir(this, x, y));
            Random r = new Random(2 ^ 4 * x + 2 ^ 6 * y);
            if (r.Next(100) < 50 && player.Level.Tiles[x, y] is TileAir)
            {
                if (r.Next(100) < 10)
                {
                    if (r.Next(100) < 10)
                        Gem.SpawnGem(x * Tile.Width, y * Tile.Height, GemValues.Purple1000);
                    else
                        Gem.SpawnGem(x * Tile.Width, y * Tile.Height, GemValues.Pink100);
                }
                else
                    Gem.SpawnGem(x * Tile.Width, y * Tile.Height, GemValues.Yellow10);
            }
        }
        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Bounds and collision

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }
        public int Width
        {
            get { return tiles.GetLength(0); }
        }
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Pause while the player is dead or time is expired.
            if (!Player.IsAlive)
            {
                // Still want to perform physics on the player.
                Player.Mode.ApplyPhysics(gameTime);
            }
            else if (ReachedExit)
            {
                // Animate the time being converted into points.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score -= seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining += gameTime.ElapsedGameTime;

                Player.Update(gameTime);

                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

                UpdateEntities(gameTime);


                for (int y = Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        tiles[x, y].onUpdate(gameTime);
                    }
                }
            }
        }

        public void setTile(Tile tile)
        {
            tiles[tile.X, tile.Y] = tile;
            if (tile.X > 0)
                tile = tiles[tile.X - 1, tile.Y].onTileNextToChange(tile);
            if (tile.X < Height - 1)
                tile = tiles[tile.X + 1, tile.Y].onTileNextToChange(tile);
            if (tile.Y > 0)
                tile = tiles[tile.X, tile.Y - 1].onTileNextToChange(tile);
            if (tile.Y < Width - 1)
                tile = tiles[tile.X, tile.Y + 1].onTileNextToChange(tile);
            tiles[tile.X, tile.Y] = tile;
        }

        private void UpdateEntities(GameTime gameTime)
        {
            foreach (Entity entity in entities)
                entity.Update(gameTime);
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].MustBeRemove)
                {
                    entities.Remove(entities[i]);
                    i--;
                }
            }
        }

        public void onTileChange(Tile tile)
        {
            if (tile.X > 0)
                tiles[tile.X, tile.Y] = tiles[tile.X - 1, tile.Y].onTileNextToChange(tile);
            if (tile.X < Width - 1)
                tiles[tile.X, tile.Y] = tiles[tile.X + 1, tile.Y].onTileNextToChange(tile);
            if (tile.Y > 0)
                tiles[tile.X, tile.Y] = tiles[tile.X, tile.Y - 1].onTileNextToChange(tile);
            if (tile.Y < Height - 1)
                tiles[tile.X, tile.Y] = tiles[tile.X, tile.Y + 1].onTileNextToChange(tile);
        }

        public void ChangeTile(Tile tile)
        {
            int newX = Width - 1;
            int newY = Height - 1;
            if (tile.X >= Width)
                newX = tile.X;
            if (tile.Y >= Height)
                newY = tile.Y;

            if (newX != Width - 1 || newY != Height - 1)
            {
                Tile[,] oldTab = tiles;
                tiles = new Tile[newX + 1, newY + 1];
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < tiles.GetLength(1); y++)
                    {
                        if (x < oldTab.GetLength(0) && y < oldTab.GetLength(1))
                            tiles[x, y] = oldTab[x, y];
                        else
                            tiles[x, y] = new TileAir(this, x, y);
                    }
                }
            }

#if DEBUG
            if (tiles[tile.X, tile.Y] is TileDepart && !(tile is TileDepart))
                player = null;
            if (tile is TileDepart)
            {
                foreach (Tile t in tiles)
                {
                    if (t is TileDepart)
                        tiles[t.X, t.Y] = new TileAir(this, t.X, t.Y);
                }
            }
            if (tile is TileExit)
            {
                foreach (Tile t in tiles)
                {
                    if (t is TileExit)
                        tiles[t.X, t.Y] = new TileAir(this, t.X, t.Y);
                }
            }
            /*if (tile is TileAir)
            {
                Vector2 tilePos = RectangleExtensions.GetBottomCenter(GetBounds(tile.X, tile.Y));
                Entity toRemove = null;
                foreach (Entity entity in entities)
                {
                    if (entity.Position.X == tilePos.X && entity.Position.Y == tilePos.Y)
                    {
                        toRemove = entity;
                    }
                }
                if (toRemove != null)
                    entities.Remove(toRemove);
            }*/
#endif

            tiles[tile.X, tile.Y] = tile;
            onTileChange(tile);
            tiles[tile.X, tile.Y] = tile;
        }

        public void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
        }

        public void OnExitReached()
        {
            Player.OnReachedExit();
            if (exitReachedSound != null)
                exitReachedSound.Play();
            reachedExit = true;
        }
        public void OnExitReached(String name, int index)
        {
            Player.OnReachedExit();
            if (exitReachedSound != null)
                exitReachedSound.Play();
            reachedExit = true;
            nameNextLevel = name;
            indexNextLevel = index;
        }

        public void StartNewLife()
        {
            Player.Reset(lastCheckPoint);
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            for (int i = 0; i <= EntityLayer; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f - 0.01f * i);

            DrawTiles(spriteBatch, posCamera);

#if DEBUG
            if (player != null)
            {
                Player.Draw(gameTime, spriteBatch, posCamera);
            }
#endif
#if!DEBUG
            Player.Draw(gameTime, spriteBatch, posCamera);
#endif
            DrawEntities(gameTime, spriteBatch, posCamera);

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
        }
        private void DrawTiles(SpriteBatch spriteBatch, Vector2 posCamera)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    Tile tile = tiles[x, y];
                    if (texture != null)
                    {
                        /*if (tiles[x, y] is TileExit)
                        {
                            Vector2 position = new Vector2(x, y) * Tile.Size + new Vector2(texture.Width / 2, 0);
                            spriteBatch.Draw(texture, new Rectangle((int)position.X - (int)posCamera.X, (int)position.Y - (int)posCamera.Y, texture.Width, texture.Height), null, Color.White, exitTileRotation, new Vector2(texture.Width / 2 + 2, texture.Height / 2 - 1), SpriteEffects.None, tile.LayerDepth);
                        }*/
                        // Draw it in screen space.
                        Vector2 pos = tile.Position - posCamera;
                        spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, tile.WidthForDraw, tile.HeightForDraw), tile.SourceRectangle, tile.Color, tile.Rotation, tile.Origin, SpriteEffects.None, tile.LayerDepth);

                    }
                }
            }
        }
        private void DrawEntities(GameTime gameTime, SpriteBatch spriteBatch, Vector2 posCamera)
        {
            foreach (Entity entity in entities)
            {
                entity.Draw(gameTime, spriteBatch, posCamera);
            }
        }

        #endregion
    }
}