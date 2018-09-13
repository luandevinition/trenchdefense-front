namespace BattleStage.Domain
{
    public class BaseUnitStatus
    {
        public float HP
        {
            get { return _baseHP + (_levelHP * _percentHPByLevel * _baseHP); }
        }

        public float Attack
        {
            get { return _baseAttack + (_levelAttack * _percentAttackByLevel * _baseAttack); }
        }
        
        public float Speed
        {
            get { return _baseSpeed + (_levelSpeed * _percentSpeedByLevel * _baseSpeed); }
        }
       
        /// <summary>
        /// Base status for each character will diff not the same.
        /// </summary>
        private readonly int _baseHP;
        private readonly int _baseAttack;
        private readonly int _baseSpeed;

        /// <summary>
        /// Level (This will using Point we get for 2 wave to increase it)
        /// </summary>
        private int _levelHP;
        private int _levelAttack;
        private int _levelSpeed;
        
        /// <summary>
        /// Percent increase by level
        /// </summary>
        private readonly float _percentHPByLevel;
        private readonly float _percentAttackByLevel;
        private readonly float _percentSpeedByLevel;

        /// <summary>
        /// Constructure
        /// </summary>
        /// <param name="baseHp"></param>
        /// <param name="baseAttack"></param>
        /// <param name="baseSpeed"></param>
        /// <param name="levelHp"></param>
        /// <param name="levelAttack"></param>
        /// <param name="levelSpeed"></param>
        /// <param name="percentHpByLevel"></param>
        /// <param name="percentAttackByLevel"></param>
        /// <param name="percentSpeedByLevel"></param>
        public BaseUnitStatus(int baseHp, int baseAttack, int baseSpeed, int levelHp, int levelAttack, int levelSpeed,
            float percentHpByLevel, float percentAttackByLevel, float percentSpeedByLevel)
        {
            _baseHP = baseHp;
            _baseAttack = baseAttack;
            _baseSpeed = baseSpeed;
            _levelHP = levelHp;
            _levelAttack = levelAttack;
            _levelSpeed = levelSpeed;
            _percentHPByLevel = percentHpByLevel;
            _percentAttackByLevel = percentAttackByLevel;
            _percentSpeedByLevel = percentSpeedByLevel;
        }

        public void IncreaseHPLevel()
        {
            _levelHP++;
        }
        
        public void IncreaseAttackLevel()
        {
            _levelAttack++;
        }
        
        public void IncreaseSpeedLevel()
        {
            _levelSpeed++;
        }
    }
}