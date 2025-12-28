namespace FindTheBug.Desktop.Reception.Dtos;

public class LabTestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public decimal Total => Math.Round(Amount - (Discount / 100 * Amount), 0);
}
