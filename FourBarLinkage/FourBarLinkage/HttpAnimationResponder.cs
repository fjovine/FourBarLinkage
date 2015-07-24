using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Concrete class (extension of HtmlResponder) capable of generating a web page showing an animation of the linkage.
	/// </summary>
	public class HttpAnimationResponder : HtmlResponder
	{
		/// <summary>
		/// The four bar linkage to be animated.
		/// </summary>
		private FourBarLinkage fourBarLinkage;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneFourBarLinkage.HttpAnimationResponder"/> class.
		/// </summary>
		/// <param name="uri">URI this class responds to.</param>
		public HttpAnimationResponder (string uri)
		{
			this.Uri = uri;
		}
		/// <summary>
		/// Homogeneous scale to be used to show the animation.
		/// </summary>
		private double Scale {
			set;
			get;		
		}
		/// <summary>
		/// Gets or sets a value indicating whether the form should be shown on html..
		/// </summary>
		/// <value><c>true</c> if this instance contains the form; otherwise, <c>false</c>.</value>
		private bool IsForm {
			set;
			get;
		}
		/// <summary>
		/// Gets or sets the ip of the client computer.
		/// </summary>
		/// <value>The ip of the client computer.</value>
		private string IpReferrer {
			set;
			get;
		}
		/// <summary>
		/// Gets or sets the local ip address in string format.
		/// </summary>
		/// <value>The local ip in string format.</value>
		private string RemoteIp {
			set;
			get;
		}
		/// <summary>
		/// Gets the value of the variable describing the linkage.
		/// </summary>
		/// <returns>The variable value.</returns>
		/// <param name="defVal">Default value to be returned if the passed variable is not contained in the passed collection.</param>
		/// <param name="variableName">Name of the variable received through the http request.</param>
		/// <param name="variables">Map from the name of the variable to their contents. This variable is decoded from the http request.</param>
		private static double GetVariableValue(double defVal, string variableName, NameValueCollection variables)
		{
			double result = defVal;
			if (variables != null) {
				string stringValue = variables.Get(variableName);
				if (stringValue != null) {
					if (!double.TryParse (stringValue, out result)) {
						result = defVal;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Creates the html code to show a numeric field or a hidden numeric field.
		/// </summary>
		/// <returns>The html code to show the field.</returns>
		/// <param name="variableName">Name of the variable to be used in the field.</param>
		/// <param name="value">Value of the variable.</param>
		/// <param name="isHidden">If set to <c>true</c> the field is hidden in the html sense.</param>
		private string GetNumericField(string variableName, double value, bool isHidden = false)
		{
			if (isHidden) {
				return string.Format ("<input type='hidden' name='{0}' value='{1}'/>&nbsp;", variableName, value);
			} else {
				return string.Format ("{0}&nbsp;<input type='text' size ='4' maxlength='20' name='{0}' value='{1}'/>&nbsp;", variableName, value);
			}
		}
		/// <summary>
		/// Creates a javascript snippet containing an array with the list of configurations describing the linkage.
		/// </summary>
		/// <returns>A javascript snippet containing the configurations of the linkage.</returns>
		private string GetPositions()
		{
			StringBuilder result = new StringBuilder();
			fourBarLinkage.ForEachConfiguration ((alpha, beta) => {
				result.Append (string.Format ("new Point({0},{1}),", alpha, beta));
			});
			return result.ToString();
		}
		/// <summary>
		/// Creates a javascript snippet containing an array with the list of points describing the trajectory of the point integral to the floating bar.
		/// </summary>
		/// <returns>A javascript snippet containing the trajectory.</returns>
		private string GetTrajectory()
		{
			StringBuilder result = new StringBuilder();
			fourBarLinkage.ForEachPoint((x, y) => {
				result.Append (string.Format ("new Point({0},{1}),", x, y));
			});
			return result.ToString();
		}
		/// <summary>
		/// Appends a javascript snippet declaring a variable and its initial value.
		/// </summary>
		/// <param name="name">Name of the variable.</param>
		/// <param name="value">Initial value of the variable.</param>
		/// <param name="sb">String builder where the javascript snippet will be appended.</param>
		private void AppendVariable(string name, double value, StringBuilder sb)
		{
			sb.Append ("\nvar ");
			sb.Append (name);
			sb.Append (" = ");
			sb.Append (value);
			sb.Append (";");
		}
		/// <summary>
		/// Generates a javascript snippet containing the initialization of all the global variable used in the javascript code.
		/// </summary>
		/// <returns>A javascript snippet declaring all the needed variables.</returns>
		private string GetVariables()
		{
			StringBuilder sb = new StringBuilder ();
			AppendVariable ("A", fourBarLinkage.a, sb);
			AppendVariable ("B", fourBarLinkage.b, sb);
			AppendVariable ("C", fourBarLinkage.c, sb);
			AppendVariable ("D", fourBarLinkage.d, sb);

			AppendVariable ("A_Left", fourBarLinkage.alpha_left, sb);
			AppendVariable ("A_Right", fourBarLinkage.alpha_right, sb);
			AppendVariable ("B_Left", fourBarLinkage.beta_left, sb);
			AppendVariable ("B_Right", fourBarLinkage.beta_right, sb);

			AppendVariable ("VisibleScale", Scale, sb);
			return sb.ToString ();
		}
		/// <summary>
		/// Initialize the specified uri and all the parameters decoding the data received in the HTTP header.
		/// </summary>
		/// <param name="uri">URI this class responds to.</param>
		/// <param name="variables">Variables of the GET HTTP method.</param>
		/// <param name="postVariables">variables of the POST HTTP method.</param>
		public override void Initialize(string remoteIp, string uri, NameValueCollection variables, NameValueCollection postVariables)
		{
			this.RemoteIp = remoteIp;
			double a = GetVariableValue (3.0, "a", postVariables);
			double b = GetVariableValue (5.0, "b", postVariables);
			double c = GetVariableValue (4.0, "c", postVariables);
			double d = GetVariableValue (6.0, "d", postVariables);
			Scale = GetVariableValue (1.0, "scale", postVariables);
			if (postVariables.Get("zoomout") != null) {
				if (Scale < 16) {
					Scale *= 2;
				}
			} else if (postVariables.Get("zoomin") != null) {
				if (Scale > 0.125) {
					Scale /= 2;
				}
			}

			if (postVariables.Get ("noform") != null) {
				IsForm = false;
			} else {
				IsForm = true;
			}

			fourBarLinkage = new FourBarLinkage (a, b, c, d);
			fourBarLinkage.PtX = GetVariableValue (0.0, "px", postVariables);
			fourBarLinkage.PtY = GetVariableValue (0.0, "py", postVariables);
			fourBarLinkage.ConfigurationComputer = new OctaveConfigurationComputer ();
			fourBarLinkage.ComputeConfigurations ();
		}
		/// <summary>
		/// Gets all the http code appending it to the passed string builder.
		/// </summary>
		/// <param name="sb">String builder to which the html code is appended.</param>
		public override void GetHttp(StringBuilder sb)
		{
			sb.Append ("<body onload='init();'>\n");
			if (this.IsForm) {
				sb.Append ("<form action='fbl.io' method=post>\n");
				sb.Append (GetNumericField ("a", fourBarLinkage.a));
				sb.Append ("\n");
				sb.Append (GetNumericField ("b", fourBarLinkage.b));
				sb.Append ("\n");
				sb.Append (GetNumericField ("c", fourBarLinkage.c));
				sb.Append ("\n");
				sb.Append (GetNumericField ("d", fourBarLinkage.d));
				sb.Append ("\n");
				sb.Append (GetNumericField ("px", fourBarLinkage.PtX));
				sb.Append ("\n");
				sb.Append (GetNumericField ("py", fourBarLinkage.PtY));
				sb.Append ("\n");
				sb.Append (GetNumericField ("scale", Scale, true));
				sb.Append ("\n");

				// creates a set of default variables covering all the possible conditions
				sb.Append (@"<input type='submit' value='Go' name='go'/>&nbsp;");
				sb.Append (@"<input type='submit' value='+' name='zoomout'/>&nbsp;");
				sb.Append (@"<input type='submit' value='-' name='zoomin'/>&nbsp;");
				sb.Append ("Standard&nbsp;cases:&nbsp;");
				sb.Append (
					string.Format ("<a href=\"/:1234/?a={0}&b={1}&c={2}&d={3}&px={4}&py={5}&scale={6}&noform=true\">Pic</a>&nbsp",
						fourBarLinkage.a, fourBarLinkage.b, fourBarLinkage.c, fourBarLinkage.d, 
						fourBarLinkage.PtX, fourBarLinkage.PtY,
						Scale
					)
				);
				sb.Append ("<a href=\"/:1234/?a=6&b=6&c=6&d=5&px=3\">1</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=4&b=6&c=6&d=5&px=3\">2</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=6&b=6&c=4&d=5&px=3\">3</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=6&b=4&c=6&d=5&px=2\">4</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=6&b=4&c=4&d=5&px=2\">5</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=4&b=4&c=6&d=5&px=2\">6</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=4&b=6&c=4&d=5&px=3\">7</a>&nbsp;");
				sb.Append ("<a href=\"/:1234/?a=4&b=4&c=4&d=5&px=2\">8</a><br/>");
			}
			if (fourBarLinkage.IsValid ()) {
				sb.Append (@"<div>
			      <canvas id=""canvas"" width=""800"" height=""600"">
			      Sorry, browser does not support canvas.
			      </canvas></div>");
			} else {
				sb.Append (@"<h1> A four-bar linkage cannot be build with the selected parameters.</h1>");		    
			}
			sb.Append(@"</body>");

		}
		/// <summary>
		/// Title of the html code.
		/// </summary>
		public override string Title {
			get {
				return "Four-bar linkage animation.";
			}
		}
		/// <summary>
		/// Javascript snippet to be included in the html code.
		/// </summary>
		public override string Script {
			get {
				return @"var canvas;
var ctx;
var timer;
" + GetVariables () +@"
var scale

var idx = 0;

var angularPositions = ["+GetPositions()+@"];
var trajectory = ["+GetTrajectory()+@"];

function Point(x,y)
{
	this.x = x;
	this.y = y;
}

function drawLinkage(idx) {
	var p = angularPositions[idx];
	var alpha = p.x;
	var beta = p.y;
	
	if (false) {
		document.write(alpha);
		document.write(';');
		document.write(beta);
		document.write(""<br/>"");
	}
	
	var PA = new Point(-D/2,0);
	var PB = new Point(-D/2+A*Math.cos(alpha), A*Math.sin(alpha));
	var PC = new Point(D/2+C*Math.cos(beta), C*Math.sin(beta));
	var PD = new Point(D/2,0);
	
	ctx.lineCap='round';
	ctx.strokeStyle=""#000000"";
	ctx.lineWidth = ""0.2""
	ctx.beginPath();
	ctx.moveTo(PA.x, PA.y);
	ctx.lineTo(PB.x, PB.y);
	ctx.stroke();
	
	ctx.beginPath();
	ctx.moveTo(PB.x, PB.y);
	ctx.lineTo(PC.x, PC.y);
	ctx.stroke();
	
	ctx.beginPath();
	ctx.moveTo(PC.x, PC.y);
	ctx.lineTo(PD.x, PD.y);
	ctx.stroke();

	// Draws the moving point on the trajectory
	ctx.beginPath();
	var curPt = trajectory[idx];
	ctx.arc(curPt.x-D/2, curPt.y, 0.15, 0, 2*Math.PI);
	ctx.fillStyle = 'blue';
	ctx.strokeStyle=""#0000FF"";
	ctx.fill();	
	ctx.stroke();
}
function init() {
	canvas = document.getElementById(""canvas"");
	ctx = canvas.getContext(""2d"");
	timer=setInterval(draw, 50);
	return timer;
}
function draw() {
	ctx.setTransform(1,0,0,1,0,0);
	ctx.clearRect(0, 0, canvas.width, canvas.height);
	ctx.fillStyle = ""#F0F0F0"";
	ctx.fillRect(0,0,canvas.width,canvas.height);
	ctx.fillStyle = ""#FF0000"";

	ctx.translate(canvas.width/2, canvas.height/2);
	ctx.scale(40*VisibleScale,-40*VisibleScale);

	ctx.lineWidth = ""0.05""
	// Draws the trajectory of the point on the floating bar
	ctx.beginPath();
	ctx.strokeStyle =""#0000FF"";
	ctx.moveTo(trajectory[0].x-D/2, trajectory[0].y);
	for(var i=1; i<trajectory.length; i++) {
		ctx.lineTo(trajectory[i].x-D/2, trajectory[i].y);
	}
	ctx.lineTo(trajectory[0].x-D/2, trajectory[0].y);
	ctx.stroke();

	// Draws the trajectories of the rotating hinges
	// Left 

	ctx.beginPath();
	ctx.strokeStyle =""#FF0000"";
	ctx.arc(-D/2, 0, A, A_Left, A_Right, true);
	ctx.stroke();
	ctx.beginPath();
	ctx.arc(-D/2, 0, A, 2*Math.PI - A_Right, 2*Math.PI - A_Left, true);
	ctx.stroke();
	// Right
	ctx.beginPath();
	ctx.strokeStyle =""#00FF00"";
	ctx.arc(D/2, 0, C, B_Left, B_Right, true);
	ctx.stroke();
	ctx.beginPath();
	ctx.arc(D/2, 0, C, 2*Math.PI-B_Right, 2*Math.PI - B_Left, true);
	ctx.stroke();

	// Draws the linkage composed by the foixed, ther rotating and the floating bars
	drawLinkage(idx);
	
	idx ++;
	if (idx >= angularPositions.length) {
		idx = 0;
	}
}
";
			}
		}

	}
}

