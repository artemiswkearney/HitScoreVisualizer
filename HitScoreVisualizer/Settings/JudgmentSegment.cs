using IPA.Config.Stores.Attributes;

namespace HitScoreVisualizer.Settings
{
	internal class JudgmentSegment
	{
		// This judgment will be applied only when the appropriate part of the swing contributes score >= this number.
		// If no judgment can be applied, the judgment for this segment will be "" (the empty string).
		[SerializedName("threshold")]
		public virtual int Threshold { get; set; } = 0;

		// The text to replace the appropriate judgment specifier with (%B, %C, %A) when this judgment applies.
		[SerializedName("text")]
		public virtual string? Text { get; set; }
	}
}