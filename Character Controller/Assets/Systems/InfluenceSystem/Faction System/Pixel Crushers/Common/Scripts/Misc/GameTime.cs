// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    public enum GameTimeMode
    {
        /// <summary>
        /// Direct mapping to Unity's Time class (e.g., Time.time).
        /// </summary>
        UnityStandard,

        /// <summary>
        /// Realtime, ignoring Time.timeScale. Never pauses.
        /// </summary>
        Realtime,

        /// <summary>
        /// Manually-controlled time. You must set GameTime.time and GameTime.deltaTime.
        /// </summary>
        Manual
    }

    /// <summary>
    /// This is a wrapper around Unity's Time class that allows you to specify a mode:
    /// UnityStandard (Time.time), Realtime (Time.realtimeSinceStartup), or Manual
    /// (you set the time values each frame).
    /// </summary>
    public static class GameTime
    {

        private static GameTimeMode s_mode = GameTimeMode.UnityStandard;
        private static float s_manualTime = 0;
        private static float s_manualDeltaTime = 0;
        private static bool s_manualPaused = false;

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            s_mode = GameTimeMode.UnityStandard;
            s_manualTime = 0;
            s_manualDeltaTime = 0;
            s_manualPaused = false;
        }
#endif

        public static GameTimeMode mode
        {
            get { return s_mode; }
            set { s_mode = value; }
        }

        public static float time
        {
            get
            {
                switch (mode)
                {
                    default:
                    case GameTimeMode.UnityStandard:
                        return Time.time;
                    case GameTimeMode.Realtime:
                        return Time.realtimeSinceStartup;
                    case GameTimeMode.Manual:
                        return s_manualTime;
                }
            }
            set
            {
                s_manualTime = value;
            }
        }

        public static float deltaTime
        {
            get
            {
                switch (mode)
                {
                    default:
                    case GameTimeMode.UnityStandard:
                        return Time.deltaTime;
                    case GameTimeMode.Realtime:
                        return Time.unscaledDeltaTime;
                    case GameTimeMode.Manual:
                        return s_manualDeltaTime;
                }
            }
            set
            {
                s_manualDeltaTime = value;
            }
        }

        public static float timeScale
        {
            get { return Time.timeScale; }
        }

        public static bool isPaused
        {
            get
            {
                switch (mode)
                {
                    default:
                    case GameTimeMode.UnityStandard:
                        return Mathf.Approximately(0, Time.timeScale);
                    case GameTimeMode.Realtime:
                        return false;
                    case GameTimeMode.Manual:
                        return s_manualPaused;
                }
            }
            set
            {
                switch (mode)
                {
                    default:
                    case GameTimeMode.UnityStandard:
                        Time.timeScale = value ? 0 : 1;
                        break;
                    case GameTimeMode.Realtime:
                        break;
                    case GameTimeMode.Manual:
                        s_manualPaused = value;
                        break;
                }
            }
        }

    }

}