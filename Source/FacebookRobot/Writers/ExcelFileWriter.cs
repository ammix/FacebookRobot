using System;
using System.IO;
using System.Text;

namespace FacebookRobot.Writers
{
	public class ExcelFileWriter : IWriter, IDisposable
	{
		readonly StreamWriter writer;

		public ExcelFileWriter(string filePath)
		{
			writer = File.AppendText(filePath);
		}

		public void Write(params string[] columnsData)
		{
			var sb = new StringBuilder();
			foreach (var column in columnsData)
				sb.Append($"{column}\t");

			//sb.Remove(sb.Length-2, 1); // remove last '\t'

			writer.WriteLine(sb);
			writer.Flush();
		}

		public void Dispose()
		{
			writer.Dispose();
		}
	}
}
