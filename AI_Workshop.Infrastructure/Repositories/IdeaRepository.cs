using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;
using AI_Workshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AI_Workshop.Infrastructure.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly AIWorkshopDbContext _dbContext;

    public IdeaRepository(AIWorkshopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Idea>> GetAllIdeasAsync()
    {
        return await _dbContext.WorkshopIdeas
            .AsNoTracking()
            .OrderByDescending(idea => idea.SubmittedDate)
            .ToArrayAsync();
    }

    public Task<Idea?> GetIdeaByIdAsync(int id)
    {
        return _dbContext.WorkshopIdeas
            .AsNoTracking()
            .FirstOrDefaultAsync(idea => idea.Id == id);
    }

    public async Task<Idea> AddIdeaAsync(Idea idea)
    {
        _dbContext.WorkshopIdeas.Add(idea);
        await _dbContext.SaveChangesAsync();
        return idea;
    }
}
