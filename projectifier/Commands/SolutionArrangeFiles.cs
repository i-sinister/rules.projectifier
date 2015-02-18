namespace ksw.projectifier.Commands
{
	using System;
	using System.ComponentModel.Design;
	using Microsoft.VisualStudio.Shell;
	class SolutionArrangeFiles
	{
		private readonly VsEngine vsEngine;
		private readonly ProjectEngine projectEngine;
		private readonly OleMenuCommand menuCommand;
		public SolutionArrangeFiles(
			VsEngine vsEngine,
			ProjectEngine projectEngine)
		{
			this.vsEngine = vsEngine;
			this.projectEngine = projectEngine;
			menuCommand = new OleMenuCommand(
				Execute,
				ProjectifierIDs.Solution.ArrangeFiles);
			menuCommand.BeforeQueryStatus += QueryStatus;
		}
		public MenuCommand MenuCommand
		{
			get { return menuCommand; }
		}
		public void Execute(object sender, EventArgs e)
		{
			projectEngine.AutoArrangeFiles(vsEngine.GetSolutionProjects());
		}
		private void QueryStatus(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (null == command)
			{
				return;
			}
			if (projectEngine.CanArrangeFiles(vsEngine.GetSolutionProjects()))
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