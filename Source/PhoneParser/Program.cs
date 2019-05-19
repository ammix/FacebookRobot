using System.IO;
using FacebookRobot;

namespace PhoneParser
{
	class Program
	{
		static void Main(string[] args)
		{
			var inFile = "in.csv";
			var outFile = "out.csv";

			using (var reader = new StreamReader(inFile))
			using (var writer = new StreamWriter(outFile))
			{
				string line;
				do
				{
					line = reader.ReadLine();
					var phone = ContactsParser.FindPhone(line) ?? line;
					writer.WriteLine(phone);
				} while (line != null);
			}
		}
	}
}
