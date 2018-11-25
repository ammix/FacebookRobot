using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests
{
	public class Class1
	{
		public static void Test()
		{
			//string emailPattern = "[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})";
			//var emailPattern = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

			var data = "Добрый день olga_savch@meta.ua 050-550-99-93";
			var testData = new List<string>
			{
				"ammix@ukr.net +380986603456",
				"ammix@ukr.net 0986603456",
				"ammix@ukr.net 098 6603456",
				"ammix@ukr.net 098 660 34 56",
				"ammix@ukr.net 098-660-34-56",
				"ammix@ukr.net (098)6603456",
				"ammix@ukr.net (098) 6603456",
				"ammix@ukr.net (098) 660 34 56",
				"ammix@ukr.net (098)-660-34-56",
				"ammix@ukr.net (098) 660-34-56",
				"ammix@ukr.net 098 660-34-56",
				"ammix@ukr.net 098  660  34-56",
				"ammix@ukr.net 098 66 03 456",
				"ammix@ukr.net 098 6 6 0 3 4 5 6",
				"ammix@ukr.net 098 6 6 0-3 4 5 6",
				"ammix@ukr.net 0 9 8 6 6 0 3 4 5 6",
				"ammix@ukr.net +380 98 6 6 0 3 4 5 6"
			};

			var emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

			//var phoneRegex = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");

			var phoneRegex = new Regex(@"\(?0(-? *\d){2}\)?(-? *\d){7}"); // (@"\(?0\d{2}\)?(-? *\d){7}");

			//var email = emailRegex.Match(testData[]);
			foreach (var t in testData)
			{
				var phone = phoneRegex.Match(t).ToString();
				phone = "38" + phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

				Console.WriteLine($"{t}: {phone}");
			}
		}
	}
}
