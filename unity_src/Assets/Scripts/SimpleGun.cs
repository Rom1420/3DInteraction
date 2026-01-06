using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRWeapons
{
    /// <summary>
    /// XR grab interactable gun that fires projectiles when the activate
    /// (trigger) action is pressed on the interactor that is holding it.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class SimpleGun : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
        [SerializeField] private Transform muzzle;
        [SerializeField] private Projectile projectilePrefab;

        [Header("Fire Settings")]
        [SerializeField] private float muzzleVelocity = 20f;
        [SerializeField] private float fireDelaySeconds = 0.2f;

        [Header("Haptics")]
        [Range(0f, 1f)]
        [SerializeField] private float hapticAmplitude = 0.6f;
        [SerializeField] private float hapticDuration = 0.08f;

        private float _nextAllowedFireTime;

        private void Reset()
        {
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        private void Awake()
        {
            if (grabInteractable == null)
                grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            grabInteractable.activated.RemoveListener(OnActivated);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            if (Time.time < _nextAllowedFireTime)
                return;

            if (projectilePrefab == null || muzzle == null)
                return;

            _nextAllowedFireTime = Time.time + fireDelaySeconds;

            var projectileInstance = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            projectileInstance.Launch(muzzle.forward * muzzleVelocity);

            SendHaptics(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor);
        }

        private void SendHaptics(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor interactor)
        {
            if (interactor?.xrController == null)
                return;

            interactor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }
    }
}
