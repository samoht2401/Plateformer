using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Plateformer
{
    public class Bouton
    {
        MouseState mouseState;
        PlatformerGame game;
        
        enum Etat
        {
            Relache,
            Appui,
        }
        Etat etat;
        Texture2D texRelache;
        Texture2D texAppui;
        Texture2D tex;
        Rectangle position;
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }
        string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        SpriteFont font;

        public Bouton(PlatformerGame game, string path, string text, Rectangle position)
        {
            this.game = game;
            etat = Etat.Relache;
            texRelache = game.Content.Load<Texture2D>(path + "Relache");
            texAppui = game.Content.Load<Texture2D>(path + "Appui");
            tex = texRelache;
            font = game.Content.Load<SpriteFont>(path + "font");
            this.position = position;
            this.text = text;
        }

        public void Update()
        {
            if (mouseState.LeftButton.Down &&
                position.Contains(new Point(Input.X, Input.Y)))
            {
                etat = Etat.Appui;
                tex = texAppui;
            }
            else
            {
                etat = Etat.Relache;
                tex = texRelache;
            }
        }

        public bool IsPress()
        {
            if (etat == Etat.Appui)
                return true;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, position, Color.White);
            spriteBatch.DrawString(font,text,new Vector2(position.X,position.Y),Color.GreenYellow);
        }
    }
}
