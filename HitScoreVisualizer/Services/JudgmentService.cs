using System.Collections.Generic;
using System.Text;
using HitScoreVisualizer.Extensions;
using HitScoreVisualizer.Settings;
using IPA.Utilities;
using TMPro;
using UnityEngine;

namespace HitScoreVisualizer.Services
{
	internal static class JudgmentService
	{
		private static readonly Judgment DefaultJudgment = new Judgment {Threshold = 0, Text = "", Color = new List<float> {1f, 1f, 1f, 1f}, Fade = false};
		private static readonly JudgmentSegment DefaultSegmentJudgment = new JudgmentSegment {Threshold = 0, Text = ""};

		private static readonly FieldAccessor<FlyingScoreEffect, TextMeshPro>.Accessor FlyingScoreEffectText = FieldAccessor<FlyingScoreEffect, TextMeshPro>.GetAccessor("_text");

		public static void Judge(FlyingScoreEffect scoreEffect, int score, int before, int after, int accuracy)
		{
			// as of 0.13, the TextMeshPro is private; use reflection to grab it out of a private field
			var text = FlyingScoreEffectText(ref scoreEffect);
			// enable rich text
			text.richText = true;
			// disable word wrap, make sure full text displays
			text.enableWordWrapping = false;
			text.overflowMode = TextOverflowModes.Overflow;

			var instance = ConfigProvider.CurrentConfig;


			var judgment = DefaultJudgment;
			int index; // save in case we need to fade
			for (index = 0; index < instance.Judgments.Count; index++)
			{
				var j = instance.Judgments[index];
				if (score >= j.Threshold)
				{
					judgment = j;
					break;
				}
			}

			Color color;
			if (judgment.Fade)
			{
				var fadeJudgment = instance.Judgments[index - 1];
				var baseColor = judgment.Color.ToColor();
				var fadeColor = fadeJudgment.Color.ToColor();
				var lerpDistance = Mathf.InverseLerp(judgment.Threshold, fadeJudgment.Threshold, score);
				color = Color.Lerp(baseColor, fadeColor, lerpDistance);
			}
			else
			{
				color = judgment.Color.ToColor();
			}

			FieldAccessor<FlyingScoreEffect, Color>.Set(scoreEffect, "_color", color);
			scoreEffect.SetField("_color", color);

			text.text = instance.DisplayMode switch
			{
				"format" => DisplayModeFormat(score, before, after, accuracy, judgment, instance),
				"textOnly" => judgment.Text,
				"numeric" => score.ToString(),
				"scoreOnTop" => $"{score}\n{judgment.Text}\n",
				_ => $"{judgment.Text}\n{score}\n"
			};
		}

		private static string DisplayModeFormat(int score, int before, int after, int accuracy, Judgment judgment, Settings.Config instance)
		{
			var formattedBuilder = new StringBuilder();
			var formatString = judgment.Text;
			var nextPercentIndex = formatString.IndexOf('%');
			while (nextPercentIndex != -1)
			{
				formattedBuilder.Append(formatString.Substring(0, nextPercentIndex));
				if (formatString.Length == nextPercentIndex + 1)
				{
					formatString += " ";
				}

				var specifier = formatString[nextPercentIndex + 1];

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
						formattedBuilder.Append(JudgeSegment(before, instance.BeforeCutAngleJudgments));
						break;
					case 'C':
						formattedBuilder.Append(JudgeSegment(accuracy, instance.AccuracyJudgments));
						break;
					case 'A':
						formattedBuilder.Append(JudgeSegment(after, instance.AfterCutAngleJudgments));
						break;
					case 's':
						formattedBuilder.Append(score);
						break;
					case 'p':
						formattedBuilder.Append($"{score / 115d * 100:0}");
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

			return formattedBuilder.Append(formatString).ToString();
		}

		private static string JudgeSegment(int scoreForSegment, IList<JudgmentSegment>? judgments)
		{
			if (judgments == null)
			{
				return string.Empty;
			}

			foreach (var j in judgments)
			{
				if (scoreForSegment >= j.Threshold)
				{
					return j.Text ?? string.Empty;
				}
			}

			return string.Empty;
		}
	}
}