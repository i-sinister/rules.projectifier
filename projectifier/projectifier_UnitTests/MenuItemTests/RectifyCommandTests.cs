namespace projectifier_UnitTests.MenuItemTests
{
	using System.Reflection;
	using System.ComponentModel.Design;
	using Microsoft.VisualStudio.Shell.Interop;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Microsoft.VisualStudio.Shell;
	using ksw.projectifier;
	[TestClass]
	public class RectifyCommandTests
	{
		/// <summary>
		/// Verify that a new menu command object gets added to the OleMenuCommandService.
		/// This action takes place In the Initialize method of the Package object
		/// </summary>
		[TestMethod]
		public void InitializeMenuCommand()
		{
			var package = new ProjectifierPackage() as IVsPackage;
			Assert.IsNotNull(package, "The object does not implement IVsPackage");
			var serviceProvider = OleServiceProviderFactory.Create();
			Assert.AreEqual(
				0,
				package.SetSite(serviceProvider),
				"SetSite did not return S_OK");
			var info = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.IsNotNull(info);
			var mcs = info.Invoke(
				package,
				new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;
			Assert.IsNotNull(mcs.FindCommand(ProjectifierIDs.Project.ArrangeFiles));
			OleServiceProviderFactory.Release(serviceProvider);
		}
		[TestMethod]
		public void RectifyCommand()
		{
			var package = new ProjectifierPackage() as IVsPackage;
			Assert.IsNotNull(package, "The object does not implement IVsPackage");
			var serviceProvider = OleServiceProviderFactory.Create();
			var uishellMock = UIShellServiceMock.GetUiShellInstance();
			serviceProvider.AddService(
				typeof(SVsUIShell), uishellMock, true);
			Assert.AreEqual(
				0,
				package.SetSite(serviceProvider),
				"SetSite did not return S_OK");
			var info = package.GetType().GetMethod(
				"Rectify",
				BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.IsNotNull(
				info,
				"Failed to get the private method MenuItemCallback throug refplection");
			info.Invoke(package, new object[] { null, null });
			serviceProvider.RemoveService(typeof(SVsUIShell));
			OleServiceProviderFactory.Release(serviceProvider);
		}
	}
}