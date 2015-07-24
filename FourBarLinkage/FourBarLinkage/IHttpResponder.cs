using System;
using System.Collections.Specialized;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// Interface abstracting the basic services to be implemented to respond to an Http request.
	/// </summary>
	public interface IHttpResponder
	{
		/// <summary>
		/// Gets or sets the URI to respond to.
		/// </summary>
		string Uri { get; set; }
		/// <summary>
		/// Initialize the specified uri and the variables decoded from the HTTP header.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="variables">Variables of the GET HTTP method.</param>
		/// <param name="postVariables">variables of the POST HTTP method.</param>
		void Initialize (string remoteIp, string uri, NameValueCollection variables, NameValueCollection postVariables);
		byte[] GetResource();
	}
}

