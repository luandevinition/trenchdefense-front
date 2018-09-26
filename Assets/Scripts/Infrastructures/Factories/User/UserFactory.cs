using Domain;
using Domain.User;

namespace Infrastructures.Factories.User
{
    public class UserFactory
    {
        public static BaseUserData Make(App.Proto.AccessCode dto)
        {
            return new BaseUserData(dto.token, new TokenID((int) dto.tokenID),
                new GameUserID((int) dto.gameUserID));
        }
    }
}