namespace ksw.projectifier
{
	using System.IO;
	public interface IOptions
	{
		string Key { get; }
		void Save(Stream stream);
		void Load(Stream stream);
	}
}