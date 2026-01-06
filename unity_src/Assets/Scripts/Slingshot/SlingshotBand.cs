using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SlingshotBand : MonoBehaviour
{
    public Transform forkAnchor;      // ForkLeft ou ForkRight
    public SlingshotPouch pouch;      // référence à la poche
    public Transform pouchAttachPoint; // référence à la poche

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void LateUpdate()
    {
        Vector3 p0 = forkAnchor.position;
        Vector3 p1 = (pouchAttachPoint != null) ? pouchAttachPoint.position : pouch.transform.position;

        lr.SetPosition(0, p0);
        lr.SetPosition(1, p1);
    }
}
