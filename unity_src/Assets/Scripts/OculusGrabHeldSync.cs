using UnityEngine;
using Oculus.Interaction;   // <- important

[RequireComponent(typeof(Grabbable))]
public class OculusGrabHeldSync : MonoBehaviour
{
    [Tooltip("Script de tir qui doit savoir si l'arme est tenue.")]
    public HandGunShooter shooter;

    private Grabbable _grabbable;

    private void Awake()
    {
        _grabbable = GetComponent<Grabbable>();
    }

    private void OnEnable()
    {
        if (_grabbable != null)
        {
            _grabbable.WhenPointerEventRaised += HandlePointerEvent;
        }
    }

    private void OnDisable()
    {
        if (_grabbable != null)
        {
            _grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (shooter == null) return;

        switch (evt.Type)
        {
            case PointerEventType.Select:
                // l'objet vient d'être sélectionné (grabbé)
                shooter.OnGrabbed();
                break;

            case PointerEventType.Unselect:
                // l'objet vient d'être relâché
                shooter.OnReleased();
                break;
        }
    }
}
