using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitScoreVisualizer.Settings
{
	internal class HSVConfig
	{
		internal static HSVConfig? Instance { get; set; }

		public virtual string? ConfigFilePath { get; set; }
	}
}