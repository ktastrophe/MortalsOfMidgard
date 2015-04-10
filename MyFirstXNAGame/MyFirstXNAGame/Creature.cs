using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
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
    
    public abstract class Creature
    {

        public Texture2D texture;
        public Color tint; //changes when attacked
        public bool isVisible, isMoving;
        public string name;

        public Vector2 curPos, newCurPos, //relative position in the visible grid
            drawPos, //curPos * tile width, tile height
        absolutePos, //position in the overall map
        newPos, //position after the player moves        
        startingPos, // random starting position
        moveDir,
        vel = new Vector2(8,8); //velocity vector = new Vector2(5,5);

        //public ClassAttribs classAttribs;
        public int curHP {get;set;}
        public int HP { get; set; }
        public int oldHP { get; set; }
        public int constitution{get;set;}
        
        public bool isDead;
        public int accuracy, attack, defense, speed;
        
        public List<Item> inventory = new List<Item>(100);
        public int gold, exp;
        
        public Texture2D skeleton;
        
       
        
    }
    public class Player : Creature
    {
        
        public int fingers, eyes, level;
        public bool isMakingFarMove = false;
        public Vector2 farMovePos = Vector2.Zero;

        public bool isBlind{get; set;}
        public bool isDeaf { get; set; }
        public bool isIntoxicated { get; set; }
        public bool isHallucinating { get; set; }
        public bool isLegless { get; set; }
        public bool isShrunken { get; set; }
        public bool isPlagued { get; set; } //may inflict other conditions temporarily; may be passed on to enemies
        public bool isPhasing { get; set; }
        public bool isInvisible { get; set; }
        public bool isRegenerating { get; set; }
        public bool isFeverish { get; set; } //inflicted by enemies, can cause hallucinations also
        public bool isParalyzed { get; set; }
        public bool isHardening {get; set;}
        public bool isTranslucent { get; set; }
        public bool isOverburdened { get; set; }
        public bool isLycanthropic { get; set; }
        public bool isSurvitalized { get; set; } //when you drink a potion that gives you more than max HP
        public bool isHeatproof { get; set; } //ring of ashes
        public bool isColdproof { get; set; } //ring of insulation
        public bool isAcidProof { get; set; } //ring of crystal skin
        public bool isFamished { get; set; } //ring of famine
        public bool isDraining { get; set; }
        public bool isMutated { get; set; }
        public bool isAttacking, isUnderAttack;

        const int lvl1exp = 0, lvl2exp = 75, lvl3exp = 180, lvl4exp = 300, lvl5exp = 475,
            lvl6exp = 640, lvl7exp = 850, lvl8exp = 1125, lvl9exp = 1485, lvl10exp = 1860;
        public int[] levelExpArray = new int[] { lvl1exp, lvl2exp, lvl3exp, lvl4exp, lvl5exp, lvl6exp, lvl7exp, lvl8exp, lvl9exp, lvl10exp };

        public string cClass, gender;
        public float oldExp;

        public Tile curTile;

        public double strength {get; set;}
        double maxStrength { get; set; }
        public int fullness { get; set; } //relating to hunger
        int morality { get; set; }
        
        int inventoryWeight { get; set; }

        //List<Usables> unpaidItems;

        public Weapon weapon;
        public Weapon subWeapon;
        public Armor armor;
        public Boots boots;
        public Cloak cloak;
        public Greaves greaves;
        public Gauntlets gauntlets;
        public List<Ring> ringsList;
        public Helmet helm;
        public Gorget gorget;

        
        
        public System.Reflection.MethodInfo methodToInvoke;
        public Item itemToUse; //item to use on another item

        Random r = new Random();

        public Player(){ }

        public Player(Texture2D t, string cl, Vector2 p, string n, string g, Weapon w, Armor a )
        {
            Random r = new Random();
            
            this.tint = Color.White;
            
            this.name = n;
            
            this.gender = g;
            
            this.texture = t;

            
            this.startingPos = p;
            this.absolutePos = p;
            this.newPos = absolutePos;
            
            //"cursor" position of the player, only changes at edges/corners of map
            this.curPos = new Vector2((Game1.visibleTilesX - 1)/2, (Game1.visibleTilesY - 1)/2);
            this.newCurPos = curPos;
            
            //position where player will be drawn, derived from curPos and tile dimensions
            this.drawPos = new Vector2(curPos.X * Tile.tWidth, curPos.Y * Tile.tHeight);
            
            this.moveDir = new Vector2(0, 0);

            this.level = 1;
            
            this.exp = 0;
            
            this.fullness = 32;
             
             
             this.gold = 500;
             
             this.cClass = cl;

             /* About the "Intuit" function: 
              * Available from the beginning of the game.
              * Alchemist should know the recipes for the initial potions he/she is carrying. 
              * Using Intuit on new potions will tell you one KNOWN ingredient at a time (e.g., dead (blank), provided
              * you have encountered (blank)).
              * Sage will be able to write what he/she has read before, AND studied via Intuit.
              * (Thus, 2 scroll minimum.)
              * Survivalist will be able to Intuit food (i.e., urns with rotten food/powder),
              * and monster properties (i.e., "Touching/eating this may carry some side effects."),
              * Blacksmith Intuits equipment (i.e., hex status, how to make), 
              * and Conjurer Intuits wands (charges), rings & amulets (good or not), and other magical items.
                Viking is a dumb brute with no intuition. */
            if (cl == "Medicine Man" || cl == "Medicine Woman")
            {
                
                this.strength = 14;
                this.constitution = 1;
                this.speed = 5;
                this.HP = r.Next(23, 28);
                this.accuracy = 80;
                this.maxStrength = 18;
                
            }
            else if (cl == "Viking")
            {
                
                //armor = new Armor("chain mail", r.Next(0,3), 3, false, 8);
                this.strength = 18;
                this.maxStrength = 25;
                this.constitution = 3;
                this.speed = 3;
                this.HP = r.Next(35, 39);
                this.accuracy = 75;
            }
            else if (cl == "Survivalist")
            {
                //armor = new Armor("hide cuirass", r.Next(0, 3), 2, false, 6);
                this.strength = 16;
                this.maxStrength = 20;
                this.constitution = 2;
                this.speed = 9;
                this.HP = r.Next(30, 35);
                int noOfArrows = r.Next(10, 21);
                for (int i = 0; i < noOfArrows; i++)
                {
                    Weapon arrow = new Weapon(Item.arrowTx, "arrow", 3, 4, 1, 7); 
                    inventory.Add(arrow);
                }
                this.accuracy = 90;
            }
            
            this.weapon = w;
            this.armor = a;
            weapon.isEquipped = true;
            armor.isEquipped = true;
            inventory.Add(weapon);
            inventory.Add(armor);
            foreach(Item it in Game1.lstInitialInv)
                inventory.Add(it);
            
            inventory.Add(new Gold(this.gold));
            


            this.attack = (int)strength / 5 + weapon.atkRating + weapon.modifier;
            //this.strength = str;
            this.curHP = HP;
            //this.HP = lvl1HP;
            this.oldHP = HP;
            accuracy += weapon.toHit;
            constitution += armor.defRating + armor.modifier + weapon.defRating;
        }

        /// <summary>
        /// CheckState
        ///     This is where status checks will be performed at the start of each turn, writing status ailments to the HUD, etc..
        ///     Also checks whether the player meets death criteria, if s/he hasn't already died of other causes
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public void CheckState(ref string m)
        {

            //check if the player has starved to death--if so, they die
            if (!isDead)
            {
                
                    
                if(Game1.totalTurns % 12 == 0)
                {
                    if (curHP < HP)
                        curHP++;
                    fullness -= 1;
                    //fullnessBar = (double)fullness / 200;
                }
                if (fullness <= 0)
                {
                    m += Die("You succumb to the effects of starvation at last. \nPress SPACE to continue...", null, "hunger");
                }
                else if (isDraining)
                {    
                    curHP--;
                    if(curHP <= 0)
                        m += Die("Your organs are ravaged by the venom in your veins. You die. \nPress SPACE to continue...", null, "poison");
                }
                
            }
            
        }


        public string Die(string m, Monster mon, string otherCause)
        {
            isDead = true;

            isAttacking = false;
            isUnderAttack = false;
            texture = skeleton;
            StreamWriter hiScoreWriter;
            if (!File.Exists(Game1.hiScoreLog))
            {
                hiScoreWriter = File.CreateText(Game1.hiScoreLog);
                hiScoreWriter.Close();
            }
            StreamReader hiScoreReader = File.OpenText(Game1.hiScoreLog);

            int thisRank = 1;
            int rank = 0;
            
            Game1.hiScoresUnsorted = new List<string>();
                
                    
                    while (!hiScoreReader.EndOfStream)
                    {
                        
                            rank++;
                        
                            string oldScore = hiScoreReader.ReadLine();
                            string oldScore2 = hiScoreReader.ReadLine(); 
                            char[] stor = new char[] { '|' };
                            string[] subStrs = oldScore.Split(stor, 3);
                            if (Convert.ToInt32(subStrs[1]) > this.level)
                                thisRank = rank + 1;
                            else if (Convert.ToInt32(subStrs[1]) == this.level)
                            {
                                
                                string[] subStrs2 = oldScore2.Split(stor, 3);
                                if (Convert.ToInt32(subStrs2[1]) > Game1.totalTurns)
                                    thisRank = rank + 1;
                                else
                                {
                                    rank++;


                                }
                            }
                            else
                            {
                                rank++;
                                
                            }
                            oldScore += "\n" + oldScore2;
                        Game1.hiScoresUnsorted.Add(rank.ToString() + ". " + oldScore);
                    }
                    
                        
            string hiScore = thisRank + ". " + this.name + ", the level |" + this.level + "| " + this.cClass + "\n";

            if (!(mon == null))
            {
                hiScore += "Slain by a " + mon.name + " in the " + Game1.curMap.name + ", turn |" + Game1.totalTurns + "|.";
            }
            else
            {
                hiScore += "Taken by " + otherCause + " in the " + Game1.curMap.name + ", turn |" + Game1.totalTurns + "|.";
            }

            if (Game1.hiScoresUnsorted.Count() > 1)
                Game1.hiScoresUnsorted.Insert(thisRank, hiScore);
            else
                Game1.hiScoresUnsorted.Add(hiScore);

            hiScoreReader.Close();


            hiScoreWriter = File.CreateText(Game1.hiScoreLog);
            
            foreach(string s in Game1.hiScoresUnsorted)
                hiScoreWriter.WriteLine(s);

            hiScoreWriter.Close();
            
                

            return m;
        }
        
        public string Attack(Monster mon, string msg)
        {
            mon.oldHP = mon.curHP;
            Random r = new Random();
            int hitChance = r.Next(0, 101);
            //Player tmpPlayer;
            //Type pType = new Player().GetType();
            if (hitChance >= (100 - this.accuracy))
            {
                int atkDmg = r.Next((this.attack - mon.constitution) - 1, (this.attack - mon.constitution) + 2);
                if (atkDmg <= 0)
                    atkDmg = 1;
                //int timesToAttack = 0;
                //if (pType.IsInstanceOfType(this))
                //{
                    atkDmg += r.Next(weapon.atkRating - 1, weapon.atkRating + weapon.modifier + 1);
                    msg += "You hit the " + mon.name + " for " + atkDmg + " damage. ";

                    mon.tint = Color.Crimson;

                    AnimationUpdater.slashAtkTexture.Origin = new Vector2(mon.curPos.X, mon.curPos.Y);
                    AnimationUpdater.slashAtkTexture.Play();
                    isAttacking = true;
                    mon.curHP -= atkDmg;
            }
            else
            {
                msg += "You miss the " + mon.name + ". ";

                
            }

            
            if (mon.curHP <= 0)
            {

                mon.isDead = true;
                msg += "The " + mon.name + " is killed. (+" + mon.exp + " XP) ";
                    this.exp += mon.exp;
                    isMakingFarMove = false;
                    foreach (Item it in mon.inventory)
                    {
                        it.curPos = new Vector2(mon.curPos.X, mon.curPos.Y);
                        it.startingPos = new Vector2(mon.absolutePos.X, mon.absolutePos.Y);
                        it.isVisible = true;
                        Game1.mapItems.Add(it);
                    }


                    msg += this.LevelUp();

            }
            return msg;
        }
         /// <summary>
         /// Called whenever a keyboard input occurs (i.e., when a turn is taken). 
         /// Depending on the outcome, player will move, remain stationary, attack, 
         /// or be attacked.
         /// </summary>
         /// <param name="k">An iterator for the # of keys pressed</param>
         /// <param name="state">Keyboard state upon UpdateInput</param>
         /// <param name="tileX">Leftmost x of the visible area</param>
         /// <param name="endTileX">Rightmost x of the visible area</param>
         /// <param name="tileY">Topmost y of the visible area</param>
         /// <param name="endTileY">Bottommost y of the visible area</param>
         /// <param name="turns">Current turn #</param>
         /// <param name="m">Message string to be displayed in messageRect</param>
         public void Move(KeyboardState state, int k, int tileX, int endTileX, int tileY, int endTileY, ref int turns, ref string m)
         {
             //if (k == 0)
             //{
                 isMoving = false;
                 this.isUnderAttack = false;
                 
                 Game1.newOverlayPos = Vector2.Zero;
                
             //these variables become true if the visible area ends at the respective map boundary 
                 bool isBorderingW = false, isBorderingE = false, isBorderingN = false, isBorderingS = false;
                 
                 if (tileX == 0)
                     isBorderingW = true;
                 else if (endTileX == Game1.maxTilesH - 1)
                     isBorderingE = true;
                 if (tileY == 0)
                     isBorderingN = true;
                 else if (endTileY == Game1.maxTilesV - 1)
                     isBorderingS = true;

                 //reset the velocity vector
                 moveDir = new Vector2(0, 0);
                 newCurPos = curPos;
                 //first, check for a diagonal key on the numpad - if pressed, reinitialize keys
                   
             
             if (state.IsKeyDown(Keys.NumPad1))
                 { //up and left
                     state = new KeyboardState(Keys.Left, Keys.Down);

                 }
                 else if (state.IsKeyDown(Keys.NumPad3))
                 { //up and right
                     state = new KeyboardState(Keys.Right, Keys.Down);

                 }
                 else if (state.IsKeyDown(Keys.NumPad7))
                 { // left and down
                     state = new KeyboardState(Keys.Left, Keys.Up);

                 }
                 else if (state.IsKeyDown(Keys.NumPad9))
                 { //right and down
                     state = new KeyboardState(Keys.Right, Keys.Up);
                 }
                
             if(isDead)
                 if(state.IsKeyDown(Keys.NumPad5) || state.IsKeyDown(Keys.Space))
                 {
            
                    Game1.gameOverScreen = true;
                    Game1.isSubScrnShowing = true;
                    return;
                 }
                 else
                    return;
            

             
                 //check for arrow key input (single or double), or double numpad input
                 if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.NumPad4))
                 {
                     moveDir.X = -vel.X;
                     if (curPos.X == 0)
                     {
                         newPos.X = 0;
                         //redrawMap = false;


                     }
                     //if visible area borders right edge of map...
                     else if ((isBorderingE && curPos.X > 3)
                         //or left edge....
                             || (isBorderingW && curPos.X <= 3))
                     {
                         //...update the player cursor's position
                         newCurPos.X -= 1;
                         newPos.X -= 1;


                     }
                     else
                     {
                         newPos.X -= 1;
                         //absolutePos.X -= 1;
                     }
                 }
                 else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.NumPad6))
                 {
                     moveDir.X = vel.X;
                     if (curPos.X == 6)
                     {
                         newPos.X = Game1.maxTilesH - 1;
                         //redrawMap = false;
                     }
                     //if visible area borders the left of the map...
                     else if ((isBorderingW && curPos.X < 3) ||
                         //or the right...
                         (isBorderingE && curPos.X >= 3))
                     {
                         //...then move the player cursor
                         newCurPos.X += 1;
                         newPos.X += 1;

                         //redrawMap = false;

                     }
                     else
                     {

                         newPos.X += 1;
                         //absolutePos.X += 1;
                     }
                 }
                 if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.NumPad2))
                 {   //if player is at the bottom, cursor position doesn't change

                     moveDir.Y = vel.Y;

                     if (curPos.Y == 6)
                     {
                         newPos.Y = Game1.maxTilesV - 1;
                         //redrawMap = false;
                         //absolutePos.Y = newPos.Y;

                     }
                     //if visible area is bordering the top of the map
                     else if ((isBorderingN && curPos.Y < 3)
                         //...or bottom of map
                         || (isBorderingS && curPos.Y >= 3))
                     {
                         //... the player cursor moves

                         newCurPos.Y += 1;
                         newPos.Y += 1;
                         //redrawMap = false;
                     }
                     else
                     {
                         newPos.Y += 1;
                         //absolutePos.Y += 1;
                     }
                 }
                 else if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.NumPad8))
                 {
                     moveDir.Y = -vel.Y;
                     //if player is at the top, cursor position doesn't change

                     if (curPos.Y == 0)
                     {

                         newPos.Y = 0;
                         //redrawMap = false;
                         //absolutePos.Y = newPos.Y;

                     }
                     //if the visible area borders the bottom of map...
                     else if ((isBorderingS && curPos.Y > 3)
                         //or the top...
                         || (isBorderingN && curPos.Y <= 3))
                     {
                         //...move the player cursor
                         newPos.Y -= 1;
                         newCurPos.Y -= 1;
                         //redrawMap = false;

                     }
                     else
                     {
                         newPos.Y -= 1;
                         //absolutePos.Y -= 1;
                     }
                 }
                 
                 //else //no key pressed, or wrong key for this function
                 //    return;

                 
                 turns++;

                 //check the player's new position against the tile in the direction moved
                 //if a solid tile exists in that spot, the player doesn't move
                 //OR, if the player walked into a shop doorway, a greeting is displayed

                 if (Game1.tileArray[(int)newPos.X, (int)newPos.Y].isSolid)
                 {
                     this.farMovePos = this.newPos; 
                     this.isMakingFarMove = false;
                     isMoving = false;
                     //if (Game1.curMap.hasQuestNPC)
                     
                     //    if (newPos == Game1.curMap.questNPC.startingPos)
                     //    {
                     //        //Tile.totalTurns++;
                     //        m = Game1.curMap.questNPC.choiceText;

                     //        Game1.isQuestDlgShowing = true;
                     //    }
                         
                     
                     //else
                     //{
                         //turnString = "NG";
                         //decreases the turn count since the player didn't move
                         turns--;
                         //takes away regenerated HP if the player got any
                         if (oldHP != curHP)
                             curHP = oldHP;

                     //}

                     newPos = absolutePos;
                     newCurPos = curPos;

                     //oldState = newState;
                     //oldTime = gameTime.TotalRealTime.TotalMilliseconds;
                     //couldn't move, so return
                     isMoving = false;
                     return;
                 }
                 else
                 {
                     bool newX = !newPos.X.Equals(absolutePos.X);
                     bool newY = !newPos.Y.Equals(absolutePos.Y);
                     isMoving = true;
                     if ((newX || newY) || (state.IsKeyDown(Keys.Space) ||
                       state.IsKeyDown(Keys.NumPad5)))
                     {
                         CheckState(ref m);
                         //if (turns % 12 == 0)
                         //{
                         //    if (curHP < HP)
                         //        curHP++;
                         //    fullness -= 1;
                         //    //fullnessBar = (double)fullness / 200;
                         //}
                         //isMoving = true;
                         //set the positional changes for the scrolling overlay according to player mov't

                         if (newCurPos.X == 3 && curPos.X == 3 )//&& !Game1.isAttacking)
                         
                             if (newX)
                                 Game1.newOverlayPos.X += (absolutePos.X - newPos.X) * Tile.tWidth;
                         if( newCurPos.Y == 3 && curPos.Y == 3)
                             if (newY)
                                 Game1.newOverlayPos.Y += (newPos.Y - absolutePos.Y) * Tile.tHeight;

                         
                     }
                     
                 }
                 //move monsters upon player move and check for attack chance

                 foreach (Monster mon in Game1.monsterArray)
                 {
                     mon.tint = Color.White;
                     
                     mon.Move(this);

                     //ATTACK CYCLE - player tries attack
                     if (mon.newPos == this.newPos && (mon.isHostile || Game1.isSafetyOff) && !mon.isMoving)// && !this.isDead)
                     {
                         
                         //this.isUnderAttack = true;
                         if (this.newPos == mon.absolutePos /*&& !this.isMoving*/)
                         //have player attack only if player moved toward a hostile monster 
                         //in an adjacent tile
                         {
                             
                             m = this.Attack(mon, /*(this.weapon),*/ m);
                             this.newPos = this.absolutePos;
                             this.newCurPos = this.curPos;
                             Game1.isSafetyOff = false;
                             mon.isHostile = true;
                             //m.newPos = m.absolutePos; //DELETE LINE iF IT DOESN'T FIX PLaYER-MONSTER OVERLAPPING PROBLEm

                         }
                         //...if neither it nor the player is dead...
                         if (mon.isDead)
                         {
                             Game1.deadMonsterArray.Add(mon);
                             continue;
                         }
                         if (!this.isDead)
                         {//... have monster attack
                             m = mon.Attack(this, /*(this.weapon),*/ m);
                             mon.newPos = mon.absolutePos;
                             
                         }
                         //this.isMakingFarMove = false;
                         this.farMovePos = this.newPos;
                         //this.isMoving = false;
                     }
                     //END ATTACK CYCLE
                         
                     

                     //monster's absolute position is finally updated
                     
                     mon.absolutePos = mon.newPos;
                     
                 }

                 foreach (QuestNPC n in Game1.curMap.questNPCs)//if (Game1.curMap.hasQuestNPC)

                     if (newPos == n.startingPos)//Game1.curMap.questNPC.startingPos)
                     {
                         //Tile.totalTurns++;
                         m = n.Interact(this, m);
                         n.Move(this);
                         //Game1.isQuestDlgShowing = true;
                     }

             //EXIT CHECK CYCLE - check if player has exited the current map - if so, load new map
             Tile t = Game1.tileArray[(int)newPos.X, (int)newPos.Y];
             if (t.isExit && !isAttacking &&(!(Game1.tileArray[(int)absolutePos.X, (int)absolutePos.Y].isExit && t.absPos == farMovePos)))
                 {
                  //we just moved onto the exit this turn
                     int lastMapType = Game1.curMap.type;
                     if ((this.newCurPos.X % (Game1.visibleTilesX-1) == 0 || this.newCurPos.Y % (Game1.visibleTilesY-1) == 0)
                         && !state.IsKeyDown(Keys.NumPad5))
                     {
                        
                         //player has just walked to edge of map - load new map

                         Game1.curMap.savedTileArray = Game1.tileArray;

                         //oldState = newState;
                         if (newPos.X == Game1.maxTilesH - 1)
                         {

                             Game1.curMap = Game1.mapArray[Game1.curMap.mapPosX + 1, Game1.curMap.mapPosY, Game1.curMap.depth];
                             newPos = new Vector2(0, newPos.Y);
                             newCurPos = new Vector2(0, newCurPos.Y);
                         }
                         else if (newPos.Y == Game1.maxTilesV - 1)
                         {
                             Game1.curMap = Game1.mapArray[Game1.curMap.mapPosX, Game1.curMap.mapPosY + 1, Game1.curMap.depth];
                             newPos = new Vector2(newPos.X, 0);
                             newCurPos = new Vector2(newCurPos.X, 0);
                         }
                         else if (newPos.X == 0)
                         {
                             Game1.curMap = Game1.mapArray[Game1.curMap.mapPosX - 1, Game1.curMap.mapPosY, Game1.curMap.depth];
                             newPos = new Vector2(Game1.maxTilesH - 1, newPos.Y);
                             newCurPos = new Vector2(Game1.visibleTilesX - 1, newCurPos.Y);
                         }
                         else if (newPos.Y == 0)
                         {
                             Game1.curMap = Game1.mapArray[Game1.curMap.mapPosX, Game1.curMap.mapPosY - 1, Game1.curMap.depth];
                             newPos = new Vector2(newPos.X, Game1.maxTilesV - 1);
                             newCurPos = new Vector2(newCurPos.X, Game1.visibleTilesY - 1);
                         }
                     }
                     else if (Game1.tileArray[(int)newPos.X, (int)newPos.Y].name == "teleport")
                     {
                         Game1.curMap = Game1.mapArray[3, 0, 0];
                         Random r = new Random();
                         do
                         {
                             newPos = new Vector2(r.Next(6,25), r.Next(6,25));
                         } while (Game1.curMap.savedTileArray[(int)newPos.X, (int)newPos.Y].isSolid);
                     }
                     else if (this.newCurPos.X % Game1.visibleTilesX - 1 > 0)//we're not in kansas anymore
                     {
                         int d = Game1.curMap.depth;
                         if (Game1.tileArray[(int)newPos.X, (int)newPos.Y].name == "exit down")
                         {

                             //player descends stairs leading down
                             for (int i = 0; i < 3; i++)
                             {
                                 for (int j = 0; j < 3; j++)
                                 {
                                     //int k = Game1.curMap.depth;
                                     if (Game1.mapArray[i, j, d + 1].stairTileUp != null)
                                     {
                                         Game1.curMap = Game1.mapArray[i, j, d + 1];
                                         //Game1.curMap.GetMap();
                                         newPos = new Vector2(Game1.curMap.stairTileUp.absPos.X, Game1.curMap.stairTileUp.absPos.Y);
                                     }
                                 }
                             }
                         }
                         //player ascends stairs leading up
                         else if (Game1.tileArray[(int)newPos.X, (int)newPos.Y].name == "exit up")
                         {
                             for (int i = 0; i < 3; i++)
                             {
                                 for (int j = 0; j < 3; j++)
                                 {
                                     if (Game1.mapArray[i, j, d - 1].stairTileDown != null)
                                     {
                                         Game1.curMap = Game1.mapArray[i, j, d - 1];
                                         ///Game1.curMap.GetMap();
                                         newPos = new Vector2(Game1.curMap.stairTileDown.absPos.X, Game1.curMap.stairTileDown.absPos.Y);
                                     }
                                 }
                             }
                         }
                     }
                 
                 
                 //redraw screen and update variables
                 Game1.curMap.GetMap();
                 Game1.curMap.areaMapTile.isDiscovered = true;
                 absolutePos = newPos;
                 curPos = newCurPos;
                 drawPos = new Vector2(curPos.X * Tile.tWidth, curPos.Y * Tile.tHeight);
                 
                 isMoving = false;

                 this.farMovePos = t.absPos; 
                 isMakingFarMove = false;
                 if (lastMapType != Game1.curMap.type)
                     SoundUpdater.BGMInst.Stop();
                     return;
             }

             //check if player has entered a shop from the outside
             if (Game1.curMap.isShopMap )//&&
                if( Game1.tileArray[(int)this.newPos.X, (int)this.newPos.Y].name == "door"
                 && !((Game1.tileArray[(int)this.newPos.X, (int)this.newPos.Y].name == 
                 Game1.tileArray[(int)this.absolutePos.X, (int)this.absolutePos.Y].name)
                 || (Game1.tileArray[(int)this.absolutePos.X, (int)this.absolutePos.Y].name == "floor")))
                {
                 //player has just stepped into the shop, so show the welcome message
                     m += "Trader " + Game1.curMap.trader.name + " welcomes you and bids you enter. ";
                     //this.isMakingFarMove = false;
                     this.farMovePos = this.newPos;
                }
                else

                    if (!(Game1.curMap.shopDoorway.isOpen)) //shop item picked up - fill door tile
                    {
                        Game1.tileArray[(int)Game1.curMap.shopDoorway.location.X,
                (int)Game1.curMap.shopDoorway.location.Y].texture = Tile.wallTexture;
                        Game1.tileArray[(int)Game1.curMap.shopDoorway.location.X,
                   (int)Game1.curMap.shopDoorway.location.Y].isSolid = true;

                    }
                    else
                    {
                        Game1.tileArray[(int)Game1.curMap.shopDoorway.location.X,
                (int)Game1.curMap.shopDoorway.location.Y].texture = Tile.floorTexture;
                        Game1.tileArray[(int)Game1.curMap.shopDoorway.location.X,
                   (int)Game1.curMap.shopDoorway.location.Y].isSolid = false;
                    }

             
             if (this.isMakingFarMove && this.farMovePos == this.newPos)
             {
                
                 this.isMakingFarMove = false;
                 //this.absolutePos = this.newPos;

             }  
             
             this.absolutePos = this.newPos;
             this.curPos = this.newCurPos;
           
                
         }
         public void Update(float elapsed)
         {
             //player will "slide" into new position (if moving)
             UpdatePos();

             if (isAttacking)
             {
                 int slashSound = (int)SoundUpdater.EffectsIDs.slashFX;
                 isAttacking = Game1.aniUpdater.UpdateAni(AnimationUpdater.slashAtkTexture, elapsed);

                 //slash sound plays if not already playing
                 Game1.sndUpdater.UpdateSound(SoundUpdater.fxList[slashSound]);

             }
             else if (isMoving)
             {
                 
                 curTile.PlayStepSound(Game1.sndUpdater, curTile.stepSnds.Length);
             }
            

         }
         public void UpdatePos()
         {
             if (isMoving)
             {
                 bool done = true;
               
                 this.drawPos.X += moveDir.X;
                 this.drawPos.Y += moveDir.Y;
                 foreach (Monster m in Game1.monsterArray) //don't start next turn until all monsters have done moving
                 {
                    if(! m.UpdatePos())
                    
                        done = false;
                 }

                 //we've fully entered a new tile
                 if (drawPos.X % Tile.tWidth == 0 && drawPos.Y % Tile.tHeight == 0)
                 {
                     Tile.moveDir = new Vector2(moveDir.X, moveDir.Y);
                     Tile.isMoving = true;
                     //Tile.Update();
                     
                     this.curPos = this.newCurPos;
                     this.drawPos = new Vector2(curPos.X * Tile.tWidth, curPos.Y * Tile.tHeight);
                     //ensures monsters still move if player didn't change location last turn
                     if(done)
                        isMoving = false;
                     //have the scrolling overlay (if present) move along with player
                     Game1.scrOverlayPos.X += Game1.newOverlayPos.X;
                     Game1.scrOverlayPos.Y += Game1.newOverlayPos.Y;
                     
                     //have the tiles start to scroll smoothly into their new positions
                     //invoke Tile.Update() in Game1.cs by first assigning the appropriate movement direction
                     
                 }
                 
                     
             }

         }

         public string LevelUp()
         {
             for (int i = this.level; i < levelExpArray.Length; i++)
             {
                 if (this.exp >= levelExpArray[i])
                 {
                     this.level = i + 1;
                     return "\n You have attained Level " + this.level + ".";
                 }
                 
                     
              }
             return "";   
         }

         /// <summary>
         /// This method contains the common code for the conditionals isDrinking, isReading, and isEating, and will call Use
         
         /// </summary>
         /// <param name="isUsing"></param>
         /// <param name="i"></param>
         /// <param name="iType"></param>
         /// <param name="nameAndQty"></param>
         /// <param name="iHeld"></param>
         /// <param name="t"></param>
         /// <param name="mStr"></param>
         /// <param name="failStr"></param>
         /// <returns></returns>
        public string UsableClick(ref bool isUsing, Item i, Item iType, Dictionary<string, int> nameAndQty, ref List<Item> iHeld,
             Type t, string mStr, string failStr)
         {
             //isEating = false;
             if (t.IsInstanceOfType(iType))
             {
                 mStr = Use(i, mStr); //plr.Use(it, tmpFood, m, ref iHeld, ref noHeld)
                 nameAndQty[i.name]--;//noHeld[i]--;
                 iHeld = new List<Item>(this.inventory);
                 //isEating = false;
                
                 

             }
             else
             {
                 mStr = failStr;
                 if(Game1.isEating)
                    this.fullness -= 5;
                 //isEating = false;
             }
             isUsing = false;
             Game1.isInvShowing = false;
             Game1.isSubScrnShowing = false;
                 return mStr;
             
         }
        public string pickUp(Item i, Map cM, string m)
         {
         
             Gold tmpG = new Gold(0);
             m = "Picked up a " + i.name + ". ";
             if (i.GetType().IsInstanceOfType(tmpG))
             {
                 Gold g = (Gold)i;
                 this.gold += g.amount;
                 //add the gold to the total, which is the last item in the inventory
                 if (!(this.inventory[this.inventory.Count - 1].GetType().IsInstanceOfType(tmpG)))
                     this.inventory.Add(new Gold(g, this.gold, g.startingPos));
                 else
                     this.inventory[this.inventory.Count - 1] = new Gold(g, this.gold, g.startingPos);
                 return m;
             }
             //insert newly acquired items before gold (if any) in inventory
             if ((this.inventory[this.inventory.Count - 1].GetType().IsInstanceOfType(tmpG)))
                 this.inventory.Insert(this.inventory.Count - 2, i);
            else
                 this.inventory.Add(i);


             if (i.isShopItem)
             {

                 if (cM.isShopMap && cM.shopDoorway.isOpen)
                 {
                     cM.shopDoorway.isOpen = false;
                     m += "The shop's door closes. ";

                 }
             }
             //Tile.totalTurns++;
             return m;
         }
         public string drop(Map cM, Item i, string m)
         {
             
             i.startingPos = this.absolutePos;
             
             this.inventory.Remove(i);
             
             //temporarily open the shop doorway in case all unpaid items have been dropped
             cM.shopDoorway.isOpen = true;
             foreach (Item it in this.inventory)
                 if (it.isShopItem)
                    //an unpaid item remains in inventory, so shop door closes
                     cM.shopDoorway.isOpen = false;
             

             m = "You drop a " + i.name + ". ";
             Game1.pickedUpItems.Remove(i);
             Game1.mapItems.Add(i);

             return m;
         }
         public string Pay(string m, Map cM)
         {
             int oldG = this.gold;
             foreach(Item i in this.inventory)
                 if (i.isShopItem)
                 {
                     if (this.gold - i.price >= 0)
                     {
                         this.gold -= i.price;
                         //trim the price from item name before displaying message
                         int priceTrim = i.name.IndexOf('$');
                         i.name = i.name.Remove(priceTrim - 2);
                         priceTrim = i.ideedname.IndexOf( '$' );
                         i.ideedname = i.ideedname.Remove(priceTrim - 2);
                         
                         m += "You buy a " + i.name + " for " + i.price + " bullion. ";
                         i.isShopItem = false;
                         
                     }
                     else
                     {
                         
                         m += "'You haven't the means to pay.' ";
                         
                     }
                     if (!i.isShopItem)
                     {
                         cM.shopDoorway.isOpen = true;
                         continue;
                     }
                     else
                     {
                         cM.shopDoorway.isOpen = false;
                         return m;
                     }
                 }
             if (oldG == this.gold)
                 m += "'You don't owe me anything...' ";
             else
             {
                 this.inventory[this.inventory.Count - 1] = new Gold(this.gold);

             }
             //Tile.totalTurns++; 
             return m;
         }
         

         public string Use(Item i, string m) 
         {
             m = i.Use(this);
             this.inventory.Remove(i);
             //Tile.totalTurns++;
             return m;
         }
         public string Look(Tile t, List<Item>lIt, List<Monster>lM, QuestNPC cNPC, 
             ref List<Item>tileItems)
         {
             string m = "\nThere you see";
             bool seenSomething = false;
             tileItems = new List<Item>();
            
             foreach (Monster mon in lM)
             {
                 if (mon.absolutePos == t.absPos)
                 {
                     m += " a " + mon.name;
                     seenSomething = true;
                     break;
                 }
             }
             if (cNPC != null && t.absPos == cNPC.absolutePos)
                 {
                     m += " a " + cNPC.name;
                     seenSomething = true;
                 }
             if (this.absolutePos == t.absPos)
             {
                 m = "You take a look at yourself."; //invoke status screen this way, as well as via button
                 seenSomething = true;
             }
             foreach (Item i in lIt)
                 {
                     if (i.startingPos == t.absPos)
                     {
                         tileItems.Add(i);
                         seenSomething = true;
                         if (m.Contains(" a "))
                         {
                             m += " and";
                             break;
                         }
                         
                     }
                 }
                 
                 
                 if(tileItems.Count == 1)
                     m += " a " + tileItems[0].name + ". ";
                 else if(tileItems.Count > 1)
                 {
                     m += " the following items (press any key): ";
                    
                     
                 }
                 else if (!seenSomething)
                     m = "You look, but nothing catches your eye. ";
                 //Tile.totalTurns++;
             return m;
         }

         public void RangedAttack(Tile t, Weapon wldWep, ref List<Item> invItems, 
             ref List<Monster> mapMnstrs, string m)
         {
             //list of inventory items' names 
             List<string> iNames = new List<String>(invItems.Count);
             for(int i = 0; i < invItems.Count; i++)
                 iNames.Add(invItems[i].name);

             Type rWtype = typeof(RangedWep);
             RangedWep rWldWep;
             if (rWtype.IsInstanceOfType(wldWep)) //this will tell us we're shooting an arrow/bolt
             {
                 rWldWep = (RangedWep)wldWep;
             }
             else
                 return;
             if (iNames.Contains(rWldWep.firedWep.name)) //if the weapon's projectile is in inventory
             {
                 int itemLoc = iNames.IndexOf(rWldWep.firedWep.name);
                 Weapon tmpFiredWep = new Weapon((Weapon)invItems[itemLoc], this.absolutePos, this.curPos, ref t);
                 
                 invItems.Remove(invItems[itemLoc]);
                 
                 tmpFiredWep.drawPos = new Vector2(this.drawPos.X, this.drawPos.Y);
                 //tmpFiredWep.startingPos = new Vector2(this.absolutePos.X, this.absolutePos.Y);
                 //tmpFiredWep.targetTile = new Tile(t);

                 //get item ready for throwing
                 tmpFiredWep.isInMotion = true;
                 tmpFiredWep.isVisible = true;

                 //tmpFiredWep.origin = new Vector2(tmpFiredWep.texture.Width / 2, tmpFiredWep.texture.Height / 2);
                 //tmpFiredWep.curPos.X += tmpFiredWep.texture.Width / 2;
                 //tmpFiredWep.curPos.Y += tmpFiredWep.texture.Height / 2;
                 Game1.mapItems.Add(tmpFiredWep);

                 Game1.isThrowing = true;
                  
                 
                 
                 //tmpFiredWep.Move(this, tmpFiredWep, ref m);
                          
             }
         }

         public string Identify(Item i, Map[,,] m)
         {
             //Usables tmp = (Usables)i;
             foreach (Map map in m)
                 if(!map.isDummyMap)
                     foreach (Item mIt in map.savedItemArray)
                     if (mIt.name.Contains(i.name))
                     {
                         mIt.name = mIt.ideedname;
                     }


             foreach (Item it in this.inventory)
                 if (it.name.Contains(i.name))
                     it.name = it.ideedname;

              
             i.name = i.ideedname;
             
             return "The object's nature is revealed: a " + i.name + ". ";

         }
    }
    public class Monster : Creature
    {
        protected static Random r;
        public bool isHostile = true; //monsters are hostile by default
        public SpriteEffects sprE = SpriteEffects.None;
        public string effectString; //for when you eat the dead monster
        public int weight;
        //public Texture2D skeleton;
        
        public System.Reflection.PropertyInfo propToAffect;

        public Monster() { }
        public Monster(Texture2D t, Texture2D d, Vector2 p, string n, int w,
            int minHP, int maxHP, int minAtk, int maxAtk, int acc, int con, int xp )
        {
            r = new Random();
            this.inventory = new List<Item>();
            this.texture = t;
            this.skeleton = d;
            this.startingPos = p;
            this.curPos = new Vector2(p.X * 80, p.Y * 80);
            this.absolutePos = p;
            this.newPos = p;

            this.name = n;
            this.weight = w;
            this.HP = r.Next(minHP, maxHP);
            this.curHP = this.HP;
            this.attack = r.Next(minAtk, maxAtk);

            this.accuracy = acc;
            this.constitution = con;
            this.exp = xp;

            this.effectString = "The flavour of the raw " + n + " left much to be desired. ";
            Type plrType = typeof(Player);
            //edit this for classes of monsters that have special fx upon eating
            this.propToAffect = plrType.GetProperty("fullness"); 
        }

        public string Attack(Player p, string msg)
        {
            Random r = new Random();
            int hitChance = r.Next(0, 101);
            p.isUnderAttack = true;
            if (hitChance >= (100 - this.accuracy))
            {
                int atkDmg = r.Next((this.attack - p.constitution) - 1, (this.attack - p.constitution) + 2);
                if (atkDmg <= 0)
                    atkDmg = 1;
                int timesToAttack = 0;

                //use Speed rating to determine how many attacks the enemy gets per attack by player
                timesToAttack = (int)Math.Ceiling((double)this.speed / (double)p.speed);
                for (int i = 0; i < timesToAttack; i++)
                {
                    p.curHP -= atkDmg;
                    msg += "The " + this.name + " hits for " + atkDmg + " damage. ";
                    //atkDmg = r.Next((this.attack - p.constitution) - 1, (this.attack - p.constitution) + 2);
                }
                p.tint = Color.Crimson;
              

                //for (int i = 0; i < timesToAttack; i++)
                    //p.curHP -= atkDmg;
                    
            }
            
                else
                    msg += "The " + this.name + " misses. ";
        

            
            
                if(p.curHP <= 0)
                {
                    //p.isDead = true;
                    msg += p.Die("\nYou die, and Midgard goes with you. Press SPACE to continue...", this, null);

                    //p.isAttacking = false;
                    //p.isUnderAttack = false;
                    //p.texture = p.skeleton;
                    
                }
                //this.isAttacking = false;

            return msg;
        }
        public void FindFreeTile(Player player)
        {
            //check that the monsters haven't moved into a solid tile
            //if they have, move them in the direction of the player instead
            //if against a wall, choose between moving down or moving left to get closer to player
                     
            
            //this.moveDir = Vector2.Zero; 
            int x = 0, y = 0;
            int tmpMstrX = (int)this.absolutePos.X, tmpMstrY = (int)this.absolutePos.Y;
            if (this.absolutePos.X <= player.newPos.X)
            {
                if (!(Game1.tileArray[tmpMstrX + 1, tmpMstrY + y].isSolid /*|| tileArray[tmpMstrX + 1, tmpMstrY + y].absPos == player.newPos*/))
                {
                    //this.moveDir.X = vel.X;
                    x = 1;
                }
            }
            if (this.absolutePos.Y <= player.newPos.Y)
            {
                if (!(Game1.tileArray[tmpMstrX + x, tmpMstrY + 1].isSolid /*|| tileArray[tmpMstrX + x, tmpMstrY + 1].absPos == player.newPos*/))
                {
                    y = 1;
                    //this.moveDir.Y = vel.Y;
                }
            }
            if (this.absolutePos.X >= player.newPos.X)
            {
                if (!(Game1.tileArray[tmpMstrX - 1, tmpMstrY + y].isSolid /*|| tileArray[tmpMstrX - 1, tmpMstrY + y].absPos == player.newPos*/))
                {
                    //this.moveDir.X = -vel.X;
                    x = -1;
                }
            }



            if (this.absolutePos.Y >= player.newPos.Y)
            {
                if (!(Game1.tileArray[tmpMstrX + x, tmpMstrY - 1].isSolid /*|| tileArray[tmpMstrX + x, tmpMstrY - 1].absPos == player.newPos*/))
                {
                    //this.moveDir.Y = -vel.Y;
                    y = -1;
                } 
            }
            //let the monster move to a free tile
            //Tile tmpTile = Game1.tileArray[tmpMstrX + x, tmpMstrY + y];

            this.newPos = new Vector2(tmpMstrX + x, tmpMstrY + y);
            this.moveDir = new Vector2( vel.X * x, vel.Y * y);
        }
        /// <summary>
        /// Makes the enemy "slide" into its new position
        /// </summary>
        /// <returns>"False" ensures monster still moves if player didn't change location last turn</returns>
        public bool UpdatePos()
        {
            
            this.drawPos.X += this.moveDir.X;
            this.drawPos.Y += this.moveDir.Y;
            
            //we've fully entered a new tile
            if (drawPos.X % Tile.tWidth == 0 && drawPos.Y % Tile.tHeight == 0)
            {
                Update();//this.curPos = this.newCurPos;
                this.drawPos = this.curPos;
                return true;
                //this.absolutePos = this.newPos;
                //isMoving = false;
                 
            }
            return false;
        }

        public virtual void Move(Player player)
        {
            this.moveDir = Vector2.Zero;
            
            if (Game1.tileArray[(int)this.absolutePos.X, (int)this.absolutePos.Y].isDiscovered
                          && this.isHostile)
            {
                //hostile enemies will move towards player once seen
                if (player.newPos.X < this.newPos.X)
                {
                    this.sprE = SpriteEffects.FlipHorizontally;//only turn towards player if hostile
                    moveDir.X = -vel.X;
                    this.newPos.X--;
                    
                    
                }

                else if (player.newPos.X > this.newPos.X)
                {
                    this.sprE = SpriteEffects.None; 
                    moveDir.X = vel.X;
                    this.newPos.X++;

                }


                if (player.newPos.Y > this.newPos.Y)
                {
                    moveDir.Y = vel.Y;
                    this.newPos.Y++;
                }
                else if (player.newPos.Y < this.newPos.Y)
                {
                    moveDir.Y = -vel.Y;
                    this.newPos.Y--;
                }

            }


            else if(!Game1.isSafetyOff)//just have the enemy move randomly to an open space
            {
                Random r = new Random();
                int rX, rY;
                do
                {
                    rX = r.Next(-1, 2);
                    rY = r.Next(-1, 2);

                } while //(tileArray[(int)m.newPos.X + rX, (int)m.newPos.Y + rY].isSolid || 
                    (new Vector2(this.newPos.X + rX, this.newPos.Y + rY) == player.newPos ||
                    this.newPos.X + rX > Game1.maxTilesH - 2 || this.newPos.X + rX < 1 ||
                    this.newPos.Y + rY > Game1.maxTilesV - 2 || this.newPos.Y + rY < 1);
                this.newPos.X += rX;
                this.newPos.Y += rY;
                this.moveDir.X = Math.Sign(rX) * vel.X;
                this.moveDir.Y = Math.Sign(rY) * vel.Y;
            }

            Tile t = Game1.tileArray[(int)this.newPos.X, (int)this.newPos.Y]; //Game1.tileArray[(int)this.absolutePos.X, (int)this.absolutePos.Y];
                if (/*t.absPos == this.newPos* &&*/ t.isSolid)
                {
                    
                    FindFreeTile(player);
                }
                //keep trader on a floor tile if he is about to leave the shop

                //if (Game1.tileArray[(int)this.newPos.X, (int)this.newPos.Y].name != "floor" && this.texture.Name == "trader" && !isHostile)
                //{
                //    this.newPos = this.absolutePos;
                //    //moveDir = Vector2.Zero;
                //}

            

            //put monsters back in their previous places if they tried to move 
            //onto an occupied tile
            foreach (Monster m2 in Game1.monsterArray)
            {


                if (this.newPos == m2.absolutePos)
                {
                    //try this to get around "waiting monster" problem
                    
                    //FindFreeTile(player);
                    this.newPos = this.absolutePos;
                    this.moveDir = Vector2.Zero;
                    //keep the break
                    
                    break;
                    
                }

            }
        }

        public void Update()
        {
            bool isInVArray = false;
            
            foreach (Tile t2 in Game1.visibleTilesArray)
            {
               if (t2.absPos == this.absolutePos) //newPos && t.isDiscovered)
               {
                    this.curPos = new Vector2(t2.vPos.X * Tile.tWidth, t2.vPos.Y * Tile.tWidth);
                    isInVArray = true;
               }
            }
            if (!isInVArray)
            {    
                isVisible = false;
                return;
            }
                    //this.moveDir = Vector2.Zero;
                    
                    Tile t = Game1.tileArray[(int)this.absolutePos.X, (int)this.absolutePos.Y];
                    if(t.isDiscovered)
                        isVisible = true;
                    else
                        isVisible = false;
                    
                   // break;
                
                
            
        }

        protected void MakeGold(int oneInChance, int amt, ref List<Item> inv)
        {
            r = new Random();
            if (r.Next(0, oneInChance) == 0) //1 in n chances enemy will have gold
            {
                Gold g;
                if (amt <= 50)
                    g = new Gold(Game1.lstGold[0], amt, this.absolutePos);
                else
                    g = new Gold(Game1.lstGold[3], amt, this.absolutePos);
                inv.Add(g);
            }
            
         }
    }
    public class FeralHound : Monster
    {
        //public string name;
        public FeralHound(Texture2D t, Texture2D d, Vector2 p) : base(t, d, p, 
            "feral hound", 9, 10, 15, 8, 12, 75, 3, 6)
        {
            this.speed = 6;
            
        }
    }
    public class GrassSnake : Monster
    { 
        public GrassSnake(Texture2D t, Texture2D d, Vector2 p) : base(t,d, p,
            "grass snake", 3, 5, 9, 4, 8, 85, 2, 3)
        {
            this.speed = 10;
            if (r.Next(0, 2) == 1)
            {
                this.effectString = "Your hasty meal has proven poisonous.";
                Type plrType = typeof(Player);
                this.propToAffect = plrType.GetProperty("isDraining");
            }

        }
    }
    public class Wolverine : Monster
    {
        public Wolverine(Texture2D t, Texture2D d, Vector2 p) : base(t,d, p,
            "wolverine", 6, 9, 14, 7, 11, 77, 4, 5)
        {
            this.speed = 5;
            
        }
    }
    public class Veinheri : Monster
    {
        public Veinheri(Texture2D t, Texture2D d, Vector2 p)
            : base(t, d, p, "veinheri", 9, 16, 22, 11, 18, 80, 4, 15)
        {
            this.speed = 7;

        }
    }
    public class TurkeyRat : Monster
    {
        public TurkeyRat(Texture2D t, Texture2D d, Vector2 p)
            : base(t, d, p, "turkey rat", 7, 13, 18, 12, 16, 85, 3, 11)
        {
            this.speed = 6;

        }
    }
    public class Villiform : Monster
    { //will later multiply if hit with a physical attack
        //and need to be burned/doused with salt water/scorched with light to be killed
        public Villiform(Texture2D t, Texture2D d, Vector2 p)
            : base(t, d, p, "villiform", 5, 14, 20, 10, 14, 80, 5, 15)
        {
            this.speed = 7;
        }
    }
    public class RedHind : Monster
    {
        public RedHind(Texture2D t, Texture2D d, Vector2 p) : base(t, d, p, "red hind", 8,
            8, 14, 5, 9, 85, 2, 4)
        {
            //r = new Random();

            //this.name = "red hind";
            isHostile = false;
            this.speed = 9;
            //this.HP = r.Next(9, 14);
            //this.attack = r.Next(6, 9);
            //this.curHP = this.HP;
            //this.weight = 8;
            //this.gold = r.Next(0, HP / 2);
            //this.accuracy = 80;
            //this.constitution = 2;
            //this.exp = 4;

        }
    }
    public class Trader : Monster
    {
        static string[] traderNames = {"Elvinde", "Trygve", "Dagfinn", "Brynjar",
        "Aksel", "Knute", "Lauritz"};
        public Trader(Texture2D t, Texture2D d, Vector2 p):base()
            
    
        {
            r = new Random();
            inventory = new List<Item>();
            texture = t;
            skeleton = d;
            startingPos = p;
            curPos = new Vector2(p.X * 80, p.Y * 80);
            absolutePos = p;
            newPos = p;
            
            base.name = traderNames[r.Next(traderNames.Length-1)];
            isHostile = false;
            base.effectString = "You cannibalize " + this.name + ". Only the most desperate and deranged souls have done as you have. ";
            constitution = 10;
            HP = r.Next(400, 700);
            attack = r.Next(30, 50);
            speed = 9;
            curHP = this.HP;
            //base.MakeGold(1, r.Next(100, HP / 2), ref this.inventory);
            accuracy = 90;
            weight = 10;
            exp = 200;
            
        }
        public override void Move(Player p)
        {
       
                base.Move(p);
                if (!isHostile && !Game1.isSafetyOff)
                {
                    if (newPos == p.newPos)// && this == Game1.curMap.trader)
                    {
                        p.isMoving = true;
                        if (p.newPos == p.absolutePos)
                            //if the player performed a stationary action, trader doesn't move
                            newPos = absolutePos;
                        else if (Game1.tileArray[(int)p.absolutePos.X,
                            (int)p.absolutePos.Y].name == "floor" ||
                            Game1.tileArray[(int)p.absolutePos.X,
                            (int)p.absolutePos.Y].name == "door")
                            //player and trader switch places (if the player wasn't in the door)
                            newPos = p.absolutePos;
                        else
                            //no move is possible so the trader stays still
                            newPos = absolutePos;
                    }
                    else if (Game1.tileArray[(int)this.newPos.X, (int)this.newPos.Y].name != "floor")// && this.texture.Name == "trader" && !isHostile)
                    {
                        this.newPos = this.absolutePos;
                        //moveDir = Vector2.Zero;
                        //}
                    }


                }
            
        }
    }
    public class QuestNPC : Monster
    
    {
        public string choiceText;
        public string[] questText;
        public int mapX, mapY, mapZ;
        public List<Keys> choiceKeys;
        public string parentQuest;

        bool isMobile = false;
        bool isGhost = false;
        
        int interactType = 0;
        public QuestNPC(Texture2D t, int mX, int mY, int mZ, string qTxt)
        {
            texture = t;
            //startingPos = p;
            mapX = mX;
            mapY = mY;
            mapZ = mZ;
            choiceText = qTxt;
        }
        //proper, scalable constructor for quest interactables with multiple choice (CURRENT)
        public QuestNPC(Texture2D t, int mX, int mY, int mZ, string qTxt, string parentQ, string[] choiceResults, int variant, List<Keys>keys, bool hasVariant)
            : this(t, mX, mY, mZ, qTxt)
        {

            Random r = new Random();
        
            
            //using the variant parameter,
            //sets one of two outcomes for choice A (if applicable)
            int i = 0;
            if (hasVariant)
            {
                i = 1;
            }
            questText = new string[choiceResults.Length - i];
            questText[0] = choiceResults[r.Next(variant)];
            for(int j = i ; j < questText.Length; j++)
                questText[j] = choiceResults[j];
            choiceKeys = new List<Keys>(keys);
            isHostile = false;
            parentQuest = parentQ;
        }

        //for quest interactables with multiple choice and multiple outcomes (OLD)
        public QuestNPC(Texture2D t, int mX, int mY, int mZ, string qTxt, string qTxtAa, string qTxtAb,
            string qTxtB, string qTxtC):this(t, mX, mY, mZ, qTxt)
        {
            
            Random r = new Random();
            //texture = t;
            ////startingPos = p;
            //mapX = mX;
            //mapY = mY;
            //choiceText = qTxt;
            int variant = r.Next(0, 2);

            //set one of two responses at random for choice A
            string variantA;
            if (variant == 0)
                variantA = qTxtAa;
            else
                variantA = qTxtAb;

            questText = new string[] { variantA, qTxtB, qTxtC };
            choiceKeys =  new List<Keys>{Keys.A, Keys.B, Keys.C};
            isHostile = false;
        }
        //for stationary, Yes/No interactables (OLD)
        public QuestNPC(Texture2D t, int mX, int mY, int mZ, string qTxt, string qTxtA):this(t, mX, mY, mZ, qTxt)
        {

            
            //texture = t;
            ////startingPos = p;
            //mapX = mX;
            //mapY = mY;
            //choiceText = qTxt;
            string resultTxt = "\nThe monument crumbles into dust.";
            qTxtA += resultTxt;
            questText = new string[] { qTxtA, resultTxt };
            choiceKeys = new List<Keys> { Keys.A, Keys.B };
            isHostile = false;
        }
        //for living, moving NPCs
        public QuestNPC(Texture2D t, string m, int mX, int mY, int mZ, bool isG, bool isM):this(t, mX, mY, mZ, m)
        {
            //texture = t;
            //mapX = mX;
            //mapY = mY;
            //choiceText = m;
            isGhost = isG;
            isMobile = isM;
            isHostile = false;
            interactType = 1;
        }
        public override void Move(Player p)
        {
            if (isMobile) //make sure this NPC is mobile, and that player isn't trying to hail them
            {
                base.Move(p);
                //Random r = new Random();
                //int rX, rY;
                //do
                //{
                //    rX = r.Next(-1, 2);
                //    rY = r.Next(-1, 2);

                //} while (Game1.tileArray[(int)newPos.X + rX, (int)newPos.Y + rY].isSolid || 
                //    //new Vector2(this.newPos.X + rX, this.newPos.Y + rY) == p.newPos ||
                //    newPos.X + rX > Game1.maxTilesH - 2 || newPos.X + rX < 1 ||
                //    newPos.Y + rY > Game1.maxTilesV - 2 || newPos.Y + rY < 1);
                //newPos.X += rX;
                //newPos.Y += rY;
                //this.moveDir.X = Math.Sign(rX) * vel.X;
                //this.moveDir.Y = Math.Sign(rY) * vel.Y;
            }
        }
        public string Interact(Player p, string m)
        {
           
                    m = choiceText;

                    if (interactType == 0)
                    {
                        Game1.isQuestDlgShowing = true;
                        p.newPos = p.absolutePos;
                        p.newCurPos = p.curPos;

                        //oldState = newState;
                        //oldTime = gameTime.TotalRealTime.TotalMilliseconds;
                        //couldn't move, so return
                        p.isMoving = false;
                        Game1.curMap.questNPC = this;
                    }
                    

            return m;
        }
    }
}
