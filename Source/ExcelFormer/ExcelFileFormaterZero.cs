using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Tests
{
	class ExcelFileFormerZero
	{
		public static void FormFile()
		{
			//var text1 = "Добрый день olga_savch@meta.ua 050 550 99-93";
			//var text2 = "liliaheppi @gmail. com. тел. 0996153183";
			//var text3 = "Dmitrenkoyulia82@gmail.com 063 8446519";
			//var text4 = "+380957168496";
			//var q = FindEmailAndPhone(text4);

			using (var reader = new StreamReader("emails_and_phones.csv"))
			using (var writer = new StreamWriter("emails_and_phones_PARSED.csv"))
			{
				while (reader.Peek() != -1)
				{
					var stringData = reader.ReadLine();
					if (string.IsNullOrEmpty(stringData))
						continue;

					stringData = ",," + stringData;

					var columns = stringData.Split(',');

					var sb = new StringBuilder();
					for (var i = 2; i < columns.Length; i++)
						sb = sb.Append(columns[i]);

					var userData = sb.ToString();
					var email = FindEmail(userData);
					var phone = FindPhone(userData);

					if (email != null && phone != null)
						userData = null;
					if (email == userData || phone == userData)
						userData = null;

					writer.WriteLine($"{columns[0]},{columns[1]},{email},{phone},{userData}");
				}
			}
		}

		private static string FindEmail(string data)
		{
			var emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
			return emailRegex.Match(data).Value;
		}

		private static string FindPhone(string data)
		{
			var emailRegex = new Regex(@"\(?0\d{2}\)?-? *\d{3}-? *\d{2}-? *\d{2}");
			var phone = emailRegex.Match(data).Value;

			if (string.IsNullOrEmpty(phone))
				return null;

			var sb = new StringBuilder("38" + phone);
			sb = sb.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
			return sb.ToString();
		}
	}
}
