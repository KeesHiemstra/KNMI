using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherMonitor.Models
{
	internal class MoonPhase
	{
		private const double TotalLengthOfCycle = 29.53;
		// Phase names
		private readonly List<string> Names = new List<string>
				{
					Phase.NewMoon,
					Phase.WaxingCrescent, Phase.FirstQuarter, Phase.WaxingGibbous,
					Phase.FullMoon,
					Phase.WaningGibbous, Phase.ThirdQuarter, Phase.WaningCrescent
				};
		private static readonly List<Phase> allPhases = new List<Phase>();
		private static readonly IReadOnlyList<string> NorthernHemisphere =
			new List<string> { "🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘", "🌑" };
		private static readonly IReadOnlyList<string> SouthernHemisphere =
			NorthernHemisphere.Reverse().ToList();

		public string Name { get; private set; }
		public string Emoji { get; set; }
		public double DaysIntoCycle { get; set; }
		public Earth.Hemispheres Hemisphere { get; set; }
		public DateTime Moment { get; }
		public double Visibility
		{
			get
			{
				const int FullMoon = 15;
				const double halfCycle = TotalLengthOfCycle / 2;

				var numerator = DaysIntoCycle > FullMoon
						// past the full moon, we want to count down
						? halfCycle - (DaysIntoCycle % halfCycle)
						// leading up to the full moon
						: DaysIntoCycle;

				return numerator / halfCycle;
			}
		}

		public MoonPhase(DateTime utcDateTime, 
			Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern)
		{
			Moment = utcDateTime;
			Hemisphere = viewFromEarth;
			InitPhases();
			CalculatePhase();
		}

		public override string ToString()
		{
			double percent = Math.Round(Visibility, 2);
			return $"The Moon for {Moment} is {DaysIntoCycle:0.00} days\n" +
						 $"into the cycle, and is showing as \"{Name}\"\n" +
						 $"with {percent:0.00}% visibility, and a face of {Emoji} " +
						 $"from the {Hemisphere.ToString().ToLowerInvariant()} hemisphere.";
		}

		private void InitPhases()
		{
			// Initialize the phases of the moon
			double period = TotalLengthOfCycle / Names.Count;
			for (int i = 0; i < Names.Count; i++)
			{
				double start = period * i;
				double end = period * (i + 1);
				allPhases.Add(new Phase(Names[i], start, end));
			}
		}

		private void CalculatePhase()
		{
			// Calculate the days into the cycle
			DaysIntoCycle = (Moment - new DateTime(2000, 1, 6, 18, 14, 0)).TotalDays % TotalLengthOfCycle;
			// Find the current phase based on the days into the cycle
			Phase currentPhase = allPhases.
				FirstOrDefault(p => DaysIntoCycle >= p.Start && DaysIntoCycle < p.End);
			if (currentPhase != null)
			{
				Name = currentPhase.Name;
				int Getindex = Names.IndexOf(Name);
				if (Hemisphere == Earth.Hemispheres.Northern)
				{
					Emoji = NorthernHemisphere[Getindex];
				}
				else
				{
					Emoji = SouthernHemisphere[Getindex];
				}
			}
		}

	}

	public static class Earth
	{
		public enum Hemispheres
		{
			Northern,
			Southern
		}
	}

	public class Phase
	{
#if DEBUG
		// In English
		public const string NewMoon = "New Moon";
		public const string WaxingCrescent = "Waxing Crescent";
		public const string FirstQuarter = "First Quarter";
		public const string WaxingGibbous = "Waxing Gibbous";
		public const string FullMoon = "Full Moon";
		public const string WaningGibbous = "Waning Gibbous";
		public const string ThirdQuarter = "Third Quarter";
		public const string WaningCrescent = "Waning Crescent";
#else
		// In Dutch
		public const string NewMoon = "Nieuwe maan";
		public const string WaxingCrescent = "Wassende sikkel";
		public const string FirstQuarter = "Eerste kwartier";
		public const string WaxingGibbous = "Toenemende maan";
		public const string FullMoon = "Volle Maan";
		public const string WaningGibbous = "Afnemende maan";
		public const string ThirdQuarter = "Laatste kwartier";
		public const string WaningCrescent = "Afnemende sikkel";
#endif

		public Phase(string name, double start, double end)
		{
			Name = name;
			Start = start;
			End = end;
		}

		public string Name { get; }

		/// <summary>
		/// The days into the cycle this phase starts
		/// </summary>
		public double Start { get; }

		/// <summary>
		/// The days into the cycle this phase ends
		/// </summary>
		public double End { get; }
	}

}
