using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Lab5TestTask.Enums;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// UserService implementation.
/// Implement methods here.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;

    private sealed class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserStatus Status { get; set; }

        public static UserResponse FromUser(User user) =>
            new()
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.Status
            };

        public User ToUser() =>
            new()
            {
                Id = Id,
                Email = Email,
                Status = Status
            };
    }

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserAsync()
    {
        var userWithMostSessions = await _dbContext.Users
            .Select(u => new
            {
                User = UserResponse.FromUser(u),
                SessionCount = u.Sessions.Count
            })
            .OrderByDescending(x => x.SessionCount)
            .FirstOrDefaultAsync();

        return userWithMostSessions?.User.ToUser();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var users = await _dbContext.Users
            .Where(u => u.Sessions.Any(s => s.DeviceType == DeviceType.Mobile))
            .Select(u => UserResponse.FromUser(u))
            .ToListAsync();

        return users.Select(u => u.ToUser()).ToList();
    }
}
