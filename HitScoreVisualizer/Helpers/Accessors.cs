using IPA.Utilities;
using TMPro;
using UnityEngine;

namespace HitScoreVisualizer.Helpers
{
	internal static class Accessors
	{
		internal static readonly FieldAccessor<FlyingObjectEffect, AnimationCurve>.Accessor MoveAnimationCurveAccessor =
			FieldAccessor<FlyingObjectEffect, AnimationCurve>.GetAccessor("_moveAnimationCurve");

		internal static readonly FieldAccessor<FlyingObjectEffect, float>.Accessor ShakeFrequencyAccessor = FieldAccessor<FlyingObjectEffect, float>.GetAccessor("_shakeFrequency");
		internal static readonly FieldAccessor<FlyingObjectEffect, float>.Accessor ShakeStrengthAccessor = FieldAccessor<FlyingObjectEffect, float>.GetAccessor("_shakeStrength");

		internal static readonly FieldAccessor<FlyingObjectEffect, AnimationCurve>.Accessor ShakeStrengthAnimationCurveAccessor =
			FieldAccessor<FlyingObjectEffect, AnimationCurve>.GetAccessor("_shakeStrengthAnimationCurve");

		internal static readonly FieldAccessor<FlyingScoreEffect, TextMeshPro>.Accessor TextAccessor = FieldAccessor<FlyingScoreEffect, TextMeshPro>.GetAccessor("_text");

		internal static readonly FieldAccessor<FlyingScoreEffect, AnimationCurve>.Accessor FadeAnimationCurveAccessor =
			FieldAccessor<FlyingScoreEffect, AnimationCurve>.GetAccessor("_fadeAnimationCurve");

		internal static readonly FieldAccessor<FlyingScoreEffect, SpriteRenderer>.Accessor SpriteRendererAccessor =
			FieldAccessor<FlyingScoreEffect, SpriteRenderer>.GetAccessor("_maxCutDistanceScoreIndicator");
	}
}