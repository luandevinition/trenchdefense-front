using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public GameObject CameraGameObject;
        
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);

        void Start()
        {
            if (CameraGameObject == null)
            {
                CameraGameObject = GameObject.Find("BattleCameraFollow");
            }
        }
        
        private void LateUpdate()
        {
            CameraGameObject.transform.position = target.position + offset;
        }
    }
}
