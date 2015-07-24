using System;
using System.Collections.Specialized;
using System.Text;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Abstract class containing a minimal infrastructure to respond to an HTTP request with an html page.
	/// A concrete class extending this should provide the following services.
	/// 1. Provide a uri to be used to select which HtmlResponder to be called in case of many uri's
	/// 2. Provide a title to the html page.
	/// 3. Optional javascript code to be included in the html page.
	/// 4. Initialization method that gets the initialization parameters before any of the previous services is requested.
	/// 5. The html code to be sent to the requester.
	/// </summary>
	public abstract class HtmlResponder : IHttpResponder
	{
		private string uri;
		/// <summary>
		/// URI to which the class will respond.
		/// </summary>
		public string Uri {
			get {
				return uri;
			}
			set {
				uri = value;
			}
		}
		/// <summary>
		/// Gets a string containing the title.
		/// </summary>
		public abstract string Title {
			get;
		}
		/// <summary>
		/// Gets a string containing the javascript code.
		/// </summary>
		public abstract string Script {
			get;
		}
		/// <summary>
		/// Initialize the specified uri, variables and postVariables.
		/// </summary>
		/// <param name="uri">URI this class responds to.</param>
		/// <param name="variables">Variables of the GET HTTP method.</param>
		/// <param name="postVariables">variables of the POST HTTP method.</param>
		public abstract void Initialize (string remoteIp, string uri, NameValueCollection variables, NameValueCollection postVariables);
		/// <summary>
		/// Gets the http code appending it to the passed string builder.
		/// </summary>
		/// <param name="sb">String builder to which the html code is appended.</param>
		public abstract void GetHttp (StringBuilder sb);
		/// <summary>
		/// Implements HttpResponder interface.
		/// Transforms the html code into a byte array to send back to the requester.
		/// </summary>
		/// <returns>A byte array containing the resource to be sent back to the requester.</returns>
		public byte[] GetResource()
		{
			StringBuilder htmlBuilder = new StringBuilder ("<!doctype html><html><head>");
			htmlBuilder.Append ("<title>"+Title+"</title>");
			string script = Script;
			if (script != null) {
				htmlBuilder.Append ("<script type='text/javascript'>" + script + "</script>");
			}
			GetHttp ( htmlBuilder);
			htmlBuilder.Append ("</html>");
			return System.Text.Encoding.UTF8.GetBytes (htmlBuilder.ToString());
		}
	}
}

