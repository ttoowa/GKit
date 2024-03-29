﻿using XInputDotNetPure;

namespace GKit.XInput;

public static class XInput {
    public const int MaxPlayerCount = 4;
    public static float AxisThreshold = 0.05f;

    public static XInputPlayer[] Players { get; }

    public static XInputPlayer FirstPlayer => Players[0];

    static XInput() {
        Players = new XInputPlayer[MaxPlayerCount];

        for (int playerI = 0; playerI < Players.Length; ++playerI) {
            XInputPlayer player = Players[playerI] = new XInputPlayer();

            player.Index = (PlayerIndex)playerI;
        }
    }

    public static void Update() {
        for (int playerI = 0; playerI < Players.Length; ++playerI) {
            XInputPlayer player = Players[playerI];

            player.Update();
        }
    }
}