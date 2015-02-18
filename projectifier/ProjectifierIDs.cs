namespace ksw.projectifier
{
	using System;
	using System.ComponentModel.Design;
	static class ProjectifierIDs
	{
		public const string PackageName = "projectifier";
		public const string PackageString = "6bb65fc2-68d2-4b1a-b9aa-c18b85b8b1ba";
		public static class Project
		{
			public static readonly Guid CommandSet =
				new Guid("ff15fc74-35dc-41eb-b557-a7e7184b862a");
			public static readonly CommandID ArrangeFiles =
				new CommandID(CommandSet, 0x0100);
			public static readonly CommandID AutoArrangeFiles =
				new CommandID(CommandSet, 0x0101);
		}
		public static class Solution
		{
			public static readonly Guid CommandSet =
				new Guid("77D5D091-657C-494C-85A9-AF7CD387C0F3");
			public static readonly CommandID ArrangeFiles = new CommandID(
				CommandSet, 0x0100);
			public static readonly CommandID AutoArrangeFiles = new CommandID(
				CommandSet, 0x0101);
		}
	}
}