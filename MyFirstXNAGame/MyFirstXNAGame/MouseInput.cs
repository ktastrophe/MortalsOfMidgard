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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MouseInput : Microsoft.Xna.Framework.GameComponent
    {
        //custom cursor - currentCursor changes between the regular and click sprites
        public Texture2D currentCursor, custCursor, custCursorDown; 
        public Vector2 mousePos = Vector2.Zero;
        public MouseState oldMState, newMState; //for the mouse buttons
        Game1 myGame;
        SpriteFont localFont;
        Player plr;


        public MouseInput(Game1 game)
            : base(game)
        {
            myGame = game;
            Mouse.WindowHandle = Game.Window.Handle;
            myGame.IsMouseVisible = false;
            Mouse.SetPosition(0, 0); 
            newMState = Mouse.GetState();
            custCursor = Game.Content.Load<Texture2D>("cursor");
            currentCursor = custCursor;
            custCursorDown = Game.Content.Load<Texture2D>("cursor click");
            mousePos = new Vector2((float)newMState.X, (float)newMState.Y);
            //Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);

            
            localFont = game.Font1;

            // TODO: Construct any child components here

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            //Mouse.SetPosition(0, 0);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        public void UpdateMouseMovt(GameTime gametime)
        {
           
            // The mouse x and y positions are returned relative to the
            // upper-left corner of the game window.
            oldMState = newMState;
            newMState = Mouse.GetState();
            if (newMState.LeftButton == ButtonState.Released)
                currentCursor = custCursor;

            //if ((Game1.sideBtnRect.Contains(newMState.X, newMState.Y)
            //    || Game1.sideBtnRect.Contains(oldMState.X, oldMState.Y))
            //    || (Game1.bottomBtnRect.Contains(newMState.X, newMState.Y)
            //    || Game1.bottomBtnRect.Contains(oldMState.X, oldMState.Y)))
            //{
                Button.MouseOver(oldMState, newMState, Game1.btnList);
            //}
            
            
            
            mousePos = new Vector2(newMState.X, newMState.Y);
            // Change background color based on mouse position.
            //backColor = new Color((byte)(mouseX / 3), (byte)(mouseY / 2), 0);
            
            base.Update(gametime);

        }
        /// <summary>
        /// InvMouseMovt
        ///     Logic that checks if cursor is over items in the inventory, and highlights the appropriate text
        /// </summary>
        /// <param name="invItems"></param>
        /// <param name="iHeld"></param>
        /// <param name="iDict"></param>
        public void InvMouseMovt(int invItems, List<Item>iHeld, Dictionary<string, int>iDict)
        {
            //bool isHighlit = false;
            if (!(newMState == oldMState))
                for (int i = 0; i < Game1.invRects.Count; i++)
                {

                    ItemRect iR = Game1.invRects[i];
                    iR.txtColor = Color.Wheat;
                    iR.bgColor = Color.SandyBrown;
                    //check items listed against mouse position 
                    if (iR.area.Contains(newMState.X, newMState.Y)) //&& !iR.area.Contains(oldMState.X, oldMState.Y))
                    {
                        iR.txtColor = Color.Peru;
                        iR.bgColor = Color.Sienna;
                        SoundEffect[] fxArr = SoundUpdater.fxList;
                            if(!iR.area.Contains(oldMState.X, oldMState.Y))
                                Game1.sndUpdater.UpdateSound(fxArr[(int)SoundUpdater.EffectsIDs.hoverFX]);
                        
                        //assign the item's texture to be drawn in the thumbnail area
                        Game1.invItemTex = iR.thumbTex;
                            
                            
                            //i++;

                        }
                    }

                
                
        }
        public void SubScrnClick(GameTime gameTime)
        {
            SoundEffect[] fxArr = SoundUpdater.fxList;

            currentCursor = custCursorDown;
            if (newMState != oldMState)
            {
                //gender selection screen - check for click on male (string M) or female (string F)
                if (Game1.chooseGender)
                {

                if (newMState.Y >= 360 && newMState.Y <= 360 + localFont.MeasureString(Game1.M).Y)
                {
                    if (newMState.X >= 280 && newMState.X <= 280 + localFont.MeasureString(Game1.M).X)
                    {
                        myGame.player.gender = Game1.M;
                        Game1.messageGender = "Young Norseman";

                        //newGame = false; 
                        Game1.chooseGender = false;
                        Game1.chooseClass = true;
                    }
                    else if (newMState.X >= 330 && newMState.X <= 330 + localFont.MeasureString(Game1.F).X)
                    {
                        myGame.player.gender = Game1.F;
                        Game1.messageGender = "Fair Norsemaiden";

                        //newGame = false;
                        Game1.chooseGender = false;
                        Game1.chooseClass = true;
                    }

                    myGame.messageString = "Voices from a higher plane speak unto you: \n" +
               Game1.messageGender + ", peace hath prevailed in Valhalla since the destruction\n " +
               "of the physical realms: Ragnarok. But we grow weary of the calm, and\n " +
               "so, hath wound back the hands of time to restore that which was.\n" +
               "We must ask now for proof, through the justness of thine actions,\n" +
               "that the realm of Midgard be worth saving. (Press any key to continue)";

                }
            }


                else if (Game1.chooseClass) //left-click on the class select screen
                {
                    Random r = new Random();

                    Player plrBase = myGame.player;

                    Weapon startWep = null; //player's starting weapon
                    bool isStWepRanged = false;

                    Armor startAmr = null;

                    Vector2 sPos = plrBase.startingPos;
                    string pName = myGame.enteredKeys;
                    string pGen = plrBase.gender;

                    string classN = null, wepN = null, amrN = null, aniTxName = null,
                     txName = null;
                    int wAtk = 0, wDef = 0, wToHit = 0, wWeight = 0,
                        amrDef = 0, amrWgt = 0,
                        aniFrameCount = 0, aniFPS = 0;

                    Texture2D wepTx = null, amrTx = null;

                    //int noOfArrows = 0;


                    AnimatedTexture pSprite = myGame.animTest;


                    //check for click on the top row of classes
                    if (newMState.Y >= 160 && newMState.Y <= 300)
                    {
                        if (newMState.X >= 50 && newMState.X <= 250)
                        {
                            classN = "Viking";
                            wepN = "battle-axe";

                            wepTx = Game.Content.Load<Texture2D>(wepN);
                            txName = "viking";
                            aniTxName = "viking ani";
                            aniFrameCount = 6;
                            aniFPS = 3;
                            amrN = "chain mail";
                            amrTx = Game.Content.Load<Texture2D>("hide cuirass");
                            amrDef = 3; amrWgt = 7;

                            wAtk = 5; wDef = 0; wToHit = 4; wWeight = 9;
                            Game1.newGame = false;
                            Game1.chooseClass = false;

                            //plr = Game1.player;
                        }
                        else if (newMState.X >= 275 && newMState.X <= 475)
                        {
                            //plr.cClass = "alchemist";
                            classN = "Medicine ";
                            wepN = "leister";
                            wepTx = Game.Content.Load<Texture2D>(wepN);
                            amrN = "hide cuirass";
                            amrTx = Game.Content.Load<Texture2D>(amrN);
                            //armor = new Armor("hide cuirass", r.Next(0, 3), 2, false, 6);

                            wAtk = 3; wDef = 1; wToHit = 8; wWeight = 4;
                            amrDef = 1; amrWgt = 4;

                            aniFrameCount = 8;
                            aniFPS = 4;
                            if (plrBase.gender == "Male")
                            {
                                classN += "Man";
                                txName = "alchemist";//("alchemist");
                                aniTxName = "alchemist frames";

                            }
                            else
                            {
                                classN += "Woman";
                                txName = "femalchemist";//("alchemist");
                                aniTxName = "femalchemist frames";

                            }
                            Game1.newGame = false;
                            Game1.chooseClass = false;
                            //plr = Game1.player;
                        }
                        else if (newMState.X > 475)
                        {
                            classN = "Survivalist";
                            wepN = "long bow";
                            isStWepRanged = true;

                            amrN = "hide cuirass";
                            amrDef = 1; amrWgt = 4;

                            amrTx = Game.Content.Load<Texture2D>(amrN);
                            wepTx = Game.Content.Load<Texture2D>(wepN);

                            //noOfArrows = r.Next(10, 21);

                            if (plrBase.gender == "Male")
                            {
                                aniTxName = "survivalist ani";
                                aniFrameCount = 3;
                                aniFPS = 3;
                            }
                            else
                            {
                                aniTxName = "survivalist f";
                                aniFrameCount = 2;
                                aniFPS = 1;
                            }
                            txName = "survivalist";


                            wAtk = 1; wDef = 0; wToHit = 5; wWeight = 5;
                            Game1.newGame = false;
                            Game1.chooseClass = false;


                        }
                        if (classN != null)
                        {
                            if (isStWepRanged)
                                startWep = new RangedWep(wepTx, wepN, r.Next(0, 2),
                                    wAtk, wDef, false, wWeight, wToHit, new Weapon(Item.arrowTx, "arrow", 3, 4, 1, 7));
                            else
                                startWep = new Weapon(wepTx, wepN, r.Next(0, 2),
                                    wAtk, wDef, false, wWeight, wToHit);

                            startAmr = new Armor(amrTx, amrN, r.Next(0, 2), amrDef, false, amrWgt);
                            /*Game1.player*/
                            plr = new Player(Game.Content.Load<Texture2D>(txName), classN,
                                sPos, pName, pGen, startWep, startAmr);
                            plr.curTile = Game1.visibleTilesArray[3, 3];
                            plr.skeleton = Game.Content.Load<Texture2D>("tombstone");
                            pSprite.Load(Game.Content, aniTxName, aniFrameCount, aniFPS);
                            Game1.newGame = false;
                            Game1.chooseClass = false;
                            myGame.player = plr;
                        }

                    }
                }
            }
            if (oldMState.LeftButton == ButtonState.Released)
                Game1.sndUpdater.UpdateSound(fxArr[(int)SoundUpdater.EffectsIDs.clickFX]);
            oldMState = newMState;

            base.Update(gameTime);
        }
        public void Click(GameTime gameTime, /*List<string> iNames,*/ 
            Dictionary<string,int>iNamesAndQty, int lineHeight, ref List<Item> iHeld, 
            /*int[] noHeld,*/ ref string m)
        {

            SoundEffect[] fxArr = SoundUpdater.fxList; 
            
            currentCursor = custCursorDown;
            if (newMState != oldMState )
            {
                
               
                if (Game1.isInvShowing)
                {
                    oldMState = newMState;
                    //int startY = 50;
                    plr.isAttacking = false; // try this to eliminate hang upon using an item next to a monster
                    
                    //use rectangles created in showInventory to compare to click area
                    for (int i = 0; i < iNamesAndQty.Count; i++)
                    {
                        if (Game1.invRects[i].area.Contains(newMState.X, newMState.Y) && iNamesAndQty.Values.ElementAt(i) > 0)//noHeld[i] > 0)
                        {

                            foreach (Item it in iHeld)
                            { //check items held against item name clicked 
                                if (it.name == iNamesAndQty.Keys.ElementAt(i))//iNames[i])
                                {
                                    
                                    //assign the item's texture to be drawn in the thumbnail area
                                    Game1.invItemTex = it.texture;
                                    Type t = it.GetType();
                                    if (Game1.isUsingItemOnItem)
                                    {
                                        object[] paramArray = new object[] { it, Game1.mapArray };

                                        m = (string)plr.methodToInvoke.Invoke(plr, paramArray);
                                        iNamesAndQty[plr.itemToUse.name]--;//noHeld[iHeld.IndexOf(plr.itemToUse)]--;
                                        plr.inventory.Remove(plr.itemToUse);
                                        Game1.isSubScrnShowing = false;
                                        Game1.isUsingItemOnItem = false;
                                        Game1.isInvShowing = false;
                                        
                                        Game1.totalTurns++;
                                        return;
                                    }
                                    if (Game1.isDropping)
                                    { //pass appropriate item to Drop
                                        //if the item is gold, give it a texture according to amount being dropped
                                        Gold g = new Gold(0);
                                        if (it.GetType().IsInstanceOfType(g))
                                        {
                                            g = (Gold)it;
                                            if (g.amount < 50)
                                                it.texture = Game.Content.Load<Texture2D>("gold small");
                                            else if (g.amount >= 50)
                                                it.texture = Game.Content.Load<Texture2D>("gold med");
                                            plr.gold -= g.amount;
                                        }
                                        m += plr.drop(Game1.curMap, it, m);
                                        Game1.totalTurns++;
                                        //Game1.pickedUpItems.Remove(it);
                                        //Game1.mapItems.Add(it);
                                        iNamesAndQty[it.name]--;//noHeld[i]--;
                                        iHeld = new List<Item>(plr.inventory);
                                        break;
                                    }
                                    else if (Game1.isEquipping)
                                    { //this will require a Player.Equip method in the future
                                        Equipment eq = new Equipment();
                                        if (it.GetType().BaseType.IsInstanceOfType(eq))
                                        {
                                            eq = (Equipment)it;
                                            if (eq.isEquipped)
                                                eq.isEquipped = false;
                                            else
                                                eq.isEquipped = true;

                                        }
                                        Game1.isInvShowing = false;
                                        Game1.isEquipping = false;
                                        Game1.isSubScrnShowing = false;
                                    }
                                    else if (Game1.isEating)
                                    { //pass chosen item to Use(eat version)

                                        Food tmpFood = new Food();

                                        m = plr.UsableClick(ref Game1.isEating, it, tmpFood, iNamesAndQty, ref iHeld, t, m,
                                            "You try to gag down the " + it.name + ", but it only results in your stomach contents coming back up. ");
                                        Game1.totalTurns++;
                                        break;

                                    }
                                    else if (Game1.isDrinking)
                                    {
                                        //pass chosen item to Use(drink version)

                                        Potion tmpPot = new Potion();



                                        m = plr.UsableClick(ref Game1.isDrinking, it, tmpPot, iNamesAndQty, ref iHeld, t, m,
                                             "There is precious little liquid in the " + it.name + ", and it does absolutely nothing. ");
                                        Game1.totalTurns++; 
                                        break;
                                        
                                    }
                                    else if (Game1.isReading)
                                    {

                                        Scroll tmpScr = new Scroll();
                                        if (t.IsInstanceOfType(tmpScr))
                                        {


                                            plr.methodToInvoke = it.methodToInvoke;
                                            plr.itemToUse = it;
                                            //numberHeld[i]--;
                                            //itemsHeld = new List<Item>(player.inventory);
                                            Game1.isReading = false;
                                            Game1.isUsingItemOnItem = true;
                                           
                                            oldMState = newMState; //important - makes sure that a "double-click" isn't produced
                                            return;

                                        }
                                        else
                                        {
                                            m = "You are not literate in the ways required to read the " + it.name + ". ";
                                            Game1.isReading = false;
                                            Game1.isInvShowing = false;
                                            Game1.isSubScrnShowing = false;
                                            Game1.totalTurns++;
                                            return;
                                        }
                                    }
                                    else if (Game1.isUsing)
                                    {
                                        Game1.isInvShowing = false;
                                        Game1.isSubScrnShowing = false;
                                        plr.itemToUse = it;
                                        return;
                                    }



                                }

                            }

                        }
                    }
                    //itemsHeld = new List<Item>(player.inventory);
                    //if cursor was over an item in the inventory, drop it and exit function


                }
                
                else if (Game1.isUsing && !Game1.isInvShowing)
                {
                    Color originalTileClr;
                    foreach (Tile t in Game1.visibleTilesArray)
                    {
                        originalTileClr = t.tileTint;
                        if (t.Contains(newMState.X, newMState.Y, Tile.tWidth))
                        {
                            float xDif = Math.Abs(plr.curPos.X - t.vPos.X), yDif = Math.Abs(plr.curPos.Y - t.vPos.Y);
                            if ((xDif == 1 || xDif == 0) &&
                            (yDif == 1 || yDif == 0))
                            {
                                plr.itemToUse.targetTile = Game1.tileArray[(int)t.absPos.X, (int)t.absPos.Y];
                                break;
                            }

                            else
                            {
                                m = "That is too far away. ";
                                return;
                            }
                    }

                    }
                    m = plr.itemToUse.Use(plr);
                    Game1.isUsing = false;
                }
                

                else if (Game1.isLooking && newMState != oldMState)
                {

                    Color originalTileClr;
                    foreach (Tile t in Game1.visibleTilesArray)
                    {
                        originalTileClr = t.tileTint;
                        if (t.Contains(newMState.X, newMState.Y, Tile.tWidth))
                        {
                            oldMState = newMState;
                            t.tileTint = originalTileClr;
                            Game1.totalTurns++;
                            m += plr.Look(t, Game1.mapItems, Game1.monsterArray, Game1.curMap.questNPC,
                                ref iHeld); //itemsHeld here refers to items "held" on clicked tile
                            if (iHeld.Count > 1)
                                return;
                        }
                    }
                    Game1.isLooking = false;
                }
                else  if (Game1.isSafetyOff)
                {
                    Color originalTileClr;
                    foreach (Tile t in Game1.visibleTilesArray)
                    {
                        originalTileClr = t.tileTint;
                        if (t.Contains(newMState.X, newMState.Y, Tile.tWidth) &&!t.isSolid)
                        {
                            oldMState = newMState;
                            t.tileTint = originalTileClr;
                            
                            plr.farMovePos = t.absPos;
                            plr.isMakingFarMove = true;
                            
                        }
                    }
                    //ends manual attacking if you haven't clicked an NPC / continues if you have
                    Game1.isSafetyOff = false;
                    
                    foreach(Monster mon in Game1.monsterArray)
                        if (plr.farMovePos == mon.absolutePos)
                        {
                            Game1.isSafetyOff = true;
                            break;
                        }
                    Game1.totalTurns++;               
                }


                else if (!(Game1.titleScreen || Game1.newGame))
                {
                    if (Game1.sideBtnRect.Contains(newMState.X, newMState.Y) ||
                    Game1.bottomBtnRect.Contains(newMState.X, newMState.Y))
                    {
                        if (!(Game1.isInvShowing || plr.isMakingFarMove || newMState == oldMState))//left-click on any of the action/menu buttons
                        //    
                        {
                            Button.MouseClick(myGame, newMState, plr, gameTime, ref m, ref myGame.header);
                        }
                    }

                    else if (new Rectangle(Game1.miniMapX, Game1.miniMapY,
                        Game1.maxTilesH * 8, Game1.maxTilesV * 8).Contains(newMState.X, newMState.Y))
                    { //trying a far move using the minimap
                        Tile tmpTile = Game1.tileArray[(newMState.X - Game1.miniMapX) / 8,
                            (newMState.Y - Game1.miniMapY) / 8];
                        if (!tmpTile.isSolid)
                        {
                            plr.farMovePos = tmpTile.absPos;
                            plr.isMakingFarMove = true;
                        }

                    }
                    else if (!plr.isMoving)

                        foreach (Tile t in Game1.visibleTilesArray)
                        {
                            if (t.Contains(mousePos.X, mousePos.Y, Tile.tWidth) && !(t.isSolid))
                            {
                                plr.farMovePos = t.absPos;
                                plr.isMakingFarMove = true;
                                //plr.isMoving = true;
                                break;
                            }
                        }

                }
            //    //oldMState = newMState;
            }
            if(oldMState.LeftButton == ButtonState.Released)
                Game1.sndUpdater.UpdateSound(fxArr[(int)SoundUpdater.EffectsIDs.clickFX]);
            oldMState = newMState;

            base.Update(gameTime);
        }


        public void rClick(GameTime gameTime, 
            ref List<Item> iHeld, /*int[] noHeld,*/ ref string m)
        {
            if(Game1.miniMapRect.Contains(newMState.X, newMState.Y) && oldMState.RightButton == ButtonState.Released)
                //if holding right mouse button over minimap area, show the larger map view
            {
                if (Game1.miniMapToggle)
                    Game1.miniMapToggle = false;

                else
                    //revert to current map's minimap
                    Game1.miniMapToggle = true;
                oldMState = newMState;
            }
            else if (/*newMState != oldMState && */oldMState.RightButton == ButtonState.Released && !Game1.isInvShowing)
            //if throwing/shooting a weapon
            {
                oldMState = newMState;

                //Color originalTileClr;
                foreach (Tile t in Game1.visibleTilesArray)
                {
                    float plrX = plr.curPos.X;
                    float plrY = plr.curPos.Y;
                    //originalTileClr = t.tileTint;

                    //Condition 1: clicked a tile that is not solid
                    if ((t.Contains(mousePos.X, mousePos.Y, Tile.tWidth) && !t.isSolid)
                        && //Condition 2: ensures only 8-way shots are possible
                        ((t.vPos.X == plrX || t.vPos.Y == plrY) //NSEW
                        || (t.vPos.X == t.vPos.Y) //up-left or down-right diag.
                        || (t.vPos.X % plrX == plrY % t.vPos.Y
                        || t.vPos.Y - plrY == plrX - t.vPos.X))) //up-right or down-left diag.
                    { //throw an item

                        //t.tileTint = originalTileClr;
                        m = "";
                        Game1.totalTurns++;

                        plr.RangedAttack(t, plr.weapon, ref plr.inventory, ref Game1.monsterArray, m);

                        break;

                    }
                }

                
            }
            oldMState = newMState;

                base.Update(gameTime);//Game1.isThrowing = false;
        }
    }
}