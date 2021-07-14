using AuthServer.AuthModels;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AuthServer.Database
{
    public static class InMemoryPersistance
    {
        public static ConcurrentDictionary<int, ApplicationUser> Users =
            new ConcurrentDictionary<int, ApplicationUser>(
                new Dictionary<int, ApplicationUser>
                {
                    //{
                    //    1,
                    //    new ApplicationUser
                    //    {
                    //        Id = 1,
                    //        NormalizedEmail = "FREEMA48@GMAIL.COM",
                    //        Email = "freema48@gmail.com"
                    //    }
                    //}
                }
            );

        public static ConcurrentDictionary<int, ApplicationRole> Roles =
            new ConcurrentDictionary<int, ApplicationRole>(
                new Dictionary<int, ApplicationRole>
                {
                            {
                                1,
                                new ApplicationRole
                                {
                                    Id = 1,
                                    Name = "ADMIN"
                                }
                            }
                }
            );
    }
}
