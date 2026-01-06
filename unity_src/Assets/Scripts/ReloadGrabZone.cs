using UnityEngine;

public class ReloadGrabZone : MonoBehaviour
{
    public ReloadController reloadController;

    [Tooltip("Tag de la main (mets LeftHand si c'est ce que tu vois dans tes logs)")]
    public string handTag = "LeftHand";

    public float cooldown = 0.5f;
    private float _lastTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ReloadGrabZone] Enter name={other.name} tag={other.tag} root={other.transform.root.name}", this);
        if (reloadController == null) return;


        if (!other.CompareTag(handTag)) return;

        if (Time.time - _lastTime < cooldown) return;
        _lastTime = Time.time;

        Debug.Log("[ReloadGrabZone] Hand entered -> ToggleReload()", this);
        reloadController.ToggleReload();
    }
}
