using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_STEAM_VR_2
using Valve.VR;
#endif

namespace TButt.Input
{

    public static class TBSteamVRActions
    {
#if TB_STEAM_VR_2

        private static SteamVR_Action_Single p_tButt_Grip_V1;

        private static SteamVR_Action_Boolean p_tButt_Grip_Button;

        private static SteamVR_Action_Boolean p_tButt_Grip_Touch;

        private static SteamVR_Action_Single p_tButt_MainTrigger_V1;

        private static SteamVR_Action_Boolean p_tButt_MainTrigger_Button;

        private static SteamVR_Action_Boolean p_tButt_MainTrigger_Touch;

        private static SteamVR_Action_Vector2 p_tButt_Joystick1_V2;

        private static SteamVR_Action_Boolean p_tButt_Joystick1_Touch;

        private static SteamVR_Action_Boolean p_tButt_Joystick1_Button;

        private static SteamVR_Action_Vector2 p_tButt_Joystick2_V2;

        private static SteamVR_Action_Boolean p_tButt_Joystick2_Touch;

        private static SteamVR_Action_Boolean p_tButt_Joystick2_Button;

        private static SteamVR_Action_Boolean p_tButt_AX_Button;

        private static SteamVR_Action_Boolean p_tButt_AX_Touch;

        private static SteamVR_Action_Boolean p_tButt_BY_Button;

        private static SteamVR_Action_Boolean p_tButt_BY_Touch;

        private static SteamVR_Action_Boolean p_tButt_Menu_Touch;

        private static SteamVR_Action_Boolean p_tButt_Menu_Button;

        private static SteamVR_Action_Boolean p_tButt_Squeeze_Button;

        private static SteamVR_Action_Single p_tButt_Squeeze_V1;

        private static SteamVR_Action_Pose p_tButt_Pose;

        private static SteamVR_Action_Skeleton p_tButt_SkeletonLeftHand;

        private static SteamVR_Action_Skeleton p_tButt_SkeletonRightHand;

        private static SteamVR_Action_Boolean p_tButt_HeadsetOnHead;

        private static SteamVR_Action_Vibration p_tButt_Haptic;

        public static void Refresh()
        {
            p_tButt_Grip_V1 = SteamVR_Actions.tButt_Grip_V1;
            p_tButt_Grip_Button = SteamVR_Actions.tButt_Grip_Button;
            p_tButt_Grip_Touch = SteamVR_Actions.tButt_Grip_Touch;
            p_tButt_MainTrigger_V1 = SteamVR_Actions.tButt_MainTrigger_V1;
            p_tButt_MainTrigger_Button = SteamVR_Actions.tButt_MainTrigger_Button;
            p_tButt_MainTrigger_Touch = SteamVR_Actions.tButt_MainTrigger_Touch;
            p_tButt_Joystick1_V2 = SteamVR_Actions.tButt_Joystick1_V2;
            p_tButt_Joystick1_Touch = SteamVR_Actions.tButt_Joystick1_Touch;
            p_tButt_Joystick1_Button = SteamVR_Actions.tButt_Joystick1_Button;
            p_tButt_Joystick2_V2 = SteamVR_Actions.tButt_Joystick2_V2;
            p_tButt_Joystick2_Touch = SteamVR_Actions.tButt_Joystick2_Touch;
            p_tButt_Joystick2_Button = SteamVR_Actions.tButt_Joystick2_Button;
            p_tButt_AX_Button = SteamVR_Actions.tButt_AX_Button;
            p_tButt_AX_Touch = SteamVR_Actions.tButt_AX_Touch;
            p_tButt_BY_Button = SteamVR_Actions.tButt_BY_Button;
            p_tButt_BY_Touch = SteamVR_Actions.tButt_BY_Touch;
            p_tButt_Menu_Touch = SteamVR_Actions.tButt_Menu_Touch;
            p_tButt_Menu_Button = SteamVR_Actions.tButt_Menu_Button;
            p_tButt_Pose = SteamVR_Actions.tButt_Pose;
            p_tButt_Squeeze_Button = SteamVR_Actions.tButt_Squeeze_Button;
            p_tButt_Squeeze_V1 = SteamVR_Actions.tButt_Squeeze_V1;
            p_tButt_SkeletonLeftHand = SteamVR_Actions.tButt_SkeletonLeftHand;
            p_tButt_SkeletonRightHand = SteamVR_Actions.tButt_SkeletonRightHand;
            p_tButt_HeadsetOnHead = SteamVR_Actions.tButt_HeadsetOnHead;
            p_tButt_Haptic = SteamVR_Actions.tButt_Haptic;
        }

        public static SteamVR_Action_Single tButt_Grip_V1
        {
            get
            {
                return p_tButt_Grip_V1;
            }
        }

        public static SteamVR_Action_Boolean tButt_Grip_Button
        {
            get
            {
                return p_tButt_Grip_Button;
            }
        }

        public static SteamVR_Action_Boolean tButt_Grip_Touch
        {
            get
            {
                return p_tButt_Grip_Touch;
            }
        }

        public static SteamVR_Action_Single tButt_MainTrigger_V1
        {
            get
            {
                return p_tButt_MainTrigger_V1;
            }
        }

        public static SteamVR_Action_Boolean tButt_MainTrigger_Button
        {
            get
            {
                return p_tButt_MainTrigger_Button;
            }
        }

        public static SteamVR_Action_Boolean tButt_MainTrigger_Touch
        {
            get
            {
                return p_tButt_MainTrigger_Touch;
            }
        }

        public static SteamVR_Action_Vector2 tButt_Joystick1_V2
        {
            get
            {
                return p_tButt_Joystick1_V2;
            }
        }

        public static SteamVR_Action_Boolean tButt_Joystick1_Touch
        {
            get
            {
                return p_tButt_Joystick1_Touch;
            }
        }

        public static SteamVR_Action_Boolean tButt_Joystick1_Button
        {
            get
            {
                return p_tButt_Joystick1_Button;
            }
        }

        public static SteamVR_Action_Vector2 tButt_Joystick2_V2
        {
            get
            {
                return p_tButt_Joystick2_V2;
            }
        }

        public static SteamVR_Action_Boolean tButt_Joystick2_Touch
        {
            get
            {
                return p_tButt_Joystick2_Touch;
            }
        }

        public static SteamVR_Action_Boolean tButt_Joystick2_Button
        {
            get
            {
                return p_tButt_Joystick2_Button;
            }
        }

        public static SteamVR_Action_Boolean tButt_Squeeze_Button
        {
            get
            {
                return p_tButt_Squeeze_Button;
            }
        }

        public static SteamVR_Action_Single tButt_Squeeze_V1
        {
            get
            {
                return p_tButt_Squeeze_V1;
            }
        }

        public static SteamVR_Action_Boolean tButt_AX_Button
        {
            get
            {
                return p_tButt_AX_Button;
            }
        }

        public static SteamVR_Action_Boolean tButt_AX_Touch
        {
            get
            {
                return p_tButt_AX_Touch;
            }
        }

        public static SteamVR_Action_Boolean tButt_BY_Button
        {
            get
            {
                return p_tButt_BY_Button;
            }
        }

        public static SteamVR_Action_Boolean tButt_BY_Touch
        {
            get
            {
                return p_tButt_BY_Touch;
            }
        }

        public static SteamVR_Action_Boolean tButt_Menu_Touch
        {
            get
            {
                return p_tButt_Menu_Touch;
            }
        }

        public static SteamVR_Action_Boolean tButt_Menu_Button
        {
            get
            {
                return p_tButt_Menu_Button;
            }
        }

        public static SteamVR_Action_Pose tButt_Pose
        {
            get
            {
                return p_tButt_Pose;
            }
        }

        public static SteamVR_Action_Skeleton tButt_SkeletonLeftHand
        {
            get
            {
                return p_tButt_SkeletonLeftHand;
            }
        }

        public static SteamVR_Action_Skeleton tButt_SkeletonRightHand
        {
            get
            {
                return p_tButt_SkeletonRightHand;
            }
        }

        public static SteamVR_Action_Boolean tButt_HeadsetOnHead
        {
            get
            {
                return p_tButt_HeadsetOnHead;
            }
        }

        public static SteamVR_Action_Vibration tButt_Haptic
        {
            get
            {
                return p_tButt_Haptic;
            }
        }
#endif
    }
}
