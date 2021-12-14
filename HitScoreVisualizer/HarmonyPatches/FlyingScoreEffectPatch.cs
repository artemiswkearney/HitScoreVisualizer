using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HitScoreVisualizer.Helpers;
using HitScoreVisualizer.Models;
using HitScoreVisualizer.Services;
using IPA.Utilities;
using SiraUtil.Affinity;
using TMPro;
using UnityEngine;
using Zenject;

namespace HitScoreVisualizer.HarmonyPatches
{
	// Roughly based on SiraLocalizer' MissedEffectSpawnerSwapper prefix patch
	// Basically "upgrades" the existing FlyingScoreEffect with a custom one
	// https://github.com/Auros/SiraLocalizer/blob/main/SiraLocalizer/HarmonyPatches/MissedEffectSpawnerSwapper.cs

	// Will most likely be rewritten once SiraUtil 3 becomes available
	internal class FlyingScoreEffectPatch : IAffinity
	{
		private static readonly MethodInfo MemoryPoolBinderOriginal = typeof(DiContainer).GetMethods()
			.First(x => x.Name == nameof(DiContainer.BindMemoryPool) && x.IsGenericMethod && x.GetGenericArguments().Length == 2)
			.MakeGenericMethod(typeof(FlyingScoreEffect), typeof(FlyingScoreEffect.Pool));

		private static readonly MethodInfo MemoryPoolBinderReplacement = SymbolExtensions.GetMethodInfo(() => MemoryPoolBinderStub(null!));

		private static readonly MethodInfo WithInitialSizeOriginal = typeof(MemoryPoolInitialSizeMaxSizeBinder<FlyingScoreEffect>)
			.GetMethod(nameof(MemoryPoolInitialSizeMaxSizeBinder<FlyingScoreEffect>.WithInitialSize), new[]
			{
				typeof(int)
			})!;

		private static readonly MethodInfo WithInitialSizeReplacement = SymbolExtensions.GetMethodInfo(() => PoolSizeDefinitionStub(null!, 0));

		private readonly BloomFontProvider _bloomFontProvider;

		public FlyingScoreEffectPatch(BloomFontProvider bloomFontProvider)
		{
			_bloomFontProvider = bloomFontProvider;
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		// ReSharper disable InconsistentNaming
		[AffinityPrefix]
		[AffinityPatch(typeof(EffectPoolsManualInstaller), nameof(EffectPoolsManualInstaller.ManualInstallBindings))]
		internal void Prefix(FlyingScoreEffect ____flyingScoreEffectPrefab)
		{
			var gameObject = ____flyingScoreEffectPrefab.gameObject;

			// we can't destroy original FlyingScoreEffect since it kills the reference given through [SerializeField]
			var flyingScoreEffect = gameObject.GetComponent<FlyingScoreEffect>();
			flyingScoreEffect.enabled = false;

			var text = Accessors.TextAccessor(ref ____flyingScoreEffectPrefab);

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

			// Once the HSV stuff is done, we reconfigure the HSV prefab font.
			_bloomFontProvider.ConfigureFont(ref text);
		}

		[AffinityTranspiler]
		[AffinityPatch(typeof(EffectPoolsManualInstaller), nameof(EffectPoolsManualInstaller.ManualInstallBindings))]
		internal IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return new CodeMatcher(instructions)
				.MatchForward(false,
					new CodeMatch(OpCodes.Ldarg_1), // push DiContainer instance (first method parameter) onto the evaluation stack
					new CodeMatch(OpCodes.Callvirt, MemoryPoolBinderOriginal), // Call method BindMemoryPool<,>()
					new CodeMatch(OpCodes.Ldc_I4_S), // push InitialSize parameter with value X onto the evaluation stack
					new CodeMatch(OpCodes.Callvirt, WithInitialSizeOriginal)) // Call method WithInitialSize(size)
				.Advance(1)
				.SetOperandAndAdvance(MemoryPoolBinderReplacement)
				.Advance(1)
				.SetOperandAndAdvance(WithInitialSizeReplacement)
				.InstructionEnumeration();
		}

		// ReSharper disable once UnusedMethodReturnValue.Local
		private static MemoryPoolIdInitialSizeMaxSizeBinder<HsvFlyingScoreEffect> MemoryPoolBinderStub(DiContainer contract)
		{
			return contract.BindMemoryPool<HsvFlyingScoreEffect, FlyingScoreEffect.Pool>();
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		// ReSharper disable once UnusedMethodReturnValue.Local
		private static MemoryPoolMaxSizeBinder<HsvFlyingScoreEffect> PoolSizeDefinitionStub(MemoryPoolIdInitialSizeMaxSizeBinder<HsvFlyingScoreEffect> contract, int initialSize)
		{
			return contract.WithInitialSize(initialSize);
		}
	}
}