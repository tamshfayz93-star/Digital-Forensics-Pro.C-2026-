namespace DigitalForensicsPro.Models;

public class EvidenceItem
{
    public string Name { get; set; } = "";
    public string FullPath { get; set; } = "";
    public long Size { get; set; }
    public string Sha256 { get; set; } = "";
    public DateTime LastWriteTime { get; set; }
    public string Type { get; set; } = "";
    public string Status { get; set; } = "تم التحقق";
}
