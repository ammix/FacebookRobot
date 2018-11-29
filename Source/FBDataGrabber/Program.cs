using System;
using System.Configuration;

namespace FacebookDataGrabber
{
	class Program
	{		
		/*
		private static readonly string facebookLogin = "lu.brendell@gmail.com";
		private static readonly string facebookPass = "{123456}";
		private static readonly string facebookGroupId = "770808563258590"; //https://www.facebook.com/groups/770808563258590/
		private static readonly string googleSpreadsheetId = "1k73Yo5gftGXyfDdZl6pfbYGlKIUnr61OMllCWSNLRCc";

		//private static readonly string facebookGroupId = "736242360081825"; Dugelna //256686171663733 U.Pak
		//private static readonly string googleSpreadsheetId = "14OYRfQ2-qdaqUvDOsdz08mwlSEyNZUeoYE0KRV1CAow"; //Dugelna
		//private static readonly string excelFilename = "Uliana.Pak.csv";
		private static readonly int memberProcessingDelay = 500;
		*/

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

			var facebookRobot = new FacebookRobot(facebookGroupId);
			facebookRobot.FacebookLogin(facebookLogin, facebookPass);
			facebookRobot.SaveNewMemberContactsAndAddToGroup(spreadSheetsWriter, memberProcessingDelay);
			facebookRobot.Close();

			//using (var writer = File.AppendText(excelFilename))
			//{
			//	var facebookRobot = new FacebookRobot(facebookGroupId);
			//	facebookRobot.FacebookLogin(facebookLogin, facebookPass);
			//	Thread.Sleep(2000);
			//	facebookRobot.SaveDataAndConfirmGroupMembers(spreadSheetsWriter); //GrabTheData
			//	facebookRobot.Close();
			//}

			Console.WriteLine("FacebookRobot stopped.");
		}
	}
}
