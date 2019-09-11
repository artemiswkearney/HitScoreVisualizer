using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static HitScoreVisualizer.Utils.ReflectionUtil;

namespace HitScoreVisualizer.Harmony_Patches
{
    [HarmonyPatch(typeof(FlyingScoreEffect), "InitAndPresent",
        new Type[] {
            typeof(NoteCutInfo),
            typeof(int),
            typeof(float),
            typeof(Vector3),
            typeof(Color)})]
    class FlyingScoreEffectInitAndPresent
    {
        public static FlyingScoreEffect currentEffect = null;
        static void Prefix(ref Vector3 targetPos, FlyingScoreEffect __instance)
        {
            if (Config.instance.useFixedPos)
            {
                // Set current and target position to the desired fixed position
                __instance.transform.position = new Vector3(Config.instance.fixedPosX, Config.instance.fixedPosY, Config.instance.fixedPosZ);
                targetPos = __instance.transform.position;
                // If there's an existing judgment effect, clear that first
                if (currentEffect != null)
                {
                    // Remove it gracefully by setting its duration to 0
                    currentEffect.setPrivateFieldBase("_duration", 0f);
                    // We don't need to clear currentEffect when it disappears, because we'll be setting it to the new effect anyway
                    currentEffect.didFinishEvent -= handleEffectDidFinish;
                }
                // Save the existing effect to clear if a new one spawns
                currentEffect = __instance;
                // In case it despawns before the next note is hit, don't try to clear it
                currentEffect.didFinishEvent -= handleEffectDidFinish; // Just in case
                currentEffect.didFinishEvent += handleEffectDidFinish;
            }
        }

        static void handleEffectDidFinish(FlyingObjectEffect effect)
        {
            effect.didFinishEvent -= handleEffectDidFinish;
            if (currentEffect == effect) currentEffect = null;
        }

        static void Postfix(FlyingScoreEffect __instance, ref Color ____color, NoteCutInfo noteCutInfo)
        {
            void judge(SaberSwingRatingCounter counter)
            {
                ScoreController.RawScoreWithoutMultiplier(noteCutInfo, out int before, out int after, out int accuracy);
                int total = before + after + accuracy;
                Config.judge(__instance, total, before, after, accuracy);

                // If the counter is finished, remove our event from it
                counter.didFinishEvent -= judge;
            }

            // Apply judgments a total of twice - once when the effect is created, once when it finishes.
            judge(noteCutInfo.swingRatingCounter);
            noteCutInfo.swingRatingCounter.didFinishEvent -= judge; // Just in case
            noteCutInfo.swingRatingCounter.didFinishEvent += judge;
        }
    }
}
