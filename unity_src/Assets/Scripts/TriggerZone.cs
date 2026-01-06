using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Script de tir de l'arme (HandGunShooter)")]
    public HandGunShooter shooter;

    [Tooltip("Tag utilisé sur le collider du bout du doigt index.")]
    public string indexFingerTag = "IndexFinger";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[TriggerZone] OnTriggerEnter avec {other.name}, tag={other.tag}", this);

        if (shooter == null)
        {
            Debug.LogWarning("[TriggerZone] shooter == null, impossible de tirer", this);
            return;
        }

        if (other.CompareTag(indexFingerTag))
        {
            Debug.Log("[TriggerZone] Index détecté -> TryShoot()", this);
            shooter.TryShoot();
        }
        else
        {
            Debug.Log($"[TriggerZone] Collider ignoré (tag != {indexFingerTag})", this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Utile pour voir si ça rentre bien en continu
        Debug.Log($"[TriggerZone] OnTriggerStay avec {other.name}, tag={other.tag}", this);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[TriggerZone] OnTriggerExit avec {other.name}, tag={other.tag}", this);
    }
}
