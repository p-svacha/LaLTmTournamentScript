using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{
    public class Track
    {
        public string Id;
        public string Name;

        public List<LeaderboardEntry> Leaderboard;

        public Track(string name, string id)
        {
            Id = id;
            Name = name;
        }

        public Track(TrackData data)
        {
            Id = data.Id;
            Name = data.Name;
        }

        public List<LeaderboardEntry> UpdateLeaderboard(string apiCallInfo, List<string> participants = null)
        {
            List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = apiCallInfo;

                // Get the leaderboard from the trackmania.io API as a JSON object. One request will return the top 50 times.
                // If more than 50 records are needed, use leaderboard/map/[id]?offset=50&length=50, incrementing offset by 50 per step.
                string result = client.DownloadString("https://trackmania.io/api/leaderboard/map/" + Id + "?length=50");

                string[] playerSplit = result.Split("{\"player\":{\"name\":\"");
                int position = 1;
                foreach (string playerResult in playerSplit)
                {
                    string[] nameSplit = playerResult.Split("\"");
                    string name = nameSplit[0];
                    if (participants == null || participants.Contains(name))
                    {
                        if (name == "{") continue;
                        string[] timeSplit = playerResult.Split("\"time\":");
                        string[] timeSplit2 = timeSplit[1].Split(",");
                        int time = int.Parse(timeSplit2[0]);

                        LeaderboardEntry entry = new LeaderboardEntry(name, position++, time);
                        leaderboard.Add(entry);
                    }
                }
            }

            Leaderboard = leaderboard;
            return leaderboard;
        }
    }
}
