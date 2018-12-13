using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SheetsQuickstart
{
	class Program
	{
		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
		static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static string ApplicationName = "Google Sheets API .NET Quickstart";

		static void Main(string[] args)
		{
			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None
					/*new FileDataStore(credPath, true)*/).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			// Create Google Sheets API service.
			var service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			// Define request parameters.
			String spreadsheetId = "1mBYYx3WFGayXH9BjVKS6CGLSUq9RYsHOeaLQC33qf88";
			String range = "A:B"; // "Class Data!A:C";
			//SpreadsheetsResource.ValuesResource.GetRequest request =
			//	service.Spreadsheets.Values.Get(spreadsheetId, range);

			// Prints the names and majors of students in a sample spreadsheet:
			// https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit

			IList<object> q = new List<object>();
			//q.Add("Іванна Іванівна");
			//q.Add("123456");
			q.Add("=HYPERLINK(\"https://www.facebook.com/100003905187715\"; \"Elisabeth Bondarenko\")");

			var newData = new ValueRange();
			newData.Values = new List<IList<object>>();
			newData.Values.Add(q);



			var appendRequest = service.Spreadsheets.Values.Append(newData, spreadsheetId, range);

			var valueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
				//INPUTVALUEOPTIONUNSPECIFIED; // (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)0;  // TODO: Update placeholder value.
			appendRequest.ValueInputOption = valueInputOption;

			var response2 = appendRequest.Execute();
			return;
			
			////service.Spreadsheets.Sheets.

			//ValueRange response = request.Execute();
			//IList<IList<Object>> values = response.Values;
			//if (values != null && values.Count > 0)
			//{
			//	Console.WriteLine("Name, Major");
			//	foreach (var row in values)
			//	{
			//		// Print columns A and E, which correspond to indices 0 and 4.
			//		Console.WriteLine("{0}, {1}", row[0], row[1]);
			//	}
			//}
			//else
			//{
			//	Console.WriteLine("No data found.");
			//}
			//Console.Read();
		}
	}
}