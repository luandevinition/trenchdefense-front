using System.Linq;
using BattleStage.Domain;
using Infrastructures.Factories.Item;

namespace Infrastructures.Factories.Zombie
{
    public class ZombieFactory
    {
        public static BattleStage.Domain.Zombie Make(App.Proto.ZombiePosition dto)
        {
            return new BattleStage.Domain.Zombie(new ZombieID(0), dto.zombie.name, (int) dto.zombie.attack,
                (int) dto.zombie.hp,
                (int) dto.zombie.speed, (int) dto.zombie.dropGold, new ResourceID(int.Parse(dto.zombie.resourceID)),
                (int) dto.position, (int) dto.time , dto.zombie.dropItems.Select(ItemFactory.Make).ToList());
        }
    }
}