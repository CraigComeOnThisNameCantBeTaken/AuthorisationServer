namespace AuthServer.AuthModels
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public string SecurityStamp { get; set; }

        // hash this?
        public string PasswordHash { get; set; }
    }
}
