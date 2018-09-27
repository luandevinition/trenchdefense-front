using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace BattleStage.Domain
{
    public class Zombie
    {
        public ZombieID ID { get; private set; }
        
        public string Name { get; private set; }
        
        public int Attack { get; private set; }
        
        public int HP { get; private set; }
        
        public int Speed { get; private set; }
        
        public int GoldDropCount { get; private set; }
        
        public ResourceID ResourceID { get; private set; }

        public int Position { get; private set; }

        public int TimeSpawn { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="attack"></param>
        /// <param name="hp"></param>
        /// <param name="speed"></param>
        /// <param name="goldDropCount"></param>
        /// <param name="resourceId"></param>
        /// <param name="position"></param>
        /// <param name="timeSpawn"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Zombie(ZombieID id, string name, int attack, int hp, int speed, int goldDropCount, ResourceID resourceId
            , int position, int timeSpawn)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (resourceId == null) throw new ArgumentNullException("resourceId");
            
            ID = id;
            Name = name;
            Attack = attack;
            HP = hp;
            Speed = speed;
            GoldDropCount = goldDropCount;
            ResourceID = resourceId;
            Position = position;
            TimeSpawn = timeSpawn;
        }
    
    }
}