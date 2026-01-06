using UnityEngine;

public class MagazineSocket : MonoBehaviour
{
    [Tooltip("Script de tir de l'arme (pour mettre à jour hasMagazine au démarrage).")]
    public HandGunShooter shooter;

    [Tooltip("Référence vers le chargeur de départ, s'il est déjà engagé.")]
    public Magazine startingMagazine;

    private void Start()
    {
        // Si tu veux que le mag de départ soit déjà attaché proprement
        if (startingMagazine != null)
        {
            startingMagazine.AttachToSocket();
        }

        if (shooter != null && startingMagazine != null)
        {
            shooter.hasMagazine = true;
        }
    }
}
