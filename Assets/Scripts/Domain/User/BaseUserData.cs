using System;

namespace Domain.User
{
    public class BaseUserData
    {
        public string Token { get; private set; }

        public string TokenID { get; private set; }

        public GameUserID GameUserID { get; private set; }


        public BaseUserData(string token, string tokenId, GameUserID gameUserId)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (tokenId == null) throw new ArgumentNullException("tokenId");
            if (gameUserId == null) throw new ArgumentNullException("gameUserId");

            Token = token;
            TokenID = tokenId;
            GameUserID = gameUserId;
        }
    }
}