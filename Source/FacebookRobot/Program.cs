﻿using System.Configuration;
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
			var memberProcessingDelay = int.Parse(ConfigurationManager.AppSettings["MemberProcessingDelayInMs"]);
			var refreshDelay = 60 * 1000 * int.Parse(ConfigurationManager.AppSettings["RefreshDelayInMin"]);

			var googleSpreadsheetId = ConfigurationManager.AppSettings["GoogleSpreadsheetId"];
			var excelFilePath = ConfigurationManager.AppSettings["ExcelFilePath"];
			var chromeOptions = ConfigurationManager.AppSettings["ChromeOptions"];

			var spreadSheetsWriter = new GoogleSpreadSheetsWriter(googleSpreadsheetId);
			var excelFileWriter = new ExcelFileWriter(excelFilePath);
			var consoleWriter = new ConsoleWriter();
			//consoleWriter.Write("FacebookRobot started.");

			var facebookRobot = new FacebookRobot(chromeOptions, facebookGroupId);
			facebookRobot.FacebookLogin(facebookLogin, facebookPass);

			var writers = new IWriter[] { spreadSheetsWriter, excelFileWriter, consoleWriter }; //TODO: if failed writes into spreadsheet multiple times
			facebookRobot.ProcessNewMemberRequests(writers, memberProcessingDelay, refreshDelay);

			excelFileWriter.Dispose();
			facebookRobot.Close();

			//consoleWriter.Write("FacebookRobot stopped.");
		}
	}
}
