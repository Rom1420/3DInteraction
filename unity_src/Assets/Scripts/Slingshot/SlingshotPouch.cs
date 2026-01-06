using UnityEngine;

public class SlingshotPouch : MonoBehaviour
{
    [Header("References")]
    public Transform slingshotRoot; // SlingshotRoot
    public Transform restAnchor; // PouchAnchor
    public Transform projectileSpawn; // ProjectileSpawn
    public Transform forkLeft; // ForkLeft
    public Transform forkRight; // ForkRight

    [Header("Projectile")]
    public Rigidbody projectilePrefab;
    public float shootForce = 25f;
    public float maxPullDistance = 0.45f; // en mètres
    public float minShootPull = 0.02f;

    [Header("Tuning")]
    public float springBackSpeed = 25f; // vitesse retour poche si relâchée

    // Runtime
    private bool isPulled;
    private Transform pullingHand; // la main qui tire (un Transform)
    private Rigidbody loadedProjectile;

    // Pull vector (local to slingshot)
    public float Pull01 { get; private set; } // 0..1
    public Vector3 PouchWorldPos => transform.position;

    private Vector3 grabOffsetWorld;

    void Start()
    {
        transform.position = restAnchor.position;
        transform.rotation = restAnchor.rotation;
    }

    void Update()
    {
        if (isPulled && pullingHand != null)
        {
            // position de la poche = main, clampée
            Vector3 desired = pullingHand.position + grabOffsetWorld;
            Vector3 fromRest = desired - restAnchor.position;
            float dist = fromRest.magnitude;

            if (dist > maxPullDistance)
                desired = restAnchor.position + fromRest.normalized * maxPullDistance;

            transform.position = desired;

            Pull01 = Mathf.InverseLerp(
                0f,
                maxPullDistance,
                Vector3.Distance(restAnchor.position, transform.position)
            );

            // Charger un projectile quand on commence à tirer
            if (loadedProjectile == null)
                LoadProjectile();
        }
        else
        {
            // retour vers repos
            transform.position = Vector3.Lerp(
                transform.position,
                restAnchor.position,
                Time.deltaTime * springBackSpeed
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                restAnchor.rotation,
                Time.deltaTime * springBackSpeed
            );

            Pull01 = Mathf.InverseLerp(
                0f,
                maxPullDistance,
                Vector3.Distance(restAnchor.position, transform.position)
            );

            // si on a un projectile chargé mais pas assez tiré -> on le “reset”
            if (loadedProjectile != null && Pull01 < 0.02f)
            {
                Destroy(loadedProjectile.gameObject);
                loadedProjectile = null;
            }
        }

        // Garder le projectile collé à la poche tant qu’il est chargé
        if (loadedProjectile != null)
        {
            loadedProjectile.transform.position = projectileSpawn.position;
            loadedProjectile.transform.rotation = projectileSpawn.rotation;
        }
    }

    private void LoadProjectile()
    {
        Debug.Log("[SlingshotDebug] LoadProjectile called");
        loadedProjectile = Instantiate(
            projectilePrefab,
            projectileSpawn.position,
            projectileSpawn.rotation
        );

        loadedProjectile.isKinematic = true;
    }

    public void BeginPull(Transform hand)
    {
        Debug.Log("[SlingshotDebug] BeginPull called");
        pullingHand = hand;
        isPulled = true;
        grabOffsetWorld = transform.position - pullingHand.position;
    }

    public void EndPull()
    {
        Debug.Log("[SlingshotDebug] EndPull called");
        // Tir si assez de tension
        float pullDist = Vector3.Distance(restAnchor.position, transform.position);

        if (loadedProjectile != null && pullDist >= minShootPull)
        {
            Shoot(pullDist);
        }
        else
        {
            // sinon, on annule
            if (loadedProjectile != null)
            {
                Destroy(loadedProjectile.gameObject);
                loadedProjectile = null;
            }
        }

        isPulled = false;
        pullingHand = null;
    }

    private void Shoot(float pullDist)
    {
        // direction = de la poche vers l’ancre repos (donc ça part vers l’avant du lance-pierre)
        Vector3 dir = (restAnchor.position - transform.position).normalized;

        loadedProjectile.isKinematic = false;

        float force = shootForce * Mathf.InverseLerp(
            0f,
            maxPullDistance,
            pullDist
        );

        loadedProjectile.AddForce(dir * force, ForceMode.VelocityChange);
        loadedProjectile = null;
    }
}
