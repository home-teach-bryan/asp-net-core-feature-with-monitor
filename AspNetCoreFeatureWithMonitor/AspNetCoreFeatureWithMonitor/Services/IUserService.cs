using AspNetCoreFeatureWithMonitor.DbContext;

namespace AspNetCoreFeatureWithMonitor.Services;

public interface IUserService
{
    (bool isValid, User user) IsValid(string name, string password);
    bool AddUser(string requestName, string requestPassword, string[] requestRoles);
}