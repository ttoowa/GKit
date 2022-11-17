using System;
using XInputDotNetPure;

namespace GKit.XInput;

public class XInputPlayer {
    public PlayerIndex Index { get; internal set; }

    public GamePadState NativeState { get; internal set; }

    //Buttons
    public Button LT { get; }

    public Button RT { get; }

    public float LT_Value { get; private set; }

    public float RT_Value { get; private set; }

    public Button LB { get; }

    public Button RB { get; }

    public Button DPad_Left { get; }

    public Button DPad_Right { get; }

    public Button DPad_Down { get; }

    public Button DPad_Up { get; }

    public Button A { get; }

    public Button B { get; }

    public Button X { get; }

    public Button Y { get; }

    public Button StickL { get; }

    public Button StickR { get; }

    public AxisValue StickL_Value { get; private set; }

    public AxisValue StickR_Value { get; private set; }

    public Button Start { get; }

    public Button Back { get; }

    public Button Guide { get; }

    internal bool IsDisposed;

    public XInputPlayer() {
        LT = new Button();
        RT = new Button();

        LB = new Button();
        RB = new Button();

        DPad_Left = new Button();
        DPad_Right = new Button();
        DPad_Down = new Button();
        DPad_Up = new Button();

        A = new Button();
        B = new Button();
        X = new Button();
        Y = new Button();

        StickL = new Button();
        StickR = new Button();

        Start = new Button();
        Back = new Button();
        Guide = new Button();
    }

    public void SetVibration(float leftMotor, float rightMotor) {
        GamePad.SetVibration(Index, leftMotor, rightMotor);
    }

    internal void Update() {
        GamePadState prevState = NativeState;
        NativeState = GamePad.GetState(Index);

        UpdateButtons();
    }

    private void UpdateButtons() {
        LT.UpdateState(CheckPressureActive(NativeState.Triggers.Left));
        RT.UpdateState(CheckPressureActive(NativeState.Triggers.Right));
        LT_Value = FilterPressureValue(NativeState.Triggers.Left);
        RT_Value = FilterPressureValue(NativeState.Triggers.Right);
        LB.UpdateState(NativeState.Buttons.LeftShoulder == ButtonState.Pressed);
        RB.UpdateState(NativeState.Buttons.RightShoulder == ButtonState.Pressed);

        DPad_Left.UpdateState(NativeState.DPad.Left == ButtonState.Pressed);
        DPad_Right.UpdateState(NativeState.DPad.Right == ButtonState.Pressed);
        DPad_Down.UpdateState(NativeState.DPad.Down == ButtonState.Pressed);
        DPad_Up.UpdateState(NativeState.DPad.Up == ButtonState.Pressed);

        A.UpdateState(NativeState.Buttons.A == ButtonState.Pressed);
        B.UpdateState(NativeState.Buttons.B == ButtonState.Pressed);
        X.UpdateState(NativeState.Buttons.X == ButtonState.Pressed);
        Y.UpdateState(NativeState.Buttons.Y == ButtonState.Pressed);

        StickL.UpdateState(NativeState.Buttons.LeftStick == ButtonState.Pressed);
        StickR.UpdateState(NativeState.Buttons.RightStick == ButtonState.Pressed);

        StickL_Value = new AxisValue(FilterPressureValue(NativeState.ThumbSticks.Left.X), FilterPressureValue(NativeState.ThumbSticks.Left.Y));
        StickR_Value = new AxisValue(FilterPressureValue(NativeState.ThumbSticks.Right.X), FilterPressureValue(NativeState.ThumbSticks.Right.Y));

        Start.UpdateState(NativeState.Buttons.Start == ButtonState.Pressed);
        Back.UpdateState(NativeState.Buttons.Back == ButtonState.Pressed);
        Guide.UpdateState(NativeState.Buttons.Guide == ButtonState.Pressed);
    }

    private bool CheckPressureActive(float value) {
        return Math.Abs(value) > XInput.AxisThreshold;
    }

    private float FilterPressureValue(float value) {
        return CheckPressureActive(value) ? value : 0f;
    }

    public class Button {
        public bool Down { get; internal set; }

        public bool Hold { get; internal set; }

        public bool Up { get; internal set; }

        public event Action OnDown;
        public event Action OnUp;

        public event Action OnDownOnce;
        public event Action OnUpOnce;

        internal Button() {
        }

        internal void ResetState() {
            Down = false;
            Hold = false;
            Up = false;
        }

        internal void UpdateState(bool onHold) {
            Down = false;
            Up = false;
            if (onHold) {
                if (!Hold) {
                    Down = true;
                    OnDownOnce?.Invoke();
                    OnDownOnce = null;
                    OnDown?.Invoke();
                }
            } else {
                if (Hold) {
                    Up = true;

                    OnUpOnce?.Invoke();
                    OnUpOnce = null;
                    OnUp?.Invoke();
                }
            }

            Hold = onHold;
        }
    }

    public struct AxisValue {
        public float X { get; }
        public float Y { get; }

        internal AxisValue(float x, float y) {
            X = x;
            Y = y;
        }
    }
}