using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaLTrackmaniaTournament
{
    public class LeaderboardEntry
    {
        public string Player;
        public int Position;
        public int Time;

        public LeaderboardEntry(string player, int position, int time)
        {
            Player = player;
            Position = position;
            Time = time;
        }

        public override string ToString()
        {
            return Position + ". " + Player + ", " + GetTimeFormatted();
        }

        public string GetTimeFormatted()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(Time);
            return string.Format("{0:D1}:{1:D2}.{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }
    }
}
