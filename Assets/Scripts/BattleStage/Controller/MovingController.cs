using UnityEngine;

namespace BattleStage.Controller
{
    public class MovingController : MonoBehaviour
    {
        public Vector3 Dicrection = Vector3.zero;

        public float MovementSpeed = 50f;
        
        void Update()
        {
            if (Dicrection == Vector3.zero) return;
            
            transform.position += Dicrection * Time.deltaTime * MovementSpeed;
        }
    }
}