using AspNetCoreFeatureWithMonitor.DbContext;

namespace AspNetCoreFeatureWithMonitor.Services;

public class UserService : IUserService
{
    private ProductContext _dbContext;
    private readonly TokenCounter _tokenCounter;

    public UserService(ProductContext dbContext, TokenCounter tokenCounter)
    {
        _dbContext = dbContext;
        _tokenCounter = tokenCounter;
    }

    public (bool isValid, User user) IsValid(string name, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(item => item.Name == name);
        if (user == null)
        {
            return (false, new User());
        }
        var isVerify = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isVerify)
        {
            return (false, new User());
        }

        _tokenCounter.Counter(user.Name);
        return (true, user);
    }

    public bool AddUser(string name, string password, string[] roles)
    {
        var isExist = _dbContext.Users.Any(item => item.Name == name);
        if (isExist)
        {
            return false;
        }
        var newUser = new User
        {
            Name = name,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = roles,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
        return true;
    }
}