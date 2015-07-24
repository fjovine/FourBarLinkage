using System;
using System.Collections.Generic;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// This class describes time varying state of the linkage.
	/// The state of the linkage is completely described by the angles alpha and beta (see documentation).
	/// </summary>
	struct State {
		/// <summary>
		/// Angle of the left rotating bar (AB) with respect to the right pointing direction of the ground bar.
		/// </summary>
		public double alpha;
		/// <summary>
		/// Angle of the right rotating bar (CD) with respect to the right pointing direction of the ground bar.
		/// </summary>
		public double beta;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneFourBarLinkage.State"/> struct.
		/// </summary>
		/// <param name="alpha">Angle of the left rotating bar (AB) with respect to the right pointing direction of the ground bar.</param>
		/// <param name="beta">Angle of the right rotating bar (CD) with respect to the right pointing direction of the ground bar.</param>
		public State(double alpha, double beta)
		{
			this.alpha = alpha;
			this.beta = beta;
		}
	}
	/// <summary>
	/// This class describes the coordinates of a point relative to the system of coordinate moving with the floating bar of the linkage.
	/// </summary>
	struct Point {
		/// <summary>
		/// X coordinate of the moving point relative to the system of coordinates moving with the floating bar of the linkage.
		/// </summary>
		public double x;
		/// <summary>
		/// Y coordinate of the moving point relative to the system of coordinates moving with the floating bar of the linkage.
		/// </summary>
		public double y;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneFourBarLinkage.Point"/> struct.
		/// </summary>
		/// <param name="x">The x coordinate of the point relative to the system of coordinates moving with the floating bar.</param>
		/// <param name="y">The y coordinate of the point relative to the system of coordinates moving with the floating bar.</param>
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
	}
		
	/// <summary>
	/// This class describes a FourBarLinkage, keeps track of its configuration but is not capable of computing the geometrical quantities
	/// needed to show it. For this task needs an external object.
	/// It keeps track of the trajectory of a point integral to the floating bar.
	/// </summary>
	public class FourBarLinkage
	{
		/// <summary>
		/// Size in arbitrary units of the left rotating bar.
		/// </summary>
		public double a {
			get;
			private set;
		}
		/// <summary>
		/// Size in arbitrary units of the floating rotating bar.
		/// </summary>
		public double b {
			get;
			private set;
		}
		/// <summary>
		/// Size in arbitrary units of the right rotating bar.
		/// </summary>
		public double c {
			get;
			private set;
		}
		/// <summary>
		/// Size in arbitrary units of the ground fixed bar.
		/// </summary>
		public double d {
			get;
			private set;
		}
		/// <summary>
		/// Extreme to the right of the alpha angle. It swings from alpha_right to alpha_left.
		/// </summary>
		public double alpha_right {
			get;
			set;
		}
		/// <summary>
		/// Extreme to the right of the beta angle. It swings from beta_right to beta_left.
		/// </summary>
		public double beta_right {
			get;
			set;
		}
		/// <summary>
		/// Extreme to the left of the alpha angle. It swings from alpha_right to alpha_left.
		/// </summary>
		public double alpha_left {
			get;
			set;
		}
		/// <summary>
		/// Extreme to the left of the beta angle. It swings from beta_right to beta_left.
		/// </summary>
		public double beta_left {
			get;
			set;
		}
		/// <summary>
		/// X coordinate in arbitrary units with respect to a system of coordinates integral to the floating bar.
		/// </summary>
		public double PtX {
			get;
			set;
		}
		/// <summary>
		/// Y coordinate in arbitrary units with respect to a system of coordinates integral to the floating bar.
		/// </summary>
		public double PtY {
			get;
			set;
		}
		/// <summary>
		/// Sets the configuration computer which is class implementing the ConfigurationComputer interface responsible for computing
		/// the time-dependent configuration.
		/// </summary>
		public IConfigurationComputer ConfigurationComputer {
			set;
			private get;
		}

		/// <summary>
		/// List of subsequent configurations of the linkage.
		/// </summary>
		private List<State> configurations = new List<State>();
		/// <summary>
		/// List of subsequent positions of the selected point integral to the floating bar.
		/// </summary>
		private List<Point> trajectory = new List<Point> ();

		/// <summary>
		/// Determines whether the passed bar sizes are compatible with the existence of a four bar linkage
		/// </summary>
		/// <returns><c>true</c> if the four-bar linkage has bars whose size is acceptable; otherwise, <c>false</c>.</returns>
		public bool IsValid()
		{
			if (this.a > this.b + this.c + this.d) {
				return false;
			}
			if (this.b > this.a + this.c + this.d) {
				return false;
			}
			if (this.c > this.a + this.b + this.d) {
				return false;
			}
			if (this.d > this.a + this.b + this.c) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneFourBarLinkage.FourBarLinkage"/> class.
		/// </summary>
		/// <param name="a">The length in arbitrary units of the left rotating bar. </param>
		/// <param name="b">The length in arbitrary units of the floating bar.</param>
		/// <param name="c">The length in arbitrary units of the right rotating bar.</param>
		/// <param name="d">The length in arbitrary units of the ground fixed bar.</param>
		public FourBarLinkage (double a, double b, double c, double d)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			ConfigurationComputer = null;
		}

		/// <summary>
		/// Adds a configuration to the list of already available configurations.
		/// </summary>
		/// <param name="alpha">Angle in radians of the left rotating bar.</param>
		/// <param name="beta">Angle in radians of the right rotating bar.</param>
		public void AddConfiguration(double alpha, double beta)
		{
			configurations.Add (new State(alpha, beta));
		}

		/// <summary>
		/// Adds a point to the trajectory already available.
		/// </summary>
		/// <param name="x">The x coordinate of the point.</param>
		/// <param name="y">The y coordinate of the point.</param>
		public void AddPoint(double x, double y)
		{
			trajectory.Add (new Point (x, y));
		}

		/// <summary>
		/// Computes the configurations calling the passed configuration computer interface.
		/// </summary>
		/// <returns><c>true</c>, if configurations were correctly computed, <c>false</c> otherwise.</returns>
		public bool ComputeConfigurations()
		{
			if (ConfigurationComputer != null) {
				return ConfigurationComputer.Compute (this);
			} else {
				return false;
			}
		}

		/// <summary>
		/// Iterator through each configuration.
		/// </summary>
		/// <param name="visitor">Visitor to be called for each available configuration.</param>
		public void ForEachConfiguration(Action<double, double> visitor) {
			foreach( var c in configurations ) {
				visitor(c.alpha, c.beta);
			}
		}
		/// <summary>
		/// Iterator through each point of the trajectory.
		/// </summary>
		/// <param name="visitor">Visitor to be called for each available point in the trajectory.</param>
		public void ForEachPoint(Action<double, double> visitor) {
			foreach( var p in trajectory ) {
				visitor(p.x, p.y);
			}
		}
	}
}