using BattleStage.Domain;

namespace Infrastructures.Factories.Weapon
{
    public class WeaponFactory
    {
        public static BattleStage.Domain.Weapon Make(App.Proto.Weapon dto)
        {
            return new BattleStage.Domain.Weapon(new WeaponID((int) dto.id), dto.name, dto.resourceId, (int) dto.damage,
                450, 150);
            // 450, 150 still dummy please update later.
        }
    }
}