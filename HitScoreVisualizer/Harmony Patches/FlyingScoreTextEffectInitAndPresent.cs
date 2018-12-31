using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreTextEffect), "InitAndPresent",
        new Type[] {
            typeof(NoteCutInfo),
            typeof(int),
            typeof(Vector3),
            typeof(Color),
            typeof(SaberAfterCutSwingRatingCounter)})]
    class FlyingScoreTextEffectInitAndPresent
    {
        static void Postfix(SaberAfterCutSwingRatingCounter saberAfterCutSwingRatingCounter, FlyingScoreTextEffect __instance, ref Color ____color, NoteCutInfo noteCutInfo)
        {
            ScoreController.ScoreWithoutMultiplier(noteCutInfo, saberAfterCutSwingRatingCounter, out int before_plus_acc, out int after, out int accuracy);
            int total = before_plus_acc + after;
            Config.judge(__instance, noteCutInfo, saberAfterCutSwingRatingCounter, ref ____color, total, before_plus_acc - accuracy, after, accuracy);
        }
    }
}
