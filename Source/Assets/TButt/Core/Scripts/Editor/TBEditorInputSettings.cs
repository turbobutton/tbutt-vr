using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using TButt.Input;
using UnityEditor.VersionControl;

namespace TButt.Editor
{
    public class TBEditorInputSettings : EditorWindow
    {
        static EditorWindow window;

        static bool hasCompiled;
        static TBSettings.TBControlSettings controlSettings;
        static TBSettings.TBControlSettings savedControlSettings;
        static bool showGlobalSettings = false;
        static int controllerLabelColumnWidth = 125;
        static float controllerLabelColumnWidthRemainder = 0;
        static float controllerColumnWidth = 0;

        public static List<EditorWindow> controllerWindows;

        static readonly string messageEnd = "\n <size=9><color=orange>via TButt Editor Input Settings</color></size>";

        public static readonly string settingsPath = "Assets/Resources/";   // Location of the TButtSettings folder.

        [MenuItem("TButt/Input Settings...", false, 100)]
        public static void ShowWindow()
        {
            hasCompiled = false;
            TBEditorStyles.SetupSharedStyles();
            ReadSettings();
            window = EditorWindow.GetWindow(typeof(TBEditorInputSettings), true, "Input Settings", true);
            ClearAllButtonMapCaches();
        }

        void OnGUI()
        {
            // Don't edit settings in play mode.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Cannot edit input settings while in play mode.", MessageType.Error);
                return;
            }

            // Waiting for it to recompile...
            if(EditorApplication.isCompiling || hasCompiled)
            {
                if (!hasCompiled)
                {
                    hasCompiled = true;
                    CloseControllerWindows();   // close windows if we're compiling
                }
                EditorGUILayout.HelpBox("Wait for compiling to finish...", MessageType.Warning);
                return;
            }

            if(window == null)
            {
                ShowWindow();
            }

            window.minSize = new Vector2(1024, 768);
            controllerLabelColumnWidthRemainder = (window.position.width - controllerLabelColumnWidth) % TBEditorSDKSettings.GetNumActiveSDKs();
            controllerColumnWidth = (window.position.width - controllerLabelColumnWidth - controllerLabelColumnWidthRemainder - 10) / TBEditorSDKSettings.GetNumActiveSDKs();
            EditorGUILayout.BeginVertical();
            ShowGlobalControlSettings();
            ShowControllerMatrix();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(new GUILayoutOption[1] { GUILayout.Height(70) });
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save and Close", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                SaveSettings();
                window.Close();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Close Without Saving", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                CloseControllerWindows();
                window.Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        void ShowGlobalControlSettings()
        {
            EditorGUILayout.BeginVertical(TBEditorStyles.sectionBox);
            EditorGUILayout.BeginVertical();
            TBEditorStyles.ShowFakeFoldoutBox(ref showGlobalSettings, "More Input Settings", TBEditorStyles.h1);
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            if (showGlobalSettings)
            {
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal();
                ShowOtherGlobalOptions();
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void ShowOtherGlobalOptions()
        {
            EditorGUIUtility.labelWidth = 225;

            // Header and description.
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Other Options", TBEditorStyles.h2);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Other input options you may wish to enable.");
            GUILayout.EndVertical();

            EditorGUILayout.Separator();

            // Toggles for misc settings.
            GUILayout.BeginVertical();
            controlSettings.useInputEvents = EditorGUILayout.ToggleLeft(new GUIContent("Use Input Events", "Adds support for TBInput.Event delegates."), controlSettings.useInputEvents);
            controlSettings.defaultEditorControlType = (TBInput.ControlType)EditorGUILayout.EnumPopup(new GUIContent("Default Editor ControlType", "Adds support for TBInput.Event delegates."), controlSettings.defaultEditorControlType);
            EditorGUI.BeginDisabledGroup(!controlSettings.supports3DOFControllers);
                controlSettings.emulate3DOFArmModel = EditorGUILayout.BeginToggleGroup(new GUIContent("Emulate 3DOF Arm Model with Hand Controllers", "Forces Vive / Oculus Touch controllers to emulate 3DOF controllers instead of their normal functionality. Useful for testing mobile VR controls in the editor."), controlSettings.emulate3DOFArmModel);
            controlSettings.handedness3DOF = (TBSettings.TBHardwareHandedness)EditorGUILayout.EnumPopup("Editor 3DOF Handedness", controlSettings.handedness3DOF);
            EditorGUILayout.EndToggleGroup();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        static void ReadSettings()
        {
            if (!Directory.Exists(settingsPath + TBSettings.settingsFolder))
                Directory.CreateDirectory(settingsPath + TBSettings.settingsFolder);

            savedControlSettings = TBDataManager.DeserializeFromFile<TBSettings.TBControlSettings>(settingsPath + TBSettings.settingsFolder + TBSettings.controlSettingsFileName + ".json", TBDataManager.PathType.ProjectFolder);
            controlSettings = savedControlSettings;
        }

        static void SaveSettings()
        {
            // Save global settings.
            if (!savedControlSettings.Equals(controlSettings))
            {
                TBEditorHelper.CheckoutAndSaveJSONFile(settingsPath + TBSettings.settingsFolder + TBSettings.controlSettingsFileName + ".json", controlSettings, TBDataManager.PathType.ProjectFolder);
                //TBDataManager.SerializeObjectToFile(controlSettings, settingsPath + TBSettings.settingsFolder + TBSettings.controlSettingsFileName + ".json", TBDataManager.PathType.ProjectFolder);
            }
            SaveAllButtonMaps();
            TBLogging.LogMessage("All input settings saved.", messageEnd);
        }

        #region CONTROLLER GROUP GUI

        public static void ShowControllerMatrix()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("", TBEditorStyles.h3, new GUILayoutOption[3] { GUILayout.Width(controllerLabelColumnWidth + controllerLabelColumnWidthRemainder + 2), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false) });

            //EditorGUILayout.LabelField("", new GUILayoutOption[1]{ GUILayout.Width(controllerLabelColumnWidth + controllerLabelColumnWidthRemainder) });

            ShowControllerMatrixHeader(TBEditorSDKSettings.GetActiveSDKs().oculus, VRPlatform.OculusPC, TBEditorStyles.sectionOverlayOculus);
            ShowControllerMatrixHeader(TBEditorSDKSettings.GetActiveSDKs().steamVR, VRPlatform.SteamVR, TBEditorStyles.sectionOverlaySteam);
            ShowControllerMatrixHeader(TBEditorSDKSettings.GetActiveSDKs().googleVR, VRPlatform.Daydream, TBEditorStyles.sectionOverlayGoogle);
            ShowControllerMatrixHeader(TBEditorSDKSettings.GetActiveSDKs().psvr, VRPlatform.PlayStationVR, TBEditorStyles.sectionOverlayPSVR);
            ShowControllerMatrixHeader(TBEditorSDKSettings.GetActiveSDKs().windows, VRPlatform.WindowsMR, TBEditorStyles.sectionOverlayWindows);
            EditorGUILayout.EndHorizontal();

            // Hand Controllers
            EditorGUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            ShowControllerGroup(controlSettings.supportsHandControllers, TBInput.ControlType.HandControllers);
            EditorGUILayout.EndHorizontal();
    
            // 3DOF Controllers
            EditorGUILayout.BeginHorizontal(TBEditorStyles.tableOddRow);
            ShowControllerGroup(controlSettings.supports3DOFControllers, TBInput.ControlType.Mobile3DOFController);
            EditorGUILayout.EndHorizontal();

            // Gamepads
            EditorGUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            ShowControllerGroup(controlSettings.supportsGamepad, TBInput.ControlType.Gamepad);
            EditorGUILayout.EndHorizontal();

            // Click Remote
            EditorGUILayout.BeginHorizontal(TBEditorStyles.tableOddRow);
            ShowControllerGroup(controlSettings.supportsClickRemote, TBInput.ControlType.ClickRemote);
            EditorGUILayout.EndHorizontal();
        }

        public static void ShowControllerMatrixHeader(bool show, VRPlatform platform, GUIStyle color)
        {
            if (show)
            {
                EditorGUILayout.BeginVertical(color, new GUILayoutOption[1] { GUILayout.Width(controllerColumnWidth) });
                GUILayout.Label(TBEditorStyles.GetPlatformIcon(platform), TBEditorStyles.h1centered);
                EditorGUILayout.EndVertical();
            }
        }

        public static void ShowControllerGroup(bool active, TBInput.ControlType controlType)
        {
            EditorGUILayout.BeginVertical();
            switch (controlType)
            {
                case TBInput.ControlType.HandControllers:
                    ShowControllerGroupLabel("Hand Controllers", ref controlSettings.supportsHandControllers);
                    break;
                case TBInput.ControlType.Mobile3DOFController:
                    ShowControllerGroupLabel("3DOF Controllers", ref controlSettings.supports3DOFControllers);
                    break;
                case TBInput.ControlType.Gamepad:
                    ShowControllerGroupLabel("Gamepads", ref controlSettings.supportsGamepad);
                    break;
                case TBInput.ControlType.ClickRemote:
                    ShowControllerGroupLabel("Click Remotes", ref controlSettings.supportsClickRemote);
                    break;
            }
            GUI.contentColor = Color.white;
            EditorGUILayout.EndVertical();

            if (TBEditorSDKSettings.GetActiveSDKs().oculus)
            {
                EditorGUILayout.BeginVertical(TBEditorStyles.sectionOverlayOculus, new GUILayoutOption[2] { GUILayout.Width(controllerColumnWidth), GUILayout.ExpandHeight(true) });
                GUI.backgroundColor = TBEditorStyles.solidOculus;
                TBEditorInputOculus.ShowOculusControllerList(active, controlType);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
            }

            if (TBEditorSDKSettings.GetActiveSDKs().steamVR)
            {
                EditorGUILayout.BeginVertical(TBEditorStyles.sectionOverlaySteam, new GUILayoutOption[2] { GUILayout.Width(controllerColumnWidth), GUILayout.ExpandHeight(true) });
                GUI.backgroundColor = TBEditorStyles.solidSteam;
                TBEditorInputSteamVR.ShowSteamVRControllerList(active, controlType);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
            }

            if (TBEditorSDKSettings.GetActiveSDKs().googleVR)
            {
                EditorGUILayout.BeginVertical(TBEditorStyles.sectionOverlayGoogle, new GUILayoutOption[2] { GUILayout.Width(controllerColumnWidth), GUILayout.ExpandHeight(true) });
                GUI.backgroundColor = TBEditorStyles.solidGoogle;
                TBEditorInputGoogle.ShowGoogleControllerList(active, controlType);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
            }

            if (TBEditorSDKSettings.GetActiveSDKs().psvr)
            {
                EditorGUILayout.BeginVertical(TBEditorStyles.sectionOverlayPSVR, new GUILayoutOption[2] { GUILayout.Width(controllerColumnWidth), GUILayout.ExpandHeight(true) });
                GUI.backgroundColor = TBEditorStyles.solidPSVR;
#if UNITY_PS4
                TBEditorInputPSVR.ShowPSVRControllerList(active, controlType);
#else
                EditorGUILayout.HelpBox("PSVR settings are only available when Unity platform is set to PS4 and the PS4 module is installed.", MessageType.Info);
#endif
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
            }

            if (TBEditorSDKSettings.GetActiveSDKs().windows)
            {
                EditorGUILayout.BeginVertical(TBEditorStyles.sectionOverlayWindows, new GUILayoutOption[2] { GUILayout.Width(controllerColumnWidth), GUILayout.ExpandHeight(true) });
                GUI.backgroundColor = TBEditorStyles.solidWindows;
                TBEditorInputWindowsMR.ShowWindowsMRControllerList(active, controlType);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
            }
        }

            static void ShowControllerGroupLabel(string label, ref bool groupToggle)
        {
            EditorGUILayout.LabelField(label, TBEditorStyles.h2, new GUILayoutOption[3] { GUILayout.Width(controllerLabelColumnWidth + controllerLabelColumnWidthRemainder), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false) });
            if (groupToggle)
            {
                GUI.contentColor = Color.green;
                groupToggle = EditorGUILayout.ToggleLeft(new GUIContent("Enabled"), groupToggle, GUILayout.Width(controllerLabelColumnWidth + controllerLabelColumnWidthRemainder));
            }
            else
            {
                groupToggle = EditorGUILayout.ToggleLeft(new GUIContent("Enable"), groupToggle, GUILayout.Width(controllerLabelColumnWidth + controllerLabelColumnWidthRemainder));
            }
        }
#endregion

#region BUTTON MAP GUI
        public static void StartButtonMapTable()
        {
            EditorGUILayout.BeginHorizontal(TBEditorStyles.tableHeaderRow);
            EditorGUILayout.LabelField("Controller Button", new GUILayoutOption[1] { GUILayout.Width(200) });
            EditorGUILayout.LabelField("Touch Sensitive", new GUILayoutOption[1] { GUILayout.Width(120) });
            EditorGUILayout.LabelField("Axis Type", new GUILayoutOption[1] { GUILayout.Width(120) });
            EditorGUILayout.LabelField("Virtual Buttons");
            EditorGUILayout.EndHorizontal();
        }

        public static void EndButtonMapTable()
        {
            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Creates the GUI of rows and buttons for all inputs of a controller.
        /// </summary>
        /// <param name="virtualButtons"></param>
        /// <param name="buttonName"></param>
        /// <param name="rowNum"></param>
        /// <param name="defaults"></param>
        public static void ShowButtonRows(ref TBInput.Button[] virtualButtons, string buttonName, int rowNum, TBInput.Button[] defaults, bool supportsTouch, bool supportsAxis1D, bool supportsAxis2D)
        {
            // Set up row coloring.
            if (rowNum % 2 > 0)
                EditorGUILayout.BeginHorizontal(TBEditorStyles.tableOddRow, TBEditorStyles.buttonList);
            else
                EditorGUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow, TBEditorStyles.buttonList);

            // Print semantic name of the button.
            EditorGUILayout.LabelField(buttonName, new GUILayoutOption[1] { GUILayout.Width(200) });

            // Show Touch support
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!supportsTouch);
            if (supportsTouch)
                EditorGUILayout.LabelField("Yes", new GUILayoutOption[1] { GUILayout.Width(120) });
            else
                EditorGUILayout.LabelField("No", new GUILayoutOption[1] { GUILayout.Width(120) });
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            // Show Axis type
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!supportsAxis1D && !supportsAxis2D);
            if (supportsAxis1D)
                EditorGUILayout.LabelField("1D Axis", new GUILayoutOption[1] { GUILayout.Width(120) });
            else if (supportsAxis2D)
                EditorGUILayout.LabelField("2D Axis", new GUILayoutOption[1] { GUILayout.Width(120) });
            else
                EditorGUILayout.LabelField("None", new GUILayoutOption[1] { GUILayout.Width(120) });
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            // Show the button map dropdowns.
            EditorGUILayout.BeginVertical();
            for (int j = 0; j < virtualButtons.Length + 1; j++)
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[2] { GUILayout.MinWidth(200), GUILayout.Height(20) });
                if (j == virtualButtons.Length)
                {
                    // Show the "+" button at the bottom of the list to add more virtual buttons to the mask.
                    if (GUILayout.Button("+", new GUILayoutOption[1] { GUILayout.Height(15) }))
                        virtualButtons = TBArrayExtensions.AddToArray<TBInput.Button>(virtualButtons, TBInput.Button.None); // virtualButtons = TBEditorInputSettings.AddToButtonArray(virtualButtons);
                }
                else
                {
                    // Show the button and a "X" button so we can change or delete the mapping.
                    if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(15) }))
                    {
                        // Reset it to "none" if this is the only button for this map, otherwise remove it from the array.
                        if (virtualButtons.Length == 1)
                            virtualButtons[j] = TBInput.Button.None;
                        else
                            virtualButtons = TBArrayExtensions.RemoveFromArray<TBInput.Button>(virtualButtons, j); // virtualButtons = TBEditorInputSettings.RemoveFromArray(virtualButtons, j);
                    }
                    if (j < virtualButtons.Length)
                    {
                        virtualButtons[j] = (TBInput.Button)EditorGUILayout.EnumPopup(virtualButtons[j]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Show the "reset to default" buttons for each input type.
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Reset to Default", new GUILayoutOption[1] { GUILayout.Height(30) }))
            {
                virtualButtons = defaults;
                TBLogging.LogMessage("Reset mapping for " + buttonName + " to " + defaults[0].ToString(), messageEnd);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Displays button maps for a given controller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controllers"></param>
        /// <param name="defaultMaps"></param>
        /// <param name="controllerName"></param>
        public static void ShowMaps<T>(ref Dictionary<string, ButtonMapGroup<TBInput.ButtonDef<T>>> controllers, List<TBInput.ButtonDef<T>> defaultMaps, string controllerName, GUIStyle SDKBackground, Color buttonColor)
        {
            EditorGUILayout.BeginVertical(SDKBackground, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
            GUI.backgroundColor = buttonColor;
            EditorGUILayout.LabelField(controllerName, TBEditorStyles.h1);
            EditorGUILayout.Separator();
            TBEditorInputSettings.StartButtonMapTable();
            for (int i = 0; i < controllers[controllerName].defs.Count; i++)
            {
                TBEditorInputSettings.ShowButtonRows(ref controllers[controllerName].defs[i].virtualButtons, controllers[controllerName].defs[i].name, i, defaultMaps[i].virtualButtons, defaultMaps[i].supportsTouch, defaultMaps[i].supportsAxis1D, defaultMaps[i].supportsAxis2D);
            }
            TBEditorInputSettings.EndButtonMapTable();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }
#endregion

#region BUTTON MAP SAVING / LOADING
        static void SaveAllButtonMaps()
        {
#if TB_OCULUS
            SaveNewButtonMapsForSDK<OVRInput.RawButton>(TBEditorInputOculus.controllers);
#endif
#if TB_STEAM_VR
            SaveNewButtonMapsForSDK<Valve.VR.EVRButtonId>(TBEditorInputSteamVR.controllers);
#endif
#if TB_GOOGLE
            SaveNewButtonMapsForSDK<GvrControllerButton>(TBEditorInputGoogle.controllers);
#endif
#if TB_PSVR && UNITY_PS4
            SaveNewButtonMapsForSDK<TBPSVRInput.Button>(TBEditorInputPSVR.controllers);
#endif
#if TB_WINDOWS_MR
            SaveNewButtonMapsForSDK<TBWindowsMRInput.Button>(TBEditorInputWindowsMR.controllers);
#endif
#if TB_STEAM_VR || TB_WINDOWS_MR
            SaveNewButtonMapsForSDK<TBXInput.TBXInputButton>(TBEditorInputXInput.controllers);
#endif
        }

        /// <summary>
        /// Iterates through a dictionary and saves any new button maps for the given SDK dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controllers"></param>
        static void SaveNewButtonMapsForSDK<T>(Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<T>>> controllers)
        {
            if (controllers.Count == 0)
                return;

            CloseControllerWindows();

            foreach (KeyValuePair<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<T>>> entry in controllers)
            {
                SaveButtonMaps<T>(entry.Value);
            }
        }

        public static void ClearAllButtonMapCaches()
        {
#if TB_OCULUS
            TBEditorInputOculus.controllers.Clear();
#endif
#if TB_STEAM_VR
            TBEditorInputSteamVR.controllers.Clear();
#endif
#if TB_GOOGLE
            TBEditorInputGoogle.controllers.Clear();
#endif
#if TB_PSVR
#endif
#if TB_WINDOWS_MR
            TBEditorInputWindowsMR.controllers.Clear();
#endif

            // Clear existing controller windows.
            if (controllerWindows != null)
                controllerWindows.Clear();
            else
                controllerWindows = new List<EditorWindow>();
        }

        private static void CloseControllerWindows()
        {
            // We need to force the controller windows to close whenever certain things happen to prevent data corruption.
            for (int i = 0; i < controllerWindows.Count; i++)
            {
                if (controllerWindows[i] != null)
                    controllerWindows[i].Close();
            }
        }

        /// <summary>
        /// Loads controller data into memory for a given SDK.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controllers"></param>
        /// <param name="controllerName"></param>
        /// <param name="defaultButtonDefs"></param>
        /// <param name="fileName"></param>
        public static string LoadController<T>(ref Dictionary<string, ButtonMapGroup<TBInput.ButtonDef<T>>> controllers, string controllerName, List<TBInput.ButtonDef<T>> defaultButtonDefs, string fileName)
        {
            ButtonMapGroup<TBInput.ButtonDef<T>> newController;
            if (controllers.TryGetValue(controllerName, out newController))
            {
                return controllerName;
            }
            else
            {
                newController = new ButtonMapGroup<TBInput.ButtonDef<T>>();
                newController.fileName = fileName;
                newController.controllerName = controllerName;
                newController.defs = defaultButtonDefs;

                // If a custom mapping exits, try to use it.
                if (File.Exists(settingsPath + TBSettings.settingsFolder + newController.fileName + ".json"))
                {
                    newController.defs = TBInput.LoadButtonDefs(defaultButtonDefs, fileName);
                    TBLogging.LogMessage("Found and loaded button maps for " + controllerName + ".", messageEnd);
                }
                else
                {
                    // If no custom mapping, we use the default one. A custom one will be saved if you make any changes.
                    TBLogging.LogMessage("Could not find custom button maps for " + controllerName + " in " + settingsPath + TBSettings.settingsFolder + newController.fileName + ".json. Created new maps from default.", messageEnd);
                }
                controllers.Add(controllerName, newController);
                return controllerName;
            }
        }

        public static void GetControllerButton<T,W>(TBControllerBase<T> controller, ref Dictionary<string, ButtonMapGroup<TBInput.ButtonDef<T>>> controllers, bool active, string overrideName = "")
        {
            if (string.IsNullOrEmpty(overrideName))
                overrideName = controller.GetName();

            if (GUILayout.Button(overrideName, TBEditorStyles.controllerSelectButton))
            {
                if (active)
                {
                    TBEditorInputSettings.LoadController<T>(ref controllers, controller.GetName(), controller.GetDefaultDefs(), controller.GetFileName());
                    TBButtonMapWindow<W>.ShowWindow(controller.GetName());
                }
            }
        }

        /// <summary>
        /// Saves button settings to JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maps"></param>
        public static void SaveButtonMaps<T>(ButtonMapGroup<TBInput.ButtonDef<T>> maps)
        {
            TBLogging.LogMessage("Writing maps for " + maps.controllerName + " to " + settingsPath + TBSettings.settingsFolder + maps.fileName + ".json...", messageEnd);
            TBInput.SerializedButtonDef[] controllerDef = new TBInput.SerializedButtonDef[maps.defs.Count];
            for (int i = 0; i < maps.defs.Count; i++)
            {
                controllerDef[i] = new TBInput.SerializedButtonDef();
                controllerDef[i].virtualButtons = maps.defs[i].virtualButtons;
            }
            //string json = TBDataManager.ToJson<TBInput.SerializedButtonDef>(controllerDef);
            //TBDataManager.SerializeObjectToFile(TBDataManager.ToJsonWrapper<TBInput.SerializedButtonDef>(controllerDef), settingsPath + TBSettings.settingsFolder + maps.fileName + ".json", TBDataManager.PathType.ResourcesFolder);

            TBEditorHelper.CheckoutAndSaveJSONFile(settingsPath + TBSettings.settingsFolder + maps.fileName + ".json", TBDataManager.ToJsonWrapper<TBInput.SerializedButtonDef>(controllerDef), TBDataManager.PathType.ResourcesFolder);
            TBLogging.LogMessage("Finished writing maps for " + maps.controllerName + ". ", messageEnd);
        }

        /// <summary>
        /// Adds a button to a button array. Helper class used by the GUI functions.
        /// </summary>
        /// <param name="oldArray"></param>
        /// <returns></returns>
        private static TBInput.Button[] AddToButtonArray(TBInput.Button[] oldArray)
        {
            TBInput.Button[] newArray = new TBInput.Button[oldArray.Length + 1];
            for (int i = 0; i < oldArray.Length; i++)
            {
                newArray[i] = oldArray[i];
            }
            newArray[newArray.Length - 1] = TBInput.Button.None;
            return newArray;
        }

        /// <summary>
        /// Removes a button from a button array. Helper class used by the GUI functions.
        /// </summary>
        private static TBInput.Button[] RemoveFromArray(TBInput.Button[] oldArray, int index)
        {
            var newArray = new List<TBInput.Button>(oldArray);
            newArray.RemoveAt(index);
            return newArray.ToArray();
        }

        [System.Serializable]
        public struct ButtonMapGroup<T>
        {
            public List<T> defs;
            public string fileName;
            public string controllerName;
        }
#endregion
    }

    /// <summary>
    /// Base class for the button map windows.
    /// </summary>
    public class TBButtonMapWindow<W> : EditorWindow
    {
        protected static EditorWindow window;
        protected static Vector2 scrollPos;

        public static void ShowWindow(string title)
        {
            window = EditorWindow.GetWindow(typeof(W), true, title + " Settings", true);
            window.minSize = TBEditorStyles.controllerMapWindowSize;
            TBEditorInputSettings.controllerWindows.Add(window);
        }
    }
}
