using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TButt
{
    /// <summary>
    /// Sample class that waits for TButt to be initialized before continuing to the next scene.
    /// Only needed in cases where you expect your game to get initialized without a headset connected.
    /// </summary>
    public class TBWaitForPlatform : MonoBehaviour
    {
        public bool overrideNextScene;
        public string sceneName;

        IEnumerator Start()
        {
            while (TBCore.GetActivePlatform() == VRPlatform.None)
                yield return null;

            if (overrideNextScene)
                SceneManager.LoadScene(sceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(TBWaitForPlatform))]
    public class TBWaitForPlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TBWaitForPlatform waitForPlatform = (TBWaitForPlatform)target;

            waitForPlatform.overrideNextScene = EditorGUILayout.Toggle("Override Next Scene", waitForPlatform.overrideNextScene);
            EditorGUI.BeginDisabledGroup(!waitForPlatform.overrideNextScene);
            waitForPlatform.sceneName = EditorGUILayout.TextField("Next Scene Name", waitForPlatform.sceneName);
            EditorGUI.EndDisabledGroup();
        }
    }
    #endif
}