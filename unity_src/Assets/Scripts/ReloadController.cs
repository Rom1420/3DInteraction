using UnityEngine;

public class ReloadController : MonoBehaviour
{
    [Header("Refs")]
    public HandGunShooter shooter;

    [Tooltip("Chargeur visuel attaché à l'arme (enfant de l'AK)")]
    public GameObject magInVisual;

    [Tooltip("Prefab du chargeur 'dans la main' (peut être juste un mesh)")]
    public GameObject magOutPrefab;

    [Header("Main du joueur")]
    [Tooltip("Transform de la main où faire apparaître le chargeur (ex: RightInteractions/Interactors/Hand)")]
    public Transform handAnchor;

    [Tooltip("Rotation locale sur Y (degrés) pour orienter le chargeur dans la main.")]
    public float magLocalYaw = 90f;



    private GameObject _currentMagOut;
    private bool _hasMagazine = true;

    private void Start()
    {
        SetMagazineState(true);
    }

    private void Update()
    {

    }

    public void ToggleReload()
    {
        if (_hasMagazine) EjectMagazine();
        else InsertMagazine();
    }

    public void EjectMagazine()
    {
        if (!_hasMagazine) return;

        Debug.Log("[ReloadController] EjectMagazine()");

        if (magInVisual != null)
            magInVisual.SetActive(false);

        if (magOutPrefab == null || handAnchor == null)
        {
            Debug.LogWarning("[ReloadController] magOutPrefab ou handAnchor manquant");
            SetMagazineState(false);
            return;
        }

        _currentMagOut = Instantiate(magOutPrefab, handAnchor.position, handAnchor.rotation);

        // SNAP dans la main
        _currentMagOut.transform.SetParent(handAnchor, false);
        _currentMagOut.transform.localPosition = Vector3.zero;
        _currentMagOut.transform.localRotation = Quaternion.Euler(0f, magLocalYaw, 0f);


        // si le prefab a un RB, on le neutralise tant qu'il est "dans la main"
        var rb = _currentMagOut.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        SetMagazineState(false);
    }

    public void InsertMagazine()
    {
        if (_hasMagazine) return;

        Debug.Log("[ReloadController] InsertMagazine()");

        // 1) supprimer le chargeur dans la main
        if (_currentMagOut != null)
        {
            Destroy(_currentMagOut);
            _currentMagOut = null;
        }

        // 2) réafficher le mag visuel dans l'arme
        if (magInVisual != null)
            magInVisual.SetActive(true);

        SetMagazineState(true);
    }

    private void SetMagazineState(bool hasMag)
    {
        _hasMagazine = hasMag;

        if (shooter != null)
            shooter.hasMagazine = hasMag;

        Debug.Log($"[ReloadController] hasMagazine={hasMag}");
    }
}
