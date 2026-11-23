using System.Diagnostics;
using System.Text;
using DigitalForensicsPro.Models;
using DigitalForensicsPro.Services;

namespace DigitalForensicsPro;

public class MainForm : Form
{
    readonly Color Bg = Color.FromArgb(12, 18, 27);
    readonly Color Panel = Color.FromArgb(24, 34, 48);
    readonly Color Panel2 = Color.FromArgb(31, 43, 60);
    readonly Color Accent = Color.FromArgb(52, 211, 197);
    readonly Color Text = Color.FromArgb(235, 242, 247);
    readonly Color Muted = Color.FromArgb(160, 174, 192);

    Panel content = null!, sidebar = null!;
    Label pageTitle = null!, pageSub = null!;
    Label totalEvidence = null!, totalSize = null!, totalHash = null!, caseLabel = null!;
    DataGridView grid = null!;
    TextBox logBox = null!;
    CaseInfo currentCase = new();
    readonly List<EvidenceItem> evidence = new();

    public MainForm()
    {
        Text = "Digital Forensics Pro - التحليل الجنائي الرقمي";
        Width = 1400; Height = 850;
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Bg;
        ForeColor = Text;
        Font = new Font("Segoe UI", 10F);
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;

        BuildUi();
        ShowDashboard();
    }

    Button NavButton(string caption, string key)
    {
        var b = new Button {
            Text = caption, Tag = key, Width = 235, Height = 52,
            FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent,
            ForeColor = Text, TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Padding = new Padding(12, 0, 18, 0), Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        b.Click += (_, _) => Navigate(key);
        return b;
    }

    void BuildUi()
    {
        sidebar = new Panel { Dock = DockStyle.Right, Width = 255, BackColor = Color.FromArgb(16, 24, 35) };
        Controls.Add(sidebar);

        var brand = new Label {
            Text = "◈  دليــل\nDigital Forensics",
            Dock = DockStyle.Top, Height = 105,
            ForeColor = Accent, Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        sidebar.Controls.Add(brand);

        var menu = new FlowLayoutPanel {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(8, 12, 8, 8),
            BackColor = Color.Transparent
        };
        sidebar.Controls.Add(menu);

        menu.Controls.Add(NavButton("⌂   لوحة التحكم", "dashboard"));
        menu.Controls.Add(NavButton("▣   إدارة القضية", "case"));
        menu.Controls.Add(NavButton("◉   مستودع الأدلة", "evidence"));
        menu.Controls.Add(NavButton("✓   التحقق من السلامة", "integrity"));
        menu.Controls.Add(NavButton("◷   الجدول الزمني", "timeline"));
        menu.Controls.Add(NavButton("□   بيانات الملف", "metadata"));
        menu.Controls.Add(NavButton("#   محلل البصمات", "hash"));
        menu.Controls.Add(NavButton("▤   التقارير", "report"));
        menu.Controls.Add(NavButton("⚙   الإعدادات", "settings"));

        var footer = new Label {
            Text = ".NET 10\nWindows 11\nأداة تعليمية",
            Dock = DockStyle.Bottom, Height = 75,
            ForeColor = Muted, TextAlign = ContentAlignment.MiddleCenter
        };
        sidebar.Controls.Add(footer);

        content = new Panel { Dock = DockStyle.Fill, BackColor = Bg, Padding = new Padding(28) };
        Controls.Add(content);
        Controls.SetChildIndex(content, 0);

        var top = new Panel { Dock = DockStyle.Top, Height = 92 };
        content.Controls.Add(top);
        pageTitle = new Label { Text = "لوحة التحكم", Dock = DockStyle.Top, Height = 46, Font = new Font("Segoe UI", 24F, FontStyle.Bold), ForeColor = Text };
        pageSub = new Label { Text = "منصة تعليمية لإدارة الأدلة الرقمية، حساب البصمات والتحقق من سلامة الملفات.", Dock = DockStyle.Top, Height = 35, ForeColor = Muted, Font = new Font("Segoe UI", 10.5F) };
        top.Controls.Add(pageSub); top.Controls.Add(pageTitle);
    }

    Panel Card(string title, string value, int width)
    {
        var p = new Panel { Width = width, Height = 125, BackColor = Panel, Margin = new Padding(8) };
        var t = new Label { Text = title, Dock = DockStyle.Top, Height = 38, ForeColor = Muted, Padding = new Padding(14, 12, 0, 0), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        var v = new Label { Text = value, Dock = DockStyle.Fill, ForeColor = Accent, Padding = new Padding(14, 4, 0, 0), Font = new Font("Segoe UI", 25F, FontStyle.Bold) };
        p.Controls.Add(v); p.Controls.Add(t);
        return p;
    }

    void ClearPage(string title, string subtitle)
    {
        foreach (Control c in content.Controls.Cast<Control>().Skip(1).ToList()) content.Controls.Remove(c);
        pageTitle.Text = title; pageSub.Text = subtitle;
    }

    void ShowDashboard()
    {
        ClearPage("لوحة التحكم", "إدارة القضية والفحص والتحقق من الأدلة الرقمية من مكان واحد.");
        var cards = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 145, WrapContents = false, BackColor = Color.Transparent };
        content.Controls.Add(cards);
        totalEvidence = (Label)Card("عدد الأدلة", evidence.Count.ToString(), 235).Controls[0];
        totalSize = (Label)Card("الحجم الكلي", FormatSize(evidence.Sum(x => x.Size)), 235).Controls[0];
        totalHash = (Label)Card("SHA-256", evidence.Count == 0 ? "0" : "محسوبة", 235).Controls[0];
        caseLabel = (Label)Card("رقم القضية", currentCase.CaseId, 235).Controls[0];
        cards.Controls.Add(totalEvidence.Parent); cards.Controls.Add(totalSize.Parent); cards.Controls.Add(totalHash.Parent); cards.Controls.Add(caseLabel.Parent);

        var info = new Panel { Dock = DockStyle.Fill, BackColor = Panel, Padding = new Padding(22) };
        content.Controls.Add(info);
        var h = new Label { Text = "مسار الفحص المقترح", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 15F, FontStyle.Bold), ForeColor = Text };
        info.Controls.Add(h);
        var steps = new Label {
            Text = "1) إنشاء القضية  →  2) إضافة الأدلة  →  3) حساب SHA-256  →  4) فحص بيانات الملفات\n\n" +
                   "5) بناء خط زمني  →  6) التحقق من السلامة  →  7) إنشاء التقرير النهائي\n\n" +
                   "تنبيه: استخدم نسخاً جنائية (Forensic Copies) عند العمل على أدلة حقيقية، ولا تعدّل الأصل.",
            Dock = DockStyle.Fill, ForeColor = Muted, Font = new Font("Segoe UI", 12F), Padding = new Padding(5, 10, 5, 5)
        };
        info.Controls.Add(steps);
    }

    void ShowCase()
    {
        ClearPage("إدارة القضية", "إنشاء ملف قضية وتوثيق بيانات الفاحص والوصف.");
        var p = FormPanel();
        AddField(p, "رقم القضية", currentCase.CaseId, out var id, 20);
        AddField(p, "اسم القضية", currentCase.CaseName, out var name, 85);
        AddField(p, "الفاحص", currentCase.Examiner, out var examiner, 150);
        AddField(p, "الوصف", currentCase.Description, out var desc, 215, true);
        var save = ActionButton("حفظ بيانات القضية");
        save.Location = new Point(20, 335);
        save.Click += (_, _) => { currentCase.CaseId = id.Text; currentCase.CaseName = name.Text; currentCase.Examiner = examiner.Text; currentCase.Description = desc.Text; Log("تم حفظ بيانات القضية."); ShowDashboard(); };
        p.Controls.Add(save);
        content.Controls.Add(p);
    }

    void ShowEvidence()
    {
        ClearPage("مستودع الأدلة", "إضافة الملفات والأدلة، حساب SHA-256 وعرض خصائص كل ملف.");
        var bar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Panel };
        content.Controls.Add(bar);
        var add = ActionButton("＋ إضافة ملفات الأدلة");
        add.Location = new Point(14, 10);
        add.Click += AddEvidence;
        bar.Controls.Add(add);
        var report = ActionButton("تصدير تقرير");
        report.Location = new Point(205, 10);
        report.Click += ExportReport;
        bar.Controls.Add(report);

        grid = new DataGridView {
            Dock = DockStyle.Fill, BackgroundColor = Panel, BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Panel2, ForeColor = Text, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Panel, ForeColor = Text, SelectionBackColor = Color.FromArgb(37, 76, 78), SelectionForeColor = Color.White };
        grid.Columns.Add("name", "اسم الدليل"); grid.Columns.Add("type", "النوع"); grid.Columns.Add("size", "الحجم"); grid.Columns.Add("hash", "SHA-256"); grid.Columns.Add("modified", "آخر تعديل"); grid.Columns.Add("status", "الحالة");
        content.Controls.Add(grid);
        RefreshGrid();
    }

    void ShowIntegrity()
    {
        ClearPage("التحقق من السلامة", "إعادة حساب SHA-256 ومقارنة البصمة المسجلة للتأكد من عدم تغير الدليل.");
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Panel, Padding = new Padding(20) };
        content.Controls.Add(p);
        var check = ActionButton("✓ بدء التحقق لجميع الأدلة");
        check.Location = new Point(20, 20); check.Width = 260;
        check.Click += (_, _) => {
            int ok = 0, changed = 0;
            foreach (var e in evidence)
            {
                try {
                    var h = HashService.ComputeSha256(e.FullPath);
                    if (string.Equals(h, e.Sha256, StringComparison.OrdinalIgnoreCase)) { e.Status = "سليم ✓"; ok++; }
                    else { e.Status = "متغير ⚠"; changed++; }
                } catch { e.Status = "تعذر القراءة"; }
            }
            Log($"اكتمل التحقق: سليم={ok}، متغير={changed}.");
            MessageBox.Show($"اكتمل التحقق.\nسليم: {ok}\nمتغير: {changed}", "نتيجة التحقق", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        p.Controls.Add(check);
        var l = new Label { Text = "منهجية التحقق:\n\n• يتم حساب SHA-256 من الملف الحالي.\n• تتم مقارنته بالبصمة التي سُجلت عند إضافة الدليل.\n• تطابق البصمتين يعني أن محتوى الملف لم يتغير منذ لحظة التسجيل.\n• اختلافهما يعني أن الملف تغير أو أن النسخة الحالية ليست نفس النسخة.\n\nأفضل ممارسة: احتفظ بالأصل للقراءة فقط واعمل على نسخة جنائية.", Dock = DockStyle.Top, Height = 300, ForeColor = Muted, Font = new Font("Segoe UI", 12F), Padding = new Padding(5, 85, 5, 5) };
        p.Controls.Add(l);
    }

    void ShowTimeline()
    {
        ClearPage("الجدول الزمني", "عرض الأحداث الأساسية المتعلقة بالأدلة والقضية.");
        var box = new ListBox { Dock = DockStyle.Fill, BackColor = Panel, ForeColor = Text, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 12F) };
        box.Items.Add($"● {currentCase.CreatedAt:yyyy-MM-dd HH:mm:ss}  إنشاء القضية {currentCase.CaseId}");
        foreach (var e in evidence.OrderBy(x => x.LastWriteTime))
            box.Items.Add($"● {e.LastWriteTime:yyyy-MM-dd HH:mm:ss}  {e.Name}  |  {e.Type}  |  {FormatSize(e.Size)}");
        content.Controls.Add(box);
    }

    void ShowMetadata()
    {
        ClearPage("بيانات الملف", "تحليل الخصائص الأساسية للملف المختار.");
        var p = FormPanel();
        var choose = ActionButton("اختيار ملف وتحليل خصائصه");
        choose.Location = new Point(20, 20);
        p.Controls.Add(choose);
        var outBox = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Location = new Point(20, 85), Width = 900, Height = 330, BackColor = Panel2, ForeColor = Text, BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(outBox);
        choose.Click += (_, _) => {
            using var d = new OpenFileDialog();
            if (d.ShowDialog() != DialogResult.OK) return;
            var fi = new FileInfo(d.FileName);
            string hash = "";
            try { hash = HashService.ComputeSha256(d.FileName); } catch (Exception ex) { hash = ex.Message; }
            outBox.Text = $"الاسم: {fi.Name}\r\nالمسار: {fi.FullName}\r\nالحجم: {fi.Length:N0} بايت\r\nالامتداد: {fi.Extension}\r\nالنوع: {GetType(fi.Extension)}\r\nإنشاء: {fi.CreationTime}\r\nآخر تعديل: {fi.LastWriteTime}\r\nآخر وصول: {fi.LastAccessTime}\r\nSHA-256: {hash}";
        };
        content.Controls.Add(p);
    }

    void ShowHash()
    {
        ClearPage("محلل البصمات", "حساب SHA-256 لملف منفرد للتحقق والمقارنة.");
        var p = FormPanel();
        var choose = ActionButton("اختيار ملف");
        choose.Location = new Point(20, 20); p.Controls.Add(choose);
        var path = new TextBox { Location = new Point(20, 82), Width = 850, ReadOnly = true, BackColor = Panel2, ForeColor = Text };
        p.Controls.Add(path);
        var hash = new TextBox { Location = new Point(20, 145), Width = 850, Height = 70, ReadOnly = true, BackColor = Panel2, ForeColor = Accent, Multiline = true };
        p.Controls.Add(hash);
        choose.Click += (_, _) => { using var d = new OpenFileDialog(); if (d.ShowDialog() == DialogResult.OK) { path.Text = d.FileName; try { hash.Text = HashService.ComputeSha256(d.FileName); } catch (Exception ex) { hash.Text = ex.Message; } } };
        content.Controls.Add(p);
    }

    void ShowReport()
    {
        ClearPage("التقارير", "إنشاء تقرير نصي مرتب عن القضية والأدلة والبصمات.");
        var p = FormPanel();
        var generate = ActionButton("إنشاء التقرير");
        generate.Location = new Point(20, 20); p.Controls.Add(generate);
        logBox = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Location = new Point(20, 85), Width = 980, Height = 430, BackColor = Panel2, ForeColor = Text };
        p.Controls.Add(logBox);
        generate.Click += (_, _) => logBox.Text = ReportService.BuildTextReport(currentCase, evidence);
        content.Controls.Add(p);
    }

    void ShowSettings()
    {
        ClearPage("الإعدادات", "إعدادات تعليمية بسيطة للبرنامج.");
        var p = FormPanel();
        p.Controls.Add(new Label { Text = "البيئة: Windows 11\r\nالمنصة: .NET 10 / WinForms\r\nاللغة: C#\r\nاتجاه الواجهة: عربي RTL\r\n\r\nملاحظة: البرنامج مصمم للتعليم والعروض الجامعية، ويستخدم SHA-256 للتحقق من سلامة الملفات.", Location = new Point(25, 25), Size = new Size(800, 250), ForeColor = Muted, Font = new Font("Segoe UI", 12F) });
        content.Controls.Add(p);
    }

    Panel FormPanel() => new() { Dock = DockStyle.Fill, BackColor = Panel, Padding = new Padding(20) };

    void AddField(Panel p, string label, string value, out TextBox box, int y, bool multi = false)
    {
        p.Controls.Add(new Label { Text = label, Location = new Point(20, y), Size = new Size(180, 30), ForeColor = Muted });
        box = new TextBox { Text = value, Location = new Point(205, y - 4), Width = 620, Height = multi ? 85 : 32, Multiline = multi, BackColor = Panel2, ForeColor = Text, BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(box);
    }

    Button ActionButton(string text) => new() { Text = text, Width = 175, Height = 38, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(37, 91, 94), ForeColor = Text, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };

    void AddEvidence(object? sender, EventArgs e)
    {
        using var d = new OpenFileDialog { Multiselect = true, Title = "اختيار ملفات الأدلة الرقمية" };
        if (d.ShowDialog() != DialogResult.OK) return;
        foreach (var path in d.FileNames)
        {
            try
            {
                var fi = new FileInfo(path);
                evidence.Add(new EvidenceItem { Name = fi.Name, FullPath = fi.FullName, Size = fi.Length, Sha256 = HashService.ComputeSha256(path), LastWriteTime = fi.LastWriteTime, Type = GetType(fi.Extension) });
            }
            catch (Exception ex) { Log($"تعذر فحص {path}: {ex.Message}"); }
        }
        Log($"تمت إضافة {d.FileNames.Length} ملف/ملفات وحساب SHA-256.");
        RefreshGrid();
    }

    void RefreshGrid()
    {
        if (grid == null) return;
        grid.Rows.Clear();
        foreach (var e in evidence)
            grid.Rows.Add(e.Name, e.Type, FormatSize(e.Size), e.Sha256.Length > 18 ? e.Sha256[..18] + "..." : e.Sha256, e.LastWriteTime.ToString("yyyy-MM-dd HH:mm"), e.Status);
    }

    void ExportReport(object? sender, EventArgs e)
    {
        using var s = new SaveFileDialog { Filter = "Text Report (*.txt)|*.txt", FileName = $"{currentCase.CaseId}_Forensic_Report.txt" };
        if (s.ShowDialog() != DialogResult.OK) return;
        File.WriteAllText(s.FileName, ReportService.BuildTextReport(currentCase, evidence), Encoding.UTF8);
        MessageBox.Show("تم حفظ التقرير بنجاح.", "التقرير", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    void Navigate(string key)
    {
        switch (key)
        {
            case "dashboard": ShowDashboard(); break;
            case "case": ShowCase(); break;
            case "evidence": ShowEvidence(); break;
            case "integrity": ShowIntegrity(); break;
            case "timeline": ShowTimeline(); break;
            case "metadata": ShowMetadata(); break;
            case "hash": ShowHash(); break;
            case "report": ShowReport(); break;
            case "settings": ShowSettings(); break;
        }
    }

    void Log(string msg) => Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");

    static string FormatSize(long bytes) => bytes switch { >= 1024L * 1024 * 1024 => $"{bytes / 1024d / 1024 / 1024:0.##} GB", >= 1024 * 1024 => $"{bytes / 1024d / 1024:0.##} MB", >= 1024 => $"{bytes / 1024d:0.##} KB", _ => $"{bytes} B" };
    static string GetType(string ext) => ext.ToLowerInvariant() switch { ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "صورة", ".pdf" => "PDF", ".doc" or ".docx" => "مستند", ".txt" or ".log" => "نصي", ".zip" or ".rar" or ".7z" => "أرشيف", ".exe" or ".dll" => "تنفيذي", _ => string.IsNullOrWhiteSpace(ext) ? "ملف" : ext.ToUpperInvariant().TrimStart('.') };
}
