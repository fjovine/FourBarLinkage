using System;
using System.IO;
using System.Collections.Generic;

namespace PlaneFourBarLinkage
{
	/// <summary>
	/// This class manages the temporary files.
	/// It generates their random names with a default extension.
	/// </summary>
	public static class TemporaryFileManager
	{
		/// <summary>
		/// Extension of the temporary files.
		/// </summary>
		private const string tmpPath = "tmp";

		private static string basePath = ".";

		/// <summary>
		/// Gets or sets the base path. The default value is the current path of the application.
		/// </summary>
		public static  string BasePath {
			set {
				basePath = value;
			}
			get {
				return basePath;
			}
		}
		/// <summary>
		/// Computes a temporary file name. It makes sure that no other files of the same file name
		/// exist in the base path.
		/// </summary>
		/// <returns>The temporary file name.</returns>
		public static string GetTemporaryFilename()
		{
			string result;
			do {
				result = Path.GetRandomFileName ();
				result = Path.ChangeExtension (result, tmpPath);
				result = Path.Combine(BasePath, result);
			} while (File.Exists(result));
			return result;
		}
		/// <summary>
		/// Deletes the temporary file.
		/// </summary>
		/// <param name="tempFilename">Temporary file name of the file to be deleted. </param>
		public static void DeleteTemporaryFile(string tempFilename)
		{
			// deletes the temporary file
			if (File.Exists (tempFilename)) {
				File.Delete (tempFilename);
			}
		}
		/// <summary>
		/// Deletes all temporary files in the base path
		/// </summary>
		public static void DeleteAllTemporaryFiles()
		{
			foreach (var fn in Directory.EnumerateFiles ("./", "*."+tmpPath)) {
				if (File.Exists (fn)) {
					File.Delete (fn);
				}
			}
		}
	}
}

