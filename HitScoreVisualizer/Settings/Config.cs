using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SemVer;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitScoreVisualizer.Settings
{
	internal class Config
	{
		// If the version number (excluding patch version) of the config is higher than that of the plugin,
		// the config will not be loaded. If the version number of the config is lower than that of the
		// plugin, the file will be automatically converted. Conversion is not guaranteed to occur, or be

		// accurate, across major versions.
		[SerializedName("majorVersion")]
		public virtual int MajorVersion { get; set; }

		[SerializedName("minorVersion")]
		public virtual int MinorVersion { get; set; }

		[SerializedName("patchVersion")]
		public virtual int PatchVersion { get; set; }

		[Ignore]
		internal Version Version => new Version(MajorVersion, MinorVersion, PatchVersion);

		// If this is true, the config will be overwritten with the plugin's default settings after an
		// update rather than being converted.
		[SerializedName("isDefaultConfig")]
		public virtual bool IsDefaultConfig { get; set; }

		// If set to "format", displays the judgment text, with the following format specifiers allowed:
		// - %b: The score contributed by the part of the swing before cutting the block.
		// - %c: The score contributed by the accuracy of the cut.
		// - %a: The score contributed by the part of the swing after cutting the block.
		// - %B, %C, %A: As above, except using the appropriate judgment from that part of the swing (as configured for "beforeCutAngleJudgments", "accuracyJudgments", or "afterCutAngleJudgments").
		// - %s: The total score for the cut.
		// - %p: The percent out of 115 you achieved with your swing's score
		// - %%: A literal percent symbol.
		// - %n: A newline.
		//
		// If set to "numeric", displays only the note score.
		// If set to "textOnly", displays only the judgment text.
		// If set to "scoreOnTop", displays both (numeric score above judgment text).
		// Otherwise, displays both (judgment text above numeric score).
		[SerializedName("displayMode")]
		public virtual string DisplayMode { get; set; } = string.Empty;

		// If enabled, judgments will appear and stay at (fixedPosX, fixedPosY, fixedPosZ) rather than moving as normal.
		// Additionally, the previous judgment will disappear when a new one is created (so there won't be overlap).
		[SerializedName("useFixedPos")]
		public virtual bool UseFixedPos { get; set; }

		[SerializedName("fixedPosX")]
		public virtual float FixedPosX { get; set; } = 0f;

		[SerializedName("fixedPosY")]
		public virtual float FixedPosY { get; set; } = 0f;

		[SerializedName("fixedPosZ")]
		public virtual float FixedPosZ { get; set; } = 0f;

		[Ignore]
		internal Vector3 FixedPos => new Vector3(FixedPosX, FixedPosY, FixedPosZ);

		// If enabled, judgments will be updated more frequently. This will make score popups more accurate during a brief period before the note's score is finalized, at some cost of performance.
		[SerializedName("doIntermediateUpdates")]
		public virtual bool DoIntermediateUpdates { get; set; }

		// Order from highest threshold to lowest; the first matching judgment will be applied
		[NonNullable]
		[SerializedName("judgments")]
		[UseConverter(typeof(ListConverter<Judgment>))]
		public virtual List<Judgment> Judgments { get; set; } = new List<Judgment>();

		// Judgments for the part of the swing before cutting the block (score is from 0-70).
		// Format specifier: %B
		[SerializedName("beforeCutAngleJudgments")]
		[UseConverter(typeof(ListConverter<JudgmentSegment>))]
		public virtual List<JudgmentSegment> BeforeCutAngleJudgments { get; set; } = new List<JudgmentSegment>();

		// Judgments for the accuracy of the cut (how close to the center of the block the cut was, score is from 0-10).
		// Format specifier: %C
		[UseConverter]
		[SerializedName("accuracyJudgments")]
		public virtual List<JudgmentSegment> AccuracyJudgments { get; set; } = new List<JudgmentSegment>();

		// Judgments for the part of the swing after cutting the block (score is from 0-30).
		// Format specifier: %A
		[UseConverter]
		[SerializedName("afterCutAngleJudgments")]
		public virtual List<JudgmentSegment> AfterCutAngleJudgments { get; set; } = new List<JudgmentSegment>();
	}
}