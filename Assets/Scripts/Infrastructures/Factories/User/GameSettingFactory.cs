using Domain.User;

namespace Infrastructures.Factories.User
{
    public class GameSettingFactory
    {
        public static GameUserSetting Make(App.Proto.User dto)
        {
            return new GameUserSetting(dto.sfx, dto.bgm, (int) dto.volume);
        }
    }
}