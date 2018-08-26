using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreTextEffect), "HandleSaberAfterCutSwingRatingCounterDidChangeEvent",
        new Type[] { typeof(SaberAfterCutSwingRatingCounter), typeof(float) })]
    class FlyingScoreTextEffectHandleSaberAfterCutSwingRatingCounterDidChangeEvent
    {
        static bool Prefix(SaberAfterCutSwingRatingCounter afterCutRating, FlyingScoreTextEffect __instance, ref Color ____color, NoteCutInfo ____noteCutInfo, int ____multiplier)
        {
            ScoreController.ScoreWithoutMultiplier(____noteCutInfo, afterCutRating, out int before, out int after);
            int total = before + after;
            Config.judge(__instance, ____noteCutInfo, afterCutRating, ref ____color, total);
            return false;
        }
    }
}
