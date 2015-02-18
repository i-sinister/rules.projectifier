﻿namespace projectifier_IntegrationTests
{
	using System;
	using ksw.projectifier;
	using Microsoft.VisualStudio.Shell.Interop;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Microsoft.VSSDK.Tools.VsIdeTesting;
	/// <summary>
	/// Integration test for package validation
	/// </summary>
	[TestClass]
	public class PackageTest
	{
		private delegate void ThreadInvoker();
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }
		[TestMethod]
		[HostType("VS IDE")]
		public void PackageLoadTest()
		{
			UIThreadInvoker.Invoke(
				(ThreadInvoker)delegate
				{
					var shellService =
						VsIdeTestHostContext.ServiceProvider
						.GetService(typeof(SVsShell)) as IVsShell;
					Assert.IsNotNull(shellService);
					IVsPackage package;
					var packageGuid = new Guid(ProjectifierIDs.PackageString);
					Assert.IsTrue(
						0 == shellService.LoadPackage(
						ref packageGuid,
						out package));
					Assert.IsNotNull(package, "Package failed to load");
				});
		}
	}
}