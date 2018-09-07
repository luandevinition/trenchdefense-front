using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator2D : MonoBehaviour
{
    public Transform Target;
    
    [SerializeField] private NavigationMesh _navigationMesh;

    [SerializeField] private bool _renderMesh;

    void Update()
    {
        if(Target == null) return;
    }

    /*
    void OnDrawGizmos()
    {
        if(!_renderMesh) return;
        
        _navigationMesh.OnDrawGizmos();
    }*/
}
