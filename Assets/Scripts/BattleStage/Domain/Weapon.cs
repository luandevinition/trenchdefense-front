using System;

namespace BattleStage.Domain
{
    public class Weapon
    {
        public WeaponID ID { get; private set; }
        
        public string Name { get; private set; }
        
        public string Collection { get; private set; }
        
        public int Attack { get; private set; }
        
        public int ShootSpeed { get; private set; }
        
        public int ReloadSpeed { get; private set; }

        public int MagCapacity { get; private set; }

        public int Range { get; private set; }
        
        public bool ThrowAble { get; private set; }
        
        public ItemType Type { get; private set; }

        public Weapon(WeaponID id, ItemType type, string name, string collection, int attack, int shootSpeed, int reloadSpeed,
            int magCapacity, int range, bool throwAble)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (collection == null) throw new ArgumentNullException("collection");
            ID = id;
            Name = name;
            Collection = collection;
            Attack = attack;
            ShootSpeed = shootSpeed;
            ReloadSpeed = reloadSpeed;
            MagCapacity = magCapacity;
            Range = range;
            ThrowAble = throwAble;
            Type = type;
        }
    }
}