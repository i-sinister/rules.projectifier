namespace ksw.projectifier
{
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using System.IO;
	using System.Runtime.InteropServices;
	using EnvDTE;
	using EnvDTE80;
	using ksw.projectifier.Commands;
	using Microsoft.VisualStudio;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
	// a package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(ProjectifierIDs.PackageString)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
	public sealed class ProjectifierPackage : Package
	{
		private VsEngine vsEngine;
		private readonly ProjectEngine projectEngine;
		private readonly IDictionary<string, IOptions> options;
		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public ProjectifierPackage()
		{
			options = new Dictionary<string, IOptions>();
			projectEngine = new ProjectEngine();
			AddOption(projectEngine.Options);
		}
		/// <summary>
		/// Initialization of the package; this method is called
		/// right after the package is sited, so this is the place
		/// where you can put all the initialization code that
		/// rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			var commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null == commandService)
			{
				return;
			}
			var dte = GetGlobalService(typeof (DTE)) as DTE2;
			var solution = GetService(typeof(SVsSolution)) as IVsSolution;
			vsEngine = new VsEngine(dte);
			projectEngine.Initialize(dte, solution);
			var projectArrange = new ProjectArrangeFiles(vsEngine, projectEngine);
			commandService.AddCommand(projectArrange.MenuCommand);
			var projectAutoArrange = new ProjectAutoArrangeFiles(vsEngine, projectEngine);
			commandService.AddCommand(projectAutoArrange.MenuCommand);
			var solutionArrange = new SolutionArrangeFiles(vsEngine, projectEngine);
			commandService.AddCommand(solutionArrange.MenuCommand);
			var solutionAutoArrange = new SolutionAutoArrangeFiles(vsEngine, projectEngine);
			commandService.AddCommand(solutionAutoArrange.MenuCommand);
		}
		private void AddOption(IOptions options)
		{
			this.options.Add(options.Key, options);
			AddOptionKey(options.Key);
		}
		protected override void OnLoadOptions(string key, Stream stream)
		{
			options[key].Load(stream);
		}
		protected override void OnSaveOptions(string key, Stream stream)
		{
			options[key].Save(stream);
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				projectEngine.Dispose();
			}
		}
	}
}