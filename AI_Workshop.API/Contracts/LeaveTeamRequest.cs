using System.ComponentModel.DataAnnotations;

namespace AI_Workshop.API.Contracts;

public class LeaveTeamRequest
{
    [Range(1, int.MaxValue)]
    public int TeamId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string? EmployeeEmail { get; set; }
}
