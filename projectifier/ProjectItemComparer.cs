namespace ksw.projectifier
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	class ProjectItemComparer : IComparer<Microsoft.Build.Evaluation.ProjectItem>
	{
		public int Compare(Microsoft.Build.Evaluation.ProjectItem x, Microsoft.Build.Evaluation.ProjectItem y)
		{
			return String.Compare(
				x.EvaluatedInclude,
				y.EvaluatedInclude,
				CultureInfo.InvariantCulture,
				CompareOptions.None);
		}
	}
}