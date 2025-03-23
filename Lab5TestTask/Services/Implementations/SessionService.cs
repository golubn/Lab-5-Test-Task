using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Lab5TestTask.Enums;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// SessionService implementation.
/// Implement methods here.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _dbContext;

    private sealed class SessionResponse
    {
        public int Id { get; set; }
        public DateTime StartedAtUTC { get; set; }
        public DateTime EndedAtUTC { get; set; }
        public DeviceType DeviceType { get; set; }
        public int UserId { get; set; }

        public static SessionResponse FromSession(Session session) =>
            new()
            {
                Id = session.Id,
                StartedAtUTC = session.StartedAtUTC,
                EndedAtUTC = session.EndedAtUTC,
                DeviceType = session.DeviceType,
                UserId = session.UserId
            };

        public Session ToSession() =>
            new()
            {
                Id = Id,
                StartedAtUTC = StartedAtUTC,
                EndedAtUTC = EndedAtUTC,
                DeviceType = DeviceType,
                UserId = UserId
            };
    }

    public SessionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session> GetSessionAsync()
    {
        var session = await _dbContext.Sessions
            .Where(s => s.DeviceType == DeviceType.Desktop)
            .OrderBy(s => s.StartedAtUTC)
            .Select(s => SessionResponse.FromSession(s))
            .FirstOrDefaultAsync();

        return session?.ToSession();
    }

    public async Task<List<Session>> GetSessionsAsync()
    {
        var endOf2024 = new DateTime(2025, 1, 1);
        
        var sessions = await _dbContext.Sessions
            .Where(s => s.User.Status == UserStatus.Active && 
                       s.EndedAtUTC < endOf2024)
            .Select(s => SessionResponse.FromSession(s))
            .ToListAsync();

        return sessions.Select(s => s.ToSession()).ToList();
    }
}
