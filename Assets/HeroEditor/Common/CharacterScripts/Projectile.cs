using System.Collections;
using System.Collections.Generic;
using BattleStage.Domain;
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

        public bool IsExplosion;
        public Transform ExplosionPrefab;

        public bool IsGrenade;

        public void Start()
        {
            if(!IsGrenade)
                Destroy(gameObject, IsExplosion ? 3f : 2f);
            else
            {
                StartCoroutine(ExplosionByTime(0.3f));
            }
        }

        IEnumerator ExplosionByTime(float existTime)
        {
            yield return new WaitForSeconds(existTime);
            Bang(gameObject);
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

        private void Bang(GameObject other)
        {
            ReplaceImpactSound(other);
            Impact.SetActive(true);
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            Destroy(gameObject, 1);

            foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

            foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
            {
                tr.enabled = false;
            }

            if (IsExplosion)
            {
                var boom = EZ_PoolManager.Spawn(ExplosionPrefab, transform.position, Quaternion.identity);
                boom.GetComponentInChildren<Damage>().DamageValue = _damageOfExplosion;
                boom.localScale *= _rangeOfExplosion;
            }
        }

        private float _damageOfExplosion = 1;
        private float _rangeOfExplosion = 1;
        public void SetDamageOfExplosion(float damage, float range)
        {
            _damageOfExplosion = damage;
            _rangeOfExplosion = (1f + range/100f);
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