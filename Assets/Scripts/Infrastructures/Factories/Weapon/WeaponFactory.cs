using BattleStage.Domain;

namespace Infrastructures.Factories.Weapon
{
    public class WeaponFactory
    {
        public static BattleStage.Domain.Weapon Make(App.Proto.Weapon dto)
        {
            return new BattleStage.Domain.Weapon(new WeaponID((int) dto.id), dto.name, dto.resourceId, (int) dto.damage,
                (int) dto.fireSpeed, 150 , (int) dto.magCapacity , (int) dto.range , dto.throwable);
        }
    }
}