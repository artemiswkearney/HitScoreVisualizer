using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using Newtonsoft.Json;
using UnityEngine;
using Version = SemVer.Version;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitScoreVisualizer.Settings
{
	internal class Config
	{
		// If the version number (excluding patch version) of the config is higher than that of the plugin,
		// the config will not be loaded. If the version number of the config is lower than that of the
		// plugin, the file will be automatically converted. Conversion is not guaranteed to occur, or be

		// accurate, across major versions.
		[NonNullable]
		[JsonProperty("majorVersion")]
		public virtual int MajorVersion { get; set; } = Plugin.Version.Major;

		[NonNullable]
		[JsonProperty("minorVersion")]
		public virtual int MinorVersion { get; set; } = Plugin.Version.Minor;

		[NonNullable]
		[JsonProperty("patchVersion")]
		public virtual int PatchVersion { get; set; } = Plugin.Version.Patch;

		[Ignore]
		[JsonIgnore]
		internal Version Version => new Version(MajorVersion, MinorVersion, PatchVersion);

		// If this is true, the config will be overwritten with the plugin' default settings after an
		// update rather than being converted.
		[JsonProperty("isDefaultConfig")]
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
		[NonNullable]
		[JsonProperty("displayMode")]
		public virtual string DisplayMode { get; set; } = string.Empty;

		// If enabled, judgments will appear and stay at (fixedPosX, fixedPosY, fixedPosZ) rather than moving as normal.
		// Additionally, the previous judgment will disappear when a new one is created (so there won't be overlap).
		[NonNullable]
		[JsonProperty("useFixedPos")]
		public virtual bool UseFixedPos { get; set; }

		[NonNullable]
		[JsonProperty("fixedPosX")]
		public virtual float? FixedPosX { get; set; } = 0f;

		[NonNullable]
		[JsonProperty("fixedPosY")]
		public virtual float? FixedPosY { get; set; } = 0f;

		[NonNullable]
		[JsonProperty("fixedPosZ")]
		public virtual float? FixedPosZ { get; set; } = 0f;

		// Only call this when this was validated beforehand
		[Ignore]
		[JsonIgnore]
		internal Vector3 FixedPos => new Vector3(FixedPosX!.Value, FixedPosY!.Value, FixedPosZ!.Value);

		// If enabled, judgments will be updated more frequently. This will make score popups more accurate during a brief period before the note's score is finalized, at some cost of performance.
		[NonNullable]
		[JsonProperty("doIntermediateUpdates")]
		public virtual bool DoIntermediateUpdates { get; set; }

		// Order from highest threshold to lowest; the first matching judgment will be applied
		[NonNullable]
		[JsonProperty("judgments")]
		[UseConverter(typeof(ListConverter<Judgment>))]
		public virtual List<Judgment> Judgments { get; set; }

		// Judgments for the part of the swing before cutting the block (score is from 0-70).
		// Format specifier: %B
		[JsonProperty("beforeCutAngleJudgments")]
		[UseConverter(typeof(ListConverter<JudgmentSegment>))]
		public virtual List<JudgmentSegment> BeforeCutAngleJudgments { get; set; }

		// Judgments for the accuracy of the cut (how close to the center of the block the cut was, score is from 0-10).
		// Format specifier: %C
		[UseConverter]
		[JsonProperty("accuracyJudgments")]
		public virtual List<JudgmentSegment> AccuracyJudgments { get; set; }

		// Judgments for the part of the swing after cutting the block (score is from 0-30).
		// Format specifier: %A
		[UseConverter]
		[JsonProperty("afterCutAngleJudgments")]
		public virtual List<JudgmentSegment> AfterCutAngleJudgments { get; set; }

		public virtual void Changed()
		{
			// this is called whenever one of the virtual properties is changed
			// can be called to signal that the content has been changed
		}

		public virtual IDisposable ChangeTransaction => null!;
	}
}