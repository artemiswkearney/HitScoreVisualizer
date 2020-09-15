using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace HitScoreVisualizer.Settings
{
	internal class Judgment
	{
		// This judgment will be applied only to notes hit with score >= this number.
		// Note that if no judgment can be applied to a note, the text will appear as in the unmodded
		// game.

		[SerializedName("threshold")]
		public virtual int Threshold { get; set; } = 0;

		// The text to display (if judgment text is enabled).
		[SerializedName("text")]
		public virtual string Text { get; set; } = string.Empty;

		// 4 floats, 0-1; red, green, blue, glow (not transparency!)
		// leaving this out should look obviously wrong
		[SerializedName("color")]
		[UseConverter(typeof(ListConverter<float>))]
		public virtual List<float> Color { get; set; } = new List<float>(4);

		// If true, the text color will be lerped between this judgment's color and the previous
		// based on how close to the next threshold it is.
		// Specifying fade : true for the first judgment in the array is an error, and will crash the
		// plugin.
		[SerializedName("fade")]
		public virtual bool Fade { get; set; } = false;
	}
}