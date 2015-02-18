namespace ksw.projectifier
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using EnvDTE;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	public class ArrangeFilesOptions : IOptions
	{
		private readonly HashSet<string> enabledProjects;
		public ArrangeFilesOptions()
		{
			IsEnabledForSolution = false;
			enabledProjects = new HashSet<string>();
		}
		public string Key
		{
			get { return "arrange-files"; }
		}
		public bool IsEnabledForSolution { get; set; }
		public void Save(Stream stream)
		{
			if (!IsEnabledForSolution && !enabledProjects.Any())
			{
				return;
			}
			using (var textWriter = new StreamWriter(stream))
			using (var jsonWriter = new JsonTextWriter(textWriter))
			{
				var serializer = JsonSerializer.CreateDefault();
				serializer.Serialize(
					jsonWriter,
					new
						{
							solution = IsEnabledForSolution,
							projects = enabledProjects.ToArray()
						});
			}
		}
		public void Load(Stream stream)
		{
			if (0 == stream.Length)
			{
				return;
			}
			using (var textReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(textReader))
			{
				var serializer = JsonSerializer.CreateDefault();
				var options = (dynamic)serializer.Deserialize(jsonReader);
				IsEnabledForSolution = ((bool?) options.solution) ?? false;
				if (null != options.projects)
				{
					foreach (var project in ((JArray) options.projects).ToObject<string[]>())
					{
						enabledProjects.Add(project);
					}
				}
			}
		}
		public void EnableAutomaticArrange(Project project)
		{
			enabledProjects.Add(project.UniqueName);
		}
		public bool IsAutomaticArrangeEnabled(Project project)
		{
			return IsEnabledForSolution || enabledProjects.Contains(project.UniqueName);
		}
		public void ToggleAutoArrange(Project project)
		{
			if (!enabledProjects.Remove(project.UniqueName))
			{
				enabledProjects.Add(project.UniqueName);
			}
		}
		public void ToggleAutoArrangeForSolution()
		{
			IsEnabledForSolution = !IsEnabledForSolution;
		}
		public void Reset()
		{
			IsEnabledForSolution = false;
			enabledProjects.Clear();
		}
	}
}