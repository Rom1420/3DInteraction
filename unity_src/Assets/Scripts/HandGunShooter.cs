using UnityEngine;

// Tir à l'AK déclenché par la zone de gâchette (bounding box)
public class HandGunShooter : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Point de sortie du tir (bout du canon).")]
    public Transform muzzle;

    [Tooltip("Prefab du projectile (doit avoir un Rigidbody).")]
    public GameObject bulletPrefab;

    [Header("Audio")]
    [Tooltip("Source audio pour le son de tir.")]
    public AudioSource shootAudio;

    [Tooltip("Clip du son de tir (wav/ogg).")]
    public AudioClip shootClip;

    [Range(0f, 1f)]
    public float shootVolume = 0.9f;

    [Tooltip("Variation de pitch pour éviter l'effet copier-coller.")]
    public Vector2 pitchRange = new Vector2(0.97f, 1.03f);

    [Header("Paramètres de tir")]
    public float bulletSpeed = 30f;

    [Tooltip("Temps minimum entre deux tirs (en secondes).")]
    public float fireCooldown = 0.15f;

    [Header("État")]
    [Tooltip("True si l'arme est actuellement tenue (grab).")]
    public bool isHeld = true;

    [Tooltip("True si un chargeur est engagé dans l'arme.")]
    public bool hasMagazine = true;

    private float _lastShotTime = -999f;

    public void TryShoot()
    {
        Debug.Log($"[HandGunShooter] TryShoot() called. isHeld={isHeld}, hasMag={hasMagazine}, dt={Time.time - _lastShotTime}");

        if (!isHeld)
        {
            Debug.Log("[HandGunShooter] Tir bloqué : isHeld == false");
            return;
        }

        if (!hasMagazine)
        {
            Debug.Log("[HandGunShooter] Tir bloqué : pas de chargeur");
            return;
        }

        if (Time.time - _lastShotTime < fireCooldown)
        {
            Debug.Log("[HandGunShooter] Tir bloqué : cooldown");
            return;
        }

        _lastShotTime = Time.time;
        Debug.Log("[HandGunShooter] TIR VALIDE -> Shoot()");
        Shoot();
    }

    private void Shoot()
    {
        if (muzzle == null || bulletPrefab == null)
        {
            Debug.LogWarning($"[HandGunShooter] Shoot() annulé : muzzle={muzzle}, bulletPrefab={bulletPrefab}");
            return;
        }

        Vector3 spawnPos = muzzle.position + muzzle.forward * 0.05f;
        Quaternion rot = muzzle.rotation * Quaternion.Euler(90f, 0f, 0f);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);
        Debug.Log($"[HandGunShooter] Bullet instanciée : {bullet.name} à {spawnPos}");

        if (!bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb = bullet.AddComponent<Rigidbody>();
            Debug.Log("[HandGunShooter] Pas de Rigidbody sur la balle, ajouté en runtime.");
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearVelocity = muzzle.forward * bulletSpeed;
        Debug.Log($"[HandGunShooter] velocity appliquée : {rb.linearVelocity}");

        // ✅ Audio: superposition + variation légère
        if (shootAudio != null && shootClip != null)
        {
            shootAudio.pitch = Random.Range(pitchRange.x, pitchRange.y);
            shootAudio.PlayOneShot(shootClip, shootVolume);
        }
        else if (shootAudio != null && shootAudio.clip != null)
        {
            // fallback si tu préfères utiliser AudioSource.clip dans l'inspector
            shootAudio.pitch = Random.Range(pitchRange.x, pitchRange.y);
            shootAudio.PlayOneShot(shootAudio.clip, shootVolume);
        }

        Destroy(bullet, 5f);
    }

    public void OnGrabbed()
    {
        Debug.Log("[HandGunShooter] OnGrabbed()");
        isHeld = true;
    }

    public void OnReleased()
    {
        Debug.Log("[HandGunShooter] OnReleased()");
        isHeld = false;
    }
}
