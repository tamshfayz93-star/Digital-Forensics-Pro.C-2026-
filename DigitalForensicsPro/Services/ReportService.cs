using System.Text;
using DigitalForensicsPro.Models;

namespace DigitalForensicsPro.Services;

public static class ReportService
{
    public static string BuildTextReport(CaseInfo c, IEnumerable<EvidenceItem> evidence)
    {
        var sb = new StringBuilder();
        sb.AppendLine("DIGITAL FORENSICS PRO - تقرير فحص جنائي رقمي");
        sb.AppendLine(new string('=', 65));
        sb.AppendLine($"رقم القضية: {c.CaseId}");
        sb.AppendLine($"اسم القضية: {c.CaseName}");
        sb.AppendLine($"الفاحص: {c.Examiner}");
        sb.AppendLine($"تاريخ الإنشاء: {c.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"الوصف: {c.Description}");
        sb.AppendLine();
        sb.AppendLine("الأدلة:");
        sb.AppendLine(new string('-', 65));
        foreach (var e in evidence)
        {
            sb.AppendLine($"الاسم: {e.Name}");
            sb.AppendLine($"المسار: {e.FullPath}");
            sb.AppendLine($"الحجم: {e.Size:N0} بايت");
            sb.AppendLine($"النوع: {e.Type}");
            sb.AppendLine($"SHA-256: {e.Sha256}");
            sb.AppendLine($"آخر تعديل: {e.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"الحالة: {e.Status}");
            sb.AppendLine();
        }
        sb.AppendLine("ملاحظة: هذا البرنامج تعليمي ولا يستبدل أدوات الطب الشرعي المعتمدة.");
        return sb.ToString();
    }
}
