using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;

namespace MyFirstXNAGame
{
    public class GameState
    {
        Player player;
        Map[,,] maps;

        public static string Save(Player plr, Map[,,] mapsArr, int turn)
        {
            GameState sG = new GameState();
            sG.player = plr;
            sG.maps = mapsArr;

            
            string filename = Path.Combine(Game1.saveFileDir, 
                plr.name + ", Lv" + plr.level + ", Turn " + turn + ", "+ DateTime.Now.ToFileTime() + ".sav");

            FileStream stream = File.Open(filename, FileMode.Create);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                serializer.Serialize(stream, sG);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
            // Dispose the container, to commit the data.
            //container.Dispose();
            return "Game saved. ";
        }
        public static GameState Load(string filename)
        {
            GameState saveFile;

            // Get the path of the save game
            string fullpath = Path.Combine(Game1.saveFileDir, filename);

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.Open,
            FileAccess.Read);
            try
            {

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                saveFile = (GameState)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (saveFile);
        }
    }
}
