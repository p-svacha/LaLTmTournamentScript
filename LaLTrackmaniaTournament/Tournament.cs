using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{
    /// <summary>
    /// A tournament is a package that holds all information required to calculate the rankings.
    /// </summary>
    public class Tournament
    {
        /// <summary>
        /// Path to the json file that contains all information about the tournament.
        /// </summary>
        private string TournamentJsonPath;

        public Dictionary<string, int> PlayerPoints;

        public Tournament(string jsonPath)
        {
            TournamentJsonPath = jsonPath;
            PlayerPoints = new Dictionary<string, int>();
        }

        public void UpdateLeaderboards(GoogleSheet sheet)
        {
            // Reload tournament data to get all updated info
            TournamentData tournamentData = TournamentData.CreateFromJson(TournamentJsonPath);

            // Recreate track objects
            List<Track> tracks = new List<Track>();
            foreach (TrackData trackData in tournamentData.Tracks) tracks.Add(new Track(trackData));

            // Clear leaderboard
            PlayerPoints.Clear();

            // Get leaderboards
            Console.WriteLine("\n########## " + DateTime.Now + "  UPDATING LEADERBOARDS ##########");
            foreach (Track track in tracks)
            {
                // Get full leaderboard of track with all players
                List<LeaderboardEntry> leaderboard = track.UpdateLeaderboard(tournamentData.TrackmaniaApiInfo, tournamentData.Players);

                // Add points of that track to total leaderboard
                Console.WriteLine("\nLeaderboard for " + track.Name + ":");
                foreach (LeaderboardEntry entry in leaderboard)
                {
                    if (PlayerPoints.ContainsKey(entry.Player)) PlayerPoints[entry.Player] += tournamentData.Points[entry.Position - 1];
                    else PlayerPoints[entry.Player] = tournamentData.Points[entry.Position - 1];
                    Console.WriteLine(entry.ToString());
                }

                Thread.Sleep(1000); // to not overload API
            }

            // Calculate points
            Console.WriteLine("\nTOTAL Leaderboard:");
            PlayerPoints = PlayerPoints.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int rank = 1;
            foreach (KeyValuePair<string, int> rankingEntry in PlayerPoints)
            {
                Console.WriteLine(rank + ". " + rankingEntry.Key + ", " + rankingEntry.Value);
                rank++;
            }

            // Write to google sheets
            WriteLeaderboardToGoogleSheet(tournamentData, tracks, sheet);
        }

        private void WriteLeaderboardToGoogleSheet(TournamentData tournamentData, List<Track> tracks, GoogleSheet sheet)
        { 
            List<List<string>> sheetContent = new List<List<string>>();

            // Row 1
            List<string> row1 = new List<string>() { "Standings" };
            for (int i = 0; i < 2 * tracks.Count + 3; i++) row1.Add("");
            row1.Add("Track Leaderboards");
            sheetContent.Add(row1);

            // Row 2
            List<string> row2 = new List<string>() { "Rank", "Player", "Points" };
            foreach (Track track in tracks)
            {
                row2.Add(track.Name);
                row2.Add("");
            }
            row2.Add("Last Updated:");
            row2.Add("Rank");
            foreach (Track track in tracks)
            {
                row2.Add(track.Name);
                row2.Add("");
            }
            sheetContent.Add(row2);

            // Row 3
            List<string> row3 = new List<string>() { "", "", "" };
            foreach (Track track in tracks)
            {
                row3.Add("Rank");
                row3.Add("Points");
            }
            row3.Add(DateTime.Now.ToString());
            row3.Add("");
            foreach (Track track in tracks)
            {
                row3.Add("Player");
                row3.Add("Time");
            }
            sheetContent.Add(row3);

            // Rows 4+
            int currentRank = 1;
            foreach (KeyValuePair<string, int> entry in PlayerPoints)
            {
                string player = entry.Key;
                List<string> row = new List<string>() { currentRank.ToString(), player, entry.Value.ToString() };
                foreach (Track track in tracks)
                {
                    LeaderboardEntry playerPos = track.Leaderboard.FirstOrDefault(x => x.Player == player);
                    if (playerPos == null)
                    {
                        row.Add("");
                        row.Add("");
                    }
                    else
                    {
                        row.Add(playerPos.Position.ToString());
                        row.Add(tournamentData.Points[playerPos.Position - 1].ToString());
                    }
                }
                row.Add("");
                row.Add(currentRank.ToString());

                foreach (Track track in tracks)
                {
                    LeaderboardEntry rankEntry = track.Leaderboard.FirstOrDefault(x => x.Position == currentRank);
                    if (rankEntry == null)
                    {
                        row.Add("");
                        row.Add("");
                    }
                    else
                    {
                        row.Add(rankEntry.Player);
                        row.Add(rankEntry.GetTimeFormatted());
                    }
                }


                currentRank++;
                sheetContent.Add(row);
            }


            sheet.WriteCells(tournamentData.GoogleSheetInfo.SheetName, sheetContent);
        }
    }

}