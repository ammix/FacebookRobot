using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FacebookRobot.Writers
{
	public class GoogleSpreadSheetsWriter: IWriter
	{
		static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static readonly string ApplicationName = "FacebookRobot";

		readonly SheetsService service;
		readonly string spreadsheetId;

		public GoogleSpreadSheetsWriter(string spreadsheetId)
		{
			UserCredential credential;

			using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None).Result;
			}

			service = new SheetsService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			this.spreadsheetId = spreadsheetId;
		}

		public void Write(params string[] columnsData)
		{
			if (columnsData[2] == null && columnsData[3] == null && columnsData[4] == null)
				return;

			const string range = "A:D";

			var columnList = new List<object>
			{
				$"=HYPERLINK(\"{columnsData[1]}\"; \"{columnsData[0]}\")", columnsData[2], columnsData[3], columnsData[4]
			};
			var body = new ValueRange {Values = new List<IList<object>> {columnList}};

			var appendRequest = service.Spreadsheets.Values.Append(body, spreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

			var response = appendRequest.Execute();
		}
	}
}