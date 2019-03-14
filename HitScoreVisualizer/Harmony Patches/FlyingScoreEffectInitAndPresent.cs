using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreEffect), "InitAndPresent",
        new Type[] {
            typeof(NoteCutInfo),
            typeof(int),
            typeof(float),
            typeof(Vector3),
            typeof(Color),
            typeof(SaberAfterCutSwingRatingCounter)})]
    class FlyingScoreEffectInitAndPresent
    {
        static void Postfix(SaberAfterCutSwingRatingCounter saberAfterCutSwingRatingCounter, FlyingScoreEffect __instance, ref Color ____color, NoteCutInfo noteCutInfo)
        {
            ScoreController.ScoreWithoutMultiplier(noteCutInfo, saberAfterCutSwingRatingCounter, out int before_plus_acc, out int after, out int accuracy);
            int total = before_plus_acc + after;
            Config.judge(__instance, noteCutInfo, saberAfterCutSwingRatingCounter, ref ____color, total, before_plus_acc - accuracy, after, accuracy);
        }
    }
}
