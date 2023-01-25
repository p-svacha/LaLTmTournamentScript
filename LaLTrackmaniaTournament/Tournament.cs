using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{
    public class Tournament
    {
        public List<int> ScoringSystem;
        public List<string> Players;
        public List<Track> Tracks;
        public int NumTracks;

        public Dictionary<string, int> PlayerPoints;

        public Tournament(List<int> scoringSystem, List<string> players, List<Track> tracks)
        {
            ScoringSystem = scoringSystem;
            Players = players;
            Tracks = tracks;
            NumTracks = tracks.Count;
            PlayerPoints = new Dictionary<string, int>();
        }

        public void UpdateLeaderboards(GoogleSheet sheet)
        {
            PlayerPoints.Clear();

            // Get leaderboards
            Console.WriteLine("\n########## " + DateTime.Now + "  UPDATING LEADERBOARDS ##########");
            foreach (Track track in Tracks)
            {
                // Get full leaderboard of track with all players
                List<LeaderboardEntry> leaderboard = track.UpdateLeaderboard(participants: Players);

                // Add points of that track to total leaderboard
                Console.WriteLine("\nLeaderboard for " + track.Name + ":");
                foreach(LeaderboardEntry entry in leaderboard)
                {
                    if(PlayerPoints.ContainsKey(entry.Player)) PlayerPoints[entry.Player] += ScoringSystem[entry.Position - 1];
                    else PlayerPoints[entry.Player] = ScoringSystem[entry.Position - 1];
                    Console.WriteLine(entry.ToString());
                }

                Thread.Sleep(1000); // to not overload API
            }

            // Calculate points
            Console.WriteLine("\nTOTAL Leaderboard:");
            PlayerPoints = PlayerPoints.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int rank = 1;
            foreach(KeyValuePair<string, int> rankingEntry in PlayerPoints)
            {
                Console.WriteLine(rank + ". " + rankingEntry.Key + ", " + rankingEntry.Value);
                rank++;
            }

            // Write to google sheets
            List<List<string>> data = new List<List<string>>();

            // Row 1
            List<string> row1 = new List<string>() { "Zwischenstand Spieler" };
            for (int i = 0; i < 4 + 2 * NumTracks; i++) row1.Add("");
            row1.Add("Zwischenstand Strecken");
            data.Add(row1);

            // Row 2
            List<string> row2 = new List<string>() { "Rang", "Spieler", "Punkte Total" };
            foreach(Track track in Tracks)
            {
                row2.Add(track.Name);
                row2.Add("");
            }
            row2.Add("Zuletzt aktualisiert:");
            row2.Add("Rang");
            foreach (Track track in Tracks)
            {
                row2.Add(track.Name);
                row2.Add("");
            }
            data.Add(row2);

            // Row 3
            List<string> row3 = new List<string>() { "", "", "" };
            foreach (Track track in Tracks)
            {
                row3.Add("Rang");
                row3.Add("Punkte");
            }
            row3.Add(DateTime.Now.ToString());
            row3.Add("");
            foreach (Track track in Tracks)
            {
                row3.Add("Spieler");
                row3.Add("Zeit");
            }
            data.Add(row3);

            // Rows 4+
            int currentRank = 1;
            foreach (KeyValuePair<string, int> entry in PlayerPoints)
            {
                string player = entry.Key;
                List<string> row = new List<string>() { currentRank.ToString(), player, entry.Value.ToString() };
                foreach (Track track in Tracks)
                {
                    LeaderboardEntry playerPos = track.Leaderboard.FirstOrDefault(x => x.Player == player);
                    if(playerPos == null)
                    {
                        row.Add("");
                        row.Add("");
                    }
                    else
                    {
                        row.Add(playerPos.Position.ToString());
                        row.Add(ScoringSystem[playerPos.Position - 1].ToString());
                    }
                }
                row.Add("");
                row.Add(currentRank.ToString());

                foreach (Track track in Tracks)
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
                data.Add(row);
            }


            sheet.WriteCells("LaL13", data);
        }
    }
}
