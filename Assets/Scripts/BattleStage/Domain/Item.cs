using System;

namespace BattleStage.Domain
{
    public class Item
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public ItemType Type { get; private set; }
        /// <summary>
        /// HACK : Count can change here 
        /// </summary>
        public int Count { get;  set; }
        public string ResourceID { get; private set; }

        public Item(int id, string name, ItemType type, int count, string resourceId)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (resourceId == null) throw new ArgumentNullException("resourceId");
            ID = id;
            Name = name;
            Type = type;
            Count = count;
            ResourceID = resourceId;
        }
    }

    public enum ItemType
    {
        AMMO308 = 1,
        AMMO10MM = 2,
        HP = 3,
        SPEED = 4,
        ROCKET = 5,
        GRENADE = 6,
        GOLD = 7,
    }
}