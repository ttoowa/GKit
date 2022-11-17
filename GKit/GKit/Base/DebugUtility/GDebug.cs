using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if OnUnity
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public static class GDebug {
        private static readonly bool onConsole;

        public static bool LogMode { get; set; } = true;

        static GDebug() {
            onConsole = GetConsoleWindow() != IntPtr.Zero;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        public static void Log(object text, GLogLevel logLevel = 0) {
            if (!LogMode) {
                return;
            }

            switch (logLevel) {
                default:
                case GLogLevel.Log:
                    LogPlatform($"GDebug_Log :: {text}");
                    break;
                case GLogLevel.Warnning:
                    LogPlatform($"GDebug_Warning :: {text}");
                    break;
                case GLogLevel.Error:
                    LogPlatform($"GDebug_Error :: {text}");
                    throw new Exception($"GLog_Error :: {text}");
            }
        }

        private static void LogPlatform(string text) {
#if OnUnity
            Debug.Log(
#else
            if (onConsole) {
                Console.WriteLine(text);
            }

            Debug.WriteLine(
#endif
                text);
        }
    }
}