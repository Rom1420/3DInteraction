using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletImpact : MonoBehaviour
{
    public float gravityAfterHit = 1f;          // multiplicateur si tu veux (optionnel)
    public float dampOnHit = 0.25f;             // 0 = stop net, 1 = conserve tout
    public float destroyAfterHitSeconds = 3f;

    private Rigidbody _rb;
    private bool _hit;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // En vol: pas de gravité
        _rb.useGravity = false;

        // Anti-tunneling recommandé
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnCollisionEnter(Collision c)
    {
        if (_hit) return;
        _hit = true;

        // Active la gravité après le premier hit
        _rb.useGravity = true;

        // Amortit la vitesse pour éviter rebonds / glissades chelou
        _rb.linearVelocity *= dampOnHit;

        // Option: réduit encore la rotation
        _rb.angularVelocity *= 0.2f;

        Destroy(gameObject, destroyAfterHitSeconds);
    }
}
