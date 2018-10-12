using System;
using BattleStage.Domain;

namespace Infrastructures.Factories.Weapon
{
    public class WeaponFactory
    {
        public static BattleStage.Domain.Weapon Make(App.Proto.Weapon dto)
        {
            return new BattleStage.Domain.Weapon(new WeaponID((int) dto.id), (ItemType) Enum.Parse(typeof(ItemType), dto.group.ammoType, true) , dto.name, dto.resourceId, (int) dto.damage,
                (int) dto.fireSpeed, 150 , (int) dto.magCapacity , (int) dto.range , dto.throwable);
        }
    }
}