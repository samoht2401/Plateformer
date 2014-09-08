﻿using System;
using System.Collections.Generic;
using SharpDX.Toolkit.Input;
using Plateformer;
using Plateformer.Tiles;
using Plateformer.Entities;

namespace Plateformer
{
    public static class Input
    {
        public static PlatformerGame Game;
        private static KeyboardState state;
        private static MouseState mouseState;

        public static void Initilize(PlatformerGame game)
        {
            Game = game;
        }

        public static void Update()
        {
            state = Game.Keyboard.GetState();
            mouseState = Game.Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key);
        }
        public static bool IsKeyUp(Keys key)
        {
            return !state.IsKeyDown(key);
        }
        public static bool IsKeyPressed(Keys key)
        {
            return state.IsKeyPressed(key);
        }
        public static bool IsKeyReleased(Keys key)
        {
            return state.IsKeyReleased(key);
        }

        public static Keys[] GetDownKeys()
        {
            List<Keys> keys = new List<Keys>();
            state.GetDownKeys(keys);
            return keys.ToArray();
        }

        public static ButtonState LeftButton { get { return mouseState.LeftButton; } }
        public static ButtonState RightButton { get { return mouseState.RightButton; } }
        public static ButtonState MiddleButton { get { return mouseState.MiddleButton; } }
        public static int WheelDelta { get { return mouseState.WheelDelta; } }

        public static int X { get { return (int)(mouseState.X * Game.Viewport.Width); } }
        public static int Y { get { return (int)(mouseState.Y * Game.Viewport.Height); } }
    }
}
