using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Camera
{
    public class CameraClass
    {
        public Matrix Transform;
        Viewport ViewPort;
        Vector2 Centre;
        public int Height, Width;
        public Vector2 CameraPosition = new Vector2(0, 2);
        public MouseState MouseStateCurrent, MouseStatePrevious;
        public CameraClass(Viewport newViewport)
        {
            ViewPort = newViewport;
        }
        public void update()//checking mouse position to change camera's position
        {
            if (MouseStateCurrent.X > Width - 20)
            {
                CameraPosition.X += 8;
                if (CameraPosition.X > 1126)
                {
                    CameraPosition.X = 1126;
                }
            }

            if (MouseStateCurrent.Y > Height - 20)
            {
                CameraPosition.Y += 8;
                if (CameraPosition.Y > 740)
                {
                    CameraPosition.Y = 740;
                }
            }
            if (MouseStateCurrent.X < 20)
            {
                CameraPosition.X -= 8;
                if (CameraPosition.X < 2)
                {
                    CameraPosition.X = 2;
                }
            }
            if (MouseStateCurrent.Y < 20)
            {
                CameraPosition.Y -= 8;
                if (CameraPosition.Y < 2)
                {
                    CameraPosition.Y = 2;
                }
            }
            Centre = new Vector2(CameraPosition.X, CameraPosition.Y);
            Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-Centre.X, -Centre.Y, 0));


        }


    }
}