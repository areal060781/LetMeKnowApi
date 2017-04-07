namespace LetMeKnowApi.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWordHash { get; set; }
        public string Salt { get; set; }        
        public string Email { get; set; }
        public int SuggestionsCreated { get; set; }
        public int Roles { get; set; }
    }
}
