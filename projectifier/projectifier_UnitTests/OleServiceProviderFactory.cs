namespace projectifier_UnitTests
{
	using Microsoft.VisualStudio.Shell.Interop;
	using Microsoft.VsSDK.UnitTestLibrary;
	public static class OleServiceProviderFactory
	{
		public static OleServiceProvider Create()
		{
			var serviceProvider = OleServiceProvider
				.CreateOleServiceProviderWithBasicServices();
			var activityLogMock =
				new GenericMockFactory(
					"MockVsActivityLog",
					new[] { typeof(IVsActivityLog) })
					.GetInstance();
			serviceProvider.AddService(
				typeof(SVsActivityLog),
				activityLogMock,
				true);
			return serviceProvider;
		}
		public static void Release(OleServiceProvider serviceProvider)
		{
			serviceProvider.RemoveService(typeof(SVsActivityLog));
		}
	}
}