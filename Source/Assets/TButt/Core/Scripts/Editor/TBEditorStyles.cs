using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TButt.Editor
{

    public static class TBEditorStyles
    {
        public static GUIStyle sectionHeader = new GUIStyle();
        public static GUIStyle h1 = new GUIStyle();
        public static GUIStyle h2 = new GUIStyle();
        public static GUIStyle h3 = new GUIStyle();
        public static GUIStyle h1centered = new GUIStyle();
        public static GUIStyle sectionBox = new GUIStyle();
        public static GUIStyle largeButton = new GUIStyle();
        public static GUIStyle smallButton = new GUIStyle();
        public static GUIStyle smallToolbarButton = new GUIStyle();
        public static GUIStyleState focusedSmallToolbarButton = new GUIStyleState();


        public static GUIStyle sdkHeaderBox = new GUIStyle();
        public static GUIStyle sdkSectionBox = new GUIStyle();
        public static GUIStyle sdkMidSectionBox = new GUIStyle();

        public static GUILayoutOption[] controllerSelectButton = { GUILayout.Height(30) };

        public static GUIStyle tableHeaderRow = new GUIStyle();
        public static GUIStyle tableEvenRow = new GUIStyle();
        public static GUIStyle tableOddRow = new GUIStyle();

        public static GUIStyle sectionOverlaySteam = new GUIStyle();
        public static GUIStyle sectionOverlayPSVR = new GUIStyle();
        public static GUIStyle sectionOverlayOculus = new GUIStyle();
        public static GUIStyle sectionOverlayGoogle = new GUIStyle();
        public static GUIStyle sectionOverlayWindows = new GUIStyle();

        public static GUIStyle sectionBoxHeader = new GUIStyle();


        public static GUILayoutOption[] buttonList = { GUILayout.ExpandWidth(true) };
        public static Color grayedOut = new Color(0.7f, 0.7f, 0.7f);

        public static Color darkBrown = new Color32(25, 11, 7, 255);
        public static Color lightBrown = new Color32(54, 36, 22, 255);
        public static Color darkBlue = new Color32(31, 52, 71, 255);
        public static Color lightBlue = new Color32(109, 142, 173, 255);
		public static Color lightGreen = new Color32 (150, 220, 150, 255);
        public static Color darken = new Color32(0, 0, 0, 100);
        public static Color lighten = new Color32(255, 255, 255, 50);
        public static Color darkButtonColor = new Color32(25, 25, 25, 255);


        public static Color overlaySteam = new Color32(22, 25, 89, 100);
        public static Color overlayPSVR = new Color32(0, 12, 255, 100);
        public static Color overlayOculus = new Color32(100, 100, 100, 100);
        public static Color overlayGoogle = new Color32(255, 144, 0, 100);
        public static Color overlayWindows = new Color32(0, 168, 255, 255);

        public static Color solidPSVR = new Color32(139, 153, 255, 255);
        public static Color solidSteam = new Color32(139, 210, 255, 255);
        public static Color solidOculus = new Color32(211, 211, 211, 255);
        public static Color solidGoogle = new Color32(255, 218, 139, 255);
        public static Color solidWindows = new Color32(0, 168, 255, 255);

        public static Vector2 controllerMapWindowSize = new Vector2(800, 600);

        static Texture[] platformImages;
        static Texture crossImage;
        static Texture checkImage;

        public static void SetupSharedStyles()
        {
            sectionHeader.fontSize = 20;
            sectionHeader.normal.textColor = Color.white;

            h1.fontSize = 18;
            h1.normal.textColor = Color.white;
            h1.alignment = TextAnchor.MiddleLeft;

            h2.fontSize = 14;
            h2.normal.textColor = Color.white;

            h3.fontSize = 12;
            h3.normal.textColor = Color.white;

            h1centered.fontSize = h1.fontSize;
            h1centered.normal.textColor = h1.normal.textColor;
            h1centered.alignment = TextAnchor.MiddleCenter;

            sectionBox.normal.background = MakeTex(1, 1, darkBlue);
            sectionBox.border = new RectOffset(10, 10, 10, 10);
            sectionBox.padding = new RectOffset(10, 10, 10, 10);

            sdkHeaderBox.normal.background = MakeTex(1, 1, lighten);
            sdkHeaderBox.border = new RectOffset(10, 10, 10, 10);
            sdkHeaderBox.padding = new RectOffset(10, 10, 10, 10);

            sdkSectionBox.normal.background = MakeTex(1, 1, darken);
            sdkSectionBox.border = new RectOffset(10, 10, 0, 10);
            sdkSectionBox.padding = new RectOffset(10, 10, 0, 10);

            sdkMidSectionBox.normal.background = MakeTex(1, 1, darken);
            sdkMidSectionBox.border = new RectOffset(0, 0, 0, 0);
            sdkMidSectionBox.padding = new RectOffset(0, 0, 0, 0);

            focusedSmallToolbarButton.background = MakeTex(1, 1, darkBlue); 
            smallToolbarButton.onFocused = focusedSmallToolbarButton;


            sectionBoxHeader.normal.background = MakeTex(600, 1, lightBlue);

            largeButton.fontSize = 15;
            largeButton.fixedHeight = 20;

            smallButton.fontSize = 10;
            smallButton.fixedHeight = 15;

            tableHeaderRow.padding = new RectOffset(0, 0, 10, 10);
            tableHeaderRow.normal.background = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.2f));
            tableEvenRow.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.025f));
            tableOddRow.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));

            sectionOverlaySteam.normal.background = MakeTex(1, 1, overlaySteam);
            sectionOverlayOculus.normal.background = MakeTex(1, 1, overlayOculus);
            sectionOverlayPSVR.normal.background = MakeTex(1, 1, overlayPSVR);
            sectionOverlayGoogle.normal.background = MakeTex(1, 1, overlayGoogle);
            sectionOverlayWindows.normal.background = MakeTex(1, 1, overlayWindows);

            sectionOverlayGoogle.padding = new RectOffset(0, 0, 15, 20);
            sectionOverlayOculus.padding = new RectOffset(0, 0, 15, 20);
            sectionOverlaySteam.padding = new RectOffset(0, 0, 15, 20);
            sectionOverlayPSVR.padding = new RectOffset(0, 0, 15, 20);
            sectionOverlayWindows.padding = new RectOffset(0, 0, 15, 20);

            platformImages = new Texture[6];
            platformImages[0] = EditorGUIUtility.Load("TButt/Icons/global.png") as Texture2D;
            platformImages[1] = EditorGUIUtility.Load("TButt/Icons/oculus.png") as Texture2D;
            platformImages[2] = EditorGUIUtility.Load("TButt/Icons/steam.png") as Texture2D;
            platformImages[3] = EditorGUIUtility.Load("TButt/Icons/playstation.png") as Texture2D;
            platformImages[4] = EditorGUIUtility.Load("TButt/Icons/google.png") as Texture2D;
            platformImages[5] = EditorGUIUtility.Load("TButt/Icons/windows.png") as Texture2D;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Texture GetPlatformIcon(VRPlatform platform)
        {
            switch(platform)
            {
                case VRPlatform.OculusMobile:
                case VRPlatform.OculusPC:
                    return platformImages[1];
                case VRPlatform.None:
                    return platformImages[0];
                case VRPlatform.Daydream:
                    return platformImages[4];
                case VRPlatform.SteamVR:
                    return platformImages[2];
                case VRPlatform.PlayStationVR:
                    return platformImages[3];
                case VRPlatform.WindowsMR:
                    return platformImages[5];
                default:
                    return null;
            }
        }

        public static void ShowFakeFoldoutBox(ref bool toggle, string title, GUIStyle style)
        {
            if (toggle)
                toggle = EditorGUILayout.Foldout(toggle, "▼ " + title, true, style);
            else
                toggle = EditorGUILayout.Foldout(toggle, "► " + title, true, style);
        }


    }
}