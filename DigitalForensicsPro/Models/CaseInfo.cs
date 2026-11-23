namespace DigitalForensicsPro.Models;

public class CaseInfo
{
    public string CaseId { get; set; } = "CASE-001";
    public string CaseName { get; set; } = "قضية فحص رقمي";
    public string Examiner { get; set; } = Environment.UserName;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Description { get; set; } = "حالة تدريبية للتحليل الجنائي الرقمي";
}
