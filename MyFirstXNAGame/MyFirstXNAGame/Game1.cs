using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Window, graphics & sound, and storage objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch nameSpriteBatch;
        public Viewport vPort;

        Rectangle messageRect;
        //StorageDevice device;
        

        //SYSTEM ITEMS AND COMPONENTS
        
        MouseInput mouseCmpt;
        public static SoundUpdater sndUpdater;
        public static AnimationUpdater aniUpdater;
        public static SysTxLoader txObj;
        public static KeyHandler keyHndlr;

        public static Vector2 scrOverlayPos = new Vector2(0.0f, 0.0f);
        public static Vector2 newOverlayPos = Vector2.Zero;
        public static int overlayX, overlayY;
        
        //protected Texture2D msgRectBG, statusBarBG, borderBG;
        //public Texture2D 
        //    sideBarBG;// packBtn, mapBtn, lookBtn, eatBtn, logBtn; //system button textures

        public Rectangle packBtnPos, mapBtnPos, logBtnPos, saveBtnPos,
            lookBtnPos, eatBtnPos, toolBtnPos, atkBtnPos;
        public static Rectangle sideBtnRect, bottomBtnRect;
        public Button packBtn, mapBtn, logBtn, saveBtn, //sidebar
            lookBtn, eatBtn, toolBtn, atkBtn; //bottom bar
        public static List<Button> btnList;
        public static Texture2D invItemTex;
        public Rectangle invItemThumb; //thumbnail area for image on the inventory screen

        //public Vector2 mousePos = Vector2.Zero;
        

        //COLORS
        protected Color backColor = Color.DarkSlateBlue; //r34, g139, b34, a255
        protected Color txtColor = Color.Wheat;
        protected Color tintColor;
        
               
        ///TEXTURES
        public List<Texture2D> lstTileTex;
        public List<Texture2D> lstFrstMnstrTex, lstDngnMnstrTex;
        public List<Texture2D> lstItemTex;

        //Overlays
        //protected Texture2D titleTex, invBG;
        //public static Texture2D overlayTex,
        //    scrOverlay, //scrolling parallax overlay;
        //tileOverlayLwrR, tileOverlayRight, tileOverlayLwrL,
        //    tileOverlayBtm; //feathered edge overlays

        public Texture2D
            traderTex, ravenTrapTex, //NPCs and monsters
            skeletonTexture,

              mtnTexture, treeTexture,
              stalagmiteTex, puddleTex; //world tiles
            
            

        //public static Texture2D grassTexture, floorTexture, wallTexture,
        //    snowTexture, snowTexture2, snowTexture3; //more world tiles
       
        public AnimatedTexture animTest; //"animation" using custom AnimatedTexture class

        public SoundEffect curBGM;

        //DEVICE STATES & TIMING
        protected KeyboardState oldState, oldInput, newInput;
        //protected MouseState oldMState; //for the mouse buttons

        protected double oldTime;

        //Player info and unique NPCs

        public Player player; //player object derived from Creature
        
        public Vector2 startPos;
        protected Monster villageTrader;
        //protected QuestNPC curNPC;
        protected Doorway tmpDoorway;
        
       
        public static Map curMap; //map the player is currently on
        protected Tile playerTile; //the player's current tile, where .isPlayerTile = true
        
        
        //Public Constants (accessible from Map.cs, if need be)
        public const int maxTilesH = 30, maxTilesV = 30;
        public const int mapScrTileWdth = 60, mapScrTileHgt = 60; //map graphic resolution / 60 
        
        //public const int visibleTilesX = 7, visibleTilesY = 7;
        public const int visibleTilesX = 8, visibleTilesY = 8; 
        public const int maxMstrs = 9;
        public const int maxMapItems = 3;
        public const int miniMapX = 560, miniMapY = 320, miniMapWdth = 240;
        
        public const int lineSkip = 28;
        

        //LISTS, ARRAYS, DICTIONARIES
        public static List<Monster> monsterArray;
        public static List<Monster> deadMonsterArray; // monsters to be removed on the next Draw call
        public static Tile[,] tileArray; //all the tiles in the map grid
        public Rectangle[,] mapScrRectArr; //tiles on the map screen
        public static Tile[,] visibleTilesArray; //only the visible tiles 
        public static List<Room> tmpRooms;
        public static List<Item> mapItems;
        List<Item> movingItems = new List<Item>();
        public static List<Item> pickedUpItems;
        
        List<Usables> lstUsables;
        public static List<Gold> lstGold;
        List<Equipment> lstEquipment;
        List<QuestNPC> lstNPC;
        List<Quest> lstQuest;
        public static List<Item> lstInitialInv; //initial inventory for all classes

        List<string> saveGameNames;
        List<string> defaultPotionNames, iDeedPotionNames, wandNames, iDeedWandNames, 
            ringNames, iDeedRingNames, defaultScrollNames, iDeedScrollNames;
        
        protected Dictionary<string, string> potionIDsandNames, scrollIDsandNames,
            ringIDsandNames, wandIDsandNames;
        //Dictionary<Potion, PropertyInfo> potionTypeandProp;
        Dictionary<Scroll, MethodInfo> scrollTypeandMethod;
        //Dictionary<Ring, PropertyInfo> ringTypeandProp;
        
        public static Rectangle[,] miniMapRects;
        public static Rectangle miniMapRect = new Rectangle(miniMapX, miniMapY, miniMapWdth, miniMapWdth);
        public static Map[,,] mapArray;
        
        //For the Inventory Screen:
        
        public List<Item> itemsHeld;
        public static Rectangle[] itemRects;
        public static List<ItemRect> invRects;
        //public int[] numberHeld;
        public Dictionary<string, int> itemTxtAndQty;
        //public List<string> itemNames;
        
        //SYSTEM VARIABLES
        public static int totalTurns = 1;
        double hpBar = 100, expBar = 0;
        
        //GAME FLOW CONTROL - BOOLEAN Switches
        public static bool newGame,
        chooseGender, chooseClass,
        isSubScrnShowing, //controls whether to check for individual screens
        isQuestScrnShowing,
        isFileSlctShowing,
            isMapShowing, //press 'm'
            miniMapToggle = true; //for highlighting the current map on the map screen
        public static bool 
            introScreen = true,
            titleScreen,
            gameOverScreen;

        bool isTinted = false;
            
        public static bool isDropping, //true if choosing something to drop from inventory (nothing to do with poop!)
         isSelling, //true if choosing something to sell from inventory
        isEating, isLooking, isReading, isDrinking,  isUsingItemOnItem,
        isUsing, //for using any item of the Tool class
        isLogShowing, //player is reading message log
        isEquipping, //temporary, to be replaced by a Character/Status Screen?
        isExiting,
        isSafetyOff, //'true' when attacking non-hostiles
        isInvShowing, //press 'i' to turn this to true
            //isAttacking,
            isQuestDlgShowing, isThrowing = false; //for suppressing update during ranged attacks

        //POSITIONING VARIABLES and System Strings
        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 spriteSpeed = new Vector2(50.0f, 50.0f);
        //float spriteMove = 0.03f;
        //public Vector2 farMovePos = Vector2.Zero;

        public SpriteFont Font1, headerFont;
        protected string hpString, lvlString, strString, goldString, mapString, expString, hungerString;
        
        protected Vector2 sideBarPos, turnStringPos, nameStringPos, hpStringPos, lvlStringPos, strStringPos, 
            goldStringPos, expStringPos, hungerStringPos,
            mapNamePos,

        introStringPos;
    
        //SET STRING VALUES
        string turnString = "Turn: ";
        public static string messageGender;
        public string messageString = "";
        public string header, footer; //for sub-screens
        Vector2 stairMap = new Vector2();

        //variables for name entry
        public string randomQuote;
        public string nameEntryPrompt = "\"Ill is the lot of he who has an ill name\". (Grettir's Saga, c.56).\r\n"
        + "What shall we call you, mortal? (press ENTER when done)";
        public string enteredKeys = "";
        protected const string letterKeys = "a b c d e f g h i j k l m n o p q r s t u v w x y z";
        public const string M = "Male";
        public const string F = "Female";
        
        protected Vector2 nameEntryFontPos;
        protected Vector2 FontOrigin;
        protected Vector2 enteredKeysFontPos;

        //File I/O and Stream objects

        public static readonly string saveFileDir = StorageContainer.TitleLocation + @"\Saves";
        public static string strLogName = StorageContainer.TitleLocation + @"\gamelog.dat"; //message log file
        public static string hiScoreLog = StorageContainer.TitleLocation + @"\hiscores.dat";
        public StreamWriter logWriter;
        string introText;
        float introTxtEnd;
        public List<string> logLines, hiScoreLines;
        public static List<string> hiScoresUnsorted;
        int startLine = 0, endLine = 13; //for the log, inventory, and other paginating screens
        
        /// <summary>
        /// CREATE BUILDING
        ///     Helper function to CreateDungeon
        /// </summary>
        /// <param name="bldgStart"></param>
        /// <param name="bldgEnd"></param>
        /// <param name="doorChoices"></param>
        /// <param name="rmTiles"></param>
        private void CreateBuilding(Vector2 bldgStart, Vector2 bldgEnd, List<Tile> doorChoices, List<Tile> rmTiles)
        {

            Random r = new Random();

            int bStartX = (int)bldgStart.X, bStartY = (int)bldgStart.Y,
                       bEndX = (int)bldgEnd.X, bEndY = (int)bldgEnd.Y;

            for (int x = bStartX; x <= bEndX; x++)
            {
                for (int y = bStartY; y <= bEndY; y++)
                {
                    if ((x == bStartX || x == bEndX || y == bStartY || y == bEndY))
                    {
                        tileArray[x, y] = new Tile(Tile.wallTexture, "stone", "wall", x, y, true);
                        if (!((x == bStartX && (y == bEndY || y == bStartY)) ||
                            (x == bEndX && (y == bStartY || y == bEndY))))
                            //this is a non-corner wall, so add it to possible door locations
                                doorChoices.Add(tileArray[x, y]);
                    }
                    else //this is a floor tile
                    {
                        tileArray[x, y] = new Tile(Tile.floorTexture, "stone", "floor", x, y, false);
                        tileArray[x, y].isBldngTile = true;
                        rmTiles.Add(tileArray[x, y]);
                    }
                }
            }

            //choose a doorway from any wall tile in the building
            Tile doorWay = (doorChoices[r.Next(doorChoices.Count)]);
            Tile tmpDr = tileArray[(int)doorWay.absPos.X, (int)doorWay.absPos.Y] =
                new Tile(Tile.floorTexture, "stone", "door", doorWay.absPos.X, doorWay.absPos.Y, false);
            tmpDoorway = new Doorway(new Vector2((int)doorWay.absPos.X, (int)doorWay.absPos.Y));
            //add the door to the roomTiles
            rmTiles.Add(tmpDr);

            tmpRooms.Add(new Room(rmTiles));
        }
        
        
        /// <summary>
        /// CREATE MAP
        ///     Creates each of the nine 30x30 overworld areas in the forest.
        ///
        /// </summary>
        private void CreateMap(int type, int i, int j, int d)
        {
            
            Random r = new Random();
            
            //Most common tile texture on this map
            Texture2D defaultTx;
            //Temp variable for random tile selection
            Texture2D tempTex;

            //gets a random terrain sprite for each tile of the world

            int startIndex;
            int endIndex;
            

            List<Tile> possibleExits = new List<Tile>(); //all the exits combined in one list
            for (int x = 0; x < maxTilesH; x++)
            {
                for (int y = 0; y < maxTilesV; y++)
                {
                    //mountains can only appear 4 tiles from the edge of the map

                    if (x <= r.Next(5) || y <= r.Next(5) || x >= maxTilesH - r.Next(5) || y >= maxTilesV - r.Next(5))
                        {

                            if (type == 0)
                            {
                                startIndex = 0;
                                endIndex = 2;
                                
                            }
                            else
                            {
                                startIndex = 0;
                                endIndex = 1;
                                
                            }
                            tempTex = lstTileTex[r.Next(startIndex, endIndex)];
                        }
                        else
                        {
                            if (type == 0)
                            {
                                startIndex = 1;
                                endIndex = 5;//tileSpriteArray.Count();
                                defaultTx = Tile.grassTexture; 

                            }



                            else
                            {
                                startIndex = 5;
                                endIndex = lstTileTex.Count;
                                defaultTx = Tile.snowTexture;
                            }
                            if (r.Next(0, 5) > 0)
                                tempTex = defaultTx;
                            else
                                tempTex = lstTileTex[r.Next(startIndex, endIndex)];
                        }

                    
                        
                    bool isSolid = false;
                    if(tempTex.Name == "mountain" || tempTex.Name == "pine tree")
                        isSolid = true;
                    Tile tempTile = new Tile(tempTex, "grass", tempTex.Name, x, y, isSolid);
                    
                    //makes sure right exit is accessible & doesn't appear for easternmost maps
                    if (x >= maxTilesH - 4 && (y >= 1 && y <= maxTilesV - 2) && i != 2)
                        possibleExits.Add(tempTile);
                    //makes sure down exit is accessible & doesn't appear for southernmost maps
                    else if (y >= maxTilesV - 4 && (x >= 1 && x <= maxTilesH - 2) && j != 2)
                        possibleExits.Add(tempTile);
                    //western exit doesn't appear for westernmost maps
                    else if (x <= 4 && (y >= 1 && y <= maxTilesV - 2) && i != 0)
                        possibleExits.Add(tempTile);
                    //northern exit doesn't appear for northernmost maps
                    else if (y <= 4 && (x >=1 && x <= maxTilesH - 2) && j != 0)
                        possibleExits.Add(tempTile);
                            
                        
                    tileArray[x, y] = tempTile;
                    //add a corresponding rectangle to the minimap for current tile
                    miniMapRects[x, y] = new Rectangle(miniMapX + (x * 8), miniMapY + (y * 8), 8, 8);

                }

            }
            
            //set up an exit to the area, with three empty tiles leading up to it
            
            //set the arrow tile in the tileArray
            foreach (Tile t in possibleExits)
            {
                if(t.absPos.X == maxTilesH - 1 || t.absPos.X == 0 ||
                    t.absPos.Y == maxTilesV - 1 || t.absPos.Y == 0)
                    tileArray[(int)t.absPos.X, (int)t.absPos.Y] =
                        new Tile(Content.Load<Texture2D>("arrow tile"), "none", "exit", t.absPos.X, t.absPos.Y, false);
                //clear the  mountains or trees from the tiles leading up to the exit

                else
                    tileArray[(int)t.absPos.X, (int)t.absPos.Y] = 
                        new Tile(Tile.grassTexture, "grass", Tile.grassTexture.Name, t.absPos.X, t.absPos.Y, false);
                
                
            }
            //set up the shop building, in the village only *for now*
            //have to add a building where player character spawns
            if (i == 0 && j==0)
            {
                Vector2 traderPos = Vector2.Zero;
                Vector2 buildingStart = new Vector2(r.Next(11, 14), r.Next(11, 14));
                Vector2 buildingEnd = new Vector2(r.Next(16, 18), r.Next(16, 18));

                List<Tile> possibleDoors = new List<Tile>();
                List<Tile> tmpRoomTiles = new List<Tile>();

                CreateBuilding(buildingStart, buildingEnd, possibleDoors, tmpRoomTiles);
                
                traderPos = new Vector2(r.Next((int)buildingStart.X + 1,
                   (int)buildingEnd.X - 1),

                   r.Next((int)buildingStart.Y + 1, (int)buildingEnd.Y - 1));

                villageTrader = new Trader(traderTex, skeletonTexture, traderPos);
                //monsterArray.Add(villageTrader);

                //make a little camp for the player to start in
                
               
            }
            return;
        }

        /// <summary>
        /// CREATE DUNGEON
        ///     Random dungeon generator
        /// </summary>
        /// <param name="i">This dungeon's x coordinate in world map array</param>
        /// <param name="j">This dungeon's y coordinate in world map array</param>
        /// <param name="d">This dungeon's z coordinate in the world map array</param>
        private void CreateDungeon(int i, int j, int d)
        {
            Texture2D[] tSpriteArray = new Texture2D[] { Tile.floorTexture, Tile.floorTexture,
                Tile.floorTexture, Tile.floorTexture, Tile.floorTexture, Tile.floorTexture,
                Tile.floorTexture, stalagmiteTex, puddleTex, Tile.wallTexture };
            Doorway[,] doorsArray = new Doorway[6,2];
            Room[] roomArray = new Room[6];
            Texture2D exitTex = Content.Load<Texture2D>("dungeon arrow");

            tmpRooms = new List<Room>();

            Random rExit = new Random();

            int northExitX = rExit.Next(5, maxTilesH - 5),
                northExitY = 0,
                southExitX = rExit.Next(5, maxTilesH - 5),
                southExitY = maxTilesV - 1,
                westExitX = 0,
                westExitY = rExit.Next(5, maxTilesV - 5),
                eastExitX = maxTilesH - 1,
                eastExitY = rExit.Next(5, maxTilesV - 5);

            for (int x = 0; x < maxTilesH; x++)
            {
                for (int y = 0; y < maxTilesV; y++)
                {
                    //fill the entire map with solid rock tiles to be replaced later
                    Texture2D tempTex = Content.Load<Texture2D>("rock");
                    
                    Tile tempTile = new Tile(tempTex, "stone", "rock", x, y, true);
                    tileArray[x, y] = tempTile;

                    //EXITS - map treated as one big room, exits treated as "doors"
                    //make Doorways to add to doorsArray; assign to "rooms" 5 and 6
                    
                    //Eastern doorway
                    if (i < 2)
                    {
                        tileArray[eastExitX, eastExitY] =
                            new Tile(exitTex, "stone", "exit", eastExitX, eastExitY, false);
                        curMap.eastDungeonExit = eastExitY;
                    }
                    doorsArray[0, 1] = new Doorway(new Vector2(eastExitX, eastExitY), 0, 1);

                    if (i == 2) // if this is an easternmost map, no easternmost exit should exist
                        //set isConnected to true so that the doorway won't be counted
                        doorsArray[0,1].isConnected = true;
                    
                    
                    
                        
                    //add a western exit except to westernmost maps
                    

                        //Y is the same as the eastern exit on the previous map
                    if (i > 0)
                    {
                        westExitY = (int)mapArray[i - 1, j, d].eastDungeonExit;

                        tileArray[westExitX, westExitY] =
                            new Tile(exitTex, "stone", "exit", westExitX, westExitY, false);
                        
                    }
                        doorsArray[0, 0] = new Doorway(new Vector2(westExitX, westExitY), 0, 0);
                    

                    if(i == 0)
                        doorsArray[0,0].isConnected = true;

                    roomArray[0] = new Room(null);
                    
                    if (j < 2)
                    {
                        tileArray[southExitX, southExitY]
                         = new Tile(exitTex, "stone", "exit", southExitX, southExitY, false);
                        curMap.southDungeonExit = southExitX;
                    }
                    doorsArray[1, 0] = new Doorway(new Vector2(southExitX, southExitY), 1, 0);
                    if(j == 2)
                        doorsArray[1,0].isConnected = true;

                    //add a north exit if not northernmost map


                    if (j > 0)
                    {
                        northExitX = (int)mapArray[i, j - 1, d].southDungeonExit;
                        tileArray[northExitX, northExitY] =
                            new Tile(exitTex, "stone", "exit", northExitX, northExitY, false);
                    }
                    doorsArray[1, 1] = new Doorway(new Vector2(northExitX, northExitY), 1, 1);
                    if (j == 0)
                    
                        doorsArray[1, 1].isConnected = true;
                    roomArray[1] = new Room(null);
                }
            }
            Random r = new Random();

            //decide which quadrants of the map have rooms 
            //(doesn't currently change anything; all quadrants have a room)
            int q1room = r.Next(0,2), q2room = r.Next(0,2), q3room = r.Next(0,2), q4room = r.Next(0,2);
            

            //quadrants with value 0 will have a room
            int[] noOfRoomsArray = new int[]{q1room, q2room, q3room, q4room};

            
            
            for (int k = 2; k <= 5; k++)
            {
                List<Tile> possibleDoors = new List<Tile>();
                Doorway tmpDoor;
                //if (noOfRoomsArray[k-2] == 0)
                //this is a quadrant with a room
                //{

                    Vector2 tmpStart = Vector2.Zero, tmpEnd = Vector2.Zero;
                    switch (k)
                    {
                        case 2:
                            tmpStart = new Vector2(r.Next(4, 8), r.Next(4, 8));
                            tmpEnd = new Vector2(r.Next(10, 14), r.Next(10, 14));

                            break;

                        case 3:
                            tmpStart = new Vector2(r.Next(16, 20), r.Next(16, 20));
                            tmpEnd = new Vector2(r.Next(22, 26), r.Next(22, 26));
                            break;
                            
                        case 4:
                            tmpStart = new Vector2(r.Next(4, 8), r.Next(16, 20));
                            tmpEnd = new Vector2(r.Next(10, 14), r.Next(22, 26));
                            break;
                        case 5:
                            tmpStart = new Vector2(r.Next(16, 20), r.Next(4, 8));
                            tmpEnd = new Vector2(r.Next(22, 26), r.Next(10, 14));
                            break;
                    }


                
                    //Vector2 buildingStart = tmpStart;
                    //Vector2 buildingEnd = tmpEnd;
                    int bStartX = (int)tmpStart.X, bStartY = (int)tmpStart.Y,
                        bEndX = (int)tmpEnd.X +1, bEndY = (int)tmpEnd.Y +1;

                    int ctrX = ((int)(bEndX - bStartX) / 2) + (int)bStartX, 
                        ctrY = ((int)(bEndY - bStartY) / 2) + (int)bStartY;
                    int[] randRowLen = new int[2] { r.Next(bStartX, ctrX - 1), r.Next(ctrX + 1, bEndX + 1) };
    
                for (int x = bStartX; x <= bEndX; x++)
                    {

                        //change the row length and col. height so that walls may not always be straight lines
                        //but make sure rooms don't bleed into one another
                        int[] randRowHgt = new int[2]{ r.Next(bStartY, ctrY-1), r.Next(ctrY+1, bEndY+1) }; 
                        
                        if(x >= randRowLen[0] && x <= randRowLen[1])
                        for (int y = bStartY; y <= bEndY; y++)
                        {
                            //check if the neighbouring tile is a floor tile; if current tile is a rock, make it a wall
                            if (/*x <= randRowLen[1] && */(y == randRowHgt[0]
                                || y == randRowHgt[1]) || (x == randRowLen[0] || x== randRowLen[1]))
                            {

                                //this is a wall
                                tileArray[x, y] = new Tile(Tile.wallTexture, "stone", "roomtile", x, y, true);
                                
                                //check if it needs to connect to another column's wall
                                if (!(tileArray[x - 1, y].isSolid) && tileArray[x, y].isSolid)
                                {
                                    int y2 = y;
                                    if (y2 < ctrY)
                                    {
                                        do
                                        {
                                            y2--;
                                            tileArray[x, y2] = new Tile(Tile.wallTexture, "stone", "roomtile", x, y2, true);

                                        } while (!(tileArray[x-1, y2].isSolid));
                                        //y--;
                                        //continue;
                                    }

                                    else if (y2 > ctrY)
                                        do
                                        {
                                            y2++;
                                            tileArray[x, y2] = new Tile(Tile.wallTexture, "stone", "roomtile", x, y2, true);

                                        } while (!(tileArray[x-1, y2].isSolid));
                                    //y--;
                                    //continue;
                                }
                                else
                                   //this wall faces out, so add it to possible door locations
                                    possibleDoors.Add(tileArray[x, y]);


                            }

                            else if ((x > randRowLen[0] && x < randRowLen[1])
                                && (y > randRowHgt[0] && y < randRowHgt[1]))
                                //it's a floor tile                            
                                tileArray[x, y] = new Tile(tSpriteArray[r.Next(0, 9)], "stone", "roomtile",
                                    x, y, false);

                        }
                        else
                            randRowLen = new int[2] { r.Next(bStartX, ctrX - 1), r.Next(ctrX + 1, bEndX + 1) };


                    }
                
                   // if (possibleDoors.Count != 0)
                    //{
                        //create Tile.tWo doorways for the room
                        Tile doorWay = (possibleDoors[r.Next(possibleDoors.Count)]),
                            doorWay2;
                        //make sure the doorways are on different tiles
                        
                        do
                        {
                            doorWay2 = (possibleDoors[r.Next(possibleDoors.Count)]);
                        }while ((doorWay == doorWay2) 
                        /*||    ((doorWay.absPos.X == doorWay2.absPos.X)
                        || (doorWay.absPos.Y == doorWay2.absPos.Y))*/);
                                
                       //create the door tile 
                         Tile tmpDoorTile = new Tile(Tile.floorTexture, "stone", "door", doorWay.absPos.X, doorWay.absPos.Y, false);
                         tileArray[(int)doorWay.absPos.X, (int)doorWay.absPos.Y] = tmpDoorTile;
                         //create a doorway object for this room
                        tmpDoor = new Doorway(tmpDoorTile.absPos, k, 0);
                        //add the doorway to the array of doors - first element is the room #,
                        //second is the door number
                        doorsArray[k,0] = tmpDoor;

                        //do the same for the second door
                        tmpDoorTile = new Tile(Tile.floorTexture, "stone", "door", doorWay2.absPos.X, doorWay2.absPos.Y, false);
                        
                        tileArray[(int)doorWay2.absPos.X, (int)doorWay2.absPos.Y] = tmpDoorTile;
                        tmpDoor = new Doorway(tmpDoorTile.absPos, k, 1);
                        doorsArray[k, 1] = tmpDoor;

                        roomArray[k] = new Room(null);
                    
                    
                //}
            }
        
            //time to make paths from door to door!
            foreach (Doorway dw in doorsArray)
            {
                int rX = (int)dw.location.X, rY = (int)dw.location.Y;
                string doorName1 = tileArray[rX,rY].name;
                
                //if this doorway isn't connected to another, proceed
                if (!(doorsArray[dw.roomNo, dw.doorNo].isConnected))
                {
                    foreach (Doorway dw2 in doorsArray)
                    {
                        int x = 0, y = 0;
                        string doorName2 = tileArray[(int)dw2.location.X,(int)dw2.location.Y].name;

                        //if doors belong to different rooms, 
                        if (!(dw.roomNo == dw2.roomNo || 
                        // and if they aren't both exits...
                                (doorName1 == "exit" && doorName1 == doorName2) ||
                                //and if this room is not already connected to an exit...
                                (doorName1 == "exit" && roomArray[dw2.roomNo].connectionNames.Contains("exit")) 
                            //and if dw2 isn't connected yet...
                        || doorsArray[dw2.roomNo, dw2.doorNo].isConnected
                        //and if the Tile.tWo rooms aren't connected already...
                        || roomArray[dw2.roomNo].connectedRooms.Contains(dw.roomNo) 
                        //and, if this is one of the left-hand rooms, it will connect with one of the right doorways
                        || dw.roomNo - dw2.roomNo >= Math.Abs(2) ))
                        //... proceed with drawing a path
                        {
                       
                            while(!(doorsArray[dw.roomNo,dw.doorNo].isConnected))
                               
                            {
                            
                                if (rX < dw2.location.X) 
                                {
                                    //don't let the path go back through the room -- only out
                                        if ((tileArray[rX +1, rY + y].name == "roomtile"
                                          ) && (y == 0 && rY != dw2.location.Y))
                                        {
                                            x  = -1;
                                            rX += x;
                                            continue;
                                        }
                                        else
                                            //the path will go 1 space right, if no room is there
                                //and if it doesn't exceed the map boundaries
                                            if (!(rX + 1 > maxTilesH - 1))x = 1;
                                            
                                }

                                else if (rX > dw2.location.X) //the path will go 1sp left, ...
                                {
                                        if ((tileArray[rX - 1, rY + y].name == "roomtile"
                                           ) && (y == 0 && rY != dw2.location.Y))
                                        {
                                            x = 1;
                                            rX += x;
                                            continue;
                                        }
                                        else
                                            if (!(rX - 1 < 0)) x = -1;
                                            
                                    
                                }

                                if (rY > dw2.location.Y) //+ 1) //the path will go 1 sp up, if no wall is there
                                {   

                                        if ((tileArray[rX + x, rY - 1].name == "roomtile"
                                            )&& (x==0 && rX != dw2.location.X))
                                        {
                                            y = 1; 
                                            rY += y; 
                                            continue;
                                        } 
                                        else
                                           if (!(rY - 1 < 0))     y = -1;
                                          
                                }


                                else if (rY < dw2.location.Y) //the path will go 1sp down, ...
                                {

                                        //don't let the path go back through the room -- only out
                                        if ((tileArray[rX + x, rY + 1].name == "roomtile")
                                            && (x == 0 && rX != dw2.location.X))
                                           
                                        {
                                            y = -1; 
                                            rY += y;
                                            continue;   
                                            
                                        }
                                        else
                                            if (!(rY + 1 > maxTilesV - 1)) y = 1;
                                    
                                }
                                rX += x; rY += y;
                                //check that the path isn't trying to replace a door or an exit tile
                                if (!(tileArray[rX,rY].name == "door" || tileArray[rX,rY].name == "exit"))
                                {
                                    //if not, draw a floor tile
                                    tileArray[rX, rY] = new Tile(tSpriteArray[r.Next(0, 8)], "stone", "floor",
                                     (float)rX, (float)rY, false);
                                    //if the path is going diagonally, connect each tile in a zig-zag
                                    if(!(x == 0 || y == 0))
                                        tileArray[rX, rY+1] = new Tile(tSpriteArray[0], "stone", "floor",
                                     (float)rX, (float)rY+1, false);

                                }


                                if (tileArray[rX, rY].absPos == dw2.location) //the path has connected to a doorway
                                {
                                    doorsArray[dw.roomNo, dw.doorNo].makeConnection( true );
                                    roomArray[dw.roomNo].addConnections(dw2, doorName2);
                                    doorsArray[dw2.roomNo, dw2.doorNo].makeConnection( true );
                                    roomArray[dw2.roomNo].addConnections(dw, doorName1);
                                    
                                }
                                x = 0; y = 0;
                            }//while(!dw.isConnected);
                            
                        }
                    }
                }

            }
            for (int x = 0; x < maxTilesH; x++)
            {
                for (int y = 0; y < maxTilesV; y++)
                {

                    //the minimap is generated
                    miniMapRects[x, y] = new Rectangle(miniMapX + (x * 8), miniMapY + (y * 8), 8, 8);
                }
            }

            //    
        }

        /// <summary>
        /// CREATE MONSTERS
        ///     Run once for every new map to fill up the monster array
        /// </summary>
        /// <param name="lstMnstrTex">List of monster textures from which to create monster objects</param>
        private void CreateMonsters(List<Texture2D> lstMnstrTex)
        {
            Random r = new Random();
            
            ConstructorInfo cnstInfo;
            Type texType = typeof(Texture2D);
            Type v2Type = typeof(Vector2);

            for (int i = 0; i <= r.Next(3, maxMstrs); i++)
            {
                Texture2D tmpTx = lstMnstrTex[r.Next(lstMnstrTex.Count)];
                Monster tmpMnstr = new Monster();
                object mnstrObject = new Object(); 
                do
                 {
                     Type mType = Type.GetType("MyFirstXNAGame."+(tmpTx.Name), true, false);
                     Type[] paramTypes = new Type[]{texType, texType, v2Type};
                     cnstInfo = mType.GetConstructor(paramTypes);
                     object[] paramObjs = new object[]{ tmpTx, skeletonTexture, new Vector2(r.Next(maxTilesH - 4), r.Next(maxTilesV - 4))};
                     mnstrObject = cnstInfo.Invoke(paramObjs);
                     tmpMnstr = (Monster)mnstrObject;
                     
                 } while (tmpMnstr.startingPos == player.absolutePos || 
                     tileArray[(int)tmpMnstr.startingPos.X, (int)tmpMnstr.startingPos.Y].isSolid
                    || tileArray[(int)tmpMnstr.startingPos.X, (int)tmpMnstr.startingPos.Y].isBldngTile == true);
                 
                monsterArray.Add(tmpMnstr);

            }
            
            return;
        }

        private void CreateQuestNPCs()
        {
            lstNPC = new List<QuestNPC>();
            lstQuest = new List<Quest>();
            Random r = new Random();
            QuestNPC tmpRaven;
            QuestNPC tipTombstone; // on same map as...
            QuestNPC wanderingSoul; // this

            List<string> tipsList = new List<string>(){"\"The catacombs claimed me, for I had naught with which\n" +
                "to cleave stone, nor the means to slip through its cracks.\"",
            "\"His humble appearance belied a warrior's heart. Let those who\n" + 
            "took up arms against him be made to pay their dues.\"", //fightin' shopkeeps
            "\"A remedy not found in time,\n"+
            "Claimed by an illness of the mind.\"", //could start to have visions of murderous ravens if choice was to kill Muninn
            "\"A knave who traded the swash for silver, his sea of riches \n"+
            "did not buoy his soul to Valhalla.\"", //referring to the fact that too much bullion makes you sink
            "For seeking equal favour with pantheons opposed, was granted an eternity \n" +
            "in the realm of ambivalence.", //i.e. Limbo
            "The milk that mends the flesh with each motion, being overdrunk, sped her \n" +
            "to her demise.", //stacking too many potions of regeneration ages you to death
            "Forever shall I roam these woods, lest I meet she who granteth flight to the Fields or to the Halls.\n" 
                       
            };

            tipTombstone = new QuestNPC(Content.Load<Texture2D>("tombstone"), r.Next(1, 3), r.Next(0, 3), 0, "The name on this headstone has worn away, but part of the epitaph remains. \nRead it? \n a. Yes b. No",
                null, new string[] { tipsList[r.Next(tipsList.Count)] + "\nThe monument crumbles into dust.", "\nThe monument crumbles into dust." }, 0, new List<Keys> { Keys.A, Keys.B }, false);

            //tipTombstone = new QuestNPC(Content.Load<Texture2D>("tombstone"), r.Next(1,3), r.Next(0,3),
            //    "The name on this headstone has worn away, but part of the epitaph remains. \nRead it? \n a. Yes b. No", 
            //    tipsList[r.Next(tipsList.Count)]);
            lstNPC.Add(tipTombstone);
            
            
            wanderingSoul = new QuestNPC(txObj.msgRectBG, "A chill passes through your body. ", tipTombstone.mapX, tipTombstone.mapY, tipTombstone.mapZ, true, true);
            lstNPC.Add(wanderingSoul);


            tmpRaven = new QuestNPC(ravenTrapTex, r.Next(0, 3), r.Next(1, 3), 0, "A raven is caught by the leg in a rusty metal trap. \n "
            + "The bone is definitely broken, and the wretch's peals of pain echo amongst the trees. \n" +
            "What will you do? (press letter key) \n" +
            "a. Free b. Watch c. Kill", null, new string[] 
            {
                //choice branch Aa - Free
            "The raven is one of Odin's birds, Muninn. \"Take heed, for I have sighted Vanir in your mortal \n"+
            "realms. You, hero who stood with the Aesir at Ragnarok, may be that which they seek\".\n"+
            "Your pack shifts with a new weight, and the raven vanishes.",
                //choice branch Ab - Free 2
            "The raven hops from the trap on its good leg, cocks its head at you slightly, and flies away.",
                //choice branch B - Leave
            "You watch, sorrowless and unflinching, as the raven gnaws at its own leg. It plucks the sinews \n"+
            "one by one, as though scavenging from a corpse, until the leg severs in two. The bloodied bird \n"+
            "looses a piercing cry as it flies off.",
            //choice branch C - Kill
            "Though the trap could have been opened with some effort, you decide to crush the helpless creature \n"
            + "underfoot. Your weight easily flattens the animal, whose hollow bones and feathers snap like dry charcoal \n" +
            "and soak up the escaping ooze."}, r.Next(2), new List<Keys>{Keys.A, Keys.B, Keys.C}, true);

            //tmpRaven = new QuestNPC(ravenTrapTex, r.Next(0, 3), r.Next(1, 3),
                

            //    //text box 1
            //  "A raven is caught by the leg in a rusty metal trap. The bone is definitely broken.\n "
            //+ "The wretch's peals of pain echo amongst the trees. \n" +
            //"What will you do? (press letter key) \n"+
            //"a. Free b. Watch c. Kill",
            //    //choice branch Aa - Free
            //"The raven is one of Odin's birds, Muninn. \"Take heed, for I have sighted Vanir in your mortal \n"+
            //"realms. You, hero who stood with the Aesir at Ragnarok, may be that which they seek\".\n"+
            //"Your pack shifts with a new weight, and the raven vanishes.",
            //    //choice branch Ab - Free 2
            //"The raven hops from the trap on its good leg, cocks its head at you slightly, and flies away.",
            //    //choice branch B - Leave
            //"You watch, sorrowless and unflinching, as the raven gnaws at its own leg. It plucks the sinews \n"+
            //"one by one, as though scavenging from a corpse, until the leg severs in two. The bloodied bird \n"+
            //"looses a piercing cry as it flies off.",
            ////choice branch C - Kill
            //"Though the trap could have been opened with some effort, you decide to crush the helpless creature \n"
            //+ "underfoot. Your weight easily flattens the animal, whose hollow bones and feathers snap like dry charcoal \n" +
            //"and soak up the escaping ooze.");
            lstNPC.Add(tmpRaven);

            //= - = QUEST 1 = - = PLIGHT OF THE VALKYRIES


            int qX = r.Next(0, 3), qY = r.Next(0, 3), qZ = 1;//qZ = r.Next(1,3);


            Quest plightOfTheValkyries = new Quest(new QuestNPC[]
            {
                new QuestNPC(Content.Load<Texture2D>("survivalist f"), qX, qY, qZ, "Before you stands a once-mighty Valkyrie.\n" +
                    "She seems to have broken with reality and is muttering to herself:\n" +
                    "\"Voices, not so long ago; speaking calamity, seeking eternity...\n" +
                    "Did you not hear them? Oh, woe is their silence to me...\"\n" +
                    "Show her a reminder of her purpose? a. Yes b. No", "Plight of the Valkyries",
                new string[] { "Outcome A", "Outcome B" }, 0, new List<Keys> { Keys.A, Keys.B }, false)
            }, 
                 "For reasons unknown, the Valkyries have been exiled from\n"+
                 "Valhalla to the mortal realms where they are confined to \n"+
                 " strange dwellings that nullify their senses. After eons of such\n" +
                 "blindness, these reapers have all but forgotten their calling: \n"+
                 "to hie fallen Norsemen to their rightful place in the afterlife.\n" +

                 "\n ~1: Find a manner to remind a Valkyrie of her purpose, thereby \n"+
                 "freeing her. (X more to go)", "Plight of the Valkyries", new Ring(), true);
            lstQuest.Add(plightOfTheValkyries);
            Quest.selectedQuest = plightOfTheValkyries;
        //lstNPC.Add(valkyrie);
        }
       
        
        /// <summary>
        /// CREATE ITEMS
        /// Generate all the items for the current map
        /// Contains loops for Usable items, Gold, and Equipment (so far)
        ///
        /// </summary>
        private void CreateMapItems()
        {
                
            Random r = new Random();
            Item tmpItem;
            Food f = new Food();
            Potion p = new Potion();
            Scroll sc = new Scroll();

            Usables randomUsable = new Usables();
            //Make a random number of items for each map, up to maxItems
            for (int it = r.Next(maxMapItems-1); it < maxMapItems; it++)
            {
                int tmpR = r.Next(0, lstUsables.Count);
               
                Type t = lstUsables[tmpR].GetType();
                randomUsable = lstUsables[tmpR];
                do
                {
                    if (t.IsInstanceOfType(f)) //food
                        tmpItem = new Food(randomUsable, new Vector2((float)r.Next(4, maxTilesH - 4),
                     (float)r.Next(4, maxTilesV - 4)));
                    else if (t.IsInstanceOfType(p)) //potion
                        tmpItem = new Potion(randomUsable, new Vector2((float)r.Next(4, maxTilesH - 4),
                     (float)r.Next(4, maxTilesV - 4)));
                    else if(t.IsInstanceOfType(sc)) //scroll
                        tmpItem = new Scroll(randomUsable, new Vector2((float)r.Next(4, maxTilesH - 4),
                     (float)r.Next(4, maxTilesV - 4)));
                    else
                        tmpItem = new Usables(randomUsable, new Vector2((float)r.Next(4, maxTilesH - 4),
                         (float)r.Next(4, maxTilesV - 4)));
                    
                }
                while(tmpItem.startingPos == player.startingPos || 
                tileArray[(int)tmpItem.startingPos.X, (int)tmpItem.startingPos.Y].isSolid);
                
            
                
                mapItems.Add(tmpItem);  
            }
            

            //Creates up to 3 gold per map
            for (int g = r.Next(4); g < 4; g++)
            {
                int tmpR = r.Next(0, lstGold.Count);


                Gold tmpGold;
                do
                {
                    tmpGold = new Gold(lstGold[tmpR], 
                        r.Next(lstGold[tmpR].minAmount, lstGold[tmpR].maxAmount),
                        new Vector2((float)r.Next(4, maxTilesH - 4), (float)r.Next(4, maxTilesV - 4)));

                } while (tmpGold.startingPos == player.startingPos || tileArray[(int)tmpGold.startingPos.X, (int)tmpGold.startingPos.Y].isSolid);
                mapItems.Add(tmpGold);
            }
            
            Ring o = new Ring();
            Equipment randomEq = new Equipment();
            for (int e = r.Next(3); e < 3; e++)
            {
                int tmpR = r.Next(0, lstEquipment.Count);
                Type t = lstEquipment[tmpR].GetType();
                randomEq = lstEquipment[tmpR];

               
                do
                {
                    if (t.IsInstanceOfType(o))
                        tmpItem = new Equipment(randomEq, new Vector2((float)r.Next(4, maxTilesH - 4),
                        (float)r.Next(4, maxTilesV - 4)));
                    else
                        tmpItem = randomEq;

                } while (tmpItem.startingPos == player.startingPos ||
                   tileArray[(int)tmpItem.startingPos.X, (int)tmpItem.startingPos.Y].isSolid);
                
                mapItems.Add(tmpItem);
            }

            //if the map has a shop, populate it with items 

                if (curMap.isShopMap)
                {

                    int i = 1; 
                    
                    
                    Usables tmpShopItem = new Usables();
                    

                    foreach (Tile t in tileArray)
                        if (t.name == "floor")
                        {
                            int tmpR = r.Next(lstUsables.Count);
                            randomUsable = lstUsables[tmpR];
                            Type tT = randomUsable.GetType();
                            if (tT.IsInstanceOfType(f)) //food
                                tmpShopItem = new Food(randomUsable, t.absPos);
                            else if(tT.IsInstanceOfType(p)) //potion
                                tmpShopItem = new Potion(randomUsable, t.absPos);
                                
                            else if(tT.IsInstanceOfType(sc)) //scroll
                                tmpShopItem = new Scroll(randomUsable, t.absPos);
                                
                            else
                                tmpShopItem = new Usables(lstUsables[r.Next(lstUsables.Count)], t.absPos);
                            tmpShopItem.isShopItem = true;
                            tmpShopItem.name += " ($" + tmpShopItem.price + ")";
                            tmpShopItem.ideedname += " ($" + tmpShopItem.price + ")";
                            mapItems.Add(tmpShopItem);
                            i++;
                        }
                    r = new Random();
                    int randShopItem = r.Next(mapItems.Count - i, mapItems.Count);
                    //place a pickaxe in the shop
                    
                    //first, copy one item in the shop to the new variable 
                    /*Item tmpItem2*/
                    
                    //now, replace the item with a new pickaxe
                    Item tmpItem2 = new Tool(Item.pickTexture, "pickaxe", 8, "Use where?", mapItems[randShopItem].startingPos, "Dig");
                    mapItems[randShopItem] = tmpItem2;
                    

                }
            return;
        }

        /// <summary>
        /// CREATEASSETSFROMFILE
        ///     reads in the text file containing texture names, and loads corresponding textures
        /// </summary>
        /// <param name="lstTex"></param>
        /// <param name="nameString"></param>
        private void CreateAssetsFromFile(List<Texture2D> lstTex, string nameString)
        {
                      
            StreamReader file = new StreamReader(nameString);

            int i = 0;
            do
            {
                //assetArray[i] = new Texture2D(GraphicsDevice, 80, 80);
                string curAsset = file.ReadLine();
                Texture2D tmpTx = Content.Load<Texture2D>(curAsset);
                tmpTx.Name = curAsset;
                lstTex.Add(tmpTx);
                 
                i++;
            }
            while (!file.EndOfStream);

            file.Close();
           

        }
        private void MsgWriter(int prevTurn, string m)
        {
            //check if the message string is over 1 line (80 characters/line) and creates additional lines accordingly
            double messageLines = Math.Ceiling((double)messageString.Length / 75);

            if (totalTurns != prevTurn && messageLines > 1 && !isInvShowing) //don't add a line for turns taken in inventory screen
                for (int l = 1; l <= messageLines; l++)
                {
                    if (((messageString.Length - 1) - messageString.LastIndexOf('\n')) > 75) //if we need another line break
                    {
                        int spaceReplace = messageString.LastIndexOf(' ', l * 60, 15);
                        messageString = messageString.Remove(spaceReplace, 1);
                        messageString = messageString.Insert(spaceReplace, "\n");
                    }
                }
        }
        private void LoadTexturesAndFonts()
        {
           
            Random r = new Random();
            
            Type p = typeof(Player);

            //*********ANIMATIONS**********************
            animTest = new AnimatedTexture(startPos, 0, 0, 0);
            //scrOverlay = new AnimatedTexture(Vector2.Zero, 0, 0, 0);
            //slashAtkTexture = null;


            AssignStrings();
            
            // *@*Load overlay textures*@*
            //Scrolling/static map overlays
            Overlay.fogTx = Content.Load<Texture2D>("forest overlay");
            Overlay.snowTx = Content.Load<Texture2D>("jotunheim overlay");
            Overlay.undergroundTx = Content.Load<Texture2D>("light radius");

            //scrOverlay = Content.Load<Texture2D>("forest overlay");
            //overlayTex = Content.Load<Texture2D>("forest overlay");


            //load tile, creature, and item textures (in that order)
            Tile.grassTexture = Content.Load<Texture2D>("grass");
            Tile.wallTexture = Content.Load<Texture2D>("village walls");
            Tile.floorTexture = Content.Load<Texture2D>("village floor");
            Tile.snowTexture = Content.Load <Texture2D>("snowscape3");
            lstTileTex = new List<Texture2D>();
            CreateAssetsFromFile(lstTileTex, StorageContainer.TitleLocation + @"\TileTexNames.dat");

            
            stalagmiteTex = Content.Load<Texture2D>("stalagmite");
            puddleTex = Content.Load<Texture2D>("puddle tile");

            //load monster/creature textures
            lstFrstMnstrTex = new List<Texture2D>();
            CreateAssetsFromFile(lstFrstMnstrTex, StorageContainer.TitleLocation + @"\ForestMnstrNames.dat");
            lstDngnMnstrTex = new List<Texture2D>();
            CreateAssetsFromFile(lstDngnMnstrTex, StorageContainer.TitleLocation + @"\DngnMnstrNames.dat");

            traderTex = Content.Load<Texture2D>("Trader");
            traderTex.Name = "trader";
            ravenTrapTex = Content.Load<Texture2D>("raven in the trap");
            
            //load item textures
            lstItemTex = new List<Texture2D>();
            //CreateAssetsFromFile(lstItemTex, "EqpTxNames.dat");

            
            Item.foodTexture = Content.Load<Texture2D>("Food");
            Item.gourdTex = Content.Load<Texture2D>("Gourd");
            Item.potionTexture = Content.Load<Texture2D>("Potion");
            Item.scrollTexture = Content.Load<Texture2D>("Scroll");
            Item.pickTexture = Content.Load<Texture2D>("pickaxe");

            Item.ringTexture = Content.Load<Texture2D>("Ring");
            Item.leisterTx = Content.Load<Texture2D>("leister");
            Item.longbowTx = Content.Load<Texture2D>("long bow");
            Item.arrowTx = Content.Load<Texture2D>("arrow");
            Item.arrowStkTx = Content.Load<Texture2D>("arrows small pile");
            Item.hideCrsTex = Content.Load<Texture2D>("hide cuirass");

            
            skeletonTexture = Content.Load<Texture2D>("skeleton");
            

            //system objects - inventory
            invItemThumb = new Rectangle(vPort.Width - 240, 50, Tile.tWidth * 2, Tile.tWidth * 2);
            invItemTex = null;

            //   BUTTONS (sidebar)  //
            //created L to R, top to bottom
            
            lookBtn = new Button(Content.Load<Texture2D>("look icon"), goldStringPos.X, goldStringPos.Y, Keys.L);
            eatBtn = new Button(Content.Load<Texture2D>("eat icon"), (goldStringPos.X + lookBtn.tex.Width ), goldStringPos.Y, Keys.E);
            toolBtn = new Button(Content.Load <Texture2D>("tools icon"), (goldStringPos.X + (2* lookBtn.tex.Width)), goldStringPos.Y, Keys.T);
            atkBtn = new Button(Content.Load<Texture2D>("attack icon"), goldStringPos.X, lookBtn.tex.Height + goldStringPos.Y, Keys.A);

            sideBtnRect = new Rectangle(vPort.Width - 245, (int)goldStringPos.Y + lineSkip, 245, 2 * (lookBtn.tex.Height));
          

            //  BUTTONS (lower button bar)  //
            // Created L to R
            
            packBtn = new Button(Content.Load<Texture2D>("pack"), vPort.X, vPort.Height, Keys.I);
            
            mapBtn = new Button(Content.Load<Texture2D>("map button"), packBtn.tex.Width, vPort.Height, Keys.M);
            
            logBtn = new Button(Content.Load<Texture2D>("log_btn"), 2*(packBtn.tex.Width), vPort.Height, Keys.B);
            
            saveBtn = new Button(Content.Load<Texture2D>("save button"), 3 * (packBtn.tex.Width), vPort.Height, Keys.S);
            bottomBtnRect = new Rectangle(-1, vPort.Height - packBtn.pos.Height - 1, vPort.Width - txObj.sideBarBG.Width, vPort.Height - packBtn.pos.Height);
            btnList = new List<Button>() { lookBtn, eatBtn, toolBtn, atkBtn, packBtn, mapBtn, logBtn, saveBtn };


            //fonts

            Font1 = Content.Load<SpriteFont>("Gabriola");
            Font1.LineSpacing = 28;
            headerFont = Content.Load<SpriteFont>("HeaderFont");


        }
        
        private void AssignStrings()
        {
            
            logWriter = File.AppendText(strLogName);
            logWriter.WriteLine("(BEGIN)");
            logWriter.Close();
            
            
            
            //load un-ID'ed scroll names
            defaultScrollNames = new List<string>{"weathered", "sun-bleached", "patchwork",
        "flaxen", "warped", "gossamer", "fibrous", "crumpled", "stitched", "moth-eaten", "dessicated" };

            iDeedScrollNames = new List<string> { "of knowledge", "of flame", "of lava strike", "of time stop", 
                "of unmaking", "of pure evil", "of cartography", "of travel" };

            //load un-ID'ed potion names
            defaultPotionNames = new List<string> {"curdling", "pulpy", "iridescent", 
            "stinging", "gritty", "balmy", "crackling", "odoriferous", "fizzling", "tingling",
            "sparkling", "fetid", "sudsy", "limpid", "saline", "amber", "congealed" };

            iDeedPotionNames = new List<string> { "of blindness", "of mutagenesis", "of plagues", 
                "of annointing",  "of convalescence", "of phasing", "of mead", "of phantasmagoria",
            "of invisibility", "of venom", "of depredation", "of potable water", "of latent power", 
            "of scleroses", "of soul seeking", "of ossification", "of experience"};
            
            //load un-ID'ed wand names
            wandNames = new List<string> {"a crooked", "a gnarled", "an alder", "a cracked", "a barbed",
            "a hollow", "an elm", "a resinous", "a pine", "an oak", "a willow", "a poplar"};

            iDeedWandNames = new List<string> {"of corporeality", //materializes spirit creatures (such as ghosts)
                "of polymorph", "of healing", "of wishing"};
            
            //load un-ID'ed ring names
            ringNames = new List<string> { "lambent", "ornate", "braided", "pave", "tarnished", 
                "patina", "knotted", "earthen", "ferrous", "jagged", "squarish" };

            iDeedRingNames = new List<string> {"of famine", "of locus mastery", "of genesis", 
                "of wasting", "of crystal skin", "of ashes", "of insulation", "of immunity", 
            "of protection", "of third sight", "of retaliation"}; //retaliation gives a chance of counterattack

            Random r = new Random();
            string tmpN, tmpID;

            //create the 2d list for matching random potion names with identifiers
            potionIDsandNames = new Dictionary<string, string>();


            while (iDeedPotionNames.Count - 1 >= 0)
            {

                tmpN = defaultPotionNames[r.Next(defaultPotionNames.Count)];
                tmpID = iDeedPotionNames[iDeedPotionNames.Count - 1]; //we're going through these 1 by 1
                potionIDsandNames.Add(tmpID, tmpN);
                defaultPotionNames.Remove(tmpN);
                iDeedPotionNames.Remove(tmpID);

            }
            //potionIDsandNames.Add("of curing", "of curing");
            //potionIDsandNames.Add("of strength", "of strength");
            //potionIDsandNames.Add("of speed", "of speed");


            //make the 2d list for matching scroll names with identifiers
            scrollIDsandNames = new Dictionary<string, string>(iDeedScrollNames.Count);


            while (iDeedScrollNames.Count - 1 >= 0)
            {
                //string tmpN, tmpID;
                tmpN = defaultScrollNames[r.Next(defaultScrollNames.Count)];
                tmpID = iDeedScrollNames[iDeedScrollNames.Count - 1]; //we're going through these 1 by 1
                scrollIDsandNames.Add(tmpID, tmpN);
                defaultScrollNames.Remove(tmpN);
                iDeedScrollNames.Remove(tmpID);

            }
            scrollIDsandNames.Add("of identification", "of identification");
            scrollIDsandNames.Add("of blessing", "of blessing");
            scrollIDsandNames.Add("of dispel hex", "of dispel hex");

            scrollTypeandMethod = new Dictionary<Scroll, MethodInfo>();


            //make the 2d list for matching ring names with id's
            ringIDsandNames = new Dictionary<string, string>(iDeedRingNames.Count);


            while (iDeedRingNames.Count - 1 >= 0)
            {
                //string tmpN, tmpID;
                tmpN = ringNames[r.Next(ringNames.Count)];
                tmpID = iDeedRingNames[iDeedRingNames.Count - 1]; //we're going through these 1 by 1
                ringIDsandNames.Add(tmpID, tmpN);
                ringNames.Remove(tmpN);
                iDeedRingNames.Remove(tmpID);

            }
        }

      

        //******************************MAIN GAME START!!!****************************
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = false;
            //graphics.IsFullScreen = true;
            
            //GamerServicesComponent gamerSrvc = new GamerServicesComponent(this);
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            GraphicsDevice.RenderState.MultiSampleAntiAlias = false;
             
            // Set this to true to make the mouse cursor visible.
            // Use the default (false) if you are drawing your own
            // cursor or don't want a cursor.
            
            sndUpdater = new SoundUpdater(this);
            aniUpdater = new AnimationUpdater(this);
            txObj = new SysTxLoader(this);
            keyHndlr = new KeyHandler(this);

            this.Components.Add(sndUpdater);
            this.Components.Add(aniUpdater);
            this.Components.Add(txObj);
            this.Components.Add(keyHndlr);

            StreamReader introReader = File.OpenText(StorageContainer.TitleLocation + @"\Intro.dat");
            while (!introReader.EndOfStream)
                introText += introReader.ReadLine() + "\n";

            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            
            // Initialize all system graphics devices/components and sound effects.
            vPort = graphics.GraphicsDevice.Viewport;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            nameSpriteBatch = new SpriteBatch(GraphicsDevice);

            //do following so that last game's message isn't written to new game's log
            messageString = "";
            logWriter = File.CreateText(strLogName);
            
            logWriter.Close();

            
            //set up all the positioning variables for static strings, sidebar text, etc.
            //LoadTexturesAndFonts depends on these values!
            sideBarPos = new Vector2(vPort.Width - txObj.sideBarBG.Width, 0);
            float offset = 17;
            nameStringPos = new Vector2(sideBarPos.X + offset, 0);
            turnStringPos = new Vector2(nameStringPos.X, nameStringPos.Y + lineSkip);
            hpStringPos = new Vector2(turnStringPos.X, turnStringPos.Y + lineSkip);
            lvlStringPos = new Vector2(hpStringPos.X, hpStringPos.Y + lineSkip);
            expStringPos = new Vector2(lvlStringPos.X, lvlStringPos.Y + lineSkip);
            strStringPos = new Vector2(expStringPos.X, expStringPos.Y + lineSkip);
            hungerStringPos = new Vector2(strStringPos.X, strStringPos.Y + lineSkip);
            goldStringPos = new Vector2(hungerStringPos.X, hungerStringPos.Y + lineSkip);
            mapNamePos = new Vector2(strStringPos.X, vPort.Height - 40);


            LoadTexturesAndFonts();
            

            //These values depend on LoadTexturesAndFonts having been called!
            introStringPos = new Vector2((vPort.Width / 2) - (Font1.MeasureString(introText).X / 2), vPort.Height);
            introTxtEnd = -(Font1.MeasureString(introText).Y);
            nameEntryFontPos = new Vector2(vPort.Width / 2 - ((int)Font1.MeasureString(nameEntryPrompt).Length() / 2), vPort.Height / 3);
            FontOrigin = new Vector2(vPort.Width - 240, 0);
            enteredKeysFontPos = new Vector2(vPort.Width / 4, 100);
            
            //farMovePos = Vector2.Zero;
            //the experience meter needs to be manually reset for new games
            expBar = 0;

            //use 80x80 sprites to fill grid


            //set up the map and visible area, as well as minimap
            tileArray = new Tile[maxTilesH, maxTilesV];
            visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
            miniMapRects = new Rectangle[maxTilesH, maxTilesV];
            tmpRooms = new List<Room>();
            
            //Create the starting map -- can be changed for purposes of testing new areas, but *RESTORE ORIGINAL (0,0,0,0)*
            CreateMap(0, 0, 0, 0);
            //CreateMap(2, 0, 0, 0);
 
            //player creation
            //First, find a free tile for random spawn point
            Random r = new Random();
            //Vector2 startPos;
            do
            {
                startPos = new Vector2(r.Next(5, 22), r.Next(5, 22));
            }while(tileArray[(int)startPos.X, (int)startPos.Y].isSolid
                || tileArray[(int)startPos.X, (int)startPos.Y].isBldngTile);

            player = new Player();
            player.startingPos = startPos;
            player.absolutePos = startPos;
            //numberHeld = new int[100]; //maximum # of items to be held, regardless of weight
            itemTxtAndQty = new Dictionary<string, int>() { };
            

            //set up map items

            mapItems = new List<Item>();
            pickedUpItems = new List<Item>();
            
            
            //INITIALIZE ITEMS
            //Define all the player properties that items can affect
            //Puts all items in the appropriate master lists
            //Have items that should appear more frequently (eg., food) be added more times
            //to the list
            
            //add those items whose ID'd names are known by default
            lstUsables = new List<Usables>(); //master list of usable/consumable items
            
            /*order for properties of potions: "of blindness", "of mutagenesis", "of plagues", 
                "of annointing",  "of convalescence", "of phasing", "of mead", "of phantasmagoria",
            "of invisibility", "of venom", "of depredation", "of potable water", "of latent power", 
            "of paralysis", "of soul seeking", "of scarring", "of experience" */
            
            Type pT = typeof(Player);
            Dictionary<string, PropertyInfo > effectPropDict = new Dictionary<string, PropertyInfo>()
            {{"You can no longer see. ", pT.GetProperty("isBlind")}, 
                {"Your ... migrates to your ***", pT.GetProperty("isMutated")},
                { "All manner of pestilence erupts within you!", pT.GetProperty("isPlagued")}, 
                {"You gain favour with the gods of ...", pT.GetProperty("morality")}, //normal or blessed, Asgard; cursed, opposite
                { "You begin to heal as would a god. ", pT.GetProperty("isRegenerating")},
                {"Your body becomes as thinnest air. ", pT.GetProperty("isPhasing")},//thin as air
                { "The world whirls before you. ", pT.GetProperty("isIntoxicated")}, 
                { "Visions spring forth from your addled mind. ", pT.GetProperty("isHallucinating")},
                { "Your form fades from view. ", pT.GetProperty("isInvisible")},
                { "", pT.GetProperty("isOverburdened") }, 
                {"You sense items. (Press any key)", pT.GetProperty("isBlind") },
                { "You feel somewhat better. ", pT.GetProperty("isIntoxicated")}, //lots of water will counter drunkenness
                { "You realize your greater potential. ", pT.GetProperty("isSurvitalized")},
                { "Your movements stiffen to a halt. ", pT.GetProperty("isParalyzed")},
                {"You sense monsters. (Press any key)", pT.GetProperty("isBlind")},
                { "You scream as every bone in your body fractures, and knits together more strongly than before.", pT.GetProperty("constitution")},  //cause player to pass out from pain?
                { "You feel wiser in the ways of the warrior.", pT.GetProperty("exp")}};
            
            //properties affected by potions
            PropertyInfo curHP = pT.GetProperty("curHP"), HP = pT.GetProperty("HP"),
            strength = pT.GetProperty("strength"), maxStr = pT.GetProperty("maxStrength"),
                        
            //properties affected by food
            fullnessProp = pT.GetProperty("fullness");

            //add some items to the initial inventory
            lstInitialInv = new List<Item>();
            Potion tmpCuringPotion = new Potion(Item.potionTexture, "of curing", "of curing", "You feel better. ", curHP, HP, 20, true);
            lstInitialInv.Add(tmpCuringPotion);
            
            
            //properties affected by scrolls
            MethodInfo identify = pT.GetMethod("Identify");


            Usables tmpFood = new Food(Item.foodTexture, "sack of rations", 2, "You stave off starvation a little longer.", fullnessProp);
            Usables tmpGourd = new Food(Item.gourdTex, "gourd", 3, "You drink from the gourd. It contains ", null);
            lstUsables.Add(tmpFood);
            lstUsables.Add(tmpGourd);
            lstInitialInv.Add(tmpFood);

            
            //add potions to master list
            int c = potionIDsandNames.Count-1 ;
            foreach ( KeyValuePair<string, PropertyInfo> propStr in effectPropDict)
            {
                lstUsables.Add(new Potion(Item.potionTexture, 
                    potionIDsandNames.Keys.ElementAt(c), potionIDsandNames.Values.ElementAt(c), propStr.Key, propStr.Value, null, 0, false));
                c--;
            }
 
            
            lstUsables.Add(new Potion(Item.potionTexture, "of strength", "of strength", "You feel stronger. ", strength, maxStr, 1, true));
            lstUsables.Add(new Potion(Item.potionTexture, "of curing", "of curing", "You feel better. ", curHP, HP, 20, true));            
            

            Scroll tmpIDScroll = new Scroll(Item.scrollTexture, "of identification", "of identification", null);
            tmpIDScroll.methodToInvoke = identify;
            scrollTypeandMethod.Add(tmpIDScroll, identify);
            lstInitialInv.Add(tmpIDScroll);

            scrollTypeandMethod.Add(new Scroll(Item.scrollTexture, "of blessing", "of blessing", null), null);
            scrollTypeandMethod.Add(new Scroll(Item.scrollTexture, "of dispel hex", "of dispel hex", null), null);
            scrollTypeandMethod.Add(new Scroll(Item.scrollTexture, "of cartography", scrollIDsandNames["of cartography"], "You become aware of your surroundings. "), null);
            scrollTypeandMethod.Add(new Scroll(Item.scrollTexture, "of travel", scrollIDsandNames["of travel"], "You warp to a new location. "), null);
            scrollTypeandMethod.Add(new Scroll(Item.scrollTexture, "of unmaking", scrollIDsandNames["of unmaking"], null), null);

   
            for (int i = 0; i < scrollTypeandMethod.Count; i++)
            {//add potions to master list
                Usables tmpScroll = scrollTypeandMethod.Keys.ElementAt(i);
                tmpScroll.methodToInvoke = scrollTypeandMethod.Values.ElementAt(i);
                lstUsables.Add(tmpScroll);
            }
           
            
            lstEquipment = new List<Equipment>();
            c = ringIDsandNames.Count-1 ;
            foreach ( KeyValuePair<string, string> rID in ringIDsandNames)
            {//add rings to master list
                string ID = ringIDsandNames.Keys.ElementAt(c);
                lstEquipment.Add(new Ring(Item.ringTexture, ID, ringIDsandNames.Values.ElementAt(c), 
                    "A ring of " + ID, 0, false));
                c--;
            }
            //Equipment tmpRing = new Ring(ringTexture,  ringIDsandNames.Values.ElementAt(r.Next(0, ringNames.Count())), 0, false);//, (float)r.Next(4, maxTilesH - 4), (float)r.Next(4, maxTilesV - 4));
            //Equipment tmpRing2 = new Ring(ringTexture, ringIDsandNames.Values.ElementAt(r.Next(0, ringNames.Count())), 0, false);//, (float)r.Next(4, maxTilesH - 4), (float)r.Next(4, maxTilesV - 4));
            //Equipment tmpRing3 = new Ring(ringTexture, ringIDsandNames.Values.ElementAt(r.Next(0, ringNames.Count())), 0, false);
            //Equipment tmpRing4 = new Ring(ringTexture, ringIDsandNames.Values.ElementAt(r.Next(0, ringNames.Count())), 0, false);

            Gold tmpGoldSm = new Gold(Content.Load<Texture2D>("gold small"), 1, 50);
            Gold tmpGoldMed = new Gold(Content.Load<Texture2D>("gold med"), 51, 101);
            
            lstGold = new List<Gold>() { tmpGoldSm, tmpGoldSm, tmpGoldSm, tmpGoldMed };
            //lstEquipment = new List<Equipment>() { tmpRing, tmpRing2, tmpRing3, tmpRing4 };

            //monster creation variables
            monsterArray = new List<Monster>(maxMstrs);
            deadMonsterArray = new List<Monster>();
            CreateMonsters(lstFrstMnstrTex);
            
            //set the array of visible tiles around player, 3 on each side, for a 7x7 grid
            //first find the upper left, then add in each direction

            visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];

            //int tileX = ((int)(startPos.X) - 3);
            //int tileY = ((int)(startPos.Y) - 3);
            int tileX = ((int)(startPos.X) - 4);
            int tileY = ((int)(startPos.Y) - 4);
            int tileOriginPosX = -Tile.tWidth, tileOriginPosY = -Tile.tHeight;
            for (int indexX = 0; indexX < visibleTilesX; indexX++)
            {

                for (int indexY = 0; indexY < visibleTilesY; indexY++)
                {
                    Tile tempTile = new Tile(tileArray[tileX, tileY], indexX, indexY);
                    tempTile.drawPos = new Vector2(tileOriginPosX, tileOriginPosY);
                    visibleTilesArray[indexX, indexY] = tempTile;
                    //if (tempTile.absPos == player.absolutePos)
                    //    player.curTile = new Tile(tempTile); //.isPlayerTile = true;

                    tileY++;
                    tileOriginPosY += Tile.tHeight;
                }
                //tileY = ((int)(startPos.Y) - 3);
                tileY = ((int)(startPos.Y) - 4);
                tileX++;
                tileOriginPosX += Tile.tWidth;
            } 
            

            //Create each map and save it to the MapArray
           
            //mapArray = new Map[3,3,4];
            mapArray = new Map[6, 3, 4];
            for (int x1 = 0; x1 < 6; x1++)
                for (int y1 = 0; y1 < 3; y1++)
                    for (int z1 = 0; z1 < 4; z1++)
                        mapArray[x1, y1, z1] = new Map();

            //create all the NPCs/actionable items that give you choices/quests throughout the game
            CreateQuestNPCs();

            //variables to hold location of the stairs to the dungeon
            r = new Random();
            int stairMapX = r.Next(1, 3), stairMapY = r.Next(0, 3);
            int stairTileX = r.Next(6, 24), stairTileY = r.Next(6, 24);
            stairMap = new Vector2(stairMapX, stairMapY); //for debugging - print out on main game screen to find stairs more easily

            int //teleportMapX = 0, teleportMapY = 0,
                teleportTileX = r.Next(6, 24), teleportTileY = r.Next(6, 24);

            ///
            //*************FOREST MAPS********************
            ///
            
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //First map -- SET BACK TO FOREST VILLAGE WHEN DONE TESTING new realms
                    if (i==0 && j==0)
                    {
                         
                        curMap = (new Map(ref tileArray, ref miniMapRects, ref monsterArray,
                            ref deadMonsterArray, ref mapItems, "The Ruined Village", /*2*/0, 0, 0, 0,
                            true, tmpRooms));
                        //this makes the first area have a shop
                        curMap.trader = villageTrader;
                        curMap.savedMonsterArray.Add(curMap.trader); 
                        curMap.shopDoorway = tmpDoorway;
                        CreateMapItems();
                        mapArray[i,j,0] = curMap;
                        curMap.areaMapTile.isDiscovered = true;
                        //curMap.hasScrollingOverlay = true;
                        //else if (teleportMapX == i && teleportMapY == j)
                    //{
                        tileArray[teleportTileX, teleportTileY] =
                            new Tile(Content.Load<Texture2D>("snowscape"), "none", "teleport", teleportTileX, teleportTileY, false);
                        //store the tile, to be matched with a dungeon's stairway up
                        tileArray[teleportTileX, teleportTileY].isExit = true;
                    //}    
                        continue;
                    }
                    
                        
                    //Each subsequent map
                    tileArray = new Tile[maxTilesH, maxTilesV];
                    visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
                    miniMapRects = new Rectangle[maxTilesH, maxTilesV];
                    monsterArray = new List<Monster>(maxMstrs);
                    mapItems = new List<Item>();
                    pickedUpItems = new List<Item>();
                    deadMonsterArray = new List<Monster>();
                    
                    curMap = (new Map(ref tileArray, ref miniMapRects, ref monsterArray,
                        ref deadMonsterArray, ref mapItems, "The Wooded Wilds", 0, i, j, 0, false, tmpRooms));
                    //curMap.overlay = fogTexture;
                    //curMap.hasScrollingOverlay = true;
                    CreateMap(0, i, j, 0);

                    

                    CreateMonsters(lstFrstMnstrTex);
                    CreateMapItems();
                    curMap.GetMap();
                    //does this map have a quest NPC? if so, add it
                    //foreach (QuestNPC q in lstNPC)
                    //{
                    //    if (q.mapX == i && q.mapY == j)
                    //    {
                    //        do
                    //        {
                    //            q.startingPos = new Vector2(r.Next(5, maxTilesH - 5), r.Next(5, maxTilesV - 5));
                    //            q.absolutePos = q.startingPos;
                    //            q.newPos = q.absolutePos;

                    //        } while (tileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid);

                    //        curMap.questNPC = q;
                    //        curMap.hasQuestNPC = true;
                    //        //make its starting tile solid
                    //        curMap.questNPCs.Add(q);
                    //        //tileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid = true;
                    //    }
                    //}
                    
                    //is this the map with stairs down? if so, create them on a random tile
                    if (stairMapX == i && stairMapY == j)
                    {
                        tileArray[stairTileX, stairTileY] =
                                   new Tile(Content.Load<Texture2D>("stair down"), "none", "exit down", stairTileX, stairTileY, false);       
                        //store the tile, to be matched with a dungeon's stairway up
                        curMap.stairTileDown = tileArray[stairTileX, stairTileY];
                    }
                    
                    curMap.savedTileArray = tileArray;
                    mapArray[i, j, 0] = curMap;
                }
            }
            
            ///
            // ***********JOTUNNHEIM MAPS**************
            ///
            
            for (int i = 3; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {//Each subsequent map
                    tileArray = new Tile[maxTilesH, maxTilesV];
                    visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
                    miniMapRects = new Rectangle[maxTilesH, maxTilesV];
                    monsterArray = new List<Monster>(maxMstrs);
                    mapItems = new List<Item>();
                    pickedUpItems = new List<Item>();
                    deadMonsterArray = new List<Monster>();

                    curMap = (new Map(ref tileArray, ref miniMapRects, ref monsterArray,
                        ref deadMonsterArray, ref mapItems, "Jotunnheim", 2, i, j, 0, false, tmpRooms));
                    //curMap.overlay = fogTexture;
                    curMap.hasScrollingOverlay = true;
                    CreateMap(2, i, j, 0);



                    CreateMonsters(lstFrstMnstrTex);
                    CreateMapItems();
                    curMap.GetMap();
                    //does this above-ground map have a quest NPC? if so, add it now
                    //foreach (QuestNPC q in lstNPC)
                    //{
                    //    if (q.mapX == i && q.mapY == j)
                    //    {
                    //        do
                    //        {
                    //            q.startingPos = new Vector2(r.Next(5, maxTilesH - 5), r.Next(5, maxTilesV - 5));

                    //        } while (tileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid);

                    //        curMap.questNPC = q;
                    //        curMap.hasQuestNPC = true;
                    //        //make its starting tile solid
                    //        tileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid = true;
                    //    }
                    //}

                    //is this the map with stairs down? if so, create them on a random tile
                    if (stairMapX == i && stairMapY == j)
                    {
                        tileArray[stairTileX, stairTileY] =
                                   new Tile(Content.Load<Texture2D>("stair down"), "none", "exit down", stairTileX, stairTileY, false);
                        //store the tile, to be matched with a dungeon's stairway up
                        curMap.stairTileDown = tileArray[stairTileX, stairTileY];
                    }
                    curMap.savedTileArray = tileArray;
                    mapArray[i, j, 0] = curMap;
                }
            }

            //*******DUNGEON MAPS (below Forest)***************
            // each floor is 3x3 maps; floors are 10, 20, and 30 metres deep

            for (int k = 1; k <= 3; k++) // depth of map
            {
                //make a stairway down on a random map on this floor
                //if lower than B1F (k > 1), the stairMapX and Y variables will change
                if (k > 1)
                {
                    stairMapX = r.Next(0, 3);
                    stairMapY = r.Next(0, 3);
                } 
                //make sure the stairs down aren't on the same map as the stairs up
                int stairsDownX, stairsDownY;
                
                    do
                    {
                        stairsDownX = r.Next(0, 3); stairsDownY = r.Next(0, 3);
                    } while (stairsDownX == stairMapX && stairsDownY == stairMapY);
               
                for (int i = 0; i < 3; i++) // x-coordinate of map
                {
                    for (int j = 0; j < 3; j++) //y-coordinate of map
                    {

                        tileArray = new Tile[maxTilesH, maxTilesV];
                        visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
                        miniMapRects = new Rectangle[maxTilesH, maxTilesV];
                        monsterArray = new List<Monster>(maxMstrs);
                        mapItems = new List<Item>();
                        pickedUpItems = new List<Item>();
                        deadMonsterArray = new List<Monster>();

                        curMap = (new Map(ref tileArray, ref miniMapRects, ref monsterArray,
                            ref deadMonsterArray, ref mapItems, k * 10 + " Metres Under", 1, i, j, k, false, tmpRooms));

                        //curMap.overlay = lightRadiusTex;
                        
                        CreateDungeon(i, j, k);
                        CreateMonsters(lstDngnMnstrTex);
                        CreateMapItems();
                        curMap.GetMap();
                        //make stairs up in a random location (since stairs down may lead to a solid tile on this floor)
                        
                        if ((stairMapX == i && stairMapY == j) || (stairsDownX == i && stairsDownY == j))
                        {
                            Random p = new Random();
                            string exitDirection;
                            do
                            {
                                //stairs up may be no closer to the edge of the map than 5 tiles 
                                stairTileX = p.Next(5, maxTilesH - 5);
                                stairTileY = p.Next(5, maxTilesV - 5);

                            }
                            while (tileArray[stairTileX, stairTileY].isSolid);// &&
                            //                            tileArray[stairTileX, stairTileY].name != "floor");
                            
                            
                                if (stairMapX == i && stairMapY == j)
                                {
                                    curMap.stairTileUp = tileArray[stairTileX, stairTileY];
                                    exitDirection = "exit up";
                                }
                                else
                                {
                                    curMap.stairTileDown = tileArray[stairTileX, stairTileY];
                                    exitDirection = "exit down";
                                }
                            
                                tileArray[stairTileX, stairTileY] =
                                new Tile(Content.Load<Texture2D>("stair down"), "none", exitDirection, stairTileX, stairTileY, false);
                            

                            //save the changes to the savedTileArray because we added a tile to the map
                            curMap.savedTileArray = tileArray;
                            
                        }
                        
                        mapArray[i, j, k] = curMap;
                    }
                }
            }

            //----!!!!!!!!!!!!!!!!--------Time To Add QUESTS AND QUEST NPCS to generated maps----!!!!!!!!!!!!!!!!!!------//
        foreach (Map m in mapArray)
            if (!m.isDummyMap)
            {
                {
                    foreach (QuestNPC q in lstNPC)
                    {
                        if (q.mapX == m.mapPosX && q.mapY == m.mapPosY && q.mapZ == m.depth)
                        {
                            do
                            {
                                q.startingPos = new Vector2(r.Next(5, maxTilesH - 5), r.Next(5, maxTilesV - 5));
                                q.absolutePos = q.startingPos;
                                q.newPos = q.absolutePos;

                            } while (m.savedTileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid);

                            m.questNPC = q;
                            m.hasQuestNPC = true;
                            //make its starting tile solid
                            m.questNPCs.Add(q);
                            //tileArray[(int)q.startingPos.X, (int)q.startingPos.Y].isSolid = true;
                        }
                    }


                    foreach (Quest q in lstQuest)
                    {
                        //properly place the Quest NPCs in their respective quest locations
                        foreach (QuestNPC npc in q.qNPCs)
                        {
                            if (m.mapPosX == npc.mapX && m.mapPosY == npc.mapY && m.depth == npc.mapZ)
                            {
                                //if the npc is supposed to reside in a solid space, have it do so
                                do
                                {
                                    npc.startingPos = new Vector2(r.Next(5, maxTilesH - 5), r.Next(5, maxTilesV - 5));
                                } while ((q.isOnSolidSpace && !m.savedTileArray[(int)npc.startingPos.X, (int)npc.startingPos.Y].isSolid)
                                    || (!q.isOnSolidSpace && m.savedTileArray[(int)npc.startingPos.X, (int)npc.startingPos.Y].isSolid));

                                npc.absolutePos = npc.startingPos;
                                npc.newPos = npc.absolutePos;
                                //m.quest = q;
                                m.hasQuestNPC = true;
                                m.questNPCs.Add(npc);
                            }
                            m.savedTileArray[(int)npc.startingPos.X, (int)npc.startingPos.Y].isSolid = false;
                            m.savedTileArray[(int)npc.startingPos.X, (int)npc.startingPos.Y].texture = m.defaultTileTx;
                        }
                    }
                }
            }

            //make sure to assign the current map's values back to the global variables
            mapItems = new List<Item>();
            curMap = mapArray[0, 0, 0];
            //curMap = mapArray[3, 0, 0];
            curMap.GetMap();
            
            
            // TODO: use this.Content to load your game content here
            mouseCmpt = new MouseInput(this);
            this.Components.Add(mouseCmpt);

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void showInventory(List<Item> invToList)
        {
            
            if (!(newGame || isInvShowing))
            {
                itemsHeld = new List<Item>(invToList);//new List<Item>(player.inventory);
                //list of names - for stacking identical items on the inventory screen
                //itemNames = new List<string>() { };//(100)
                itemTxtAndQty = new Dictionary<string, int>() { };
                invRects = new List<ItemRect>();
                foreach (Item it in itemsHeld)
                {
                    int i = 0;

                    if (itemTxtAndQty.TryGetValue(it.name, out i))
                    {
                        //more than 1 exists, so add the amount of this item
                        itemTxtAndQty[it.name]++;
                    }

                    else
                    { //first, is this an equipped item?
                        Type e = typeof(Equipment);

                        if (it.GetType().IsSubclassOf(e))
                        {
                            string eqStr = " (equipped)";
                            Equipment tmpEq = (Equipment)it;
                            //int equippedItem = itemNames.IndexOf(it.name);
                            if (tmpEq.isEquipped && !(it.name.Contains(eqStr)))
                                it.name += eqStr;
                            else if (!(tmpEq.isEquipped) && it.name.Contains(eqStr))
                                it.name = it.name.Trim(eqStr.ToCharArray());


                        }
                        itemTxtAndQty.Add(it.name, 1);
                        invRects.Add(new ItemRect(it.texture));
                    }

                    //if (itemNames.Contains(it.name))



                }
                

                //create rectangles to compare to click area
                itemRects = new Rectangle[itemTxtAndQty.Count];//[iNames.Count];
                int startY = 50;
                //invRects = new List<ItemRect>(itemTxtAndQty.Count);
                for (int x = 0; x < itemTxtAndQty.Count/*iNames.Count*/; x++)
                {
                    itemRects[x] = new Rectangle(120, startY+2, ((int)Font1.MeasureString
                        (itemTxtAndQty.Keys.ElementAt(x)/*iNames[i]*/.PadLeft(40)).X), Game1.lineSkip);
                    invRects[x].area = new Rectangle(120, startY + 2, ((int)Font1.MeasureString
                        (itemTxtAndQty.Keys.ElementAt(x)/*iNames[i]*/.PadLeft(40)).X), Game1.lineSkip);
                    invRects[x].txtColor = Color.Wheat;
                    invRects[x].bgColor = Color.SandyBrown;
                    
                    startY += lineSkip;
                }
                isInvShowing = true;
                isSubScrnShowing = true;
            }
            else //exit inventory
            {
                isInvShowing = false;
                isSubScrnShowing = false;
            }
            return;
        }

        public void showMap()
        {
            int x = Map.mapView.X;
            int y = Map.mapView.Y = 42;
            
            Map.scaleFactor.X = ((float)vPort.Width / (float)txObj.mapScrBG.Width);
            Map.scaleFactor.Y = ((float)(vPort.Height - 2 * y) / (float)txObj.mapScrBG.Height);
            if (!newGame && !isMapShowing)
            {
                Map.mapView = new Rectangle(x, y, (int)(txObj.mapScrBG.Width * Map.scaleFactor.X), (int)(txObj.mapScrBG.Height * Map.scaleFactor.Y));
                //source is a section of the main map image
                //Map.mapScrnSourceRect = new Rectangle(
                //    txObj.mapScrBG.Width - Map.mapView.Width - ((txObj.mapScrBG.Width - Map.mapView.Width)/2) + (curMap.mapScrRect.X - (txObj.mapScrBG.Width/2)),
                //txObj.mapScrBG.Height - Map.mapView.Height - ((txObj.mapScrBG.Height - Map.mapView.Height)/2) + (curMap.mapScrRect.Y - (txObj.mapScrBG.Height/2)),
                //Map.mapView.Width, Map.mapView.Height);
                Map.mapScrnSourceRect = new Rectangle(0, 0, txObj.mapScrBG.Width, txObj.mapScrBG.Height);
                curMap.drawPos = new Vector2(curMap.mapScrRect.X * Map.scaleFactor.X,
                        (curMap.mapScrRect.Y * Map.scaleFactor.Y) + Map.mapView.Y);
                isMapShowing = true;
            }
            else
                isMapShowing = false;
            return;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Update(GameTime gameTime)
        {
            int lastTurn = totalTurns;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            if (introScreen)
            {
                introStringPos.Y -= 0.5f;
                KeyboardState newState = Keyboard.GetState();
                
                if (newState.GetPressedKeys().Length > 0 || introStringPos.Y < (introTxtEnd ))
                {
                    introScreen = false;
                    titleScreen = true;
                    oldInput = newState;
                    
                }
            }
            else if (titleScreen)
            {
                
                sndUpdater.PlayTitleMusic(gameTime, false);
                KeyboardState newState = Keyboard.GetState();
                if (newState.IsKeyDown(Keys.Enter) && !oldInput.IsKeyDown(Keys.Enter))
                {
                    titleScreen = false;
                    newGame = true;
                    //oldInput = newState;
                    //return;
                }
                oldInput = Keyboard.GetState();
                mouseCmpt.UpdateMouseMovt(gameTime);
                return;
            }



            else if (newGame)
            {
                sndUpdater.PlayTitleMusic(gameTime, true);
                DirectoryInfo d = new DirectoryInfo(saveFileDir);
                //check whether save directory exists and has any save files in it
                if (!isFileSlctShowing)


                    if (Directory.Exists(saveFileDir) && d.GetFiles("*.sav").Length > 0)
                    {
                        //show file select screen
                        isFileSlctShowing = true;

                    }


                //get the player's name
                UpdateName(gameTime);
                //set intro message to be shown on first turn
                mouseCmpt.UpdateMouseMovt(gameTime);

                if (mouseCmpt.newMState.LeftButton == ButtonState.Pressed)
                {
                    
                    mouseCmpt.SubScrnClick(gameTime);
                    UpdateInput(gameTime);
                }
                
            }



        //keeps keyboard polling from occurring too often 
            else if (gameTime.TotalRealTime.TotalMilliseconds - oldTime >= 150)
            //(!player.isMoving)
            {

                if (isTinted) // if on map screen, have current map blink
                { //also pertains to Look function - have moused-over tile blink
                    tintColor = Color.White;
                    isTinted = false;
                }
                else
                {
                    tintColor = Color.Moccasin;
                    isTinted = true;
                }


                //if the player was attacked last turn, they only flash red until this point
                player.tint = Color.White;

                //see what the following conditional does... if bad, remove
                KeyboardState kbState = Keyboard.GetState();
                
                if(!Tile.isMoving)
                    UpdateInput(gameTime);
                //player.CheckState(ref messageString);

                //play the map's BGM (if not already playing)
                if (SoundUpdater.BGMInst == null || (SoundUpdater.BGMInst.State == SoundState.Stopped))
                {  
                    curMap.bgmInst = sndUpdater.PlayBGM(curMap.bgm);
                }
                if (isLooking || isUsing || isSafetyOff)
                {
                    Color originalTileClr;
                    foreach (Tile t in visibleTilesArray)
                    {
                        originalTileClr = t.tileTint;
                        if (t.Contains(mouseCmpt.newMState.X, mouseCmpt.newMState.Y, Tile.tWidth))

                            if (isTinted)
                                t.tileTint = tintColor;
                            else
                                t.tileTint = originalTileClr;
                    }

                }

                //Trader (if one exists on current map) shuffles in place
                if (monsterArray.Contains(villageTrader))
                {

                    //"animates" the shopkeeper - cheatily :)
                    if (monsterArray[monsterArray.IndexOf(villageTrader)].sprE == SpriteEffects.None)
                        monsterArray[monsterArray.IndexOf(villageTrader)].sprE = SpriteEffects.FlipHorizontally;
                    else
                        monsterArray[monsterArray.IndexOf(villageTrader)].sprE = SpriteEffects.None;

                }

            }
            else //main game is in play, so do continuous game updates
            {
                //Player standing/moving animations get updated

                animTest.UpdateFrame(elapsed);
                aniUpdater.Update(gameTime);

                player.Update(elapsed);

                if (mouseCmpt.newMState.LeftButton == ButtonState.Pressed)
                {
                    //movingItems = new List<Item>();
                    //mouseCmpt.Click(gameTime, itemNames, lineSkip, ref itemsHeld, numberHeld, ref messageString);
                    mouseCmpt.Click(gameTime, itemTxtAndQty, lineSkip, ref itemsHeld, ref messageString);
                    UpdateInput(gameTime);
                }
                if ((mouseCmpt.newMState.RightButton == ButtonState.Pressed && !isThrowing && !player.isDead))
                {
                    movingItems = new List<Item>();
                    mouseCmpt.rClick(gameTime, ref itemsHeld, /*numberHeld,*/ ref messageString);

                    UpdateInput(gameTime);
                }
                mouseCmpt.UpdateMouseMovt(gameTime);

                //suspend mouse input while item(s) are moving (i.e., being thrown)
                if (isThrowing)
                {



                    foreach (Item i in mapItems)
                    {
                        if (i.isInMotion && !movingItems.Contains(i))
                        {
                            movingItems.Add(i);


                        }
                    }

                    foreach (Item it in movingItems)
                        it.Move(player, it, ref messageString);


                }

            }
            //update tile draw positions
            if (Tile.isMoving && !player.isMoving)
                Tile.Update();
            



            //update the autoscrolling foreground/background overlay, if applicable
            curMap.UpdateOverlayPos();
            

            //update the mouse position and, if applicable, button click state

            //if (mouseCmpt.newMState.LeftButton == ButtonState.Pressed)
           
            //{
            //    //movingItems = new List<Item>();
            //    //mouseCmpt.Click(gameTime, itemNames, lineSkip, ref itemsHeld, numberHeld, ref messageString);
            //    mouseCmpt.Click(gameTime, itemTxtAndQty, lineSkip, ref itemsHeld, ref messageString);
            //    UpdateInput(gameTime);
            //}
            //if ((mouseCmpt.newMState.RightButton == ButtonState.Pressed && !isThrowing && !player.isDead))
            //{
            //    movingItems = new List<Item>();
            //    mouseCmpt.rClick(gameTime, ref itemsHeld, /*numberHeld,*/ ref messageString);
                
            //    UpdateInput(gameTime);
            //}
            //mouseCmpt.UpdateMouseMovt(gameTime);

            if (Game1.isInvShowing)
            {
                mouseCmpt.InvMouseMovt(itemTxtAndQty.Count, itemsHeld, itemTxtAndQty);
            }

            //format the message string
            MsgWriter(lastTurn, messageString);
            

            
            
            
            base.Update(gameTime);

        }
        //this is where the player enters their name at the start of a game
        
        public void UpdateName(GameTime gameTime)
        {
            //enteredKeys = "";
            if (!chooseGender)
            {
                KeyboardState newInput = Keyboard.GetState();
                foreach (Keys key in newInput.GetPressedKeys())
                {

                    if (oldInput.IsKeyUp(key) && newInput.IsKeyDown(Keys.Enter))
                    {
                        if (enteredKeys == "")
                            enteredKeys = "Nameless One";
                        //player is done entering name, so have them choose a gender
                        

                        chooseGender = true;

                    }
                    else if (oldInput.IsKeyUp(key))
                    {
                        if (letterKeys.Contains(key.ToString().ToLower()))
                            if (enteredKeys.Length < 1 && oldState.IsKeyUp(key))
                            {//forces the first letter to be a capital, disregarding any previous input
                                oldState = newInput;
                                enteredKeys += key.ToString();
                            }
                            else
                                //second or later letter, so convert it to lower and add it to string
                                enteredKeys += key.ToString().ToLower();
                    }
                }
                totalTurns = 0;
                oldInput = newInput;

            }
        }

        

        /// <summary>
        /// Delimits the visible grid around the player based on their map position
        /// </summary>
        /// <param name="baseX">leftmost X coordinate</param>
        /// <param name="baseY">topmost Y coordinate</param>
        /// <param name="endX"> rightmost X coordinate</param>
        /// <param name="endY">bottommost Y coordinate</param>
        private void UpdateVisibleArea(ref int baseX, ref int baseY, ref int endX,
            ref int endY)
        {
            
            baseX = ((int)(player.absolutePos.X) - 3);
            if (baseX < 0) baseX = 0;
            else if (baseX > maxTilesH - visibleTilesX) baseX = maxTilesH - visibleTilesX;
            
            baseY = ((int)(player.absolutePos.Y) - 3);
            if (baseY < 0) baseY = 0;
            else if (baseY > maxTilesV - visibleTilesY) baseY = maxTilesV - 7;
            
            endX = ((int)(player.absolutePos.X) + 3);
            if (endX > maxTilesH - 1) endX = maxTilesH - 1;
            
            endY = ((int)(player.absolutePos.Y) + 3);
            if (endY > maxTilesV - 1) endY = maxTilesV - 1;
            
            
            //update visible area only if it is within bounds of the tileArray
            visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
            int drawPosX = -Tile.tWidth;
            int drawPosY = -Tile.tHeight;
            
            for (int indexX = 0; indexX < visibleTilesX; indexX++)
            {
                for (int indexY = 0; indexY < visibleTilesY; indexY++)
                {
                    
                    Tile tempTile = new Tile(tileArray[indexX + baseX, indexY + baseY], indexX, indexY);
                    tempTile.drawPos = new Vector2(drawPosX, drawPosY);
                    visibleTilesArray[indexX, indexY] = tempTile;

                    if (tempTile.vPos == player.curPos)//(tempTile.absPos == player.absolutePos)
                    {
                        //set the player's tile for use in discovering adjacent tiles
                        player.curTile = tileArray[indexX + baseX, indexY + baseY];
                    }
                    drawPosY += Tile.tHeight;
                }
                drawPosX += Tile.tWidth;
                drawPosY = -Tile.tHeight;
                
            }
            

        }

        private void UpdateDiscoveredArea()
        {
            //update discovered tiles - 6x6 out of 7x7 area 
            //(unless at a map edge - then light up the corresponding edge)

            int vX = 1, vY = 1, maxX = 5, maxY = 5;
            if (visibleTilesArray[0, 0].absPos.X == 0)
                vX = 0;
            else if (visibleTilesArray[visibleTilesX - 1, visibleTilesY - 1].absPos.X == maxTilesH - 1)
                maxX = 6;
            if (visibleTilesArray[0, 0].absPos.Y == 0)
                vY = 0;
            else if (visibleTilesArray[visibleTilesX - 1, visibleTilesY - 1].absPos.Y == maxTilesV - 1)
                maxY = 6;

            //set isDiscovered = true for tiles that have just been discovered,
            //with the following layout:
            /* _____
             _|     |_
            |         |
            |    p    |   p = player
            |_       _|
              |_____|  */
            for (int x2 = vX; x2 <= maxX; x2++)
            {
            
                for (int y2 = vY; y2 <= maxY; y2++)
                {
                    NextTile:
                    //make sure corner tiles remain undiscovered
                    if (!((x2 == vX && y2 == vY) || (x2 == maxX && y2 == vY) ||
                        (x2 == vX && y2 == maxY) || (x2 == maxX && y2 == maxY)))
                    {
                        Tile tmpTile = visibleTilesArray[x2, y2];
                        
                        Tile absTile = tileArray[(int)tmpTile.absPos.X, (int)tmpTile.absPos.Y];
                        
                        
                        
                        //make sure shops/rooms stay darkened until entered
                        foreach (Room r in tmpRooms)
                        {
                            if (r.roomTiles.Contains(absTile))
                            {
                                absTile.tileTint = Color.DarkSlateGray;
                                if (!(r.roomTiles.Contains(player.curTile)))
                                {

                                    if (y2 != maxY)
                                    {
                                        y2++;
                                        goto NextTile;
                                    }
                                    else
                                        return;
                                }
                                else
                                {
                                    //r.isDiscovered = true;
                                    break;
                                }
                            }  
                        }
                        absTile.isDiscovered = true;

                        absTile.tileTint = Color.White;
                        
                    }
                }
            }
        }

        public void UpdateInput(GameTime gameTime)
        {
            if (player.oldHP != player.curHP)
                hpBar = (double)player.curHP / (double)player.HP * (double)100;

            if (player.oldExp != player.exp)
                expBar = (double)player.exp / (double)player.levelExpArray[player.level] * (double)100;
            //lastTurn = totalTurns;

            hpString = "HP:  ";
            hpString = hpString.PadRight(10, ' ') + player.curHP.ToString() + "/" + player.HP.ToString();
            lvlString = "Level: " + player.level.ToString();
            expString = "EXP: ";
            expString = expString.PadRight(10, ' ') + player.exp.ToString() + "/" + player.levelExpArray[player.level];
            strString = "STR: ";
            strString = strString.PadRight(10, ' ') + player.strength.ToString();
            hungerString = "Hunger: ";
            hungerString = hungerString.PadRight(10, ' ');
            goldString = player.gold + " Bullion";
            //turnString = "Turn: " + totalTurns;
            mapString = curMap.name; 
            
            int lastTurn = totalTurns; 
            player.oldHP = player.curHP;
            player.oldExp = player.exp;
             
            KeyboardState newState = Keyboard.GetState();
            newInput = newState;

            
            player.newCurPos = player.curPos; //player "cursor" position
            
            //demarcate the edges of the map
            int tileX = 0, tileY = 0, endTileX = 0, endTileY = 0;

            UpdateVisibleArea(ref tileX, ref tileY, ref endTileX, ref endTileY);

            Keys[] pressedKeys = new Keys[2];

            //check for right-click--start secondary weapon fire & sound effect
            if (isThrowing && !player.isDead)
            {
                sndUpdater.UpdateSound(SoundUpdater.fxList[(int)SoundUpdater.EffectsIDs.throwFX]);
                pressedKeys = new Keys[] { Keys.None };
            }


            else if (player.isMakingFarMove && !player.isDead)
            {
                List<Keys> farMoveKeys = new List<Keys>(); //adds keypresses based on the far move direction
                //if (player.farmovepos == player.newpos)
                //{
                //    player.farmovepos = vector2.zero;
                //    player.ismakingfarmove = false;
                //    player.absolutepos = player.newpos;
                //    //return;
                //}
                //int x = 0, y = 0;
                int playerX = (int)player.newPos.X;
                int playerY = (int)player.newPos.Y;

                if (player.farMovePos.X > playerX)
                   
                    farMoveKeys.Add(Keys.Right);

                else if (player.farMovePos.X < playerX)
                    
                    farMoveKeys.Add(Keys.Left);

                if (player.farMovePos.Y > playerY)
                    
                    farMoveKeys.Add(Keys.Down);

                else if (player.farMovePos.Y < playerY)
                    
                    farMoveKeys.Add(Keys.Up);

                farMoveKeys.CopyTo(pressedKeys);

            }
            

            else
                pressedKeys = (newState.GetPressedKeys());
 
            ReInitializeInput:
            //Loop checks all the keys being pressed and updates the sprite's position and turn #
            
            for (int i = 0; i < pressedKeys.Length; i++)
            {
            //first, check for conditions that require specific input--if not rec'd, pass to next turn    
                
            // If game is over or player pressed Esc, must make choice with Y or N
                if (isExiting || gameOverScreen) // player.isDead || 
                {
                    if (newState.IsKeyDown(Keys.N))//(currentKey == Keys.N)
                    { //exit with n
                        logWriter.Close();
                        File.Delete(strLogName);
                        
                        this.Exit();
                    }
                    else if (newState.IsKeyDown(Keys.Y))//(currentKey == Keys.Y)
                    {
                        
                        if (player.isDead)//player.curHP <= 0)
                        { //game had ended, so start a new game
                            oldState = newState;
                            chooseGender = false;
                            chooseClass = false;
                            enteredKeys = "";
                            totalTurns = 0;
                            newGame = true;
                            gameOverScreen = false;
                            isSubScrnShowing = false;
                            LoadContent();
                            
                        }
                        else //resume current game
                        {
                            isExiting = false;
                            //player.isDead = false;
                            return;
                        }
                    }
                    else
                        return;
                }
                else if (isEating && !isInvShowing)
                {
                    
                    if (newState.IsKeyDown(Keys.Y))
                    {

                        Food tmpFood = new Food();
                        Type t = player.itemToUse.GetType();

                        if (t.IsInstanceOfType(tmpFood))
                        {
                            messageString = player.Use(player.itemToUse, messageString);
                            mapItems.Remove(player.itemToUse);
                        }
                        else
                        {

                            messageString = "You try to gag down the " + player.itemToUse.name +
                                ", but it only results in your stomach contents coming back up.";
                            player.fullness -= 5;

                        }
                        //isEating = false;
                        //return;
                    }

                    else if (newState.IsKeyDown(Keys.N) || (player.itemToUse == null))
                    {
                        showInventory(player.inventory);
                        //isInvShowing = true;
                        break;
                    }
                }
              
                
                else if (isLooking)
                {
                    //messageString = "Look where? (left-click on a tile) ";
                    oldState = newState;
                    newState = Keyboard.GetState();

                    if (isInvShowing && newState.IsKeyDown(Keys.Enter))
                    //player was looking at a tile last turn, so show tile's "inventory"
                    {
                        isLooking = false;
                        isInvShowing = false;
                        pressedKeys = new Keys[] { Keys.Enter };
                        goto ReInitializeInput;

                    }

                    else
                    {
                        if (!isInvShowing)
                            showInventory(itemsHeld);

                        break;
                        
                    }


                }
                
                else if (isLogShowing)// || isInvShowing)
                {
                    if (newState.IsKeyDown(Keys.Down))
                    {
                        if (endLine < logLines.Count - 1)
                        {
                            startLine++;
                            endLine++;
                        }
                        return;

                    }
                    else if (newState.IsKeyDown(Keys.Up))
                    {
                        if (startLine > 0)
                        {
                            startLine--;
                            endLine--;
                        }
                        return;
                    }
                    else if (newState.IsKeyDown(Keys.Escape))
                    {//close log screen
                        isLogShowing = false;
                        break;
                    }
                }
                
                else if (isQuestDlgShowing)
                {
                    QuestNPC q = curMap.questNPC;
                    if (q.choiceKeys.Contains(pressedKeys[i]))//currentKey
                    {

                        messageString = q.questText[q.choiceKeys.IndexOf(pressedKeys[i])];

                        oldState = newState;
                        isQuestDlgShowing = false;
                        //currentKey = Keys.None;
                        q = null;
                        totalTurns++;
                        continue;

                    }
                    else
                        return;
                    //continue;
                }
                

                if (newState.IsKeyDown(Keys.Escape) && !isInvShowing) //(currentKey == Keys.Escape && !isInvShowing)
                {
                    //player may quit game with Y or continue with N, next turn
                    messageString = "Will you not fight on and into legend, " + player.name + "? \n" +
                        "(Y)ea, I shall. \n" +
                        "(N)ay, I must shuffle off this mortal coil.";
                    isExiting = true;
                    //player.isDead = true; //not really, just faking it to get choice branch. ;)
                    return;

                }
                //writes the last turn's message to the log file before it becomes an empty string
                logWriter = File.AppendText(strLogName);
                
                if (messageString != "")
                    logWriter.WriteLine(totalTurns.ToString() + "| " + messageString);
                logWriter.Close();
                    
                if (!(isDropping || isSelling || isEating || isDrinking || isReading ||
                    (i == pressedKeys.Length-1 && pressedKeys.Length > 1)))//(player.isUnderAttack || player.isAttacking)))
                //erases any message from the previous turn, but not from other screens or during mouse actions
                //resets inventory thumbnail graphic for next showing of inventory screen
                {
                    invItemTex = null;
                    messageString = "";
                    //isSafetyOff = false;
                }
                
                //because a key was pressed, exit any subscreen that may be showing
                //if(!isLooking)
                isInvShowing = false;
                isMapShowing = false;
                isQuestScrnShowing = false;
                if(!isUsingItemOnItem)
                    isSubScrnShowing = false;
                
                isDropping = false;
                isSelling = false;
                isEating = false;
                isDrinking = false;
                isReading = false;
                isLooking = false;
                isEquipping = false;
                isLogShowing = false;
                isUsing = false;
               
                header = "Inventory";
                
                
                Keys currentKey = pressedKeys[i];
                
                //checks if a hotkey was pressed
                if(!player.isMakingFarMove)
                    keyHndlr.Update(gameTime, currentKey, player, ref messageString, ref header);

                
                //check for move key input
                if (!player.isMoving)
                {

                    player.Move(new KeyboardState(pressedKeys), i, tileX, endTileX, tileY, endTileY, ref totalTurns, ref messageString);
                    if (!player.isMoving)
                        break; //DO NOT TAKE THIS LINE OUT, or diagonal moves to new maps will crash game
                    
                    //if(!player.isAttacking)
                    //    player.curTile.PlayStepSound(sndUpdater, player.curTile.stepSnds.Length);


                }
                
                
            } //for loop end bracket

            if (!player.isMoving || player.isMakingFarMove)
            {//update the boundary variables if the player could move

                //UpdateVisibleArea(ref tileX, ref tileY, ref endTileX, ref endTileY);
                UpdateDiscoveredArea();
                
            }

            
            //set the new cursor position for each monster and item

            UpdateDiscoveredArea();

               
            //curMap.savedVisTileArray = visibleTilesArray;
                foreach (QuestNPC q in curMap.questNPCs)
                    q.Update();
           
            //Update items picked up and dropped
                foreach (Item it in Game1.mapItems)

                    it.Update();
            foreach (Item it in pickedUpItems)
                {
                    mapItems.Remove(it);
                }


                //Update the monsters killed
                
                foreach (Monster m in monsterArray)
                {
                    //    mon.Update();
                    //m.tint = Color.White;
                    if (m.isDead)
                    {
                        deadMonsterArray.Add(m);
                        //this is the dead monster "item" that can be picked up and eaten

                        mapItems.Add(new Food(m));
                        //play the bone-clattering sfx because a monster was killed
                        sndUpdater.UpdateSound(SoundUpdater.fxList[(int)SoundUpdater.EffectsIDs.bonebreakFX]);

                    }
                }



                foreach (Monster m2 in deadMonsterArray)
                    monsterArray.Remove(m2);


                
                turnString = "Turn: " + totalTurns;
            // Update saved states.
            oldState = newState;
            oldInput = newInput = newState;
            //if (!(tileArray[(int)player.newPos.X, (int)player.newPos.Y].isExit)) 
            oldTime = gameTime.TotalRealTime.TotalMilliseconds;
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (introScreen)
            {
                //GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                spriteBatch.DrawString(Font1, introText, introStringPos, txtColor);
                spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                if (introStringPos.Y <= -(Font1.MeasureString(introText).Y * .67))
                {
                    spriteBatch.DrawString(Font1, "\'Who\'?", new Vector2(introStringPos.X, vPort.Height / 2), txtColor); ;
                }
                spriteBatch.End();
                
            }
            else if (titleScreen)
            {

                spriteBatch.Begin(); 
                spriteBatch.Draw(txObj.titleTex, Vector2.Zero, Color.White);
                spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White);
                spriteBatch.End();

            }
            else if (newGame)//if this is a new game, get and set the player's name
            {
                //graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                spriteBatch.DrawString(Font1, nameEntryPrompt, nameEntryFontPos, txtColor);
                //player.name = enteredKeys;
                messageRect = new Rectangle(298, 308, 152, lineSkip);
                
                spriteBatch.Draw(txObj.statusBarBG, messageRect, Color.Tan);
                
                    
                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(messageRect.X + Font1.MeasureString(enteredKeys).X),
                        messageRect.Y + (lineSkip - 3), 10, 2), tintColor);
                

                spriteBatch.DrawString(Font1, enteredKeys, new Vector2(300, 303), Color.White);
                spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);

                spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White); 
                spriteBatch.End();
                if (chooseGender)
                {
                    spriteBatch.Begin();
                    
                    spriteBatch.DrawString(Font1, "And thy gender?", new Vector2(300, 
                        340), txtColor);
                    Rectangle mRect = new Rectangle(280, 360, (int)Font1.MeasureString(M).X, (int)Font1.MeasureString(M).Y); 
                    spriteBatch.DrawString(Font1, M, new Vector2(280, 360), txtColor);
                    
                    Rectangle fRect = new Rectangle(330, 360, (int)Font1.MeasureString(F).X, (int)Font1.MeasureString(F).Y);   
                    spriteBatch.DrawString(Font1, F, new Vector2(330, 360), txtColor);
                    spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                    spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White);
                    spriteBatch.End();
                }
                else if (chooseClass)
                {
                    spriteBatch.Begin();    
                    spriteBatch.Draw(Content.Load<Texture2D>("Class Select Screen"), Vector2.Zero, Color.White);
                    spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White);
                    //spriteBatch.Draw(borderBG, Vector2.Zero, Color.White); 
                    spriteBatch.End();
                }
                
            }
            else if (isSubScrnShowing)
            {
                spriteBatch.Begin();
                //spriteBatch.Draw(invBG, Vector2.Zero, Color.White);


                spriteBatch.DrawString(headerFont, header, //attempt to draw the header in the centre
                    new Vector2((vPort.Width * 0.8f) - (headerFont.MeasureString(header).Length() / 2), 45), Color.Bisque);

                /*else*/
                if (isInvShowing)
                {
                    //graphics.GraphicsDevice.Clear(Color.Black);
                    //spriteBatch.Begin();
                    //spriteBatch.Draw(invBG, Vector2.Zero, Color.White);
                    if (isUsingItemOnItem)
                        header = "Use on Which?";

                    //spriteBatch.DrawString(headerFont, header, //attempt to draw the header in the centre
                        //new Vector2((vPort.Width * 0.8f) - (headerFont.MeasureString(header).Length() / 2), 45), Color.Bisque);
                    int line = 50, index = 0;
                    //int lineSkip = 21;
                    foreach (KeyValuePair<string, int> iNQ in itemTxtAndQty)
                    {
                        //Rectangle r = itemRects[index];
                        Rectangle r = invRects[index].area;
                        spriteBatch.Draw(txObj.statusBarBG, r, invRects[index].bgColor);//itemsHeld[index].bgColor);
                        spriteBatch.DrawString(Font1, iNQ.Value.ToString() + " x" + iNQ.Key.PadLeft(30), new Vector2(120, line),
                            invRects[index].txtColor);//itemsHeld[index].txtColor);

                        line += lineSkip;
                        index++;
                    }

                    if (!(invItemTex == null))
                        spriteBatch.Draw(invItemTex, invItemThumb, Color.White);
                    spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                    spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White);
                    //spriteBatch.End();

                }
                else if (isLogShowing)
                {
                    //GraphicsDevice.Clear(Color.Black); 
                    //spriteBatch.Begin();
                    //spriteBatch.DrawString(headerFont, header, new Vector2((vPort.Width * 0.8f) - (headerFont.MeasureString(header).X / 2), 65), Color.Bisque);
                    //StreamReader logReader = File.OpenText(strLogName);
                    int line = 4;
                    for (int i = startLine; i <= endLine; i++)
                    {
                        if (i < logLines.Count)
                            spriteBatch.DrawString(Font1, logLines[i], new Vector2(95, (line + 1) * lineSkip), txtColor);
                        line++;
                    }
                    spriteBatch.DrawString(Font1, "(Up and Down Arrows to scroll. Any other key to close.)",
                        new Vector2((vPort.Width / 4), vPort.Height - 70), Color.Bisque);
                    spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                    //spriteBatch.End();
                }
                else if (isQuestScrnShowing)
                {
                    //spriteBatch.Begin();
                    //spriteBatch.DrawString(headerFont, header, new Vector2((vPort.Width * 0.8f) - (headerFont.MeasureString(header).X / 2), 65), Color.Bisque);

                    int line = 4;
                    foreach (Quest q in lstQuest)
                        spriteBatch.DrawString(Font1, q.name, new Vector2(95, (line - 1) * lineSkip), txtColor);

                    spriteBatch.DrawString(Font1, Quest.selectedQuest.qDesc, new Vector2(220, (line + 1) * lineSkip), txtColor);


                    spriteBatch.DrawString(Font1, "(Up and Down Arrows to scroll. Any other key to close.)",
                        new Vector2((vPort.Width / 4), vPort.Height - 70), Color.Bisque);
                    spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                    //spriteBatch.End();
                }
                else if (isMapShowing)
                {

                    //spriteBatch.Begin();
                    //Header should read "Map" or similar
                    //spriteBatch.DrawString(headerFont, header, new Vector2(vPort.Width / 2 - (headerFont.MeasureString(header).X / 2), 0), Color.Bisque);
                    spriteBatch.Draw(txObj.mapScrBG, Map.mapView, Map.mapScrnSourceRect, Color.White);

                    if (isTinted)
                        spriteBatch.Draw(txObj.mapScrTint, curMap.drawPos, curMap.mapScrRect, Color.White,
                            0,
                            Vector2.Zero, Map.scaleFactor, SpriteEffects.None, 0.1f);

                    spriteBatch.DrawString(Font1, "Left-Click to see discovered areas in that realm. Press any key to close.",
                        new Vector2((vPort.Width / 4), vPort.Height - (lineSkip + Font1.LineSpacing)), Color.Goldenrod);
                    //}
                    base.Update(gameTime);

                    //spriteBatch.End();

                }
                else if (gameOverScreen)
                {
                    header = "The Mighty Fallen"; 
                    int line = 3;
                   
                         int i = 0;
                    while (i < hiScoresUnsorted.Count() && i < 13)
                    {    spriteBatch.DrawString(Font1, hiScoresUnsorted[i], new Vector2(95, (line + 1) * lineSkip), txtColor);
                    i++; line++;
                    }   
                    
                    spriteBatch.DrawString(Font1, "Try again? (Y)es, (N)o",
                        new Vector2((vPort.Width / 2), vPort.Height - 100), Color.Bisque);
                    spriteBatch.Draw(txObj.borderBG, Vector2.Zero, Color.White);
                }
                spriteBatch.End();
            }
            else //Draw main game screen 
            {
                GraphicsDevice.Clear(backColor);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);
                //graphics.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
                //graphics.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
                //graphics.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;


                spriteBatch.Draw(txObj.sideBarBG, sideBarPos, Color.SlateBlue);
                //draw system items


                //player name and turn#
                spriteBatch.DrawString(Font1, turnString, turnStringPos, txtColor);
                spriteBatch.DrawString(Font1, player.name.ToString(), nameStringPos, Color.AntiqueWhite);

                //draw a LIFE BAR behind HP amount
                //first the grey one, for total life capacity

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(hpStringPos.X + Font1.MeasureString("HP:     ").X),
                    (int)hpStringPos.Y + 10, 100, lineSkip), Color.DimGray);
                //second a red one, for current life, as a percentage

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(hpStringPos.X + Font1.MeasureString("HP:     ").X),
                    (int)hpStringPos.Y + 10, (int)hpBar, lineSkip), Color.Green);
                spriteBatch.DrawString(Font1, hpString, hpStringPos, txtColor);

                //buttons
                foreach (Button b in btnList)
                    spriteBatch.Draw(b.tex, b.pos, b.tint);

                //Draw Play Area - starting with visible tiles
                for (int i = 0; i < visibleTilesX; i++)
                {
                    for (int j = 0; j < visibleTilesY; j++)
                    {

                        Tile vTile = visibleTilesArray[i, j];
                        Tile aTile = tileArray[(int)vTile.absPos.X, (int)vTile.absPos.Y];
                        spriteBatch.Draw(vTile.texture, vTile.drawPos, vTile.tileTint);
                        //spriteBatch.Draw(vTile.texture, new Vector2(i * Tile.tWidth, j * Tile.tWidth), vTile.tileTint);
                        if (aTile.overlayTx != null)
                            spriteBatch.Draw(aTile.overlayTx, new Vector2(i * Tile.tWidth, j * Tile.tWidth), null, Color.White,
                                0, Vector2.Zero, 1, aTile.sprFX, 0);
                    }
                }


                //draw creatures and items if they are within viewing range
                //Draw order (back-to-front): items, creatures & NPCs, player

                foreach (Item i in mapItems)
                {

                    if (i.isVisible)
                    {
                        spriteBatch.Draw(i.texture, new Vector2(i.drawPos.X, i.drawPos.Y), null,
                             i.tint, i.rotN, i.origin, 1, i.sprFX, 0);
                    }

                }

                foreach (Monster m in monsterArray)
                {
                    if (m.isVisible)
                    {
                        //draw on main map
                        //spriteBatch.Draw(m.texture, new Rectangle((int)(m.curPos.X), (int)(m.curPos.Y), Tile.tWidth, Tile.tWidth),
                        //    null, m.tint, 0, Vector2.Zero, m.sprE, 0);
                        spriteBatch.Draw(m.texture, new Rectangle((int)(m.drawPos.X), (int)(m.drawPos.Y), Tile.tWidth, Tile.tWidth),
                                null, m.tint, 0, Vector2.Zero, m.sprE, 0);
                    }


                }
                foreach (QuestNPC q in curMap.questNPCs)//(curMap.questNPC != null && curMap.questNPC.isVisible == true)
                {
                    //QuestNPC q = curMap.questNPC;
                    if (q.isVisible)
                        spriteBatch.Draw(q.texture, new Vector2(q.curPos.X, q.curPos.Y), Color.White);
                }


                //draw main character
                if (!player.isDead)
                    animTest.DrawFrame(spriteBatch, new Vector2(player.drawPos.X, player.drawPos.Y), player.tint);
                else
                    spriteBatch.Draw(player.skeleton, new Vector2(player.drawPos.X, player.drawPos.Y), Color.White);



                if (player.isAttacking)
                    AnimationUpdater.slashAtkTexture.DrawFrame(spriteBatch, AnimationUpdater.slashAtkTexture.Origin, Color.White);


                //draw miniMap (if miniMapToggle = true) or larger area view (if miniMapToggle = false)
                if (miniMapToggle)
                {
                    foreach (Tile bT in tileArray)
                    {
                        if (bT.isDiscovered)
                            spriteBatch.Draw(bT.texture, miniMapRects[(int)bT.absPos.X, (int)bT.absPos.Y], bT.tileTint);
                        else
                            spriteBatch.Draw(txObj.msgRectBG, miniMapRects[(int)bT.absPos.X, (int)bT.absPos.Y], bT.tileTint);

                        foreach (Item i in mapItems)
                        {
                            if (i.startingPos == bT.absPos && bT.isDiscovered)
                            {
                                //spriteBatch.Draw(i.texture, new Vector2((t.vPos.X) * 80, (t.vPos.Y) * 80), Color.White);
                                spriteBatch.Draw(i.texture, miniMapRects[(int)i.startingPos.X, (int)i.startingPos.Y], Color.White);
                            }

                        }
                        foreach (Monster m in monsterArray)
                        {

                            if (m.absolutePos == bT.absPos && bT.isDiscovered)
                            {
                                //draw on mini map
                                spriteBatch.Draw(m.texture, miniMapRects[(int)m.absolutePos.X, (int)m.absolutePos.Y], Color.White);
                            }

                        }
                        //draw player on mini-map

                    }
                    spriteBatch.Draw(player.texture, miniMapRects[(int)player.absolutePos.X, (int)player.absolutePos.Y], tintColor);
                }
                else
                {

                    foreach (Tile t in Map.areaDisplayArr[curMap.type])
                    {

                        if (t.isDiscovered)
                        {
                            if (curMap.areaMapTile == t)
                            {
                                t.tileTint = tintColor;
                            }
                            spriteBatch.Draw(t.texture, new Rectangle((int)t.absPos.X, (int)t.absPos.Y, 70, 70), t.tileTint);
                        }
                    }
                }



                //draw an EXPERIENCE BAR behind EXP amount
                //first the grey one, for the total needed for the next level

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(expStringPos.X + Font1.MeasureString("EXP:     ").X),
                    (int)expStringPos.Y + 10, 100, lineSkip), Color.DimGray);
                //second a red one, for current exp, as a percentage

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(expStringPos.X + Font1.MeasureString("EXP:     ").X),
                    (int)expStringPos.Y + 10, (int)expBar, lineSkip), Color.MediumOrchid);
                spriteBatch.DrawString(Font1, expString, expStringPos, txtColor);

                //draw a STRENGTH bar behind STR amount
                //first the grey one, for any strength lost out of total class strength

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(strStringPos.X + Font1.MeasureString("STR:     ").X),
                    (int)strStringPos.Y + 10, 100, lineSkip), Color.DimGray);
                //second a green one, for max strength of this class

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(strStringPos.X + Font1.MeasureString("STR:     ").X),
                    (int)strStringPos.Y + 10, 100 * (int)(player.strength / player.strength), lineSkip), Color.Firebrick);
                spriteBatch.DrawString(Font1, strString, strStringPos, txtColor);

                //draw a HUNGER bar behind Hunger amount
                //first the grey one, for any fullness lost out of total

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(hungerStringPos.X + Font1.MeasureString("Hunger:").X),
                    (int)hungerStringPos.Y + 10, 100, lineSkip), Color.DimGray);
                //second a green one, for max strength of this class

                spriteBatch.Draw(txObj.statusBarBG, new Rectangle((int)(hungerStringPos.X + Font1.MeasureString("Hunger:").X),
                    (int)hungerStringPos.Y + 10, player.fullness, lineSkip), Color.Yellow);
                spriteBatch.DrawString(Font1, hungerString, hungerStringPos, txtColor);

                spriteBatch.DrawString(Font1, lvlString, lvlStringPos, txtColor);
                spriteBatch.DrawString(Font1, goldString, goldStringPos, txtColor);

                spriteBatch.DrawString(Font1, mapString, mapNamePos, txtColor);

                //FOR DEBUGGING - REMOVE FROM FINAL
                spriteBatch.DrawString(Font1, "To stairs: " + stairMap.X.ToString() + ", " + stairMap.Y.ToString(), new Vector2(0, bottomBtnRect.Y - lineSkip), txtColor);
                
                //draw the scrolling parallax overlay

                if (!curMap.hasScrollingOverlay)
                    spriteBatch.Draw(txObj.overlayTex,
                        new Vector2((player.drawPos.X + player.texture.Width / 2) - txObj.overlayTex.Width / 2,
                            (player.drawPos.Y + player.texture.Height / 2) - txObj.overlayTex.Height / 2), Color.Black);
                else
                {
                    spriteBatch.Draw(txObj.overlayTex, curMap.overlay.destRectTopL, curMap.overlay.srcRectTopL, Color.Black);
                    spriteBatch.Draw(txObj.overlayTex, curMap.overlay.destRectTopR, curMap.overlay.srcRectTopR, Color.Black);

                    spriteBatch.Draw(txObj.overlayTex, curMap.overlay.destRectBtmL, curMap.overlay.srcRectBtmL, Color.Black);
                    spriteBatch.Draw(txObj.overlayTex, curMap.overlay.destRectBtmR, curMap.overlay.srcRectBtmR, Color.Black);

                    //spriteBatch.Draw(txObj.overlayTex, new Rectangle(0, 0, overlayX, txObj.overlayTex.Height - overlayY), new Rectangle(txObj.overlayTex.Width - overlayX, overlayY, overlayX, txObj.overlayTex.Height - overlayY), Color.Black);
                    //spriteBatch.Draw(txObj.overlayTex, new Rectangle(overlayX, 0, txObj.overlayTex.Width - overlayX, txObj.overlayTex.Height - overlayY), new Rectangle(0, overlayY, txObj.overlayTex.Width - overlayX, txObj.overlayTex.Height - overlayY), Color.Black);

                    //spriteBatch.Draw(txObj.overlayTex, new Rectangle(0, txObj.overlayTex.Height-overlayY, overlayX, overlayY), new Rectangle(txObj.overlayTex.Width - overlayX, 0, overlayX, overlayY), Color.Black);
                    //spriteBatch.Draw(txObj.overlayTex, new Rectangle(overlayX, txObj.overlayTex.Height-overlayY, txObj.overlayTex.Width - overlayX, overlayY), new Rectangle(0, 0, txObj.overlayTex.Width - overlayX, overlayY), Color.Black);


                }

                messageRect = new Rectangle(10, 10, ((int)Font1.MeasureString(messageString).X), (int)Font1.MeasureString(messageString).Y);//((int)Font1.MeasureString(messageString).Y));
                spriteBatch.Draw(txObj.msgRectBG, messageRect, Color.Gainsboro);





                spriteBatch.DrawString(Font1, messageString, new Vector2(10), Color.White);


                spriteBatch.Draw(mouseCmpt.currentCursor, mouseCmpt.mousePos, Color.White);

                spriteBatch.End();
                // TODO: Add your drawing code here

            }

            base.Draw(gameTime);

        }
    }

}
