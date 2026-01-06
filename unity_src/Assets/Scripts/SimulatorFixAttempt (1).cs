using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class XRLoadReset{
    static XRLoadReset(){UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;}
    private static void PlayModeChange(UnityEditor.PlayModeStateChange playmode){
        if (playmode == UnityEditor.PlayModeStateChange.ExitingPlayMode){
            if (XRGeneralSettings.Instance != null &&
                XRGeneralSettings.Instance.Manager != null){
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
            Debug.Log("XR Loader OFF");
        }
        if (playmode == UnityEditor.PlayModeStateChange.EnteredPlayMode){
            if (XRGeneralSettings.Instance != null &&
                XRGeneralSettings.Instance.Manager != null){
                XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
            Debug.Log("XR Loader ON");
        }
    }
}
#endif