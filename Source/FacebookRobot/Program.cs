using System.Configuration;
using FacebookRobot.Writers;

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

			var spreadSheetsWriter = new GoogleSpreadSheetsWriter(googleSpreadsheetId);
			var excelFileWriter = new ExcelFileWriter(excelFilePath);				
			var consoleWriter = new ConsoleWriter();
			//consoleWriter.Write("FacebookRobot started.");

			var facebookRobot = new FacebookRobot(facebookGroupId);
			facebookRobot.FacebookLogin(facebookLogin, facebookPass);

			var writers = new IWriter[] { spreadSheetsWriter, excelFileWriter, consoleWriter }; //TODO: if failed writes into spreadsheet multiple times
			facebookRobot.ProcessNewMemberRequests(writers, memberProcessingDelay);

			excelFileWriter.Dispose();
			facebookRobot.Close();

			//consoleWriter.Write("FacebookRobot stopped.");
		}
	}
}
