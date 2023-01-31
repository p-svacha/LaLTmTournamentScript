using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace LaLTrackmaniaTournament
{
    /// <summary>
    /// To make it work, add the service account 
    /// kartenspielreader-937@kartenspiel-1604237807698.iam.gserviceaccount.com
    /// too the allowed editors of the sheet!
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("------------- Welcome to the Trackmania Tournament Script ------------\n");

            Console.WriteLine("Paste the path to the json file of the tournament you want to run. (You can just drag and drop the file in the console)");
            Console.WriteLine("For an example of how the file should look see {program_path}/Tournaments/example_tournament.json");

            string tournamentJsonPath = Console.ReadLine();

            // Check if file exists
            if(!File.Exists(tournamentJsonPath))
            {
                Console.WriteLine("The file " + tournamentJsonPath + " does not exist");
                Console.ReadKey();
                return;
            }

            // Create tournament data from JSON
            TournamentData tournamentData = TournamentData.CreateFromJson(tournamentJsonPath);

            // Connect to Google Sheets
            GoogleSheet googleSheet = new GoogleSheet(tournamentData.GoogleSheetInfo.ApplicationName, tournamentData.GoogleSheetInfo.DocumentId);

            if(googleSheet.SheetsService == null)
            {
                Console.WriteLine("Error occured - Aborting...");
                Console.ReadKey();
                return;
            }

            // Create tournament object
            Tournament tournament = new Tournament(tournamentJsonPath);

            // Loop update every x minutes
            while (true)
            {
                tournament.UpdateLeaderboards(googleSheet);
                Thread.Sleep(900000); // update every 15 minutes
            }
        }
    }


}
