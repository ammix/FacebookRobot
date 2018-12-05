using System;
using System.Configuration;
using System.IO;

namespace FacebookRobot
{
	class Program
	{		
		static void Main(string[] args)
		{
			var facebookLogin = ConfigurationManager.AppSettings["FacebookLogin"];
			var facebookPass = ConfigurationManager.AppSettings["FacebookPass"];
			var facebookGroupId = ConfigurationManager.AppSettings["FacebookGroupId"];
			var memberProcessingDelay = int.Parse(ConfigurationManager.AppSettings["MemberProcessingDelay"]);

			var googleSpreadsheetId = ConfigurationManager.AppSettings["GoogleSpreadsheetId"];
			var excelFilePath = ConfigurationManager.AppSettings["ExcelFilePath"];

			Console.WriteLine("FacebookRobot started.");
			var spreadSheetsWriter = new GoogleSpreadSheetsWriter(googleSpreadsheetId);
			var excelFileWriter = new ExcelFileWriter(excelFilePath);

			var facebookRobot = new FacebookRobot(facebookGroupId);
			facebookRobot.FacebookLogin(facebookLogin, facebookPass);

			var writers = new IWriter[] { spreadSheetsWriter, excelFileWriter };
			facebookRobot.SaveNewMemberContactsAndAddToGroup(writers, memberProcessingDelay);

			excelFileWriter.Dispose();
			facebookRobot.Close();

			Console.WriteLine("FacebookRobot stopped.");
		}
	}
}
