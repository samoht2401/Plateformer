using System;
using SharpDX.Toolkit.Graphics;

namespace Plateformer.Animations
{
    /// <summary>
    /// Représente un animation 2D.
    /// </summary>
    /// <remarks>
    /// Cette clase ne gère que des animation dont les images sont carré.
    /// </remarks>
    public class Animation
    {
        /// <summary>
        /// Toute les images de l'animation mise en ligne horizontal.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Durée qu'est affichée chaque image.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// Défini, si l'animation tourne en boucle.
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Donne le nombre d'image dans l'animation.
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Donne la largeur d'une image de l'animation.
        /// </summary>
        public int FrameWidth
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Donne la hauteur d'une image de l'animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Constructeur d'une nouvelle animation.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}
