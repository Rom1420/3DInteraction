using UnityEngine;

namespace VRWeapons
{
    /// <summary>
    /// Lightweight projectile that moves using physics and destroys itself
    /// after a lifetime or on collision.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 5f;
        [SerializeField] private float despawnDelayOnHit = 0f;

        private Rigidbody _rigidbody;
        private float _deathTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _deathTime = Time.time + lifetimeSeconds;
        }

        private void Update()
        {
            if (Time.time >= _deathTime)
            {
                Destroy(gameObject);
            }
        }

        public void Launch(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }

        private void OnCollisionEnter(Collision _)
        {
            // Let the projectile exist a brief moment to allow particle/audio effects.
            Destroy(gameObject, despawnDelayOnHit);
        }
    }
}
