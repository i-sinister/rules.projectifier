namespace projectifier_UnitTests
{
	using Microsoft.VisualStudio.Shell.Interop;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using ksw.projectifier;
	[TestClass]
	public class PackageTest
	{
		[TestMethod]
		public void CreateInstance()
		{
			var package = new ProjectifierPackage();
		}
		[TestMethod]
		public void IsIVsPackage()
		{
			var package = new ProjectifierPackage();
			Assert.IsNotNull(
				package as IVsPackage,
				"The object does not implement IVsPackage");
		}
		[TestMethod]
		public void SetSite()
		{
			var package = new ProjectifierPackage() as IVsPackage;
			Assert.IsNotNull(
				package,
				"The object does not implement IVsPackage");
			var serviceProvider = OleServiceProviderFactory.Create();
			Assert.AreEqual(
				0,
				package.SetSite(serviceProvider),
				"SetSite did not return S_OK");
			Assert.AreEqual(
				0,
				package.SetSite(null),
				"SetSite(null) did not return S_OK");
			OleServiceProviderFactory.Release(serviceProvider);
		}
	}
}