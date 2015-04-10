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
    public class Tile
    {
        //Global Tile Textures/size variables
        public static Texture2D grassTexture, floorTexture, wallTexture,
            snowTexture, snowTexture2, snowTexture3;
        public static int tWidth = 80;
        public static int tHeight = 80;
        public static Vector2 moveDir = Vector2.Zero;
        public static bool isMoving = false;

        public Vector2 absPos; //this is the tile's absolute position in the full map (20x20)
        public Vector2 drawPos;

        public Texture2D texture, overlayTx;
        public SpriteEffects sprFX;

        public bool isSolid, isPlayerTile, isDiscovered, 
            isBldngTile, //human-occupied building = monsters don't generate there 
            isExit  = false;
        public string name;
        public string type; //for sound fx. only two needed so far: grass and stone
        public Vector2 vPos;// this is for derived tiles     

        public Color tileTint; //for as-yet undiscovered tiles
        Rectangle tileRect; //for the Look function, the area-view minimap, and far moves 

        public SoundEffect[] stepSnds; //array of possible sound effects when stepping on this tile

        public enum SoundFX { };
     
        Random r; 

        
        public Tile(Tile t) //for setting a thrown object's targetTile
        {
            this.vPos = new Vector2(t.vPos.X, t.vPos.Y);
            this.absPos = new Vector2(t.absPos.X, t.absPos.Y);
            this.isSolid = t.isSolid;
            this.isDiscovered = t.isDiscovered;
            this.tileRect = t.tileRect;
        }
        
        public Tile(Texture2D t, string tType, string tName, float x, float y, bool s) //absolute position tiles
        {
            
            int grassStep1 = (int)SoundUpdater.EffectsIDs.grassStepFX1,
                grassStep2 = (int)SoundUpdater.EffectsIDs.grassStepFX2,
                grassStep3 = (int)SoundUpdater.EffectsIDs.grassStepFX3,
                stoneStep1 = (int)SoundUpdater.EffectsIDs.stoneStepFX1,
                stoneStep2 = (int)SoundUpdater.EffectsIDs.stoneStepFX2;

            
            SoundEffect[] fxArr = SoundUpdater.fxList; 
            this.absPos = new Vector2(x, y);
            //if (plrPos == this.absPos) //if the absolute positions of player and tile are the same
              //  this.isPlayerTile = true; //this is the tile the player is on
            this.texture = t;
            this.name = tName;
            this.type = tType;

            //set tile sound effects
            if (tType == "grass" || tType == "snow")
                stepSnds = new SoundEffect[] { fxArr[grassStep1], fxArr[grassStep2], fxArr[grassStep3] };
            else if (tType == "stone")
                stepSnds = new SoundEffect[] { fxArr[stoneStep1], fxArr[stoneStep2] };
            else
                stepSnds = new SoundEffect[] { fxArr[grassStep2] };
            this.tileTint = Color.DarkSlateGray;
            
            this.isSolid = s;
            if (tName.Contains("exit"))// == "exit" || tName == "exit down" || tName == "exit up")
                this.isExit = true;
           
        }
        public Tile(Tile t, int relX, int relY) //derivative constructor for tiles in the visible grid
        {
            this.absPos = t.absPos; //absPos becomes tile's absolute position
            vPos = Vector2.Zero;
            tileTint = Color.White;
            this.name = t.name;
            this.texture = t.texture;
            this.isExit = t.isExit;
            this.isSolid = t.isSolid;
            
            this.tileTint = t.tileTint;
            int absX = (int)t.absPos.X, absY = (int)t.absPos.Y;
            int rightX = Game1.visibleTilesX - 2, bottomY = Game1.visibleTilesY - 2;
            //this.isDiscovered = t.isDiscovered;
           
            //this.isPlayerTile = t.isPlayerTile;
            this.vPos = new Vector2(relX, relY); //vPos becomes position in viewable area
            
          //determining tile overlay to use, depending on discovery of adjacent tiles

            t.overlayTx = null;

            if (t.isDiscovered)
            {
                
                if (relX == rightX && !Game1.tileArray[absX + 1, absY].isDiscovered) //on right-hand edge
                {
                    //if (Game1.tileArray[absX - 1, absY].isDiscovered)
                    //{
                    if (/*relX == rightX */relY == bottomY) //bottom right corner
                    {
                        t.overlayTx = Game1.txObj.tileOverlayLwrR;


                    }
                    else if (/*relX == rightX && */relY == 1)//top right corner
                    {
                        t.overlayTx = Game1.txObj.tileOverlayLwrR;
                        t.sprFX = SpriteEffects.FlipVertically;
                    }

                //}
                    else //flat right edge
                    {
                        t.overlayTx = Game1.txObj.tileOverlayRight;
                        t.sprFX = SpriteEffects.None;
                    }
                    t.tileTint = Color.White;


                }
                else if (relX == 1 && !Game1.tileArray[absX - 1, absY].isDiscovered) //on left-hand edge
                {
                    //if (Game1.tileArray[absX + 1, absY].isDiscovered)
                    //{
                    if (/*(Game1.tileArray[absX, absY - 1].isDiscovered && !Game1.tileArray[absX-1, absY].isDiscovered) || */relY == bottomY) //bottom left corner
                    {
                        t.overlayTx = Game1.txObj.tileOverlayLwrL;

                    }
                    else if (/*(Game1.tileArray[absX, absY + 1].isDiscovered && !Game1.tileArray[absX-1, absY].isDiscovered) || */relY == 1) //top left corner
                    {
                        t.overlayTx = Game1.txObj.tileOverlayLwrL;
                        t.sprFX = SpriteEffects.FlipVertically;
                    }
                    else //flat left edge
                    {
                        t.overlayTx = Game1.txObj.tileOverlayRight;
                        t.sprFX = SpriteEffects.FlipHorizontally;
                    }
                    t.tileTint = Color.White;
                }
                
                else if (relX > 1 && relX < rightX)
                //== (Game1.visibleTilesX - 1) / 2 ||
                //(relX == 2) || relX == Game1.visibleTilesX - 3) //middle flat edges
                {
                    
                    if (relY == bottomY && !(Game1.tileArray[absX, absY + 1].isDiscovered))
                    {//bottom

                        t.overlayTx = Game1.txObj.tileOverlayBtm;
                        //t.tileTint = Color.White;
                    }
                    else if (relY == 1 && !(Game1.tileArray[absX, absY - 1].isDiscovered))
                    {//top

                        t.overlayTx = Game1.txObj.tileOverlayBtm;
                        t.sprFX = SpriteEffects.FlipVertically;
                        //t.tileTint = Color.White;
                    }
                    t.tileTint = Color.White;
                }
                

                //    if(relX == Game1.visibleTilesX - 2 && !(Game1.tileArray[absX+1, absY].isDiscovered))//right-hand column
                //{
                //    if ((relY == (Game1.visibleTilesY - 1) / 2) //flat right edge
                //    || (relY == Game1.visibleTilesY - 3)//lower right corner
                //    || (relY == 2)) //upper right corner
                //    {
                //        t.overlayTx = Game1.txObj.tileOverlayRight;
                //        t.sprFX = SpriteEffects.None;
                //        //t.tileTint = Color.White;

                //    }

                //}

                //else if (relX == 1 && !(Game1.tileArray[absX - 1, absY].isDiscovered)) //left-hand column

                //{
                //    if ((relY == (Game1.visibleTilesY - 1) / 2) //flat left edge
                //     || (relY == Game1.visibleTilesY - 3) //lower left corner
                //     || (relY == 2))
                //    {
                //        t.overlayTx = Game1.txObj.tileOverlayRight;
                //        t.sprFX = SpriteEffects.FlipHorizontally;
                //        //t.tileTint = Color.White;

                //    }

                //}
                //else if (relX == (Game1.visibleTilesX -1 ) / 2 ||
                //    (relX == 2) || relX == Game1.visibleTilesX - 3 ) //middle flat edges
                //{
                //    if (relY == Game1.visibleTilesY - 2 && !(Game1.tileArray[absX, absY + 1].isDiscovered))
                //    {//bottom
                //        t.overlayTx = Game1.txObj.tileOverlayBtm;
                //        //t.tileTint = Color.White;
                //    }
                //    else if (relY == 1 && !(Game1.tileArray[absX, absY - 1].isDiscovered))
                //    {//top

                //        t.overlayTx = Game1.txObj.tileOverlayBtm;
                //        t.sprFX = SpriteEffects.FlipVertically;
                //        //t.tileTint = Color.White;
                //    }


                //}
                //if (this.overlayTx != null)
                //    return;
                //if (relY == Game1.visibleTilesY - 2 && !(Game1.tileArray[absX, absY].isDiscovered))
                ////bottom corners   
                //{
                //    if (relX == 1)
                //    {//left corner
                //        if (!Game1.tileArray[absX + 1, absY].isDiscovered) 
                //            t.overlayTx = Game1.txObj.tileOverlayLwrL;
                //        t.tileTint = Color.White;
                //    }
                //    else if (relX == Game1.visibleTilesX - 2) //right corner
                //    {
                //        if (!Game1.tileArray[absX - 1, absY].isDiscovered) 
                //            t.overlayTx = Game1.txObj.tileOverlayLwrR;
                //        t.tileTint = Color.White;
                //    }
                //}
                //else if (relY == 1 && !(Game1.tileArray[absX, absY].isDiscovered))
                ////top corners
                //{
                //    if (relX == 1)
                //    {//left corner

                //            t.overlayTx = Game1.txObj.tileOverlayLwrL;

                //        t.sprFX = SpriteEffects.FlipVertically;
                //        t.tileTint = Color.White;
                //    }
                //    else if (relX == Game1.visibleTilesX - 2) //right corner
                //    {
                //        t.overlayTx = Game1.txObj.tileOverlayLwrR;
                //        t.sprFX = SpriteEffects.FlipVertically;
                //        t.tileTint = Color.White;
                //    }
                //}
            }
            //else //if ((relX > 1 && relX < rightX) && (relY > 1 && relY < bottomY))
            //{
            //    t.overlayTx = null;
            //    //t.isDiscovered = true;
            //    //t.tileTint = Color.White;
            //}
                 
            
                       
        }
        public bool Contains(float x, float y, int tSize)
        {
            this.tileRect = new Rectangle((int)this.vPos.X * tSize, (int)this.vPos.Y * tSize,
                tSize, tSize);
            if (this.tileRect.Contains((int)x, (int)y))
                return true;
            else
                return false;
        }

        public static void Update()
        {

            for (int indexX = 0; indexX < Game1.visibleTilesX; indexX++)
            {
                for (int indexY = 0; indexY < Game1.visibleTilesY; indexY++)
                {
                    Game1.visibleTilesArray[indexX, indexY].drawPos.X -= moveDir.X;
                    Game1.visibleTilesArray[indexX, indexY].drawPos.Y -= moveDir.Y;
                }
            }    
            if ((Game1.visibleTilesArray[Game1.visibleTilesX - 1, Game1.visibleTilesY - 1].drawPos.X % Tile.tWidth == 0 &&
                    Game1.visibleTilesArray[Game1.visibleTilesX - 1, Game1.visibleTilesY - 1].drawPos.Y % Tile.tHeight == 0))
                {
                    moveDir = Vector2.Zero;
                    isMoving = false;
                }

            
            
            
        }
        public void PlayStepSound(SoundUpdater sU, int soundChoices)
        {
            r = new Random();
            int choice = r.Next(soundChoices);
            sU.UpdateSound(this.stepSnds[choice]);
                    
            return;
        
        }
    }
    //Structure Doorway
    //holds information on doorways in dungeons so they can be connected
    public struct Doorway
    {
        public Vector2 location; //position in the tileArray (30x30)
        public int roomNo, doorNo;
        public bool isConnected;
        public bool isOpen; //changes door to wall when false (unsolid to solid)
        public Doorway(Vector2 l)
        {
            location = l;
            isOpen = true;
            roomNo = 0; doorNo = 0; isConnected = false;

        }
        public Doorway(Vector2 l, int r, int d)
        {
            location = l;
            roomNo = r;
            doorNo = d;
            isConnected = false;
            isOpen = true;
            //connectedRoom = 10;
        }
        public void makeConnection(bool c)
        {
            isConnected = c;
            //connectedRoom = cR;
        }
    }

    public class Room
    {
        public List<int> connectedRooms;
        public bool isDiscovered; //determines whether to reveal roomTiles
        public List<Tile> roomTiles; //tiles contained within room
        public List<string> connectionNames; //holds the names (types) of the connected doorway tiles
        public Room(List<Tile> rT)
        {
            connectedRooms = new List<int>{10, 10};
            connectionNames = new List<string>();
            roomTiles = rT;
        }
        //this function tells the room what other rooms it's connected to,
        //so that both doors won't connect to the same room
        public void addConnections(Doorway d, string s)
        {
            connectedRooms[d.doorNo] = d.roomNo;
            connectionNames.Add(s);
            
        }
        
    }
}
