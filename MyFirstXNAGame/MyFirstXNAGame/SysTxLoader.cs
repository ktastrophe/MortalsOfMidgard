using System;
using System.Collections.Generic;
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
    /// /// This is where the game's GUI and system graphics are going to be loaded, in the final version, 
    /// to slim down Game1.cs. 
    /// </summary>
    public class SysTxLoader : Microsoft.Xna.Framework.GameComponent
    {
        public Texture2D titleTex, msgRectBG, statusBarBG, borderBG,
        mapScrBG, mapScrTint,
            sideBarBG, invBG;// packBtn, mapBtn, lookBtn, eatBtn, logBtn; //system button textures

        //Generic holder variables for overlays (replaced with map-specific objects) 
        public Texture2D overlayTex,
            scrOverlay, 
        //Tile overlays
        tileOverlayLwrR, tileOverlayRight, tileOverlayLwrL,
            tileOverlayBtm; //feathered edge overlays

        

        public SysTxLoader(Game game)
            : base(game)
        {
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            //Tile overlays
            tileOverlayRight = Game.Content.Load<Texture2D>("right edge overlay");
            tileOverlayLwrR = Game.Content.Load<Texture2D>("right low corner overlay");
            tileOverlayLwrL = Game.Content.Load<Texture2D>("left low corner overlay");
            tileOverlayBtm = Game.Content.Load<Texture2D>("bottom edge overlay");

            //system textures 
            borderBG = Game.Content.Load<Texture2D>("fs_border");
            invBG = Game.Content.Load<Texture2D>("inventory bg alt");
            sideBarBG = Game.Content.Load<Texture2D>("sidebar bg");

            titleTex = Game.Content.Load<Texture2D>("title screen");
            mapScrBG = Game.Content.Load<Texture2D>("yggdrassil v3");
            mapScrTint= Game.Content.Load<Texture2D>("yggdrassil highlight");
            msgRectBG = Game.Content.Load<Texture2D>("messageBG");
            statusBarBG = Game.Content.Load<Texture2D>("barsBG");

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            

            base.Update(gameTime);
        }
    }
}