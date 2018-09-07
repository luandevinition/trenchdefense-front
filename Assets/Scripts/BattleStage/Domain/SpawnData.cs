using UnityEngine;

namespace BattleStage.Domain
{
    public class SpawnData
    {
        public Vector3 Position
        {
            get { return _position; }
        }

        public int EnemyResourceId
        {
            get { return _enemyResourceID; }
        }
        
        private readonly Vector3 _position;

        private readonly int _enemyResourceID;

        public SpawnData(Vector3 position, int enemyResourceID)
        {
            _position = position;
            _enemyResourceID = enemyResourceID;
        }
    }
}
