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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace MyFirstXNAGame
{
    /// <summary>
    /// Loads and updates all animations of the AnimatedTexture class.
    
    /// </summary>
    public class AnimationUpdater : Microsoft.Xna.Framework.GameComponent
    {
        public static AnimatedTexture slashAtkTexture ;
       
                            
        public AnimationUpdater(Game1 game)
            : base(game)
        {
            slashAtkTexture = new AnimatedTexture(Vector2.Zero, 0, 0, 0);
            slashAtkTexture.Load(Game.Content, "slash attack ani", 7, 30);
            slashAtkTexture.Stop();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
           
            

            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            Overlay curOverlay = Game1.curMap.overlay;
            int overX = Game1.overlayX, overY = Game1.overlayY;
            int overWdth = Game1.txObj.overlayTex.Width, overHght = Game1.txObj.overlayTex.Height; 

            curOverlay.destRectTopL = new Rectangle(0, 0, overX, overHght - overY);
            curOverlay.srcRectTopL = new Rectangle(overWdth - overX,
                overY, overX, overHght - overY);

            curOverlay.destRectTopR = new Rectangle(overX, 0, 
                overWdth - overX, overHght - overY);
            curOverlay.srcRectTopR = new Rectangle(0, overY, overWdth - overX,
                overHght - overY);

            curOverlay.destRectBtmL = new Rectangle(0, overHght - overY, overX, overY);
            curOverlay.srcRectBtmL = new Rectangle(overWdth - overX, 0, overX, overY);

            curOverlay.destRectBtmR = new Rectangle(overX, overHght - overY, overWdth - overX, overY);
            curOverlay.srcRectBtmR = new Rectangle(0, 0, overWdth - overX, overY);

            Game1.curMap.overlay = curOverlay;

            base.Update(gameTime);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public bool UpdateAni(AnimatedTexture aniTx, float elapsed)
        {
            // TODO: Add your update code here
            aniTx.UpdateFrame(elapsed);

            if (aniTx.Frame / (aniTx.framecount - 1) == 1)
            {

                aniTx.Stop();
                return false;
                
            }
            return true;
            //base.Update(gameTime);
        }
    }
}