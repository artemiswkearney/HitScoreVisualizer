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
            ScoreController.ScoreWithoutMultiplier(____noteCutInfo, afterCutRating, out int before_plus_acc, out int after, out int accuracy);
            int total = before_plus_acc + after;
            Config.judge(__instance, ____noteCutInfo, afterCutRating, ref ____color, total, before_plus_acc - accuracy, after, accuracy);
            return false;
        }
    }
}
