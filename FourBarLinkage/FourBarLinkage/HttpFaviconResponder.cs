using System;
using System.Collections.Specialized;
using System.IO;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Concrete class implementing the a responder protocol used to transmit back the favicon to the host computer. 
	/// Favicon stands for Favourite Icon and is normally shown on the browser tab to identify 
	/// the page.
	/// </summary>
	public class HttpFaviconResponder : IHttpResponder
	{
		private string uri;
		/// <summary>
		/// Gets or sets the URI to respond to.
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
		/// Initializes a new instance of the <see cref="PlaneFourBarLinkage.HttpFaviconResponder"/> class.
		/// </summary>
		public HttpFaviconResponder ()
		{
			Uri = "/favicon.ico";
		}
		/// <summary>
		/// Initialize the specified uri and the variables decoded from the HTTP header.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="variables">Variables of the GET HTTP method.</param>
		/// <param name="postVariables">variables of the POST HTTP method.</param>
		public void Initialize (string remoteIp, string uri, NameValueCollection variables, NameValueCollection postVariables)
		{
			// Nothing to be done
		}
		/// <summary>
		/// Gets the favicon extracted from the program properties.
		/// </summary>
		/// <returns>A byte array containing the favicon.</returns>
		public byte[] GetResource()
		{
			Console.WriteLine ("Favicon Sent");
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			Stream icon =  assembly.GetManifestResourceStream("FourBarLinkage.favicon.png");
			using (MemoryStream ms = new MemoryStream())
			{
				icon.CopyTo(ms);
				return ms.ToArray();
			}
		}
	}
}



