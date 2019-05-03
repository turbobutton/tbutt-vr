using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Handles logging for TButt classes. Can be turned off in the TButt Editor Settings panel.
/// </summary>
public static class TBLogging
{
    [Conditional("TB_ENABLE_LOGS")]
    public static void LogMessage(string message, Object obj = null, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");

        if(obj == null)
            UnityEngine.Debug.Log(message + end);
        else
            UnityEngine.Debug.Log(message + end, obj);
    }

    [Conditional("TB_ENABLE_LOGS")]
    public static void LogWarning(string message, Object obj = null, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");

        if(obj == null)
            UnityEngine.Debug.LogWarning(message + end);
        else
            UnityEngine.Debug.LogWarning(message + end, obj);
    }

    [Conditional("TB_ENABLE_LOGS")]
    public static void LogError(string message, Object obj = null, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");
#if UNITY_EDITOR
        if(obj == null)
            UnityEngine.Debug.LogError("<color=red>" + message + "</color>" + end);
        else
            UnityEngine.Debug.LogError("<color=red>" + message + "</color>" + end, obj);
#else
        UnityEngine.Debug.LogError(message + end);
#endif
    }

    private static string FormatForEditor(string text)
    {
#if UNITY_EDITOR
        return "\n <size=9><color=orange>" + text + "</color></size>";
#else
        return "\n" + text;
#endif
    }
}
