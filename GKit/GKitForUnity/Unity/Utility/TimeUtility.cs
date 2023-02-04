using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity.Unity.Utility;

public static class TimeUtility {
    public static float DeltaTimePerTargetFps => Time.deltaTime / Application.targetFrameRate;

    public static float GetDeltaTimePer(float targetFrameRate = 60) {
        return Time.deltaTime / Application.targetFrameRate;
    }
}