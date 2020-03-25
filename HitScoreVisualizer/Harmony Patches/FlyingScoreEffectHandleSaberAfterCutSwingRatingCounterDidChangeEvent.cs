using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreEffect), "HandleSaberSwingRatingCounterDidChangeEvent",
        new Type[] { typeof(SaberSwingRatingCounter), typeof(float) })]
    class FlyingScoreEffectHandleSaberAfterCutSwingRatingCounterDidChangeEvent
    {
        static bool Prefix(SaberSwingRatingCounter saberSwingRatingCounter, FlyingScoreEffect __instance, NoteCutInfo ____noteCutInfo)
        {
            if (Config.instance.doIntermediateUpdates)
            {
                ScoreModel.RawScoreWithoutMultiplier(____noteCutInfo, out int before, out int after, out int accuracy);
                int total = before + after + accuracy;
                Config.judge(__instance, ____noteCutInfo, saberSwingRatingCounter, total, before, after, accuracy);
            }
            return false;
        }
    }
}
