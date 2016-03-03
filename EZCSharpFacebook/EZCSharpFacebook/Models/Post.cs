namespace EZCSharpFacebook.Models
{
    public class Post
    {
        public string FacebookID { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string FromFacebokUserID { get; set; }
        public string FromFacebookUserName { get; set; }
        public string StatusType { get; set; }
        public int Shares { get; set; }
    }
}