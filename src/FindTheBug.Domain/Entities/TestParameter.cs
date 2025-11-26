using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class TestParameter : BaseAuditableEntity
{
    public Guid DiagnosticTestId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? ReferenceRangeMin { get; set; }
    public decimal? ReferenceRangeMax { get; set; }
    public string DataType { get; set; } = "Numeric"; // Numeric, Text, Boolean
    public int DisplayOrder { get; set; }

    // Navigation properties
    public DiagnosticTest DiagnosticTest { get; set; } = null!;
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
