using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitScoreVisualizer.Settings
{
	internal class HSVConfig
	{
		internal static HSVConfig? Instance { get; set; }

		public virtual string? ConfigFilePath { get; set; }


		public void Changed()
		{
			// this is called whenever one of the properties is changed
			// can be called to signal that the content has been changed
		}

		public IDisposable ChangeTransaction => null!;
	}
}