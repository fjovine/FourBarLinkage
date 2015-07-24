using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Entry point of the FourBarLinkage simulator.
	/// This class implements an HTTP server capable of sending to a remote client a web page simulating graphically 
	/// the movement of four-bar linkage. The parameters describing the linkage (side dimensions and coordinates of
	///  an additional point integral to the floating bar whose trajectory is to be build) are received and decoded from the 
	/// client.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Listener of the HTTP protocol.
		/// </summary>
		private HttpListener listener = new HttpListener();
		/// <summary>
		/// Sets the default responder, i.e. the HttpResponder object to be used in the default uri /
		/// </summary>
		/// <value>The default responder.</value>
		public HtmlResponder defaultResponder {
			set;
			private get;
		}
		/// <summary>
		/// Dictionary storing the map uri->http responder class to be used.
		/// </summary>
		private Dictionary<string, IHttpResponder> responders = new Dictionary<string, IHttpResponder> ();

		public void RegisterResponder(IHttpResponder httpResponder)
		{
			string uri = httpResponder.Uri;
			responders.Add (uri, httpResponder);
		}
		/// <summary>
		/// Entry point method.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("FourBar linkage");
			TemporaryFileManager.DeleteAllTemporaryFiles ();
			Program program = new Program();
			program.defaultResponder = new HttpAnimationResponder ("animation");
			program.RegisterResponder (new HttpFaviconResponder ());
			program.Start();
		}
		/// <summary>
		/// Start this instance, i.e. instantiates an HTTP server
		/// responding to the 1234 TCP/IP port.
		/// </summary>
		public void Start()
		{
			listener.Prefixes.Add("http://*:1234/");
			listener.Start();
			Console.WriteLine("Listening, hit enter to stop");
			listener.BeginGetContext(new AsyncCallback(GetContextCallback), null);
			Console.ReadLine();
			listener.Stop();
		}
		/// <summary>
		/// Decodes the posted data from the HTTP header.
		/// </summary>
		/// <returns>A NameValueCollection, i.e. a map that maps the name of the variable to its value</returns>
		/// <param name="request">Request HTTP header.</param>
		public static NameValueCollection GetRequestPostData(HttpListenerRequest request)
		{
			NameValueCollection result = null;
			if (request.HasEntityBody) {
				using (System.IO.Stream body = request.InputStream) { // here we have data
					using (System.IO.StreamReader reader = new System.IO.StreamReader (body, request.ContentEncoding)) {
						result = HttpUtility.ParseQueryString (reader.ReadToEnd ());
					}
				}
			}
			return  result;
		}
		/// <summary>
		/// Actual method that responds to an http request.
		/// </summary>
		/// <param name="result">Result.</param>
		public void GetContextCallback(IAsyncResult result)
		{
			HttpListenerContext context = listener.EndGetContext (result);
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			IHttpResponder chosenResponder;
			NameValueCollection receivedVariables = null;

			if (!responders.TryGetValue (request.Url.LocalPath, out chosenResponder)) {
				chosenResponder = defaultResponder;
			}
			Console.WriteLine(string.Format("Request {0} ", request.Url.LocalPath));

			if (request.HttpMethod == "POST") {
				receivedVariables = GetRequestPostData (request);
				foreach (string key in receivedVariables.Keys) {
					Console.WriteLine (string.Format ("Query:      {0} = {1}", key, receivedVariables [key]));
				}
			} else {
				receivedVariables = request.QueryString;
			}

			chosenResponder.Initialize (request.RemoteEndPoint.Address.ToString (), request.Url.AbsoluteUri, request.QueryString, receivedVariables);
			byte[] buffer = chosenResponder.GetResource ();
			response.ContentLength64 = buffer.Length;

			using (System.IO.Stream outputStream = response.OutputStream) {
				outputStream.Write (buffer, 0, buffer.Length);
			}
			listener.BeginGetContext (new AsyncCallback (GetContextCallback), null);
		}
	}
}
