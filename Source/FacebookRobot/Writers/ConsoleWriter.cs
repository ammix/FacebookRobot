using System;
using System.Text;

namespace FacebookRobot.Writers
{
	public class ConsoleWriter : IWriter
	{
		public void Write(params string[] columnsData)
		{
			var name = columnsData[0] ?? "";
			var email = columnsData[2] ?? "";
			var phone = columnsData[3] ?? "";
			var data = columnsData[4] ?? "";

			var sb = new StringBuilder();
			sb.Append(name);
			sb.Append(new String(' ', 35 - name.Length));
			sb.Append(email);
			sb.Append(new String(' ', 35 - email.Length));
			sb.Append(phone);
			sb.Append(new String(' ', 25 - phone.Length));
			sb.Append(data);

			Console.WriteLine(sb);
		}
	}
}
