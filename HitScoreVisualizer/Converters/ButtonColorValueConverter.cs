namespace HitScoreVisualizer.Converters
{
	public static class ButtonColorValueConverter
	{
		public static string Convert(bool ok = false)
		{
			return ok ? "#22dd76" : "#ff0d72";
		}
	}
}