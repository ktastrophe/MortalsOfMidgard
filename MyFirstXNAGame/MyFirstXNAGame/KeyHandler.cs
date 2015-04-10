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
using System.IO;


namespace MyFirstXNAGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KeyHandler : Microsoft.Xna.Framework.GameComponent
    {
        Game1 myGame;
        public KeyHandler(Game1 game)
            : base(game)
        {
            myGame = game;
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Keys k, Player p, ref string m, ref string h)
        {
            //letter key actions that DO consume a turn -- no return
            // (ordered roughly by prevalence of use)
            if (k == (Keys.OemComma) || k == (Keys.OemPlus))
            //(currentKey == Keys.OemComma || currentKey == Keys.OemPlus) //pick up item with ,
            {
                bool itemPickedUp = false;
                foreach (Item it in Game1.mapItems)
                {
                    if (it.startingPos == p.absolutePos)
                    {
                        itemPickedUp = true;
                        m += p.pickUp(it, Game1.curMap, m);
                        Game1.pickedUpItems.Add(it);

                    }
                }
                if (!itemPickedUp)
                    m += "You see nothing of use here.";
                //break;

            }
            else if (k == (Keys.E))//(currentKey == Keys.E)
            {
                if (!Game1.isEating)
                {
                    p.itemToUse = null;
                    Game1.isEating = true;
                    h = "Eat Which? (click on an item)";
                    //check if player's tile has food on it first

                    foreach (Item it in Game1.mapItems)
                        if (p.curTile.absPos == it.startingPos)
                        {
                            p.itemToUse = it;
                            m = "There is a " + it.name + " here.\n Eat it? (Y/N)";
                            return;
                        }

                    myGame.showInventory(p.inventory);
                    //isInvShowing = true;

                }
            }
            else if (k == (Keys.I) || k == (Keys.Multiply))// == Keys.I || currentKey == Keys.Multiply) 
            //show inventory screen with 'i' or Numpad *
            {
                //makes sure proper version of screen is shown

                h = "Pack of Worldly Goods";
                //isDropping = false; isSelling = false; isEating = false;
                myGame.showInventory(p.inventory);
                //break;
                return;
            }
            else if (k == (Keys.Q))
            //(currentKey == Keys.Q) //is going to drink
            {

                Game1.isDrinking = true;
                h = "Drink Which? (click on an item)";

                myGame.showInventory(p.inventory);
                //Game1.isInvShowing = true;
            }
            else if (k == (Keys.R))
            //(currentKey == Keys.R) // is going to read
            {
                h = "Read Which? (click on an item)";

                Game1.isReading = true;
                myGame.showInventory(p.inventory);
                //isInvShowing = true;
            }
            
            //drop items with 'd'
            else if (k == (Keys.D))//(currentKey == Keys.D)
            {
                h = "Drop Which? (click on items, then press Enter)";
                Game1.isDropping = true;
                myGame.showInventory(p.inventory);
                //Game1.isInvShowing = true;


            }
            else if (k == (Keys.T))
            {
                if (!Game1.isUsing)
                {
                    Game1.isUsing = true;
                    h = "Use Which?";
                    myGame.showInventory(p.inventory);
                    m = "Use on what? (Left-click on a tile)";
                    //Game1.isInvShowing = true;

                }
            }

            else if (k == (Keys.W))
            //(currentKey == Keys.W) //Wield/Wear--to be replaced later by a Character/Status Screen?
            {
                if (!Game1.isEquipping)
                {

                    h = "Wield/Wear/Remove Which?";
                    Game1.isEquipping = true;
                    myGame.showInventory(p.inventory);
                    //Game1.isInvShowing = true;
                }

            }
            
            else if (k == (Keys.L))
            //(currentKey == Keys.L)
            {

                if (!Game1.isLooking)
                {
                    m = "Look where? (left-click on a tile) ";
                    h = "Items Stacked Here: (press Enter)";
                    Game1.isLooking = true;
                    //return; //remove if no longer working
                    //myGame.UpdateInput(gameTime);
                }
            }
            else if (k == (Keys.P))
            //(currentKey == Keys.P) //pay if in a shop with unpaid items
            {
                if (Game1.tileArray[(int)p.newPos.X, (int)p.newPos.Y].name == "floor")
                {
                    m = p.Pay(m, Game1.curMap);
                }
                else
                    m = "You need not pay anyone here. ";
            }
            else if (k == Keys.A)
            {
                m = "Attack which?";
                Game1.isSafetyOff = true;
            }

                //letter key actions NOT consuming a turn -- DO return
            else if (k == (Keys.S))
            //(currentKey == Keys.S) //save game function (future)
            {
                if (!Directory.Exists(Game1.saveFileDir))
                    Directory.CreateDirectory(Game1.saveFileDir);

                m = GameState.Save(p, Game1.mapArray, Game1.totalTurns);
                return;
            }
            else if (k == (Keys.M))//(currentKey == Keys.M) //show map screen with 'm'
            {
                h = "Map";
                myGame.showMap();
                Game1.isSubScrnShowing = true;
                //break;
                return;
            }

            else if (k == Keys.X)
            {
                if(!Game1.isQuestScrnShowing)
                {
                    h = "Exploits of Valor";
                    Game1.isQuestScrnShowing = true;
                    Game1.isSubScrnShowing = true;
                }
            }

            else if (k == (Keys.B))
            //(currentKey == Keys.B) //shows message log
            {
                if (!Game1.isLogShowing)
                {
                    myGame.logWriter.Close();
                    StreamReader logReader = File.OpenText(Game1.strLogName);
                    h = "Message Log";
                    myGame.logLines = new List<string>();
                    int l = 0;
                    do
                    {
                        l++;
                        myGame.logLines.Add(/*l.ToString() +*/logReader.ReadLine());

                    } while (!logReader.EndOfStream);
                    logReader.Close();

                    Game1.isLogShowing = true;
                    Game1.isSubScrnShowing = true;
                    return;

                }

            }
            
            else
            {
                //no valid key pressed
            }
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}