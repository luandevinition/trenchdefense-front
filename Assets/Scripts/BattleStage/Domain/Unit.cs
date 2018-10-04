using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace BattleStage.Domain
{
    public class Unit
    {
        public UnitID ID { get; private set; }
        
        public string Name { get; private set; }
        
        public int Attack { get; private set; }
        
        public int HP { get; private set; }
        
        public int Speed { get; private set; }
        
        public ResourceID ResourceID { get; private set; }
        
        public WeaponID  BaseWeaponID { get; private set; }
        
        public WeaponID BaseGranedaID { get; private set; }

        public Weapon[] Weapons { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="attack"></param>
        /// <param name="hp"></param>
        /// <param name="speed"></param>
        /// <param name="resourceId"></param>
        /// <param name="baseWeaponId"></param>
        /// <param name="baseGranedaId"></param>
        /// <param name="weapons"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Unit(UnitID id, string name, int attack, int hp, int speed, ResourceID resourceId, WeaponID baseWeaponId,
            WeaponID baseGranedaId, Weapon[] weapons)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (resourceId == null) throw new ArgumentNullException("resourceId");
            if (baseWeaponId == null) throw new ArgumentNullException("baseWeaponId");
            if (weapons == null) throw new ArgumentNullException("weapons");
            
            ID = id;
            Name = name;
            Attack = attack;
            HP = hp;
            Speed = speed;
            ResourceID = resourceId;
            BaseWeaponID = baseWeaponId;
            BaseGranedaID = baseGranedaId;
            Weapons = weapons;
        }
    }
}