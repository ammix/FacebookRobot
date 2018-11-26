using System;

namespace FacebookDataGrabber
{
	class Program
	{
		private static readonly string facebookLogin = "lu.brendell@gmail.com";
		private static readonly string facebookPass = "{123456}";
		private static readonly string facebookGroupId = "736242360081825"; //256686171663733 U.Pak
		private static readonly string googleSpreadsheetId = "14OYRfQ2-qdaqUvDOsdz08mwlSEyNZUeoYE0KRV1CAow";
		//private static readonly string excelFilename = "Uliana.Pak.csv";
		//private static readonly int dalayForProcessingOneMember = 500;

		static void Main(string[] args)
		{
			Console.WriteLine("FacebookRobot started.");
			var spreadSheetsWriter = new GoogleSpreadSheetsWriter(googleSpreadsheetId);

			var facebookRobot = new FacebookRobot(facebookGroupId);
			facebookRobot.FacebookLogin(facebookLogin, facebookPass);
			facebookRobot.SaveDataAndConfirmGroupMembers(spreadSheetsWriter/*, dalayForProcessingOneMember*/);
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
