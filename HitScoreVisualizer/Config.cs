using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HitScoreVisualizer
{
    class Config
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

        // If set to "numeric", displays only the note score.
        // If set to "textOnly", displays only the judgment text.
        // If set to "scoreOnTop", displays both (numeric score above judgment text).
        // Otherwise, displays both (judgment text above numeric score).
        [DefaultValue("")]
        public string displayMode;

        // Order from highest threshold to lowest; the first matching judgment will be applied
        public Judgment[] judgments;

        // path to where the config is saved
        private const string FILE_PATH = "/UserData/HitScoreVisualizerConfig.json";

        private const string DEFAULT_JSON = @"{
  ""majorVersion"": 2,
  ""minorVersion"": 0,
  ""patchVersion"": 2,
  ""isDefaultConfig"": true,
  ""displayMode"": ""textOnTop"",
  ""judgments"": [
    {
      ""threshold"": 110,
      ""text"": ""Fantastic"",
      ""color"": [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      ""threshold"": 101,
      ""text"": ""<size=80%>Excellent</size>"",
      ""color"": [
        0.0,
        1.0,
        0.0,
        1.0
      ]
},
    {
      ""threshold"": 90,
      ""text"": ""<size=80%>Great</size>"",
      ""color"": [
        1.0,
        0.980392158,
        0.0,
        1.0
      ]
    },
    {
      ""threshold"": 80,
      ""text"": ""<size=80%>Good</size>"",
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
      ""text"": ""<size=80%>Decent</size>"",
      ""color"": [
        1.0,
        0.0,
        0.0,
        1.0
      ],
      ""fade"": true
    },
    {
      ""text"": ""<size=80%>Way Off</size>"",
      ""color"": [
        0.5,
        0.0,
        0.0,
        1.0
      ],
      ""fade"": true
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
                // put config update logic here
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

        public static void judge(FlyingScoreTextEffect text, ref Color color, int score)
        {
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

            if (instance.displayMode == "textOnly")
            {
                text.text = judgment.text;
                return;
            }
            if (instance.displayMode == "numeric")
            {
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
    }
}
