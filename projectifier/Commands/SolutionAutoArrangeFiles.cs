namespace ksw.projectifier.Commands
{
	using System;
	using System.ComponentModel.Design;
	using Microsoft.VisualStudio.Shell;
	class SolutionAutoArrangeFiles
	{
		private readonly VsEngine vsEngine;
		private readonly ProjectEngine projectEngine;
		private readonly OleMenuCommand menuCommand;
		public SolutionAutoArrangeFiles(
			VsEngine vsEngine,
			ProjectEngine projectEngine)
		{
			this.vsEngine = vsEngine;
			this.projectEngine = projectEngine;
			menuCommand = new OleMenuCommand(
				Execute,
				ProjectifierIDs.Solution.AutoArrangeFiles);
			menuCommand.BeforeQueryStatus += QueryStatus;
		}
		public MenuCommand MenuCommand
		{
			get { return menuCommand; }
		}
		public void Execute(object sender, EventArgs e)
		{
			projectEngine.Options.ToggleAutoArrangeForSolution();
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
				command.Visible = true;
				command.Enabled = true;
				command.Checked = projectEngine.Options.IsEnabledForSolution;
				command.Text = command.Checked
					? "Disable auto arrange files in all projects"
					: "Enable auto arrange files in all projects";
			}
			else
			{
				command.Visible = false;
				command.Enabled = false;
			}
		}
	}
}