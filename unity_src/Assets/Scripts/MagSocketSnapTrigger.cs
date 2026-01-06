using UnityEngine;

public class MagSocketSnapTrigger : MonoBehaviour
{
    public Magazine magazine;

    private void OnTriggerEnter(Collider other)
    {
        if (magazine == null) return;

        if (other.CompareTag("MagSocket"))
        {
            magazine.AttachToSocket();
        }
    }
}
