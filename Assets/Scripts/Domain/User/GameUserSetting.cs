namespace Domain.User
{
    public class GameUserSetting
    {
        public bool EnableSFX { get; private set; }

        public bool EnableBGM { get; private set; }

        public int VolumeValue { get; private set; }

        public GameUserSetting(bool enableSFX, bool enableBgm, int volumeValue)
        {
            EnableSFX = enableSFX;
            EnableBGM = enableBgm;
            VolumeValue = volumeValue;
        }
    }
}