using HarmonyLib;
using HitScoreVisualizer.Utils;
using UnityEngine;

namespace HitScoreVisualizer.Harmony_Patches
{
	[HarmonyPatch(typeof(FlyingScoreEffect), "InitAndPresent", typeof(NoteCutInfo), typeof(int), typeof(float), typeof(Vector3), typeof(Quaternion), typeof(Color))]
	internal class FlyingScoreEffectInitAndPresent
	{
		private static FlyingScoreEffect _currentEffect = null!;

// ReSharper disable InconsistentNaming
		private static void Prefix(ref Vector3 targetPos, FlyingScoreEffect __instance)
// ReSharper restore InconsistentNaming
		{
			if (Config.instance.useFixedPos)
			{
				var transform = __instance.transform;

				// Set current and target position to the desired fixed position
				transform.position = new Vector3(Config.instance.fixedPosX, Config.instance.fixedPosY, Config.instance.fixedPosZ);
				targetPos = transform.position;

				// If there's an existing judgment effect, clear that first
				if (_currentEffect != null)
				{
					// Remove it gracefully by setting its duration to 0
					_currentEffect.setPrivateField("_duration", 0f);

					// We don't need to clear currentEffect when it disappears, because we'll be setting it to the new effect anyway
					_currentEffect.didFinishEvent -= HandleEffectDidFinish;
				}

				// Save the existing effect to clear if a new one spawns
				_currentEffect = __instance;

				// In case it despawns before the next note is hit, don't try to clear it
				_currentEffect.didFinishEvent += HandleEffectDidFinish;
			}
		}

		private static void HandleEffectDidFinish(FlyingObjectEffect effect)
		{
			effect.didFinishEvent -= HandleEffectDidFinish;
			if (_currentEffect == effect)
			{
				_currentEffect = null!;
			}
		}

// ReSharper disable InconsistentNaming
		private static void Postfix(FlyingScoreEffect __instance, ref Color ____color, NoteCutInfo noteCutInfo)
// ReSharper restore InconsistentNaming
		{
			void Judge(SaberSwingRatingCounter counter)
			{
				ScoreModel.RawScoreWithoutMultiplier(noteCutInfo, out var before, out var after, out var accuracy);
				var total = before + after + accuracy;
				Config.Judge(__instance, noteCutInfo, counter, total, before, after, accuracy);

				// If the counter is finished, remove our event from it
				counter.didFinishEvent -= Judge;
			}

			// Apply judgments a total of twice - once when the effect is created, once when it finishes.
			Judge(noteCutInfo.swingRatingCounter);
			noteCutInfo.swingRatingCounter.didFinishEvent += Judge;
		}
	}
}