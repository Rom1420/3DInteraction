using UnityEngine;
using Oculus.Voice; // Meta Voice SDK
using Meta.WitAi.Json; // for WitResponseNode

public class LookAtSound : MonoBehaviour
{
    [Header("References")]
    public Camera vrCamera;                 // Assign CenterEyeAnchor or MainCamera
    public AppVoiceExperience voice;        // Assign Wit.ai AppVoiceExperience prefab
    public float rayLength = 5f;

    [Header("Audio Clips")]
    public AudioClip firstAudio;            // Audio joué au premier regard
    public AudioClip secondAudio;           // Audio joué sur commande "explique"

    [Header("Rotation Settings")]
    public float rotationSpeed = 2f;        // Vitesse de rotation vers le joueur

    private AudioSource audioSource;
    private bool hasPlayedFirstAudio = false;
    private bool isLookingAt = false;
    private bool wasLookingAt = false;
    private bool shouldLookAtPlayer = false;
    private bool hasPlayedSecondAudioThisTranscription = false;
    private Quaternion originalRotation;

    void Awake()
    {
        if (vrCamera == null) vrCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        originalRotation = transform.rotation; // Sauvegarder la rotation initiale
        Debug.Log("LookAtSound script initialized.");
    }

    void Start()
    {
        // Hook up voice events
        if (voice != null)
        {
            voice.VoiceEvents.OnResponse.AddListener(OnVoiceResponse);
            voice.VoiceEvents.OnError.AddListener(OnVoiceError);
            voice.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
            voice.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);
        }
        else
        {
            Debug.LogWarning("AppVoiceExperience (voice) not assigned. Voice commands won't work.");
        }
    }

    void LateUpdate()
    {
        if (vrCamera == null) return;

        Vector3 origin = vrCamera.transform.position;
        Vector3 direction = vrCamera.transform.rotation * Vector3.forward;

        isLookingAt = false;

        // Raycast pour détecter si on regarde ce personnage
        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength))
        {
            // Si le raycast touche cet objet (celui qui a ce script)
            if (hit.collider.gameObject == gameObject)
            {
                isLookingAt = true;

                // Jouer le premier audio uniquement la première fois
                if (!hasPlayedFirstAudio && audioSource != null && firstAudio != null)
                {
                    audioSource.clip = firstAudio;
                    audioSource.Play();
                    hasPlayedFirstAudio = true;
                    Debug.Log($"First look at {gameObject.name} - Playing first audio");
                }

                // Activer l'écoute vocale quand on regarde
                if (!wasLookingAt && voice != null)
                {
                    voice.Activate();
                    Debug.Log($"Started listening - looking at {gameObject.name}");
                }
            }
        }

        // Désactiver l'écoute quand on arrête de regarder
        if (!isLookingAt && wasLookingAt && voice != null)
        {
            voice.Deactivate();
            shouldLookAtPlayer = false;
            Debug.Log($"Stopped listening - not looking at {gameObject.name}");
        }

        wasLookingAt = isLookingAt;

        // Rotation vers le joueur quand il écoute
        if (shouldLookAtPlayer && vrCamera != null)
        {
            Vector3 directionToPlayer = vrCamera.transform.position - transform.position;
            directionToPlayer.y = 0; // Garder la rotation horizontale seulement
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else if (!shouldLookAtPlayer)
        {
            // Revenir à la rotation d'origine
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Voice events
    private void OnVoiceResponse(WitResponseNode response)
    {
        string text = response["text"];
        Debug.Log($"Heard: {text}");

        // Réactiver l'écoute si on regarde toujours le personnage
        if (isLookingAt && voice != null)
        {
            voice.Activate();
            Debug.Log($"Reactivating voice listening - still looking at {gameObject.name}");
        }
    }

    private void OnVoiceError(string error, string message)
    {
        Debug.LogError($"Voice Error: {error} - {message}");
    }

    private void OnPartialTranscription(string text)
    {
        Debug.Log($"Partial transcription: {text}");
        
        if (string.IsNullOrEmpty(text)) return;
        
        text = text.ToLower();
        
        // Le personnage se tourne vers le joueur dès qu'il entend quelque chose
        if (isLookingAt)
        {
            shouldLookAtPlayer = true;
            
            // Jouer le deuxième audio dès qu'on entend "jeu", "je" ou "non" - mais une seule fois par transcription
            if (!hasPlayedSecondAudioThisTranscription && (text.Contains("jeu") || text.Contains("je") || text.Contains("non")))
            {
                if (audioSource != null && secondAudio != null)
                {
                    audioSource.clip = secondAudio;
                    audioSource.Play();
                    hasPlayedSecondAudioThisTranscription = true;
                    Debug.Log($"Voice command detected - Playing second audio on {gameObject.name}");
                }
            }
        }
    }

    private void OnFullTranscription(string text)
    {
        Debug.Log($"Full transcription: {text}");
        
        // Réinitialiser le flag pour la prochaine transcription
        hasPlayedSecondAudioThisTranscription = false;
        
        // Réactiver l'écoute si on regarde toujours le personnage
        if (isLookingAt && voice != null)
        {
            voice.Activate();
            Debug.Log($"Reactivating voice listening after full transcription - still looking at {gameObject.name}");
        }
    }
}