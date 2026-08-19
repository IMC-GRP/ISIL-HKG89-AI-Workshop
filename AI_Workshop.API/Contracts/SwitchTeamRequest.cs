using System.ComponentModel.DataAnnotations;

namespace AI_Workshop.API.Contracts;

public class SwitchTeamRequest
{
    [Range(1, int.MaxValue)]
    public int TargetTeamId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? EmployeeName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string? EmployeeEmail { get; set; }
}
