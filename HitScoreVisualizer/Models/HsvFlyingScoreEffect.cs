﻿using HitScoreVisualizer.Services;
using HitScoreVisualizer.Settings;
using UnityEngine;
using Zenject;

namespace HitScoreVisualizer.Models
{
	internal class HsvFlyingScoreEffect : FlyingScoreEffect
	{
		private JudgmentService _judgmentService = null!;
		private Configuration? _configuration;

		private NoteCutInfo _noteCutInfo;

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

			// =====================================================================================================================

			base.InitAndPresent(noteCutInfo, multiplier, duration, targetPos, rotation, color);

			if (_configuration != null)
			{
				// Apply judgments a total of twice - once when the effect is created, once when it finishes.
				Judge(_noteCutInfo.swingRatingCounter);
			}
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

		private void Judge(ISaberSwingRatingCounter swingRatingCounter)
		{
			ScoreModel.RawScoreWithoutMultiplier(swingRatingCounter, _cutDistanceToCenter, out var before, out var after, out var accuracy);
			var total = before + after + accuracy;
			var timeDependence = Mathf.Abs(_noteCutInfo.cutNormal.z);
			_judgmentService.Judge(ref _text, ref _color, total, before, after, accuracy, timeDependence);
		}
	}
}