using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// Structure: Overlay
    /// Declares lots of stuff related to the graphical overlays on maps.
    /// </summary>
    public struct Overlay
    {
        //global graphics variables
        public static Texture2D fogTx, snowTx, undergroundTx;
        
        //Geometry objects associated with Scrolling Overlays
        public Rectangle destRectTopL, destRectTopR, destRectBtmL, destRectBtmR,
            srcRectTopL, srcRectTopR, srcRectBtmL, srcRectBtmR;

        public Vector2 velocity; //controls scroll speed
        
        public bool isScrolling; //assumes parallax foreground
    }

    public struct Teleport
    { }

    public class Map : Game1
    {
        //globals for map screen
        public static Rectangle
            mapScrnSourceRect,
            mapView;
        
        public static Rectangle[] mapRectArray = new Rectangle[]{
            new Rectangle(520, 303, 281, 222), new Rectangle(520, 
                    303, 
                    281, 222)};
        
        public static Vector2 scaleFactor;
       

        //FOR DRAWING ON MAP SCREEN
        public Rectangle mapScrRect; //the rectangular area to highlight, representing this area in the main map
        public Vector2 drawPos; //the origin of mapScrRect on the map screen
        //public static int typeClicked = 0; //sets the region to be shown on the map subscreen
        
        
        public int mapPosX, mapPosY, depth; //the position of this map in the game's map array
        public string name;
        public Monster trader;
        public List<QuestNPC> questNPCs;
        public QuestNPC questNPC;
        //public Quest quest;
        public SoundEffect bgm;
        public SoundEffectInstance bgmInst;
        public bool isShopMap,  //true if there is a shop on this map
            isDummyMap, //true if the map hasn't been fully generated, and is just a placeholder
        isDiscovered, hasQuestNPC;
        public Tile[,] savedTileArray;
        public Tile[,] savedVisTileArray;
        public int eastDungeonExit, southDungeonExit;
        public Tile stairTileUp, stairTileDown;
        public Doorway shopDoorway;
        public List<Room> savedRooms;
        public int type; //0 for forest/grassland, 1 for catacombs

        public Overlay overlay;
        public bool hasScrollingOverlay = true; //default is parallax foreground
        
        public Texture2D mapOverlayTx, defaultTileTx; //tiles

        //FOR DRAWING IN MINI-MAP AREA
        public static Tile[][,] areaDisplayArr = new Tile[3][,]{new Tile[3,3], new Tile[3,3], new Tile[3,3]};
        public Rectangle[,] savedMMapRects;
        public Tile areaMapTile;

        public List<Monster> savedMonsterArray, savedDMonsterArray;

        public List<Item> savedItemArray;
        
        /// <summary>
        /// Creates a placeholder (dummy) Map
        /// </summary>
        public Map() 
        {
            isDummyMap = true;
        }

        /// <summary>
        /// Creates a map in the map array.
        /// </summary>
        /// <param name="tileArr">The array of Tiles that make up the map</param>
        /// <param name="rectArr">The array of Rectangles that make up the minimap</param>
        /// <param name="monsArr">The list containing live monsters on this map</param>
        /// <param name="deadMons">The list containing dead monsters on this map</param>
        /// <param name="itemArr">The list containing items on this map</param>
        /// <param name="n">The map name to be displayed in the status area</param>
        /// <param name="t">The map type (determines BGM)</param>
        /// <param name="posX">X position in the game's map array</param>
        /// <param name="posY">Y position in the game's map array</param>
        /// <param name="d">Z position (depth) in the game's map array</param>
        /// <param name="hasShop">True if the map has a shop, false otherwise</param>
        /// <param name="rooms">Rooms on this map (applies to dungeon maps)</param>
        
        public Map(ref Tile[,] tileArr, ref Rectangle[,] rectArr, ref List<Monster> monsArr,
            ref List<Monster> deadMons, ref List<Item> itemArr, string n, int t, int posX, int posY, int d,
            bool hasShop, List<Room> rooms)
        {

            name = n;
            type = t;
            mapPosX = posX;
            mapPosY = posY;
            depth = d;

            questNPCs = new List<QuestNPC>();

            if (t == 0)
            { // Forest/Grasslands Area
                defaultTileTx = Tile.grassTexture;
                overlay.velocity = new Vector2(0.25f, 0.25f);
                mapOverlayTx = Overlay.fogTx;
                overlay.isScrolling = true;
                mapScrRect = mapRectArray[0];
                bgm = SoundUpdater.forestBGM;
            }
            else if (t == 2)
            { // Snow Area
                defaultTileTx = Tile.snowTexture;

                mapOverlayTx = Overlay.snowTx;
                overlay.velocity = new Vector2(2.0f, 2.0f);
                overlay.isScrolling = true;
                //mapScrPos = new Vector2(txObj.mapScrBG.Width * 0.85f, 0);
                mapScrRect = mapRectArray[1];
                bgm = SoundUpdater.forestBGM; //to be changed (obviously)
                posX = mapPosX - (t + 1);

            }
            else if (t == 1)
            { // Catacombs/Dungeon Area
                defaultTileTx = Tile.floorTexture;
                mapOverlayTx = Overlay.undergroundTx;
                hasScrollingOverlay = false;
                overlay.isScrolling = false;
                overlay.velocity = Vector2.Zero;
                mapScrRect = mapRectArray[0];
                bgm = SoundUpdater.dungeonBGM;
            }//if (t == 0)
            
            
            //else
            
            
            areaDisplayArr[t][posX, posY] = new Tile(defaultTileTx, "", "", Game1.miniMapRect.X + 10 + (Tile.tWidth * posX), Game1.miniMapRect.Y + 10 + (Tile.tHeight * posY), false);
            areaMapTile = areaDisplayArr[t][posX, posY];
            
            savedTileArray = new Tile[maxTilesH, maxTilesV];
            savedTileArray = tileArr;
            savedVisTileArray = new Tile[visibleTilesX, visibleTilesY];
            savedVisTileArray = visibleTilesArray;
            savedMMapRects = new Rectangle[maxTilesH, maxTilesV];
            savedMMapRects = rectArr;
            savedMonsterArray = new List<Monster>();
            savedMonsterArray = monsArr;
            savedDMonsterArray = new List<Monster>();
            savedDMonsterArray = deadMons;
            savedItemArray = new List<Item>();
            savedItemArray = itemArr;
            isShopMap = hasShop;
            savedRooms = new List<Room>(rooms);
            questNPC = null;

        }
        public void GetMap()
        {
            Game1.tileArray = new Tile[maxTilesH, maxTilesV];
            Game1.visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
            Game1.miniMapRects = new Rectangle[maxTilesH, maxTilesV];
            Game1.monsterArray = new List<Monster>(maxMstrs);
            Game1.mapItems = new List<Item>();
            Game1.pickedUpItems = new List<Item>();
            Game1.deadMonsterArray = new List<Monster>();
            Game1.tmpRooms = new List<Room>();

            Game1.tileArray = savedTileArray;
            Game1.visibleTilesArray = savedVisTileArray;
            Game1.miniMapRects = savedMMapRects;
            Game1.monsterArray = savedMonsterArray;
            Game1.deadMonsterArray = savedDMonsterArray;
            Game1.mapItems = savedItemArray;
            Game1.tmpRooms = savedRooms;
            Game1.txObj.overlayTex = mapOverlayTx;
            Game1.txObj.scrOverlay = mapOverlayTx;
            
            

        }
        
        /// <summary>
        /// Delimits the visible grid around the player based on their map position
        /// </summary>
        /// <param name="baseX">leftmost X coordinate</param>
        /// <param name="baseY">topmost Y coordinate</param>
        /// <param name="endX"> rightmost X coordinate</param>
        /// <param name="endY">bottommost Y coordinate</param>
        public void UpdateVisibleArea(ref int baseX, ref int baseY, ref int endX,
            ref int endY)
        {
            baseX = ((int)(player.absolutePos.X) - 3);
            if (baseX < 0) baseX = 0;
            else if (baseX > maxTilesH - visibleTilesX) baseX = maxTilesH - 7;

            baseY = ((int)(player.absolutePos.Y) - 3);
            if (baseY < 0) baseY = 0;
            else if (baseY > maxTilesV - visibleTilesY) baseY = maxTilesV - 7;

            endX = ((int)(player.absolutePos.X) + 3);
            if (endX > maxTilesH - 1) endX = maxTilesH - 1;

            endY = ((int)(player.absolutePos.Y) + 3);
            if (endY > maxTilesV - 1) endY = maxTilesV - 1;

            //update visible area only if it is within bounds of the tileArray
            visibleTilesArray = new Tile[visibleTilesX, visibleTilesY];
            for (int indexX = 0; indexX <= visibleTilesX - 1; indexX++)
            {

                for (int indexY = 0; indexY <= visibleTilesY - 1; indexY++)
                {
                    Tile tempTile = new Tile(tileArray[baseX, baseY], indexX, indexY);

                    visibleTilesArray[indexX, indexY] = tempTile;

                    if (tempTile.vPos == player.curPos)//(tempTile.absPos == player.absolutePos)
                    {
                        //set the player cursor's position for drawing
                        //player.curPos = new Vector2(tempTile.vPos.X * tWidth, tempTile.vPos.Y * tWidth);
                        //set the player's tile for use in discovering adjacent tiles
                        player.curTile = tileArray[baseX, baseY];
                    }
                    baseY++;
                }
                baseY = ((int)(player.absolutePos.Y) - 3);
                if (baseY < 0) baseY = 0;
                else if (baseY > maxTilesV - visibleTilesY) baseY = maxTilesV - visibleTilesY;
                baseX++;
            }
        }

        public void UpdateDiscoveredArea()
        {
            //update discovered tiles - 6x6 out of 7x7 area 
            //(unless at a map edge - then light up the corresponding edge)
            //int vX = 2, vY = 2, maxX = 4, maxY = 4;
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
            /*
             _ _____ _
            |_|     |_|
            |         |
            |    p    |   p = player
            |_       _|
            |_|_____|_|  */
            for (int x2 = vX; x2 <= maxX; x2++)
            {

                for (int y2 = vY; y2 <= maxY; y2++)
                {
                NextTile:
                    //make sure corner tiles remain undiscovered
                    //if (!((x2 == vX && y2 == vY) || (x2 == maxX && y2 == vY) ||
                    //    (x2 == vX && y2 == maxY) || (x2 == maxX && y2 == maxY)))
                    //{
                        Tile tmpTile = visibleTilesArray[x2, y2];

                        Tile absTile = tileArray[(int)tmpTile.absPos.X, (int)tmpTile.absPos.Y];



                        //make sure shops/rooms stay darkened until entered into
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

                    //}
                }
            }
        }
        public void UpdateOverlayPos()
        {
            //this check creates seamless wrapping for the overlay
            //overlays are 560 x 560 px

            //condition: player is moving past the max X of the image, so
            //reset X position to 50% to create seamless effect

            if (overlayX + scrOverlayPos.X < -(txObj.overlayTex.Width / 2))
                Game1.scrOverlayPos.X = (txObj.overlayTex.Width / 2) + Tile.tWidth;
            //condition: player is moving past the min Y of the image, so
            //reset Y position to 50% to create seamless effect
            if (overlayY + scrOverlayPos.Y < -(txObj.overlayTex.Height / 2))
                Game1.scrOverlayPos.Y = (txObj.overlayTex.Height / 2) + Tile.tHeight;

            //update pos'n of autoscrolling foregrounds/backgrounds  
            Game1.scrOverlayPos.X += overlay.velocity.X;
            Game1.scrOverlayPos.Y += overlay.velocity.Y;

            Game1.overlayX = (int)Game1.scrOverlayPos.X % txObj.overlayTex.Width;

            Game1.overlayY = (int)Game1.scrOverlayPos.Y % txObj.overlayTex.Height;

        }
    }
}
