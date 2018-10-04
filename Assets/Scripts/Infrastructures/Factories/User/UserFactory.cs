using System.Collections.Generic;
using System.Linq;
using BattleStage.Domain;
using Domain;
using Domain.User;
using Infrastructures.Factories.Weapon;

namespace Infrastructures.Factories.User
{
    public class UserFactory
    {
        public static BaseUserData Make(App.Proto.AccessCode dto)
        {
            return new BaseUserData(dto.token, dto.tokenID,
                new GameUserID((int) dto.gameUserID));
        }
        
        public static GameUser Make(App.Proto.User dto)
        {
            return new GameUser(dto.name, GameSettingFactory.Make(dto));
        }
        
        public static Unit Make(App.Proto.CharacterStatus dto)
        {
            return Make(dto.character,dto.weapons.Select(WeaponFactory.Make).ToArray());
        }

        public static Unit Make(App.Proto.Character dto, BattleStage.Domain.Weapon[] weapons)
        {
            return new Unit(new UnitID(0), dto.name, (int) dto.attack, (int) dto.hp, (int) dto.speed,
                new ResourceID(int.Parse(dto.resourceID)), weapons.First().ID, null, weapons);
        }
    }
}