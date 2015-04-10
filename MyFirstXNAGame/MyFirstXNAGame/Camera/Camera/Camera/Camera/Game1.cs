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

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;



        public  CameraClass camera;//create camera object!!!!!!!!!!

        Texture2D backGroundTexture;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }



        protected override void Initialize()
        {


            base.Initialize();
        }

       


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new CameraClass(GraphicsDevice.Viewport);// we have to use a viewport to create it!!!!!!!!!
            camera.Height = GraphicsDevice.Viewport.Height;//set sizes for viewport!!!!!!
            camera.Width = GraphicsDevice.Viewport.Width;

            backGroundTexture = Content.Load<Texture2D>("background");

        }



        protected override void UnloadContent()
        {

        }



        protected override void Update(GameTime gameTime)
        {

            camera.MouseStateCurrent = Mouse.GetState();//to give position of mouse. In my game I controlled it with mouse but you can just give the position of your character


            camera.update();//here you have to write your codes to move camera in a way you want it to move in my game I wanted it to move when mouse come closer to sides.

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.Transform);//this method could be a problem for you just try to find correct version for 3.0 if its not working
            spriteBatch.Draw(backGroundTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
