using System;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Interface to be implemented by a concrete class able to compute the configuration of the four bar linkage.
	/// </summary>
	public interface IConfigurationComputer
	{
		/// <summary>
		/// Compute configurations of the passed four bar linkage.
		/// </summary>
		/// <param name="device">Four-bar linkage whose configuration is to be computed.</param>
		bool Compute(FourBarLinkage device);
	}
}
