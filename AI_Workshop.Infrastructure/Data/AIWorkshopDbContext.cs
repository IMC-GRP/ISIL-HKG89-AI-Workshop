using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AI_Workshop.Infrastructure.Data;

public class AIWorkshopDbContext : DbContext
{
    public AIWorkshopDbContext(DbContextOptions<AIWorkshopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Idea> WorkshopIdeas => Set<Idea>();
    public DbSet<ScheduleItem> WorkshopScheduleItems => Set<ScheduleItem>();
    public DbSet<WorkshopTeam> WorkshopTeams => Set<WorkshopTeam>();
    public DbSet<WorkshopTeamMember> WorkshopTeamMembers => Set<WorkshopTeamMember>();
    public DbSet<WorkshopSettings> WorkshopSettings => Set<WorkshopSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Idea>(entity =>
        {
            entity.ToTable("WorkshopIdea", "dbo");
            entity.HasKey(idea => idea.Id);

            entity.Property(idea => idea.Id).HasColumnName("IdeaId");
            entity.Property(idea => idea.Title).HasColumnName("Title");
            entity.Property(idea => idea.Category).HasColumnName("Category").HasConversion<string>();
            entity.Property(idea => idea.Description).HasColumnName("Description");
            entity.Property(idea => idea.TeamLeaderName).HasColumnName("TeamLeaderName");
            entity.Property(idea => idea.TeamLeaderEmail).HasColumnName("TeamLeaderEmail");
            entity.Property(idea => idea.ProblemToSolve).HasColumnName("ProblemToSolve");
            entity.Property(idea => idea.ProposedSolution).HasColumnName("ProposedSolution");
            entity.Property(idea => idea.TargetUsers).HasColumnName("TargetUsers");
            entity.Property(idea => idea.ToolsAndTechnologies).HasColumnName("ToolsAndTechnologies");
            entity.Property(idea => idea.ExpectedValue).HasColumnName("ExpectedValue");
            entity.Property(idea => idea.AdditionalNotes).HasColumnName("AdditionalNotes");
            entity.Property(idea => idea.SubmittedBy).HasColumnName("SubmittedBy");
            entity.Property(idea => idea.SubmittedDate).HasColumnName("SubmittedDate");
            entity.Property(idea => idea.Status).HasColumnName("Status").HasConversion<string>();
        });

        modelBuilder.Entity<ScheduleItem>(entity =>
        {
            entity.ToTable("WorkshopSchedule", "dbo");
            entity.HasKey(item => item.Id);

            entity.Property(item => item.Id).HasColumnName("ScheduleItemId");
            entity.Property(item => item.DayNumber).HasColumnName("DayNumber");
            entity.Property(item => item.StartTime).HasColumnName("StartTime");
            entity.Property(item => item.EndTime).HasColumnName("EndTime");
            entity.Property(item => item.Type).HasColumnName("ScheduleItemType").HasConversion<string>();
            entity.Property(item => item.Title).HasColumnName("Title");
            entity.Property(item => item.Description).HasColumnName("Description");
            entity.Property(item => item.Location).HasColumnName("Location");
            entity.Property(item => item.DisplayOrder).HasColumnName("DisplayOrder");
        });

        modelBuilder.Entity<WorkshopTeam>(entity =>
        {
            entity.ToTable("WorkshopTeam", "dbo");
            entity.HasKey(team => team.Id);

            entity.Property(team => team.Id).HasColumnName("TeamId");
            entity.Property(team => team.IdeaId).HasColumnName("IdeaId");
            entity.Property(team => team.TeamName).HasColumnName("TeamName");
            entity.Property(team => team.MaxMembers).HasColumnName("MaxMembers");
            entity.Property(team => team.CreatedDate).HasColumnName("CreatedDate");
            entity.Property(team => team.IsRegistrationOpen).HasColumnName("IsRegistrationOpen");

            entity.Ignore(team => team.RegistrationCloseDate);

            entity.HasMany(team => team.Members)
                .WithOne()
                .HasForeignKey(member => member.TeamId)
                .HasPrincipalKey(team => team.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Idea>()
                .WithOne()
                .HasForeignKey<WorkshopTeam>(team => team.IdeaId)
                .HasPrincipalKey<Idea>(idea => idea.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkshopTeamMember>(entity =>
        {
            entity.ToTable("WorkshopTeamMember", "dbo");
            entity.HasKey(member => member.Id);

            entity.Property(member => member.Id).HasColumnName("TeamMemberId");
            entity.Property(member => member.TeamId).HasColumnName("TeamId");
            entity.Property(member => member.EmployeeName).HasColumnName("EmployeeName");
            entity.Property(member => member.EmployeeEmail).HasColumnName("EmployeeEmail");
            entity.Property(member => member.JoinedDate).HasColumnName("JoinedDate");
            entity.Property(member => member.IsTeamLeader).HasColumnName("IsTeamLeader");

            entity.HasIndex(member => member.EmployeeEmail)
                .IsUnique();
        });

        modelBuilder.Entity<WorkshopSettings>(entity =>
        {
            entity.ToTable("WorkshopSettings", "dbo");
            entity.Property<int>("WorkshopSettingsId").HasColumnName("WorkshopSettingsId");
            entity.HasKey("WorkshopSettingsId");

            entity.Property(settings => settings.IsTeamRegistrationOpen).HasColumnName("IsTeamRegistrationOpen");
            entity.Property(settings => settings.TeamRegistrationCloseDate).HasColumnName("TeamRegistrationCloseDate");
        });
    }
}
