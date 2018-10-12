using System;

namespace BattleStage.Domain
{
    public class DropItem
    {
        public Item ItemOfZombie { get; private set; }

        public float RateDrop { get; private set; }

        public DropItem(Item itemOfZombie, float rateDrop)
        {
            if (itemOfZombie == null) throw new ArgumentNullException("itemOfZombie");
            ItemOfZombie = itemOfZombie;
            RateDrop = rateDrop;
        }
    }
}