
using System;

namespace Domain.User
{
    public class LeaderboardRecord
    {
        public string UserName;
        public int rank;
        public string result;

        public LeaderboardRecord(string userName, int rank, string result)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            UserName = userName;
            this.rank = rank;
            this.result = result;
        }
    }
}