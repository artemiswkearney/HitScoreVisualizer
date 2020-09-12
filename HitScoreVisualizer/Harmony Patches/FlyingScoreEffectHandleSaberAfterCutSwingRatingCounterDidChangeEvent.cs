using HarmonyLib;

namespace HitScoreVisualizer.Harmony_Patches
{
	[HarmonyPatch(typeof(FlyingScoreEffect), "HandleSaberSwingRatingCounterDidChangeEvent", typeof(SaberSwingRatingCounter), typeof(float))]
	internal class FlyingScoreEffectHandleSaberAfterCutSwingRatingCounterDidChangeEvent
	{
// ReSharper disable InconsistentNaming
		private static bool Prefix(SaberSwingRatingCounter saberSwingRatingCounter, FlyingScoreEffect __instance, NoteCutInfo ____noteCutInfo)
// ReSharper enable InconsistentNaming

		{
			if (Config.instance.doIntermediateUpdates)
			{
				ScoreModel.RawScoreWithoutMultiplier(____noteCutInfo, out var before, out var after, out var accuracy);
				var total = before + after + accuracy;
				Config.Judge(__instance, ____noteCutInfo, saberSwingRatingCounter, total, before, after, accuracy);
			}

			return false;
		}
	}
}