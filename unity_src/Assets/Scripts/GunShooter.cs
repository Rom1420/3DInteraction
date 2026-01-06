using UnityEngine;

public class GunShooter : MonoBehaviour
{
    [Header("Shoot setup")]
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float bulletSpeed = 30f;

    [Header("Trigger")]
    [Range(0f, 1f)] public float triggerFireThreshold = 0.75f;
    [Range(0f, 1f)] public float triggerReleaseThreshold = 0.25f; // hysteresis to avoid jitter

    [Header("State")]
    public bool isHeld = false;
    public OVRInput.Controller currentHand = OVRInput.Controller.RTouch; // default right hand

    private bool triggerLatched = false;

    void Update()
    {
        if (!isHeld) return;

        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, currentHand);

        // Fire once when the analog trigger crosses the threshold
        if (!triggerLatched && triggerValue >= triggerFireThreshold)
        {
            Shoot();
            triggerLatched = true;
        }
        else if (triggerLatched && triggerValue <= triggerReleaseThreshold)
        {
            triggerLatched = false;
        }
    }

    public void OnGrabbedRight()
    {
        currentHand = OVRInput.Controller.RTouch;
        isHeld = true;
        triggerLatched = false;
    }

    public void OnGrabbedLeft()
    {
        currentHand = OVRInput.Controller.LTouch;
        isHeld = true;
        triggerLatched = false;
    }

    public void OnReleased()
    {
        isHeld = false;
        triggerLatched = false;
    }

    private void Shoot()
    {
        if (muzzle == null || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = muzzle.forward * bulletSpeed;
        }

        // TODO: sound, particles, recoil, etc.
    }
}
