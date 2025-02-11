namespace decorehubweb.Pages.Calculator.Models;

public class Channels
{
    public int Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public decimal adsPercent { get; set; }
    public decimal comissionPercent { get; set; }
    public decimal taxPercent { get; set; }
    public List<VariableComission> variableComission { get; set; } = new();

}

public class VariableComission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal startValue { get; set; }
    public decimal endValue { get; set; }
    public decimal percent { get; set; }
}