namespace Convenient.Visualizer.Models.Emit
{
    public class EmitResultModel
    {
        public bool Success { get; set; }
        public DiagnosticModel[] Diagnostics { get; set; } = new DiagnosticModel[0];
    }
}