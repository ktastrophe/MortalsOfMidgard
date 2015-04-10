using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class ItemRect
    {
        public Rectangle area;
        public static int x = 120, height = Game1.lineSkip;
        public Color bgColor, txtColor;
        //public KeyValuePair<string, int> nameAndQty;
        public List<Item> lstItems;
        public Texture2D thumbTex;
        public ItemRect(Texture2D thumb)
        {
            thumbTex = thumb;
        }
    }
    public abstract class Item
    {
        public static Texture2D
            foodTexture, gourdTex, potionTexture, scrollTexture, ringTexture, pickTexture, //items
            leisterTx, longbowTx, arrowTx, arrowStkTx, //weapon textures 
            hideCrsTex; //armor textures

        public string name, ideedname, description;
        public bool isVisible,
            isShopItem, //to be made "false" once item is bought.
            isInMotion = false; //"true" while item is being thrown

        public int weight, price, 
            ideedPrice;//to be taken by constructors as a parameter
        public Color tint = Color.White; //by default, no tint
        public Vector2 startingPos; //{get; set;}
        public Vector2 curPos, drawPos;
        public string effectString;
        public Texture2D texture;
        public SpriteEffects sprFX = SpriteEffects.None; //primarily for fired weapons/throwing items
        public float rotN = 0; //rotation for drawing when thrown in other directions
        public Vector2 origin = Vector2.Zero;
        public Tile targetTile; //target of throwing action

        public Color txtColor = Color.Wheat;
        public Color bgColor = Color.SandyBrown;
        //generic integer for restorative or deleterious effects of consumable items
        public abstract int modifyByAmount { get; set; }
        public System.Reflection.PropertyInfo propToAffect; // player attribute to be altered when item is used
        public System.Reflection.MethodInfo methodToInvoke;
        
        public object[] paramArray; //parameter list for items that invoke methods

        public abstract string Use(Player p);
        
        public void Update()
        {
            if (!isInMotion)
                foreach (Tile t2 in Game1.visibleTilesArray)
                {
                    Tile t = Game1.tileArray[(int)this.startingPos.X, (int)this.startingPos.Y];
                    if (t2.absPos == startingPos && t.isDiscovered)
                    {

                        drawPos = new Vector2(t2.vPos.X * Tile.tWidth, t2.vPos.Y * Tile.tWidth);
                        isVisible = true;
                        break;
                    }
                    else
                        isVisible = false;
                }
        }

        public void Move(Player p, Item i, ref string m)
        {
            //Allow player to shoot self in future, to get rid of parasitic enemies or something
            //if (!(this.absolutePos == t.absPos)) //if not aiming at self
            //{
            float targetX = this.targetTile.vPos.X;
            float targetY = this.targetTile.vPos.Y;
            
            if (!(targetX == this.curPos.X && targetY == this.curPos.Y))//(!(xDif == 0 && yDif == 0))
            {

                float xDif = (this.targetTile.vPos.X * Tile.tWidth) - this.drawPos.X;
                float yDif = (this.targetTile.vPos.Y * Tile.tWidth) - this.drawPos.Y;
                if (xDif < 0)
                {
                    this.drawPos.X -= 20;
                    //this.startingPos.X -= 1;
                    //this.sprFX = SpriteEffects.FlipHorizontally;
                    //this.rotN = 180;
                    
                    
                }
                else if (xDif > 0)
                {
                    
                    //this.rotN = -25;
                    this.drawPos.X += 20;
                    //this.startingPos.X += 1;
                }
           
                

                if (yDif < 0)
                {
                    this.drawPos.Y -= 20;
                    //this.startingPos.Y -= 1;
                    //if (this.rotN > 0)
                    //    this.rotN = (270 + this.rotN) / 2;
                    //else
                    //    this.rotN = 270;
                    //this.sprFX = SpriteEffects.FlipVertically;
                }
                else if(yDif > 0)
                {
                    this.drawPos.Y += 20;
                    //this.startingPos.Y += 1;
                    //if (this.rotN > 0)
                    //    this.rotN = (90 + this.rotN) / 2;
                    //else
                    //    this.rotN = 90;
                }
                               

                //if projectile entered a new tile
                if (this.drawPos.X % Tile.tWidth == 0
                    && this.drawPos.Y % Tile.tWidth == 0)
                { 
                    this.curPos.X = drawPos.X / Tile.tWidth;
                    this.curPos.Y = drawPos.Y / Tile.tHeight;
                    this.startingPos = Game1.visibleTilesArray[(int)this.curPos.X, (int)this.curPos.Y].absPos;
                    foreach (Monster mnstr in Game1.monsterArray)

                    {//check to see if a monster was hit before reaching targetTile
                        if (this.startingPos == mnstr.absolutePos)
                        {
                            this.isInMotion = false;
                            Game1.isThrowing = false;

                            Weapon w = new Weapon();
                            if (i.GetType().IsInstanceOfType(w))
                                m = p.Attack(mnstr, /*(Weapon),i*/ m);

                            this.startingPos = new Vector2(mnstr.absolutePos.X, mnstr.absolutePos.Y);

                            if (!mnstr.isDead)
                            {
                                Random r = new Random();

                                //have there be a chance to recover arrows from the monster's carcass
                                if (r.Next(0, 4) == 0)
                                    mnstr.inventory.Add(this);
                                Game1.mapItems.Remove(this);//have the monsters drop them when they die
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                this.startingPos = this.targetTile.absPos;
                //this.curPos.X = targetX; this.curPos.Y = targetY;
                this.isInMotion = false;
                Game1.isThrowing = false;
                //Game1.mapItems.Add(this);
            }
            
        }
        
    }
    public class Gold : Item //constructor for each size of gold pile
    {
        //public int modifyByAmount { get; set; }
        public int amount;
        public int minAmount, maxAmount;

        public override int modifyByAmount { get; set; }

        public Gold(Texture2D tex, int mnAmt, int mxAmt) //maximum count for this particular size of gold pile
        {
            texture = tex;
            minAmount = mnAmt;
            maxAmount = mxAmt;
            
               
        }
        public Gold(Gold g, int amt, Vector2 p) //copy constructor for individual gold items on map
        {
            texture = g.texture;
            amount = amt;

            name = amt + " bullion";
            weight = (int)Math.Round((double)amt / 3.0);
            startingPos = p;
            
        }

        public Gold(int amt) //basic constructor for gold in inventory
        {
            name = amt + " bullion";
            amount = amt;
            description = "The currency of your people: crude coins and trinkets smithed from precious metals.";
        }
        public override string Use(Player p)
        {
            return this.effectString;
        }
        //public override string Use(Creature c)
        //{
        //    return this.effectString;
        //}

    }
    public class Usables : Item
    {
        public override int modifyByAmount { get; set; } //generic integer for restorative or deleterious effects of consumable items
        
        public bool alteredState; //for potions that alter player state, such as isBlind, isPoisoned, etc.
        protected static Color[] itemTintColors = {Color.Azure, Color.Bisque, Color.SeaShell,
                                            Color.DarkViolet, Color.Olive, Color.PapayaWhip,
                                            Color.Khaki, Color.BurlyWood, Color.Plum, Color.Peru,
                                            Color.DarkGreen, Color.OrangeRed,
                                            Color.White, Color.Black};
        
        protected Random r;
        
        //copy constructor
        public Usables(Usables u, Vector2 p)
        {
            //u.MemberwiseClone();
            texture = u.texture;
            name = u.name;
            ideedname = u.ideedname;
            weight = u.weight;
            price = u.price;
            isShopItem = u.isShopItem;
            modifyByAmount = u.modifyByAmount;
            propToAffect = u.propToAffect;
            methodToInvoke = u.methodToInvoke;
            alteredState = u.alteredState;
            effectString = u.effectString;
            startingPos = p;
            tint = u.tint;
            //paramArray = u.paramArray;

        }
        public Usables() { }

        //baSE constructor
        public Usables(Texture2D tex, string n, int w, string e, Vector2 p)
        {
            texture = tex;
            name = n;
            effectString = e;
            startingPos = p;
            weight = w;
        }
        
        public override string Use(Player p)   
        {
            return this.effectString;
        }
        //public override string Use(Creature c)
        //{
        //    return this.effectString;
        //}
        
        
    }
    public class Food : Usables
    {
        
        static Type pT = typeof(Player);
        static Dictionary<KeyValuePair<string, PropertyInfo>, int> gourdAndProps = new Dictionary<KeyValuePair<string, PropertyInfo>, int>()
        {
        { new KeyValuePair<string, PropertyInfo>("spirits. You feel the effects immediately. ", pT.GetProperty("isIntoxicated")), 0}, //...duh
        { new KeyValuePair<string, PropertyInfo>("powder. You feel a sudden and complete satiety. ", pT.GetProperty("fullness")), 100}, //a powder that makes you feel full, for a while, before your Hunger drops to original level
        { new KeyValuePair<string, PropertyInfo>("cream. It's rich and filling.", pT.GetProperty("fullness")), 20}, //a gourd containing rich cream, which actually does fill you up
        { new KeyValuePair<string, PropertyInfo>("spoiled cream. You wretch uncontrollably.", pT.GetProperty("fullness")), -10} //spoiled cream, same effect as wretchweed
        };
        
        public Food() { }
        public Food(Monster m) //for dead monsters
        {
            this.texture = m.skeleton;
            this.name = "dead " + m.name;
            this.effectString = m.effectString;
            //if(this.effectString.Contains("***"))
                //this.effectString = this.effectString.Replace("***", m.name);
            this.startingPos = m.absolutePos;
            this.weight = m.weight;
            this.modifyByAmount = weight / 2;
            this.propToAffect = m.propToAffect;
            
        }
        
        public Food(Usables u, Vector2 p):base(u,p)
        {
            //fullnessRestored = modifyByAmount;
            
        }
        public Food(Texture2D tex, string n, int w, string e, System.Reflection.PropertyInfo prop) :
            base(tex, n, w, e, Vector2.Zero)
        {
            
            price = 5;
            modifyByAmount = 10; //to be taken away from the player's hunger
            effectString = e;
            if (prop == null) //this is a gourd, so assign it random contents/effect
            {
                r = new Random();
                int i = r.Next(gourdAndProps.Count);

                KeyValuePair<KeyValuePair<string, PropertyInfo>, int> kvp =
                    new KeyValuePair<KeyValuePair<string, PropertyInfo>, int>(gourdAndProps.Keys.ElementAt(i), gourdAndProps.Values.ElementAt(i));

                prop = kvp.Key.Value;
                modifyByAmount = kvp.Value;
                effectString += kvp.Key.Key;
            }
            propToAffect = prop;
            
                
        //    //startingPos = new Vector2(x, y);
        }
        public override string Use(Player p)
        {
            //p.fullness += this.fullnessRestored; 
            bool b = false;
            int i = 0;
            Type t = this.propToAffect.PropertyType;
            if (t.IsInstanceOfType(b))
                propToAffect.SetValue(p, true, null);
            else if (t.IsInstanceOfType(i))
                propToAffect.SetValue(p, ((int)propToAffect.GetValue(p, null)+ modifyByAmount), null);
            return this.effectString;
        }
        //public override string Use(Creature c)
        //{
        //    return this.effectString;
        //}
        
    }
    public class Potion : Usables
    {
        bool maxesOut;
        PropertyInfo maxPropValue;
        public Potion() { }
        public Potion(Texture2D tex, string id, string n, string e, PropertyInfo p, PropertyInfo maxPropVal, int modAmt, bool max)
            : base(tex, n, 5, e, Vector2.Zero)
        
        {
            r = new Random();
            //tint = new Color(itemTintColors[r.Next(itemTintColors.Length)], 255);
            
            price = 50;
            ideedname = "potion " + id; 
            if (!(n.Contains("of ")))
                name += " potion";
            else
                name = ideedname;
            propToAffect = p;
            modifyByAmount = modAmt; //for pot. of curing
            description = "An unmarked phial of mysterious origin.";
            maxesOut = max;
            maxPropValue = maxPropVal;
        }
        public Potion(Usables u, Vector2 p)
            : base(u, p)
        {
            
        }
        public override string Use(Player p)
        {
            Type b = typeof(bool);

            if (!this.propToAffect.PropertyType.Equals(b))
            {//casting supports integers and floating-point numbers
                
                if (maxesOut)
                {
                    int difference = System.Convert.ToInt32(maxPropValue.GetValue(p, null)) - System.Convert.ToInt32(propToAffect.GetValue(p, null));
                    modifyByAmount = difference;
                }
                propToAffect.SetValue(p, (System.Convert.ToInt32(propToAffect.GetValue(p, null)) + modifyByAmount), null);
            } return "You drink the " + this.name + ". " + this.effectString;
        }

    }
    public class Scroll : Usables
    {
        
        public Scroll(){ }
        public Scroll(Texture2D tex, string id, string n, string e)
            :base(tex, n, 1, e, Vector2.Zero)
        {
            r = new Random();
            //tint = new Color(itemTintColors[r.Next(itemTintColors.Length)], 255); 
            ideedname = "scroll " + id;
            if (!(n.Contains("of ")))
                name += " scroll";
            else
                name = ideedname;
            //paramArray = p;
           
            
            price = 100;
            //startingPos = new Vector2(x, y);
            description = "Its contents escape the reader until they are read in full.";
        }
        public Scroll(Usables u, Vector2 p)
            : base(u, p)
        {
            
        }

    }
    public class Wand : Usables
    {
    }

    public class Tool : Item
    { //includes pickaxes, grappling hooks, etc
        public override string Use(Player p)
        {
            object[] paramArr = new object[] { p };
            return (string)methodToInvoke.Invoke(this, paramArr);
            
        }
        public override int modifyByAmount
        {
            get;
            set;
        }
        public Tool(Texture2D tex, string n, int w, string effStr, Vector2 sPos, string mName)
        {
            texture = tex;
            name = n;
            weight = w;
            effectString = effStr;
            startingPos = sPos;
            Type toolType = typeof(Tool);
            this.methodToInvoke = toolType.GetMethod(mName);
        }
        
        public string Dig(Player p) //method for the pickaxe
        {
            string m = "";
            //will change tile from solid to not, if solid
            if (this.targetTile.isSolid)
            {
                this.targetTile.isSolid = false;
                switch (this.targetTile.type)
                {
                    case "grass":
                        this.targetTile.texture = Tile.grassTexture;
                        //message for pickaxe - assign different msg for real axe
                        m = "You clumsily hack and stab at the tree. (make turns pass here) Finally, it is felled.";
                        break;
                    case "stone":
                        this.targetTile.texture = Tile.floorTexture;
                        break;
                }
            }
            //In order to produce the effect of turns passing while performing long actions, should
            //feed the Player.Move function a bunch of Key.5's (stationary action) in a loop.. but how to
            //interrupt?
            return m;
        }
        public string Grapple(Tile t) //method for the grappling hook
        {
            return "";
        }
        
    }
    public class Equipment : Item
    {
        public int modifier;
        public int atkRating, defRating;
        public bool isEquipped, isHexed;
        public override int modifyByAmount { get; set; }
        
        public Equipment(Equipment e, Vector2 p) 
        {
            texture = e.texture;
            name = e.name;
            ideedname = e.ideedname;
            weight = e.weight;
            price = e.price;
            isShopItem = e.isShopItem;
            modifyByAmount = e.modifyByAmount;
            propToAffect = e.propToAffect;
            methodToInvoke = e.methodToInvoke;
            //alteredState = e.alteredState;
            effectString = e.effectString;
            startingPos = p;

        }
        public Equipment() { }
        public Equipment(Texture2D tex, string n, int w, string e, Vector2 p)
        {
            texture = tex;
            name = n;
            effectString = e;
            startingPos = p;
            weight = w;
        }
        public void Equip() { return; }
        public override string Use(Player p)
        {
            return this.effectString;
        }
        //public override string Use(Creature c)
        //{
        //    return this.effectString;
        //}
    }
    
    public class Ring : Equipment
    {
        public Ring(){ }
        
        public Ring(Texture2D tex, string id, string n, string e, int m, bool h)
            : base(tex, n, 5, e, Vector2.Zero)
        
        {
            price = 50;
            ideedname = "ring " + id; 
            if (!(n.Contains("of ")))
                name += " ring";
            else
                name = ideedname;
            modifier = m;
            isHexed = h;
            weight = 1;
            price = 500;
        }
        public Ring(Equipment e, Vector2 p)
            : base(e, p)
        {
            
        }
    }
    public class Weapon : Equipment
    {
        public int toHit;
        public int range = 1;
        //public bool hasRangedAttack; 
        public Weapon()
        {
        }
        
        /// <summary>
        /// Creates object of class Weapon
        /// </summary>
        /// <param name="tex">The weapon's sprite.</param>
        /// <param name="n">Weapon name.</param>
        /// <param name="m">Initial attack modifier (0 to +2).</param>
        /// <param name="atk">Weapon's base attack rating.</param>
        /// <param name="def">Weapon's defense rating.</param>
        /// <param name="h">Hex status.</param>
        /// <param name="w">Weapon weight.</param>
        /// <param name="tH">To Hit- Added to Player's accuracy.</param>
        
        public Weapon(Texture2D tex, string n, int m, int atk, int def, bool h, 
            int w, int tH)
            : base(tex, n, 5, n, Vector2.Zero)
        {
            
            modifier = m;
            isHexed = h;
            atkRating = atk;
            defRating = def;
            
            toHit = tH;
            targetTile = null;
        }
        public Weapon(Texture2D tex, string n, int atk, //constructor for fired projectile weapons
            int r, int w, int tH) //r is the number of tiles weapon can travel to attack
        {
            texture = tex;
            name = n;
            atkRating = atk;
            weight = w;
            toHit = tH;
            range = r;
        }
        public Weapon(Weapon w, Vector2 aPos, Vector2 cPos, ref Tile tgtT) 
        {//copy constructor (for projectile weapons)
            texture = w.texture;
            name = w.name;
            price = w.price;
            
            modifier = w.modifier;
            isHexed = w.isHexed;
            atkRating = w.atkRating;
            defRating = w.defRating;
            weight = w.weight;
            toHit = w.toHit;

            //get item ready for throwing
            targetTile = new Tile(tgtT);
            startingPos = new Vector2(aPos.X, aPos.Y);
            curPos = new Vector2(cPos.X, cPos.Y);
           
            //isInMotion = true;
            //isVisible = true;
            
            
        }
        
        
    }
    public class RangedWep : Weapon //weapon strength altered upon Throw/Shoot
    {
        public Weapon firedWep; //holds the projectile associated with this weapon

        public RangedWep(Texture2D tex, string n, int m, int atk, int def, bool h,
            int w, int tH, Weapon fW) :
            base(tex, n, m, atk, def, h, w, tH)
        {
            firedWep = fW;
        }
        public RangedWep(Texture2D tex, string n, int m, int atk, int def, bool h,
            int w, int tH) :
            base(tex, n, m, atk, def, h, w, tH)
        { }

    }
    public class Armor : Equipment
    {
        protected int protectArea = 0; //0 = torso, 1 = head, 2 = neck, 3 = gauntlets, 4 = gloves, 
        //5 = chausses, 6 = greaves, 7 = boots

        public Armor(Texture2D tx, string n, int m, int
            def, bool h, int w)
        {
            texture = tx;
            name = n;
            modifier = m;
            defRating = def;
            isHexed = h;
            weight = w;
        }
    }
    public class Cloak : Equipment //Armor
    {
    }
    public class Helmet : Equipment //Armor
    { }
    public class Greaves : Equipment //Armor
    { }
    public class Chausses : Equipment //Armor 
    { }
    public class Boots : Equipment //Armor
    { }
    public class Gorget : Equipment //Armor
    { }
    public class Gauntlets : Equipment //Armor
    { }
    public class Amulet : Equipment
    { }
    

}