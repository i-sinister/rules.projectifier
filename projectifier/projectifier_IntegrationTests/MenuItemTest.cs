namespace projectifier_IntegrationTests
{
	using System;
	using System.Diagnostics;
	using ksw.projectifier;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Microsoft.VsSDK.IntegrationTestLibrary;
	using Microsoft.VSSDK.Tools.VsIdeTesting;
	[TestClass]
	public class MenuItemTest
	{
		private delegate void ThreadInvoker();
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }
		/// <summary>
		///A test for lauching the command and closing the associated dialogbox
		///</summary>
		[TestMethod]
		[HostType("VS IDE")]
		public void LaunchCommand()
		{
			UIThreadInvoker.Invoke(
				(ThreadInvoker)delegate
				{
					var purger = new DialogBoxPurger(
						NativeMethods.IDOK,
						"projectifier\n\nRectified!");
					try
					{
						purger.Start();
						var testUtils = new TestUtils();
						testUtils.ExecuteCommand(ProjectifierIDs.Project.ArrangeFiles);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(DateTime.Now);
						Debug.WriteLine(ex);
					}
					finally
					{
						Assert.IsTrue(
							purger.WaitForDialogThreadToTerminate(),
							"The dialog box has not shown");
					}
				});
		}
	}
}