using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Handles logging for TButt classes. Can be turned off in the TButt Editor Settings panel.
/// </summary>
public static class TBLogging
{
    [Conditional("TB_ENABLE_LOGS")]
    public static void LogMessage(string message, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");
        UnityEngine.Debug.Log(message + end);
    }

    [Conditional("TB_ENABLE_LOGS")]
    public static void LogWarning(string message, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");
        UnityEngine.Debug.LogWarning(message + end);
    }

    [Conditional("TB_ENABLE_LOGS")]
    public static void LogError(string message, string end = "")
    {
        if (string.IsNullOrEmpty(end))
            end = FormatForEditor("via TButt");
#if UNITY_EDITOR
        UnityEngine.Debug.LogError("<color=red>" + message + "</color>" + end);
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
