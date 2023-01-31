using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{

    public class GoogleSheet
    {

        private string[] Scopes = { SheetsService.Scope.Spreadsheets }; // Change this if you're accessing Drive or Docs
        private string SpreadSheetId = "1BeVSvlm3uZCIV302iWv8bzSZ0gOFzY9Tq5WtuVzdivc";
        public SheetsService SheetsService { get; private set; }

        /// <summary>
        /// ID has to be stored in "credentials.json" in the root directory with "Copy always" set in its properties.
        /// <br/> The content for that file can be found in my project overview google tables sheet.
        /// </summary>

        public GoogleSheet(string applicationName, string spreadSheetId)
        {

            SpreadSheetId = spreadSheetId;

            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy
            try
            {
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                // Create Google Sheets API service.
                SheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });

                Console.WriteLine("Successfully connected to the Google Spreadsheet.\n");
            } 
            
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("\n\"credentials.json\" is missing in the root directory or has not set \"Copy always\" in its properties.");
                Console.WriteLine("If the file is missing, the content for it can be found in my project overview google tables sheet.\n");
            }      
        }

        /// <summary>
        /// Writes the cells of the sheet with the given values, starting at A1
        /// </summary>
        public void WriteCells(string sheetName, List<List<string>> values)
        {
            string range = sheetName + "!A1:AZ";
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";//COLUMNS

            List<IList<object>> cellValues = new List<IList<object>>();
            foreach (List<string> row in values)
            {
                List<object> rowValues = new List<object>();
                foreach (string cell in row) rowValues.Add(cell);
                cellValues.Add(rowValues);
            }
            valueRange.Values = cellValues;

            SpreadsheetsResource.ValuesResource.UpdateRequest update = SheetsService.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result = update.Execute();
        }
    }
}
