namespace ksw.projectifier.Commands
{
	using System;
	using System.ComponentModel.Design;
	using Microsoft.VisualStudio.Shell;
	class ProjectAutoArrangeFiles
	{
		private readonly VsEngine vsEngine;
		private readonly ProjectEngine projectEngine;
		private readonly OleMenuCommand menuCommand;
		public ProjectAutoArrangeFiles(
			VsEngine vsEngine,
			ProjectEngine projectEngine)
		{
			this.vsEngine = vsEngine;
			this.projectEngine = projectEngine;
			menuCommand = new OleMenuCommand(
				Execute,
				ProjectifierIDs.Project.AutoArrangeFiles);
			menuCommand.BeforeQueryStatus += QueryStatus;
		}
		public MenuCommand MenuCommand
		{
			get { return menuCommand; }
		}
		public void Execute(object sender, EventArgs e)
		{
			projectEngine.Options.ToggleAutoArrange(
				vsEngine.GetCurrentProject());
		}
		private void QueryStatus(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (null == command)
			{
				return;
			}
			var project = vsEngine.GetCurrentProject();
			if (projectEngine.CanArrangeFiles(project))
			{
				command.Visible = true;
				command.Enabled = !projectEngine.Options.IsEnabledForSolution;
				command.Checked = projectEngine.Options
					.IsAutomaticArrangeEnabled(project);
				command.Text = command.Enabled && command.Checked
					? "Disable auto arrange files in project"
					: "Enable auto arrange files in project";
			}
			else
			{
				command.Visible = false;
				command.Enabled = false;
				command.Checked = false;
			}
		}
	}
}