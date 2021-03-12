using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HitScoreVisualizer.Helpers;
using HitScoreVisualizer.Models;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using Zenject;

namespace HitScoreVisualizer.Harmony_Patches
{
	// Roughly based on SiraLocalizer' MissedEffectSpawnerSwapper prefix patch
	// Basically "upgrades" the existing FlyingScoreEffect with a custom one
	// https://github.com/Auros/SiraLocalizer/blob/main/SiraLocalizer/HarmonyPatches/MissedEffectSpawnerSwapper.cs
	[HarmonyPatch(typeof(EffectPoolsManualInstaller), nameof(EffectPoolsManualInstaller.ManualInstallBindings))]
	internal class FlyingScoreEffectPatch
	{
		private static readonly MethodInfo MemoryPoolBinderReplacement = SymbolExtensions.GetMethodInfo(() => MemoryPoolBinderStub(null!));

		private static readonly MethodInfo WithInitialSizeReplacement = SymbolExtensions.GetMethodInfo(() => WithInitialSizeStub(null!, 0));

// ReSharper disable once SuggestBaseTypeForParameter
// ReSharper disable InconsistentNaming
		[HarmonyPrefix]
		internal static void Prefix(FlyingScoreEffect ____flyingScoreEffectPrefab)
// ReSharper disable InconsistentNaming
		{
			var gameObject = ____flyingScoreEffectPrefab.gameObject;

			// we can't destroy original FlyingScoreEffect since it kills the reference given through [SerializeField]
			var flyingScoreEffect = gameObject.GetComponent<FlyingScoreEffect>();
			flyingScoreEffect.enabled = false;

			var hsvScoreEffect = gameObject.GetComponent<HsvFlyingScoreEffect>();

			if (!hsvScoreEffect)
			{
				var hsvFlyingScoreEffect = gameObject.AddComponent<HsvFlyingScoreEffect>();

				// Serialized fields aren't filled in correctly in our own custom override, so copying over the values using FieldAccessors
				var flyingObjectEffect = (FlyingObjectEffect) flyingScoreEffect;
				FieldAccessor<FlyingObjectEffect, AnimationCurve>.Set(hsvFlyingScoreEffect, "_moveAnimationCurve", Accessors.MoveAnimationCurveAccessor(ref flyingObjectEffect));
				FieldAccessor<FlyingObjectEffect, float>.Set(hsvFlyingScoreEffect, "_shakeFrequency", Accessors.ShakeFrequencyAccessor(ref flyingObjectEffect));
				FieldAccessor<FlyingObjectEffect, float>.Set(hsvFlyingScoreEffect, "_shakeStrength", Accessors.ShakeStrengthAccessor(ref flyingObjectEffect));
				FieldAccessor<FlyingObjectEffect, AnimationCurve>.Set(hsvFlyingScoreEffect, "_shakeStrengthAnimationCurve", Accessors.ShakeStrengthAnimationCurveAccessor(ref flyingObjectEffect));


				FieldAccessor<FlyingScoreEffect, TextMeshPro>.Set(hsvFlyingScoreEffect, "_text", Accessors.TextAccessor(ref flyingScoreEffect));
				FieldAccessor<FlyingScoreEffect, AnimationCurve>.Set(hsvFlyingScoreEffect, "_fadeAnimationCurve", Accessors.FadeAnimationCurveAccessor(ref flyingScoreEffect));
				FieldAccessor<FlyingScoreEffect, SpriteRenderer>.Set(hsvFlyingScoreEffect, "_maxCutDistanceScoreIndicator", Accessors.SpriteRendererAccessor(ref flyingScoreEffect));
			}
		}

		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();

			codes[9] = new CodeInstruction(OpCodes.Callvirt, MemoryPoolBinderReplacement);
			codes[11] = new CodeInstruction(OpCodes.Callvirt, WithInitialSizeReplacement);

			return codes.AsEnumerable();
		}

		// ReSharper disable once UnusedMethodReturnValue.Local
		private static MemoryPoolIdInitialSizeMaxSizeBinder<HsvFlyingScoreEffect> MemoryPoolBinderStub(DiContainer contract)
		{
			return contract.BindMemoryPool<HsvFlyingScoreEffect, FlyingScoreEffect.Pool>();
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		// ReSharper disable once UnusedMethodReturnValue.Local
		private static MemoryPoolMaxSizeBinder<HsvFlyingScoreEffect> WithInitialSizeStub(MemoryPoolIdInitialSizeMaxSizeBinder<HsvFlyingScoreEffect> contract, int size)
		{
			return contract.WithInitialSize(size);
		}
	}
}