using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Rigidbody))]
public class Magazine : MonoBehaviour
{
    [Header("Refs")]
    public Transform socketTransform;
    public HandGunShooter shooter;

    [Header("Components")]
    public Rigidbody rb;
    public Grabbable grabbable;              
    public Collider solidCollider;           
    public Collider socketTriggerCollider;    

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (grabbable == null) grabbable = GetComponent<Grabbable>();

        // Sécurité : éviter de traverser le sol
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void AttachToSocket()
    {
        if (socketTransform == null)
        {
            Debug.LogWarning("[Magazine] socketTransform null");
            return;
        }

        Debug.Log("[Magazine] AttachToSocket");

        // Parent au socket puis local à zéro (ton souhait)
        transform.SetParent(socketTransform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Physique OFF
        rb.isKinematic = true;
        rb.useGravity = false;

        // IMPORTANT: quand il est inséré -> pas grabbable
        if (grabbable != null) grabbable.enabled = false;

        if (shooter != null) shooter.hasMagazine = true;
    }

    public void DetachFromSocket()
    {
        Debug.Log("[Magazine] DetachFromSocket");

        transform.SetParent(null);

        // Physique ON
        rb.isKinematic = false;
        rb.useGravity = true;

        // Maintenant il devient grabbable
        if (grabbable != null) grabbable.enabled = true;

        if (shooter != null) shooter.hasMagazine = false;
    }
}
