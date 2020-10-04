using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;

namespace HitScoreVisualizer
{
	public class Config
	{
		public static Config instance;

		// If true, this config will not overwrite the existing config file.
		// (This gets set if a config from a newer version is detected.)
		[JsonIgnore]
		public bool noSerialize;

		public struct Judgment
		{
			// This judgment will be applied only to notes hit with score >= this number.
			// Note that if no judgment can be applied to a note, the text will appear as in the unmodded
			// game.
			[DefaultValue(0)]
			public int threshold;

			// The text to display (if judgment text is enabled).
			[DefaultValue("")]
			public string text;

			// 4 floats, 0-1; red, green, blue, glow (not transparency!)
			[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] // leaving this out should look obviously wrong
			public float[] color;

			// If true, the text color will be lerped between this judgment's color and the previous
			// based on how close to the next threshold it is.
			// Specifying fade : true for the first judgment in the array is an error, and will crash the
			// plugin.
			[DefaultValue(false)]
			public bool fade;
		}

		// Judgments for individual parts of the swing (angle before, angle after, accuracy).
		public struct SegmentJudgment
		{
			// This judgment will be applied only when the appropriate part of the swing contributes score >= this number.
			// If no judgment can be applied, the judgment for this segment will be "" (the empty string).
			[DefaultValue(0)]
			public int threshold;

			// The text to replace the appropriate judgment specifier with (%B, %C, %A) when this judgment applies.
			public string text;
		}

		// If the version number (excluding patch version) of the config is higher than that of the plugin,
		// the config will not be loaded. If the version number of the config is lower than that of the
		// plugin, the file will be automatically converted. Conversion is not guaranteed to occur, or be
		// accurate, across major versions.
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int majorVersion;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int minorVersion;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int patchVersion;

		// If this is true, the config will be overwritten with the plugin's default settings after an
		// update rather than being converted.
		public bool isDefaultConfig;

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
		[DefaultValue("")]
		public string displayMode;

		// If enabled, judgments will appear and stay at (fixedPosX, fixedPosY, fixedPosZ) rather than moving as normal.
		// Additionally, the previous judgment will disappear when a new one is created (so there won't be overlap).
		[DefaultValue(false)]
		public bool useFixedPos;

		[DefaultValue(0f)]
		public float fixedPosX;

		[DefaultValue(0f)]
		public float fixedPosY;

		[DefaultValue(0f)]
		public float fixedPosZ;

		// If enabled, judgments will be updated more frequently. This will make score popups more accurate during a brief period before the note's score is finalized, at some cost of performance.
		[DefaultValue(false)]
		public bool doIntermediateUpdates;

		// Order from highest threshold to lowest; the first matching judgment will be applied
		public Judgment[] judgments;

		// Judgments for the part of the swing before cutting the block (score is from 0-70).
		// Format specifier: %B
		public SegmentJudgment[] beforeCutAngleJudgments;

		// Judgments for the accuracy of the cut (how close to the center of the block the cut was, score is from 0-10).
		// Format specifier: %C
		public SegmentJudgment[] accuracyJudgments;

		// Judgments for the part of the swing after cutting the block (score is from 0-30).
		// Format specifier: %A
		public SegmentJudgment[] afterCutAngleJudgments;

		// path to where the config is saved
		private const string FILE_PATH = "/UserData/HitScoreVisualizerConfig.json";

		private const string DEFAULT_JSON = @"{
  ""majorVersion"": 2,
  ""minorVersion"": 4,
  ""patchVersion"": 4,
  ""isDefaultConfig"": true,
  ""displayMode"": ""format"",
  ""judgments"": [
    {
      ""threshold"": 115,
      ""text"": ""%BFantastic%A%n%s"",
      ""color"": [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      ""threshold"": 101,
      ""text"": ""<size=80%>%BExcellent%A</size>%n%s"",
      ""color"": [
        0.0,
        1.0,
        0.0,
        1.0
      ]
},
    {
      ""threshold"": 90,
      ""text"": ""<size=80%>%BGreat%A</size>%n%s"",
      ""color"": [
        1.0,
        0.980392158,
        0.0,
        1.0
      ]
    },
    {
      ""threshold"": 80,
      ""text"": ""<size=80%>%BGood%A</size>%n%s"",
      ""color"": [
        1.0,
        0.6,
        0.0,
        1.0
      ],
      ""fade"": true
    },
    {
      ""threshold"": 60,
      ""text"": ""<size=80%>%BDecent%A</size>%n%s"",
      ""color"": [
        1.0,
        0.0,
        0.0,
        1.0
      ],
      ""fade"": true
    },
    {
      ""text"": ""<size=80%>%BWay Off%A</size>%n%s"",
      ""color"": [
        0.5,
        0.0,
        0.0,
        1.0
      ],
      ""fade"": true
    }
  ],
  ""beforeCutAngleJudgments"": [
    {
      ""threshold"": 70,
      ""text"": ""+""
    },
    {
      ""text"": ""  ""
    }
  ],
  ""accuracyJudgments"": [
    {
      ""threshold"": 15,
      ""text"": ""+""
    },
    {
      ""text"": ""  ""
    }
  ],
  ""afterCutAngleJudgments"": [
    {
      ""threshold"": 30,
      ""text"": ""+""
    },
    {
      ""text"": ""  ""
    }
  ]
}";

		public static readonly Config DEFAULT_CONFIG = JsonConvert.DeserializeObject<Config>(DEFAULT_JSON,
			new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate});

		public static readonly Judgment DEFAULT_JUDGMENT = new Judgment {threshold = 0, text = "", color = new float[] {1f, 1f, 1f, 1f}, fade = false};

		public static readonly SegmentJudgment DEFAULT_SEGMENT_JUDGMENT = new SegmentJudgment {threshold = 0, text = ""};

		public static string FullPath => Environment.CurrentDirectory.Replace('\\', '/') + FILE_PATH;

		public static void Load()
		{
			Plugin.LoggerInstance.Info("Loading config...");
			if (!File.Exists(FullPath))
			{
				Plugin.LoggerInstance.Info("Writing default config.");
				// if the config file doesn't exist, save the default one
				ResetToDefault();
				Save(true);
				return;
			}

			var configJson = File.ReadAllText(FullPath);
			var loaded = JsonConvert.DeserializeObject<Config>(configJson,
				new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate});
			if (!Validate(loaded))
			{
				Plugin.LoggerInstance.Info("Falling back to default config (file will not be overwritten)");
				// don't try to modify the original default when disabling serialization
				instance = DEFAULT_CONFIG.MemberwiseClone() as Config;
				// since we couldn't read the existing config, don't overwrite it
				instance.noSerialize = true;
				return;
			}

			if (Outdated(loaded))
			{
				if (loaded.isDefaultConfig)
				{
					loaded = DEFAULT_CONFIG;
					instance = loaded;
					Save();
					return;
				}

				// put config update logic here
				var isDirty = false; // set to true if you modify the config
				if (loaded.majorVersion == 2 && loaded.minorVersion == 0)
				{
					loaded.beforeCutAngleJudgments = new SegmentJudgment[] {DEFAULT_SEGMENT_JUDGMENT};
					loaded.accuracyJudgments = new SegmentJudgment[] {DEFAULT_SEGMENT_JUDGMENT};
					loaded.afterCutAngleJudgments = new SegmentJudgment[] {DEFAULT_SEGMENT_JUDGMENT};
					loaded.minorVersion = 1;
					loaded.patchVersion = 0;
					isDirty = true;
				}

				if (loaded.majorVersion == 2 && loaded.minorVersion == 1)
				{
					// Beat Saber version 1.0.0 increased the max score given for cut accuracy from 10 to 15, and thus the max total score from 110 to 115.
					// We assume that anyone whose config contained a judgment requiring an accuracy score of 10 or a total score of 110 intended those values
					// to refer to the highest achievable score rather than those exact numbers, and thus update them accordingly.
					// As we can't know what users would want done with their other judgment thresholds, those are left unchanged.
					if (loaded.judgments != null)
					{
						for (var i = 0; i < loaded.judgments.Length; i++)
						{
							if (loaded.judgments[i].threshold == 110)
							{
								loaded.judgments[i].threshold = 115;
							}
						}
					}

					if (loaded.accuracyJudgments != null)
					{
						for (var i = 0; i < loaded.accuracyJudgments.Length; i++)
						{
							if (loaded.accuracyJudgments[i].threshold == 10)
							{
								loaded.accuracyJudgments[i].threshold = 15;
							}
						}
					}

					loaded.minorVersion = 2;
					loaded.patchVersion = 0;
					isDirty = true;
				}

				if (loaded.majorVersion == 2 && loaded.minorVersion == 2 || loaded.minorVersion == 3)
				{
					// Leaving this on to preserve identical behavior to previous versions.
					// However, since the option is non-default, a line for it will be generated in the config.
					loaded.doIntermediateUpdates = true;

					loaded.minorVersion = 4;
					loaded.patchVersion = 1;
					isDirty = true;
				}

				if (loaded.majorVersion == 2 && loaded.minorVersion == 4 && loaded.patchVersion < 4)
				{
					loaded.patchVersion = 4;
					isDirty = true;
				}

				instance = loaded;
				if (isDirty)
				{
					Save();
				}
			}

			instance = loaded;
		}

		public static void Save(bool force = false)
		{
			Plugin.LoggerInstance.Info("Writing file...");
			if (instance.noSerialize && !force)
			{
				return;
			}

			File.WriteAllText(FullPath, JsonConvert.SerializeObject(instance,
				Formatting.Indented,
				new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate}));
			Plugin.LoggerInstance.Info("File written.");
		}

		public static bool Validate(Config config)
		{
			if (TooNew(config))
			{
				Plugin.LoggerInstance.Info("Config is for a newer version of HitScoreVisualizer!");
				return false;
			}

			var judgmentsValid = true;
			foreach (var j in config.judgments)
			{
				if (!ValidateJudgment(j))
				{
					judgmentsValid = false;
				}
			}

			if (!judgmentsValid)
			{
				return false;
			}

			return true;
		}

		public static bool ValidateJudgment(Judgment judgment)
		{
			if (judgment.color.Length != 4)
			{
				Plugin.LoggerInstance.Warn($"Judgment \"{judgment.text}\" with threshold {judgment.threshold} has invalid color!");
				Plugin.LoggerInstance.Warn("Make sure to include exactly 4 numbers for each judgment's color!");
				return false;
			}

			return true;
		}

		public static bool Outdated(Config config)
		{
			if (config.majorVersion < Plugin.Version.Major)
			{
				return true;
			}

			return config.minorVersion < Plugin.Version.Minor;
		}

		public static bool TooNew(Config config)
		{
			if (config.majorVersion > Plugin.Version.Major)
			{
				return true;
			}

			return config.minorVersion > Plugin.Version.Minor;
		}

		public static void ResetToDefault()
		{
			instance = DEFAULT_CONFIG;
		}




	}
}