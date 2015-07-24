using System;
using System.IO;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Class capable of computing the configuration of the four-bar linkage using octave, an open-source package aimed at numerical computations
	/// compatible with matlab. <see cref="http://www.gnu.org/software/octave/"/> 
	/// </summary>
	public class OctaveConfigurationComputer: IConfigurationComputer
	{
		/// <summary>
		/// octave snippet for computing the configuration.
		/// </summary>
		private static string executable =@"
global alpha = 0;
global alpha_left = pi;
global alpha_right = 0;
global beta_left = pi;
global beta_right = 0;

function y = f(beta)
	global a b c d alpha;
	#y = (d + c*cos(beta) - a*cos(alpha)) ^ 2 + (c*sin(beta) - a*sin(alpha)) ^2 - b^2;
	y = a^2-b^2 +c^2 + d^2 - 2*a*d*cos(alpha) + 2*c*d*cos(beta) - 2*a*c*cos(alpha-beta);
endfunction

#
# This function gets the sides of a triangle and returns a vector
# with two values: the square of its area and its height with respect to side c.
# If the three numbers do not describe a triangle, the square of its area is negative and the second element
# in the vector is meaningless and set to zero.
# The Heron's formula to compute the area of te triangle is used.
#
function [sqa, h] = compute_triangle_height_relative_to_side_c(a,b,c)
	#fprintf(stderr, ""compute_triangle_height_relative_to_side_c(%f,%f,%f)\n"",a,b,c);
	# semiperimeter to be used in Heron's formula
	p = (a+b+c)/2;
	# square of the area	
	sqa = p * (p-a) * (p-b) * (p-c);
	if (sqa > 0) 
		h = 2*sqrt(sqa)/c;
	else
		h = 0;
	endif
	#fprintf(stderr, ""sqa =%f h=%f\n"", sqa, h);
endfunction

function result = compute(steps_per_angle)
	global a b c d alpha alpha_left alpha_right beta_left beta_right;

	# Classify the four-bar linkage.
	[sq_area, height] = compute_triangle_height_relative_to_side_c(a+b, c, d);
	if (sq_area > 0) 
		beta_right = asin(height / c);
		if ((a+b)^2 < d^2 + c^2) 
			beta_right = pi - beta_right;
		end
	endif
	
	[sq_area, height] = compute_triangle_height_relative_to_side_c(abs(a-b), c, d);
	if (sq_area > 0)
		beta_left = asin(height / c);
		if ((a-b)^2 < d^2+c^2)
			beta_left = pi - beta_left;
		endif
	endif
		
	[sq_area, height] = compute_triangle_height_relative_to_side_c(c+b, a, d);
	if (sq_area > 0)
		alpha_left = asin(height / a);
		if ((b+c)^2 > d^2 + a^2)
			alpha_left = pi - alpha_left;
		endif
	endif
	
	[sq_area, height] = compute_triangle_height_relative_to_side_c(abs(c-b), a, d);
	if (sq_area > 0)
		alpha_right = asin(height / a);
		if ((b-c)^2 > d^2 + a^2)
			alpha_right = pi - alpha_right;
		endif
	endif

	#
	# Classification of the four-bar linkage
	#
	n = ((d + b - a - c) >= 0) * 4;
	n += ((c + d - a - b) >= 0) * 2;
	n += ((c + b - a - d) >= 0) * 1;
	
	alpha_min = 0;
	alpha_max = 0;
	swingle = true;
	#fprintf(stderr, ""N=%d alpha_right %f alpha_left %f beta_right %f beta_left %f\n"", n, alpha_right, alpha_left, beta_right, beta_left);
	switch (n)
		case {1, 7}
			alpha_min = 0;
			alpha_max = 2*pi;
			swingle = false;
		case {2, 4}
			alpha_min = alpha_right;
			alpha_max = alpha_left;
		case {0, 6}
			alpha_min = -alpha_left;
			alpha_max = alpha_left;
		case {3, 5}
			alpha_min = alpha_right;
			alpha_max = 2*pi - alpha_right;
	endswitch
	#fprintf(stderr, ""n =%d alpha_min=%f alpha_max= %f\n"", n, alpha_min, alpha_max);
 	
	#
	# Compute the four-bar linkage configuration i.e. the angles alpha and beta of the rotating sides
	# Divides the a-side movement in equal angles: if the rotation is complite, no direction tranition
	# is done, otherwise the angles oscillate between the minimum and maximum angle
	#
	if (swingle)
		result = zeros (2*steps_per_angle, 2);
	else
		result = zeros (steps_per_angle, 2);
	endif
	idx = 1;
	initialApprox = pi/2;
	first = true;
	delta_beta = 0;
	swing = alpha_max - alpha_min;
	alpha = alpha_min;
	while (alpha <= alpha_max)
		[beta, fval, info] = fsolve(@f, initialApprox);			
		alpha += swing / steps_per_angle;
		result(idx, 1) = alpha;
		result(idx, 2) = beta;
		if (first) 
			first = false;
		else
			delta_beta = swing / steps_per_angle;
			if (beta < initialApprox) 
				delta_beta = -delta_beta;
			endif
		endif
		initialApprox = beta + 3*delta_beta;
		idx++;
	endwhile

	if (swingle)
		while (alpha >= alpha_min)
			[beta, fval, info] = fsolve(@f, initialApprox);			
			alpha -= swing / steps_per_angle;
			result(idx, 1) = alpha;
			result(idx, 2) = beta;
			delta_beta = swing / steps_per_angle;
			if (beta < initialApprox) 
				delta_beta = -delta_beta;
			endif
			initialApprox = beta + 3*delta_beta;
			idx++;
		endwhile
	endif
endfunction

#
# Known the angles of the rotating bars alpha and beta, this funcion computes the positions of the passed point (x,y)
# relative to a coordinate system having origin on the extreme of bar a (A) and x axis aligned to the floating bar
# with respect to the absolute coordinate system (having origin on the hinge of bar a and x axis aligned to the
# ground bar
#
function point = compute_absolute_position_point_on_floating_bar_plane(x,y, alpha, beta)
	global a b c d;
	# translated point
	ptx = [x, y];
	
	# rotation matrix
	gamma = atan2 (c*sin(beta) - a*sin(alpha), d+c*cos(beta)-a*cos(alpha));
	rot = [cos(gamma), sin(gamma); -sin(gamma), cos(gamma)];
	point = ptx * rot;	
	point += [a * cos(alpha), a * sin(alpha)];
	#point = ptx;
endfunction

#
# Returns the trajectory of the point (x,y) on the floating bar space as the four-bar linkage moves
# with the passed alpha, beta angles
#
function tr = compute_trajectory(x,y, config)
	rowsconfig = rows(config);
	tr = zeros(rowsconfig, 2);
	for idx = 1:rowsconfig
		pt = compute_absolute_position_point_on_floating_bar_plane(x, y, config(idx,1), config(idx,2));
		tr(idx, 1) = pt(1);
		tr(idx, 2) = pt(2);
	endfor
endfunction
";
		/// <summary>
		/// Creates an octave snippet declaring a global variable.
		/// </summary>
		/// <returns>Octave snippet declaring a global variable.</returns>
		/// <param name="name">Name of the global variable.</param>
		/// <param name="value">Value of the global variable.</param>
		private static string DeclareVariable(string name, double value) {
			return string.Format ("global {0} = {1}\n", name, value);
		}
		/// <summary>
		/// Generates the octave code.
		/// </summary>
		/// <returns>The octave code.</returns>
		/// <param name="fbl">four-bar linkage to be used.</param>
		/// <param name="divisor">Number of steps in which the left angle-span is to be divided. Each angle is used to compute a configuration.</param>
		private static string GenerateOctaveCode(FourBarLinkage fbl, int divisor) 
		{
			string result = TemporaryFileManager.GetTemporaryFilename ();
			using (TextWriter tw = File.CreateText (result)) {
				tw.WriteLine (DeclareVariable("a", fbl.a));
				tw.WriteLine (DeclareVariable("b", fbl.b));
				tw.WriteLine (DeclareVariable("c", fbl.c));
				tw.WriteLine (DeclareVariable("d", fbl.d));
				tw.Write (executable);
				tw.Write (string.Format("r = compute({0});", divisor));

				foreach (var varName in new string[] {"alpha_left", "alpha_right", "beta_left", "beta_right"}) {
					tw.Write (string.Format("disp({0});", varName));
				}
				tw.WriteLine ("disp(r);");
				tw.WriteLine ("puts(\"----\\n\");");
				tw.WriteLine (string.Format("disp(compute_trajectory({0},{1}, r));", fbl.PtX, fbl.PtY));
			}
			return result;
		}
		/// <summary>
		/// Compute configurations of the passed four bar linkage.
		/// The steps the method performs are the following.
		/// 1. Generate the octave code using the parameters defining the linkage.
		/// 2. Call octave and get the results back from the stdout stream.
		/// 3. Decode the results of the octave code storing the computed values into the object describing the instance of four-bar linkage to be simulated.
		/// </summary>
		/// <param name="device">Four-bar linkage whose configuration is to be computed.</param>
		public bool Compute(FourBarLinkage device)
		{
			System.Diagnostics.Process octave = new System.Diagnostics.Process ();
			octave.StartInfo.UseShellExecute = false;
			octave.StartInfo.CreateNoWindow = false;
			octave.StartInfo.FileName = "octave";
			string tempFilename = GenerateOctaveCode (device, 300);
			octave.StartInfo.Arguments = "-qf " + tempFilename;
			octave.StartInfo.RedirectStandardInput = true;
			octave.StartInfo.RedirectStandardError = true;
			octave.StartInfo.RedirectStandardOutput = true;
			// Launch the octave process
			octave.Start ();
			string output = octave.StandardOutput.ReadToEnd ();
			string error = octave.StandardError.ReadToEnd ();
			if (error.Length > 0) {
				// If there is something in the stderr file it means that there is some error.
				// The stdout content as well as the octave source code (with the line numbers) are shown
				string[] code = File.ReadAllLines (tempFilename);
				for (int i = 0; i < code.Length; i++) {
					Console.WriteLine ("{0,4} {1}", i + 1, code [i]);
				}
				Console.WriteLine ("Error: " + error);
			}

			int rowCounter =0;
			bool bSaveConfig = true;
			//Console.WriteLine ("output " + output);
			using (TextWriter tw = File.CreateText ("./output.txt")) {
				tw.Write (output);
			}


			// The results are sent back trough the stdout stream
			// The sequence is as follows
			// 1. The extrema angles
			// 2. The configuration (list of alpha, beta angles)
			// 3. The trajectory (list of P (x,y) coordinates)
			foreach (var line in output.Split('\n')) {
				if (rowCounter++ < 4) {
					double val = double.Parse (line);
					// Console.WriteLine (string.Format ("{0} - {1}", rowCounter, val));
					switch (rowCounter) {
					case 1:
						device.alpha_left = val;
						break;
					case 2:
						device.alpha_right = val;
						break;
					case 3:
						device.beta_left = val;
						break;
					case 4:
						device.beta_right = val;
						break;
					}
				} else {
					if (line.StartsWith("----")) {
						bSaveConfig = false;
					}
					var ss = line.Split (new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (ss.Length < 2) {
						continue;
					}
					if (bSaveConfig) {
						double alpha = double.Parse (ss [0]);
						double beta = double.Parse (ss [1]);
						device.AddConfiguration (alpha, beta);
					} else {
						double x = double.Parse (ss [0]);
						double y = double.Parse (ss [1]);
						device.AddPoint (x, y);
					}
				}
			}
			TemporaryFileManager.DeleteTemporaryFile(tempFilename);
			return true;
		}
	}
}

