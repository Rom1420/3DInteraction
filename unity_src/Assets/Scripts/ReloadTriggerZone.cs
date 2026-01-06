using UnityEngine;

public class ReloadTriggerZone : MonoBehaviour
{
    public ReloadController reloadController;
    public string indexFingerTag = "IndexFinger";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(indexFingerTag)) return;
        reloadController.ToggleReload();
    }
}
