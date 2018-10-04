
using System;

namespace Domain.User
{
    public class LeaderboardRecord
    {
        public string UserName;
        public int rank;
        public int score;

        public LeaderboardRecord(string userName, int rank, int score)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            UserName = userName;
            this.rank = rank;
            this.score = score;
        }
    }
}