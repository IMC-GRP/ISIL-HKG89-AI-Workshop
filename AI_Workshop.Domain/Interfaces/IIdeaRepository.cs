using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Domain.Interfaces;

public interface IIdeaRepository
{
    Task<IReadOnlyCollection<Idea>> GetAllIdeasAsync();
    Task<Idea?> GetIdeaByIdAsync(int id);
    Task<Idea> AddIdeaAsync(Idea idea);
}
