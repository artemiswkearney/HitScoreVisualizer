using HitScoreVisualizer.Services;
using HitScoreVisualizer.Settings;
using UnityEngine;
using Zenject;

namespace HitScoreVisualizer.Models
{
	internal class HsvFlyingScoreEffect : FlyingScoreEffect
	{
		private JudgmentService _judgmentService = null!;
		private Configuration? _configuration;

		private NoteCutInfo? _noteCutInfo;

		[Inject]
		internal void Construct(JudgmentService judgmentService, ConfigProvider configProvider)
		{
			_judgmentService = judgmentService;
			_configuration = configProvider.GetCurrentConfig();
		}

		public override void InitAndPresent(in NoteCutInfo noteCutInfo, int multiplier, float duration, Vector3 targetPos, Quaternion rotation, Color color)
		{
			_noteCutInfo = noteCutInfo;

			if (_configuration?.UseFixedPos ?? false)
			{
				// Set current and target position to the desired fixed position
				targetPos = _configuration!.FixedPos;
				transform.position = targetPos;
			}

			_color = color;
			_saberSwingRatingCounter = noteCutInfo.swingRatingCounter;
			_cutDistanceToCenter = noteCutInfo.cutDistanceToCenter;
			_saberSwingRatingCounter.RegisterDidChangeReceiver(this);
			_saberSwingRatingCounter.RegisterDidFinishReceiver(this);
			_registeredToCallbacks = true;

			if (_configuration == null)
			{
				ScoreModel.RawScoreWithoutMultiplier(_saberSwingRatingCounter, _cutDistanceToCenter, out var beforeCutRawScore, out var afterCutRawScore, out var cutDistanceRawScore);
				_text.text = GetScoreText(beforeCutRawScore + afterCutRawScore);
				_maxCutDistanceScoreIndicator.enabled = cutDistanceRawScore == 15;
				_colorAMultiplier = beforeCutRawScore + afterCutRawScore > 103.5 ? 1f : 0.3f;
			}
			else
			{
				_maxCutDistanceScoreIndicator.enabled = false;

				// Apply judgments a total of twice - once when the effect is created, once when it finishes.
				Judge(_noteCutInfo.Value.swingRatingCounter, 30);
			}

			InitAndPresent(duration, targetPos, rotation, false);
		}

		protected override void ManualUpdate(float t)
		{
			var color = _color.ColorWithAlpha(_fadeAnimationCurve.Evaluate(t));
			_text.color = color;
			_maxCutDistanceScoreIndicator.color = color;
		}

		public override void HandleSaberSwingRatingCounterDidChange(ISaberSwingRatingCounter swingRatingCounter, float rating)
		{
			if (_configuration == null)
			{
				base.HandleSaberSwingRatingCounterDidChange(swingRatingCounter, rating);
				return;
			}

			if (_configuration.DoIntermediateUpdates)
			{
				Judge(swingRatingCounter);
			}
		}

		public override void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
		{
			if (_configuration != null)
			{
				Judge(saberSwingRatingCounter);
			}

			base.HandleSaberSwingRatingCounterDidFinish(saberSwingRatingCounter);
		}

		private void Judge(ISaberSwingRatingCounter swingRatingCounter, int? assumedAfterCutScore = null)
		{
			ScoreModel.RawScoreWithoutMultiplier(swingRatingCounter, _cutDistanceToCenter, out var before, out var after, out var accuracy);
			after = assumedAfterCutScore ?? after;
			var total = before + after + accuracy;
			var timeDependence = Mathf.Abs(_noteCutInfo!.Value.cutNormal.z);
			_judgmentService.Judge(ref _text, ref _color, total, before, after, accuracy, timeDependence);
		}
	}
}