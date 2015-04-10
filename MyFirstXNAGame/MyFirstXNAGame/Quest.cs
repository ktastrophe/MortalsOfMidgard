using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFirstXNAGame
{
    class Quest
    {
        public string name;
        public QuestNPC[] qNPCs;
        public string qDesc;
        //public int mapX, mapY, mapZ;
        public bool isOnSolidSpace = false;
        Item reqdItem;

        public static Quest selectedQuest;

        public Quest(){}
        public Quest(QuestNPC[] npcs, string desc, string n, Item reqdIt, bool isSolid)
        {
            qNPCs = npcs;
            qDesc = desc;
            isOnSolidSpace = isSolid;
            reqdItem = reqdIt;
            name = n;
 
        }
        public void ObjectiveFulfill()
        {
        }
    }
}
