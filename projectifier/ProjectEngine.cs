namespace ksw.projectifier
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using EnvDTE80;
	using Microsoft.Build.Evaluation;
	using Microsoft.VisualStudio;
	using Microsoft.VisualStudio.Shell.Interop;
	using Project = EnvDTE.Project;
	using MsBuildProject = Microsoft.Build.Evaluation.Project;
	class ProjectEngine :
		IDisposable,
		IVsSolutionEvents3
	{
		private static readonly HashSet<string> TargetTypes =
			new HashSet<string>
			{
				"Compile",
				"Content",
				"None",
				"EmbeddedResource"
			};
		private readonly ArrangeFilesOptions options;
		private DTE2 dte;
		private IVsSolution solution;
		private uint vsSolutionCookie = uint.MaxValue;
		public ProjectEngine()
		{
			options = new ArrangeFilesOptions();
		}
		public void Initialize(DTE2 dte, IVsSolution solution)
		{
			this.dte = dte;
			this.solution = solution;
			AdviseSolutionEvents();
			((Events2)dte.Events).ProjectItemsEvents.ItemAdded += ProjectItemAdded;
			((Events2)dte.Events).ProjectItemsEvents.ItemRenamed += ProjectItemRenamed;
		}
		public ArrangeFilesOptions Options
		{
			get { return options; }
		}
		public bool CanArrangeFiles(Project project)
		{
			if (null == project)
			{
				return false;
			}
			// only cs projects are supported;
			if (!project.FullName.EndsWith(".csproj"))
			{
				return false;
			}
			return true;
		}
		public bool CanArrangeFiles(IEnumerable<Project> projects)
		{
			return projects.Any(CanArrangeFiles);
		}
		public void ArrangeFiles(Project project)
		{
			if (!CanArrangeFiles(project))
			{
				return;
			}
			if (project.IsDirty)
			{
				project.Save();
			}
			var msBuildProject = new Microsoft.Build.Evaluation.Project(project.FullName);
			foreach (var itemType in GetOrderedItemTypes(msBuildProject))
			{
				var items = FindItems(msBuildProject, itemType);
				Array.Sort(items, new ProjectItemComparer());
				msBuildProject.RemoveItems(items);
				foreach (var item in items)
				{
					ReinsertItem(msBuildProject, item);
				}
			}
			msBuildProject.Save();
			ProjectCollection
				.GlobalProjectCollection
				.UnloadProject(msBuildProject);
		}
		public void AutoArrangeFiles(IEnumerable<Project> projects)
		{
			foreach (var project in projects)
			{
				ArrangeFiles(project);
			}
		}
		private ProjectItem[] FindItems(MsBuildProject project, string itemType)
		{
			return project.Items
				.Where(item => itemType == item.ItemType)
				.ToArray();
		}
		private IEnumerable<string> GetOrderedItemTypes(MsBuildProject project)
		{
			var previousItemType = string.Empty;
			var itemTypes = new List<string>();
			foreach (var item in project.Items)
			{
				if (!string.Equals(
					item.ItemType,
					previousItemType,
					StringComparison.InvariantCultureIgnoreCase))
				{
					if (TargetTypes.Contains(item.ItemType))
					{
						itemTypes.Add(item.ItemType);
					}
					previousItemType = item.ItemType;
				}
			}
			return itemTypes;
		}
		private void ReinsertItem(MsBuildProject project, ProjectItem item)
		{
			var metadata = item
				.Metadata
				.Select(
					md =>
						new KeyValuePair<string, string>(
							md.Name,
							Microsoft.Build.Evaluation.Project.GetMetadataValueEscaped(md)))
				.ToArray();
			project.AddItemFast(
				item.ItemType,
				item.UnevaluatedInclude,
				metadata);
		}
		public void AutoArrangeFiles(Project project)
		{
			options.EnableAutomaticArrange(project);
		}
		private void ProjectItemAdded(EnvDTE.ProjectItem item)
		{
			ArrangeFilesIfNecessary(item.ContainingProject);
		}
		private void ProjectItemRenamed(EnvDTE.ProjectItem item, string oldname)
		{
			ArrangeFilesIfNecessary(item.ContainingProject);
		}
		private void ArrangeFilesIfNecessary(Project project)
		{
			if (options.IsAutomaticArrangeEnabled(project))
			{
				ArrangeFiles(project);
			}
		}
		#region IVsSolutionEvents implementation
		int IVsSolutionEvents.OnAfterOpenProject(
			IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}
		int IVsSolutionEvents3.OnQueryCloseProject(
			IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}
		int IVsSolutionEvents3.OnBeforeCloseProject(
			IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterLoadProject(
			IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnQueryUnloadProject(
			IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnBeforeUnloadProject(
			IVsHierarchy pRealHierarchy,
			IVsHierarchy pStubHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterOpenSolution(
			object pUnkReserved, int fNewSolution)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnQueryCloseSolution(
			object pUnkReserved, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnBeforeCloseSolution(
			object pUnkReserved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterCloseSolution(
			object pUnkReserved)
		{
			options.Reset();
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterMergeSolution(
			object pUnkReserved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnBeforeOpeningChildren(
			IVsHierarchy pHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterOpeningChildren(
			IVsHierarchy pHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnBeforeClosingChildren(
			IVsHierarchy pHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterClosingChildren(
			IVsHierarchy pHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents3.OnAfterOpenProject(
			IVsHierarchy pHierarchy, int fAdded)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnQueryCloseProject(
			IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnBeforeCloseProject(
			IVsHierarchy pHierarchy, int fRemoved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnAfterLoadProject(
			IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnQueryUnloadProject(
			IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnBeforeUnloadProject(
			IVsHierarchy pRealHierarchy,
			IVsHierarchy pStubHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnAfterOpenSolution(
			object pUnkReserved, int fNewSolution)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnQueryCloseSolution(
			object pUnkReserved, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnBeforeCloseSolution(
			object pUnkReserved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnAfterCloseSolution(
			object pUnkReserved)
		{
			options.Reset();
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnAfterMergeSolution(
			object pUnkReserved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents2.OnAfterOpenProject(
			IVsHierarchy pHierarchy, int fAdded)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnQueryCloseProject(
			IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnBeforeCloseProject(
			IVsHierarchy pHierarchy, int fRemoved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnAfterLoadProject(
			IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnQueryUnloadProject(
			IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnBeforeUnloadProject(
			IVsHierarchy pRealHierarchy,
			IVsHierarchy pStubHierarchy)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnAfterOpenSolution(
			object pUnkReserved, int fNewSolution)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnQueryCloseSolution(
			object pUnkReserved, ref int pfCancel)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
		{
			return VsConstants.S_OK;
		}
		int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
		{
			options.Reset();
			return VsConstants.S_OK;
		}
		#endregion IVsSolutionEvents implementation
		private void AdviseSolutionEvents()
		{
			UnadviseSolutionEvents();
			solution.AdviseSolutionEvents(this, out vsSolutionCookie);
		}
		private void UnadviseSolutionEvents()
		{
			if (vsSolutionCookie != uint.MaxValue)
			{
				solution.UnadviseSolutionEvents(vsSolutionCookie);
				vsSolutionCookie = uint.MaxValue;
			}
		}
		public void Dispose()
		{
			UnadviseSolutionEvents();
			((Events2)dte.Events).ProjectItemsEvents.ItemAdded -= ProjectItemAdded;
			((Events2)dte.Events).ProjectItemsEvents.ItemRenamed -= ProjectItemRenamed;
			solution = null;
			dte = null;
		}
	}
}