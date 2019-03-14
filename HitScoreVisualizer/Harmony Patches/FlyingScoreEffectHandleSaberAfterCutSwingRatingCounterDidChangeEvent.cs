using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreEffect), "HandleSaberAfterCutSwingRatingCounterDidChangeEvent",
        new Type[] { typeof(SaberAfterCutSwingRatingCounter), typeof(float) })]
    class FlyingScoreEffectHandleSaberAfterCutSwingRatingCounterDidChangeEvent
    {
        static bool Prefix(SaberAfterCutSwingRatingCounter saberAfterCutSwingRatingCounter, FlyingScoreEffect __instance, ref Color ____color, NoteCutInfo ____noteCutInfo)
        {
            ScoreController.ScoreWithoutMultiplier(____noteCutInfo, saberAfterCutSwingRatingCounter, out int before_plus_acc, out int after, out int accuracy);
            int total = before_plus_acc + after;
            Config.judge(__instance, ____noteCutInfo, saberAfterCutSwingRatingCounter, ref ____color, total, before_plus_acc - accuracy, after, accuracy);
            return false;
        }
    }
}
