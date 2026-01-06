using UnityEngine;
using System.Collections;

public class HitMaterialSwap : MonoBehaviour
{
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material hitMaterial;
    [SerializeField] string ammoTag = "Ammo";
    [SerializeField] float revertDelay = 2f;

    Coroutine revertRoutine;

    void Reset()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(ammoTag)) return;
        ApplyHitMaterial();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ammoTag)) return;
        ApplyHitMaterial();
    }

    void ApplyHitMaterial()
    {
        if (targetRenderer == null || hitMaterial == null || normalMaterial == null) return;

        targetRenderer.material = hitMaterial; // uses an instance; avoids altering sharedMaterial
        if (revertRoutine != null) StopCoroutine(revertRoutine);
        revertRoutine = StartCoroutine(RevertAfterDelay());
    }

    IEnumerator RevertAfterDelay()
    {
        yield return new WaitForSeconds(revertDelay);
        targetRenderer.material = normalMaterial;
        revertRoutine = null;
    }
}
