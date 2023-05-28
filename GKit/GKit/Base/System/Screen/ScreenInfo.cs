using System;
using System.Windows;
#if OnUnity
using UnityEngine;
using Screen = UnityEngine.Screen;
#else
using System.Windows.Forms;
using System.Windows.Media;
using Point = System.Drawing.Point;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
#if OnUnity
    /// <summary>
    ///     화면에 대한 정보를 제공하는 클래스입니다.
    /// </summary>
    public class ScreenInfo {
        private const float SampleOffset = 0.7f;
        public float DPI => Screen.dpi;

        public Vector2 ScreenPixelSize { get; private set; }
        public Vector2 ScreenUnitSize { get; private set; }
        public float Pixel2Unit { get; private set; }
        public float Unit2Pixel { get; private set; }
        public event Action OnSizeChanged;

        private Camera camera;

        public ScreenInfo(GLoopEngine loopEngine, Camera camera) {
            this.camera = camera;

            loopEngine.AddLoopAction(Update);
            UpdateInfo();
        }

        private void Update() {
            InspectSize();
        }

        private void InspectSize() {
            if (ScreenPixelSize.x != Screen.width || ScreenPixelSize.y != Screen.height) {
                //사이즈 변경 감지됨
                UpdateInfo();
                OnSizeChanged?.Invoke();
            }
        }

        public void UpdateInfo() {
            if (ScreenPixelSize.x != Screen.width || ScreenPixelSize.y != Screen.height) {
                //사이즈 변경 감지됨
                ScreenPixelSize = new Vector2(Screen.width, Screen.height);
                float height = 2f * camera.orthographicSize;
                float width = height * camera.aspect;
                ScreenUnitSize = new Vector2(width, height);
                Pixel2Unit = width / (ScreenPixelSize.x + SampleOffset);
                Unit2Pixel = (ScreenPixelSize.x + SampleOffset) / width;

                OnSizeChanged?.Invoke();
            }
        }
    }
#else
    public static class ScreenInfo {
        /// <summary>
        ///     기본 모니터의 화면 크기를 가져옵니다.
        /// </summary>
        public static Vector2 CurrentSize =>
            new((float)SystemParameters.PrimaryScreenWidth, (float)SystemParameters.PrimaryScreenHeight);

        /// <summary>
        ///     모든 모니터를 포함한 화면 전체 크기를 가져옵니다.
        /// </summary>
        public static Vector2Int VirtualSize =>
            new(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

        public static Vector2Int VirtualPosition {
            get {
                Screen[] screens = Screen.AllScreens;
                Vector2Int virtualLocation = new();
                for (int i = 0; i < screens.Length; i++) {
                    if (screens[i].Bounds.Location.X < virtualLocation.x)
                        virtualLocation.x = screens[i].Bounds.Location.X;
                    if (screens[i].Bounds.Location.Y < virtualLocation.y)
                        virtualLocation.y = screens[i].Bounds.Location.Y;
                }

                return virtualLocation;
            }
        }

        public static int MonitorCount => Screen.AllScreens.Length;
        public static Screen MainScreen => Screen.PrimaryScreen;

        public static Screen GetScreen(int index) {
            return Screen.AllScreens[index];
        }

        public static Screen GetScreen(Vector2Int point) {
            return Screen.FromPoint(new Point(point.x, point.y));
        }

        public static float GetUIScale(Visual visual) {
            return (float)PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice.M11;
        }
    }
#endif
}