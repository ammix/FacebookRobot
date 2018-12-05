using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests
{
	public static class Class2
	{
		public static void Process()
		{
			using (var writer = File.AppendText(@"C:\Users\maksym.brendel\Dropbox\#share\export_clients_20181126160929_phones+.csv"))
			using (var reader = File.OpenText(@"C:\Users\maksym.brendel\Dropbox\#share\export_clients_20181126160929_phones.csv"))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null) break;

					var goodPhone = FindPhone(line);
					writer.WriteLine(goodPhone);
					Console.WriteLine(goodPhone);
				}
			}
		}

		public static string FindPhone(string data)
		{
			if (data == null) return null;

			var emailRegex = new Regex(@"\(?0(-? *\d){2}\)?(-? *\d){7}");
			var phone = emailRegex.Match(data).Value;

			if (string.IsNullOrEmpty(phone))
				return null;

			var sb = new StringBuilder("38" + phone);
			sb = sb.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
			return sb.ToString();
		}
	}
}
