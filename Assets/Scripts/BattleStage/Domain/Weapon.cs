using System;

namespace BattleStage.Domain
{
    public class Weapon
    {
        public WeaponID ID { get; private set; }
        
        public int Attack { get; private set; }
        
        public int ShootSpeed { get; private set; }
        
        public int ReloadSpeed { get; private set; }


        public Weapon(WeaponID id, int attack, int shootSpeed, int reloadSpeed)
        {
            if (id == null) throw new ArgumentNullException("id");
            
            ID = id;
            Attack = attack;
            ShootSpeed = shootSpeed;
            ReloadSpeed = reloadSpeed;
        }
    }
}