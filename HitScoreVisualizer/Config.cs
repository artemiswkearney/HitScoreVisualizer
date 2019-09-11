using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

using static HitScoreVisualizer.Utils.ReflectionUtil;

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
  ""patchVersion"": 0,
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
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate });

        public static readonly Judgment DEFAULT_JUDGMENT = new Judgment
        {
            threshold = 0,
            text = "",
            color = new float[] { 1f, 1f, 1f, 1f },
            fade = false
        };

        public static readonly SegmentJudgment DEFAULT_SEGMENT_JUDGMENT = new SegmentJudgment
        {
            threshold = 0,
            text = ""
        };

        public static string fullPath => Environment.CurrentDirectory.Replace('\\', '/') + FILE_PATH;

        public static void load()
        {
            Plugin.log("Loading config...");
            if (!File.Exists(fullPath))
            {
                Plugin.log("Writing default config.");
                // if the config file doesn't exist, save the default one
                resetToDefault();
                save(true);
                return;
            }
            string configJson = File.ReadAllText(fullPath);
            Config loaded = JsonConvert.DeserializeObject<Config>(configJson,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate });
            if (!validate(loaded))
            {
                Plugin.log("Falling back to default config (file will not be overwritten)");
                // don't try to modify the original default when disabling serialization
                instance = DEFAULT_CONFIG.MemberwiseClone() as Config;
                // since we couldn't read the existing config, don't overwrite it
                instance.noSerialize = true;
                return;
            }
            if (outdated(loaded))
            {
                if (loaded.isDefaultConfig)
                {
                    loaded = DEFAULT_CONFIG;
                    instance = loaded;
                    save();
                    return;
                }
                // put config update logic here
                bool isDirty = false; // set to true if you modify the config
                if (loaded.majorVersion == 2 && loaded.minorVersion == 0)
                {
                    loaded.beforeCutAngleJudgments = new SegmentJudgment[] { DEFAULT_SEGMENT_JUDGMENT };
                    loaded.accuracyJudgments = new SegmentJudgment[] { DEFAULT_SEGMENT_JUDGMENT };
                    loaded.afterCutAngleJudgments = new SegmentJudgment[] { DEFAULT_SEGMENT_JUDGMENT };
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
                        for (int i = 0; i < loaded.judgments.Length; i++)
                        {
                            if (loaded.judgments[i].threshold == 110)
                            {
                                loaded.judgments[i].threshold = 115;
                            }
                        }
                    }
                    if (loaded.accuracyJudgments != null)
                    {
                        for (int i = 0; i < loaded.accuracyJudgments.Length; i++)
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
                    loaded.patchVersion = 0;
                    isDirty = true;
                }
                instance = loaded;
                if (isDirty) save();
            }
            instance = loaded;
        }

        public static void save(bool force = false)
        {
            Plugin.log("Writing file...");
            if (instance.noSerialize && !force) return;
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(instance,
                Formatting.Indented,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate }));
            Plugin.log("File written.");
        }

        public static bool validate(Config config)
        {
            if (tooNew(config))
            {
                Plugin.log("Config is for a newer version of HitScoreVisualizer!");
                return false;
            }
            bool judgmentsValid = true;
            foreach (Judgment j in config.judgments)
            {
                if (!validateJudgment(j)) judgmentsValid = false;
            }
            if (!judgmentsValid) return false;
            return true;
        }

        public static bool validateJudgment(Judgment judgment)
        {
            if (judgment.color.Length != 4)
            {
                Console.WriteLine("Judgment \"" + judgment.text + "\" with threshold " + judgment.threshold +
                    "has invalid color!");
                Console.WriteLine("Make sure to include exactly 4 numbers for each judgment's color!");
                return false;
            }
            return true;
        }

        public static bool outdated(Config config)
        {
            if (config.majorVersion < Plugin.majorVersion) return true;
            if (config.minorVersion < Plugin.minorVersion) return true;
            return false;
        }

        public static bool tooNew(Config config)
        {
            if (config.majorVersion > Plugin.majorVersion) return true;
            if (config.minorVersion > Plugin.minorVersion) return true;
            return false;
        }

        public static void resetToDefault()
        {
            instance = DEFAULT_CONFIG;
        }

        public static void judge(FlyingScoreEffect scoreEffect, int score, int before, int after, int accuracy)
        {
            // as of 0.13, the TextMeshPro is private; use reflection to grab it out of a private field
            TextMeshPro text = scoreEffect.getPrivateField<TextMeshPro>("_text");
            // enable rich text
            text.richText = true;
            // disable word wrap, make sure full text displays
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;


            Judgment judgment = DEFAULT_JUDGMENT;
            int index; // save in case we need to fade
            for (index = 0; index < instance.judgments.Length; index++)
            {
                Judgment j = instance.judgments[index];
                if (score >= j.threshold)
                {
                    judgment = j;
                    break;
                }
            }

            Color color;
            if (judgment.fade)
            {
                Judgment fadeJudgment = instance.judgments[index - 1];
                Color baseColor = toColor(judgment.color);
                Color fadeColor = toColor(fadeJudgment.color);
                float lerpDistance = Mathf.InverseLerp(judgment.threshold, fadeJudgment.threshold, score);
                color = Color.Lerp(baseColor, fadeColor, lerpDistance);
            }
            else
            {
                color = toColor(judgment.color);
            }
            scoreEffect.setPrivateField("_color", color);

            if (instance.displayMode == "format")
            {
                StringBuilder formattedBuilder = new StringBuilder();
                string formatString = judgment.text;
                int nextPercentIndex = formatString.IndexOf('%');
                while (nextPercentIndex != -1)
                {
                    formattedBuilder.Append(formatString.Substring(0, nextPercentIndex));
                    if (formatString.Length == nextPercentIndex + 1)
                    {
                        formatString += " ";
                    }
                    char specifier = formatString[nextPercentIndex + 1];

                    switch (specifier)
                    {
                        case 'b':
                            formattedBuilder.Append(before);
                            break;
                        case 'c':
                            formattedBuilder.Append(accuracy);
                            break;
                        case 'a':
                            formattedBuilder.Append(after);
                            break;
                        case 'B':
                            formattedBuilder.Append(judgeSegment(before, instance.beforeCutAngleJudgments));
                            break;
                        case 'C':
                            formattedBuilder.Append(judgeSegment(accuracy, instance.accuracyJudgments));
                            break;
                        case 'A':
                            formattedBuilder.Append(judgeSegment(after, instance.afterCutAngleJudgments));
                            break;
                        case 's':
                            formattedBuilder.Append(score);
                            break;
                        case 'p':
                            formattedBuilder.Append(string.Format("{0:0}", score / 115d * 100));
                            break;
                        case '%':
                            formattedBuilder.Append("%");
                            break;
                        case 'n':
                            formattedBuilder.Append("\n");
                            break;
                        default:
                            formattedBuilder.Append("%" + specifier);
                            break;
                    }

                    formatString = formatString.Remove(0, nextPercentIndex + 2);
                    nextPercentIndex = formatString.IndexOf('%');
                }
                formattedBuilder.Append(formatString);

                text.text = formattedBuilder.ToString();
                return;
            }

            if (instance.displayMode == "textOnly")
            {
                text.text = judgment.text;
                return;
            }
            if (instance.displayMode == "numeric")
            {
                text.text = score.ToString();
                return;
            }
            if (instance.displayMode == "scoreOnTop")
            {
                text.text = score + "\n" + judgment.text + "\n";
                return;
            }
            text.text = judgment.text + "\n" + score + "\n";
        }

        public static Color toColor(float[] rgba)
        {
            return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
        }

        public static string judgeSegment(int scoreForSegment, SegmentJudgment[] judgments)
        {
            if (judgments == null) return "";
            foreach(SegmentJudgment j in judgments)
            {
                if (scoreForSegment >= j.threshold) return j.text;
            }
            return "";
        }
    }
}
