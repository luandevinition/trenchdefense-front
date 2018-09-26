namespace Domain.User
{
    public class GameUserSetting
    {
        public bool MuteSFX { get; private set; }

        public bool MuteBGM { get; private set; }

        public int VolumeValue { get; private set; }

        public GameUserSetting(bool muteSfx, bool muteBgm, int volumeValue)
        {
            MuteSFX = muteSfx;
            MuteBGM = muteBgm;
            VolumeValue = volumeValue;
        }
    }
}