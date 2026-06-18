using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ShopRestApi.Tests.Helpers
{
    public static class UserManagerMockHelper
    {
        public static Mock<UserManager<TUser>>
            MockUserManager<TUser>()
            where TUser : class
        {
            var store =
                new Mock<IUserStore<TUser>>();

            return new Mock<UserManager<TUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }
    }
}
