using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
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
        static string ApplicationName = "LaL Trackmania Tournament";
        static string SpreadSheetId = "1BeVSvlm3uZCIV302iWv8bzSZ0gOFzY9Tq5WtuVzdivc";

        static void Main(string[] args)
        {
            
            GoogleSheet googleSheet = new GoogleSheet(ApplicationName, SpreadSheetId);

            /*
            List<List<string>> writeValues = new List<List<string>>();
            for (int y = 0; y < 10; y++)
            {
                writeValues.Add(new List<string>());
                for(int x =0; x < 10; x++)
                {
                    writeValues[y].Add(x + "/" + y);
                }
            }

            googleSheet.WriteCells("LaL13", writeValues);
            */

            // Tournament
            List<int> scoringSystem = new List<int>()
            {
                50, 45, 41, 38, 35, 32, 30, 28, 26, 24, 22, 20, 18, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
            };
            List<string> players = new List<string>()
            {
                "Micka_TM.",
                "xTigershark777",
                "Teuflum",
                "twist__",
                "F9.eLconn21",
                "Mury99",
                "Novu.",
                "mimexdd",
                "ShcrTM",
                "crispp..",
                "ImotJr.",
                "Mawi__",
                "DexteR.771",
                "Magorian0212",
                "L0udis",
                "Scrapie98",
                "Hobbit.TM",
                "GNR-HASKELL",
                "Ikarus.",
                "sophie.ice",
                "bead-",
                "jandro-._",
            };
            List<Track> tracks = new List<Track>()
            {
                new Track("Glacialis", "5P0k2lR5MP_kIGDMv2pJUfgEmzm"),
                new Track("ThunderStorm", "vVYgYYofH1tTGsprVDMYExZlHBj"),
                new Track("OPUS", "Cyea0ZPP1AVqzqiqLezlCYw5LP7"),
                new Track("Ensorcellment", "rmsEpxmBoDpKDQH70zrNbxdb0j3"),
                new Track("Rcadia Mix - Oxygen", "FnkPqlcbilVydv7NgGL_2dUSPxf"),
                new Track("Last Outpost", "YewzuEnjmnh_ShMW1cX0puuZHcf"),
            };
            Tournament tournament = new Tournament(scoringSystem, players, tracks);

            while (true)
            {
                tournament.UpdateLeaderboards(googleSheet);
                Thread.Sleep(900000); // update every 15 minutes
            }
        }
    }


}
