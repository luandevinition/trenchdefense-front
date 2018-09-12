namespace BattleStage.Domain
{
    public class BaseUnitStatus
    {
        public int HP { get; private set; }
        
        public int Attack { get; private set; }
        
        public int Speed { get; private set; }
        
       
        private int _baseHP;
        private int _baseAttack;
        private int _baseSpeed;

        private int levelHP;
        private int levelAttack;
        private int levelSpeed;
        
        private float percentHPByLevel;
        private float percentAttackByLevel;
        private float percentSpeedByLevel;
        
        
        
    
    }
}