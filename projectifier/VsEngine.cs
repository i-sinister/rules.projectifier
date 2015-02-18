namespace ksw.projectifier
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using EnvDTE;
	using EnvDTE80;
	class VsEngine
	{
		private readonly DTE2 dte;
		public VsEngine(DTE2 dte)
		{
			this.dte = dte;
		}
		public Project GetCurrentProject()
		{
			var selectedProjects = (Array)dte.ActiveSolutionProjects;
			// only support 1 selected project
			if (1 != selectedProjects.Length)
			{
				return null;
			}
			return (Project)selectedProjects.GetValue(0);
		}
		public IEnumerable<Project> GetSolutionProjects()
		{
			return dte.Solution.Projects.Cast<Project>();
		}
	}
}