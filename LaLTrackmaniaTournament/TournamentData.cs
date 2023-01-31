using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{
    /// <summary>
    /// Tournament data as it is loaded from a tournament.json.
    /// </summary>
    public class TournamentData
    {
        public List<string> Players { get; set; }
        public List<TrackData> Tracks { get; set; }
        public List<int> Points { get; set; }
        public GoogleSheetData GoogleSheetInfo { get; set; }
        public string TrackmaniaApiInfo { get; set; }

        public static TournamentData CreateFromJson(string path)
        {
            TournamentData tournamentData;
            string json = File.ReadAllText(path);
            tournamentData = JsonSerializer.Deserialize<TournamentData>(json);
            Console.WriteLine("Tournament Data successfully loaded from JSON.\n");
            return tournamentData;
        }
    }


    // Track data within a tournament data as it is loaded from a tournament.json
    public class TrackData
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    // Google sheet data within a tournament data as it is loaded from a tournament.json
    public class GoogleSheetData
    {
        public string ApplicationName { get; set; }
        public string DocumentId { get; set; }
        public string SheetName { get; set; }
    }
}
