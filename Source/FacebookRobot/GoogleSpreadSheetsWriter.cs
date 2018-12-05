using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FacebookRobot
{
	public interface IWriter
	{
		void Write(params string[] columnsData);
	}

	public class ExcelFileWriter : IWriter, IDisposable
	{
		readonly StreamWriter writer;

		public ExcelFileWriter(string filePath)
		{
			writer = File.AppendText(filePath);
		}

		public void Write(params string[] columnsData)
		{
			var sb = new StringBuilder();
			foreach (var column in columnsData)
				sb.Append($"{column}\t");

			//sb.Remove(sb.Length-2, 1); // remove last '\t'

			writer.WriteLine(sb);
			writer.Flush();

			Console.WriteLine(sb);


			//writer.Write(hyperlink.Name, hyperlink.Link, email, phone, adjustedData);

			//var q = string.Format(columnsData);

			//foreach (var column in columnsData)
			//	writer.Write($"{column},");

			//foreach (var column in columnsData)
			//	writer.Write($"{column},");
			//writer.WriteLine();

			//string text = $"{hyperlink.Name},{hyperlink.Link},{email},{phone},{adjustedData}";
		}

		public void Dispose()
		{
			writer.Dispose();
		}
	}

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
			const string range = "A:E";

			var columnList = new List<object>(columnsData);
			var body = new ValueRange {Values = new List<IList<object>> {columnList}};

			var appendRequest = service.Spreadsheets.Values.Append(body, spreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

			var response = appendRequest.Execute();
		}
	}
}