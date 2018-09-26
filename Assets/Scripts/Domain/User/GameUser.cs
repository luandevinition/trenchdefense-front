using System;

namespace Domain.User
{
    public class GameUser
    {
        public string Name;

        public GameUserSetting GameSetting;

        public GameUser(string name, GameUserSetting gameSetting)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (gameSetting == null) throw new ArgumentNullException("gameSetting");
            
            Name = name;
            GameSetting = gameSetting;
        }
    }
}