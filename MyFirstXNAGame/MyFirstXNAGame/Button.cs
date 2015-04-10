using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFirstXNAGame
{
    public class Button
    {
        public Texture2D tex;
        public Rectangle pos, statPos, hoverPos;
        public Color tint = Color.White;
        public Color altTint = Color.PaleGoldenrod;
        public Keys keyMap;
        
        public Button(Texture2D t, float x, float y, Keys k)
        {
            keyMap = k;
            tex = t;
            pos = new Rectangle((int)x, (int)y + t.Height, t.Width, t.Height);
            statPos = new Rectangle((int)x, (int)y + t.Height, t.Width, t.Height); 
            hoverPos = new Rectangle((int)x + 2, ((int)y + t.Height) - 2, t.Width, t.Height);

        }
        public Button(Texture2D t, int x, int y, Keys k)
        {
            keyMap = k;
            tex = t;
            pos = new Rectangle(x, y - t.Height, t.Width, t.Height);
            statPos = new Rectangle(x, y - t.Height, t.Width, t.Height);
            hoverPos = new Rectangle(x + 2, (y - t.Height) - 2, t.Width, t.Height);
        }
        
        /// <summary>
        /// Gets called when left-click is registered on a GUI button
        /// </summary>
        /// <param name="myGame">Game object</param>
        /// <param name="newMState">New MouseState</param>
        /// <param name="plr">Player object</param>
        /// <param name="gameTime">Snapshot of game timing values</param>
        /// <param name="m">Message string for display on game field</param>
        public static void MouseClick(Game1 myGame, MouseState newMState, Player plr, GameTime gameTime, ref string m, ref string h)
        {
            SoundEffect[] fxArr = SoundUpdater.fxList;
            foreach (Button b in Game1.btnList)
                if (b.pos.Contains(newMState.X, newMState.Y))
                {
                    Game1.sndUpdater.UpdateSound(fxArr[(int)SoundUpdater.EffectsIDs.clickFX]);
                    Game1.keyHndlr.Update(gameTime, b.keyMap, plr, ref m, ref h);
                    return;
                }

            
        }
        public static void MouseOver(MouseState oState, MouseState nState, List<Button> bList)
        {
            Rectangle tmpRect;
            SoundEffect[] fxArr = SoundUpdater.fxList;
            foreach (Button b in bList)
            {
                if (b.statPos.Contains(nState.X, nState.Y))  
                {

                    if (b.statPos.Contains(oState.X, oState.Y))
                    {
                        continue;
                    }

                        b.tint = new Color(b.altTint.ToVector4());//(b.altTint, 255);
                        
                        Game1.sndUpdater.UpdateSound(fxArr[(int)SoundUpdater.EffectsIDs.hoverFX]);
                    
                    tmpRect = b.hoverPos;
                    
                }
                else 
                {
                    b.tint = Color.White;
                    tmpRect = b.statPos;
                    
                }
                
                
                b.pos = new Rectangle(tmpRect.X, tmpRect.Y, tmpRect.Width, tmpRect.Height);
            }
        }
    }
}
