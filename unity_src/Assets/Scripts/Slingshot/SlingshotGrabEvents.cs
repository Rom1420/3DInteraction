using UnityEngine;

public class SlingshotGrabEvents : MonoBehaviour
{
    public SlingshotPouch pouch;

    [Header("Hand Transforms (assign in Inspector)")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("Optional: point to compare distances from (else uses pouch transform)")]
    public Transform pouchGrabPoint;

    public void OnGrab()
    {
        Debug.Log("[SlingshotDebug] OnGrab called");
        if (pouch == null) return;

        Transform refPoint = pouchGrabPoint != null ? pouchGrabPoint : pouch.transform;

        float dL = leftHand  ? Vector3.Distance(leftHand.position,  refPoint.position) : float.PositiveInfinity;
        float dR = rightHand ? Vector3.Distance(rightHand.position, refPoint.position) : float.PositiveInfinity;

        Transform chosen = (dL <= dR) ? leftHand : rightHand;

        if (chosen != null)
            pouch.BeginPull(chosen);
    }

    public void OnRelease()
    {
        Debug.Log("[SlingshotDebug] OnRelease called");
        if (pouch == null) return;
        pouch.EndPull();
    }
}
