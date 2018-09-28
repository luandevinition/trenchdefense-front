using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// General behaviour for projectiles: bullets, rockets and other.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public List<Renderer> Renderers;
        public GameObject Trail;
        public GameObject Impact;
	    public Rigidbody Rigidbody;

        IEnumerator DespawnedAfter(float second)
        {
            yield return  new WaitForSeconds(second);
            Bang();
        }
        
             
        void OnSpawned()
        {
            
            StartCoroutine(DespawnedAfter(3));
            Impact.SetActive(false);
            foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }

            foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
            {
                tr.enabled = true;
            }
        }

	    public void Update()
	    {
		    if (Rigidbody != null && Rigidbody.useGravity)
		    {
			    transform.right = Rigidbody.velocity.normalized;
		    }
	    }
        
        public void OnTriggerEnter(Collider other)
        {
            Bang(other.gameObject);
        }

        public void OnCollisionEnter(Collision other)
        {
            Bang(other.gameObject);
        }
        
        private void Bang()
        {
            foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

            foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
            {
                tr.enabled = false;
            }
            
            EZ_PoolManager.Despawn(gameObject.transform);

        }

        private void Bang(GameObject other)
        {
            ReplaceImpactSound(other);
            Impact.SetActive(true);

            foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

	        foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
	        {
		        tr.enabled = false;
			}
            
            EZ_PoolManager.Despawn(gameObject.transform);
		}

        private void ReplaceImpactSound(GameObject other)
        {
            var sound = other.GetComponent<AudioSource>();

            if (sound != null && sound.clip != null)
            {
                Impact.GetComponent<AudioSource>().clip = sound.clip;
            }
        }
    }
}