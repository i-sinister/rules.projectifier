namespace ksw.projectifier.Commands
{
	using System;
	using System.ComponentModel.Design;
	using Microsoft.VisualStudio.Shell;
	using MsBuildProject = Microsoft.Build.Evaluation.Project;
	class ProjectArrangeFiles
	{
		private readonly OleMenuCommand menuCommand;
		private readonly VsEngine vsEngine;
		private readonly ProjectEngine projectEngine;
		public ProjectArrangeFiles(
			VsEngine vsEngine,
			ProjectEngine projectEngine)
		{
			this.vsEngine = vsEngine;
			this.projectEngine = projectEngine;
			menuCommand = new OleMenuCommand(
				Execute,
				ProjectifierIDs.Project.ArrangeFiles);
			menuCommand.BeforeQueryStatus += QueryStatus;
		}
		public MenuCommand MenuCommand
		{
			get { return menuCommand; }
		}
		public void Execute(object sender, EventArgs e)
		{
			projectEngine.ArrangeFiles(vsEngine.GetCurrentProject());
		}
		private void QueryStatus(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (null == command)
			{
				return;
			}
			if (projectEngine.CanArrangeFiles(vsEngine.GetCurrentProject()))
			{
				command.Enabled = true;
				command.Visible = true;
			}
			else
			{
				command.Enabled = false;
				command.Visible = false;
			}
		}
	}
}