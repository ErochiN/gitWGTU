namespace GitВГТУ
{
    public interface IUserService
    {
        Task<bool> IniUser(string email, string password);
        Task<bool> RegisterAsync(string firstName, string lastName, string group, string email, string password);
        Task<int> GetUserId(string email);
    }
}
