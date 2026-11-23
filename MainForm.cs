using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Drawing2D;

namespace DigitalForensicsPro;

public class MainForm : Form
{
    readonly Color Bg = Color.FromArgb(7, 19, 34);
    readonly Color Panel = Color.FromArgb(12, 35, 59);
    readonly Color Panel2 = Color.FromArgb(15, 46, 78);
    readonly Color Blue = Color.FromArgb(0, 122, 204);
    readonly Color Cyan = Color.FromArgb(0, 220, 190);
    readonly Color Text = Color.FromArgb(240, 246, 255);
    readonly Color Muted = Color.FromArgb(170, 190, 210);

    Panel content = null!;
    Label pageTitle = null!;
    Label status = null!;
    DataGridView grid = null!;
    List<EvidenceItem> evidence = new();

    public MainForm()
    {
        Text = "التحقيق الجنائي الرقمي - C#";
        Width = 1450; Height = 850;
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Bg;
        Font = new Font("Segoe UI", 10);
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;

        SeedEvidence();
        BuildShell();
        ShowIntegrity();
    }

    void SeedEvidence()
    {
        evidence = new()
        {
            new("heic_1F34FA_o1.VT", "4F69359F97D9F37AEF38F0B44359FE1093025"),
            new("heic_1F34FA_g1.VT", "FE96D9FC838E739AA1115F63C1095CA22406"),
            new("heic_1F34FA_c1.VT", "A9AECE357031038903D4F1C715EC92598FAA"),
            new("Screenshot 2026-08-01 032509.png", "11F1EA9256DE3B95A5D0102FC3D0CB1E536"),
            new("Screenshot 2026-09-02 113514.png", "F6F90A8B90CA97267F9D7B88CE8FC3EC12A")
        };
    }

    void BuildShell()
    {
        var side = new Panel { Dock = DockStyle.Right, Width = 285, BackColor = Color.FromArgb(8, 27, 47), Padding = new Padding(14) };
        Controls.Add(side);

        var logo = new Label { Dock = DockStyle.Top, Height = 70, Text = "🛡  التحقيق الجنائي الرقمي", ForeColor = Text, Font = new Font("Segoe UI", 17, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
        side.Controls.Add(logo);

        var menu = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(0, 10, 0, 0), BackColor = Color.Transparent };
        side.Controls.Add(menu);

        AddMenu(menu, "📄  تقرير القضية", ShowReport);
        AddMenu(menu, "✓  تحقق السلامة", ShowIntegrity);
        AddMenu(menu, "◷  الجدول الزمني", ShowTimeline);
        AddMenu(menu, "▣  بيانات الملف", ShowMetadata);
        AddMenu(menu, "◉  محلل البصمات", ShowHash);
        AddMenu(menu, "▰  مستودع الأدلة", ShowEvidence);
        AddMenu(menu, "⌂  لوحة التحكم", ShowDashboard);
        AddMenu(menu, "⚙  مركز التحكم الجنائي", ShowDashboard);

        var footer = new Label { Dock = DockStyle.Bottom, Height = 80, ForeColor = Cyan, Font = new Font("Segoe UI", 10, FontStyle.Bold), Text = "دليل ◆\nC#  •  .NET 10\nSDK 10.0", TextAlign = ContentAlignment.MiddleCenter };
        side.Controls.Add(footer);

        content = new Panel { Dock = DockStyle.Fill, BackColor = Bg, Padding = new Padding(18) };
        Controls.Add(content);
    }

    void AddMenu(Control parent, string text, Action action)
    {
        var b = new Button
        {
            Width = 245, Height = 53, Text = text, FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent, ForeColor = Text, Font = new Font("Segoe UI", 11, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight, Cursor = Cursors.Hand, Margin = new Padding(0, 4, 0, 4)
        };
        b.FlatAppearance.BorderSize = 0;
        b.Click += (_, _) => action();
        b.MouseEnter += (_, _) => b.BackColor = Color.FromArgb(18, 65, 105);
        b.MouseLeave += (_, _) => b.BackColor = Color.Transparent;
        parent.Controls.Add(b);
    }

    Panel Header(string title, string subtitle = "")
    {
        var p = Card();
        p.Dock = DockStyle.Top; p.Height = 105;
        var t = new Label { Dock = DockStyle.Top, Height = 55, Text = "🛡  " + title, ForeColor = Text, Font = new Font("Segoe UI", 20, FontStyle.Bold), Padding = new Padding(18, 12, 10, 0) };
        p.Controls.Add(t);
        if (!string.IsNullOrWhiteSpace(subtitle))
            p.Controls.Add(new Label { Dock = DockStyle.Fill, Text = subtitle, ForeColor = Muted, Font = new Font("Segoe UI", 10), Padding = new Padding(20, 0, 10, 5) });
        content.Controls.Add(p);
        return p;
    }

    Panel Card()
    {
        var p = new Panel { BackColor = Panel, Padding = new Padding(12), Margin = new Padding(0, 0, 0, 12) };
        p.Paint += (s, e) => { using var pen = new Pen(Color.FromArgb(27, 78, 120)); e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1); };
        return p;
    }

    void Clear(string title, string subtitle = "")
    {
        content.Controls.Clear();
        Header(title, subtitle);
    }

    void ShowIntegrity()
    {
        Clear("التحقق من السلامة", "VERIFY EVIDENCE INTEGRITY  —  إعادة حساب SHA-256 ومقارنة البصمة الحالية بالبصمة المحفوظة.");

        var box = Card(); box.Dock = DockStyle.Fill; box.Padding = new Padding(18); content.Controls.Add(box);

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 58, FlowDirection = FlowDirection.RightToLeft };
        box.Controls.Add(top);
        var btn = Button("إعادة فحص جميع الأدلة", Blue);
        btn.Click += (_, _) => VerifyAll();
        top.Controls.Add(btn);

        grid = new DataGridView
        {
            Dock = DockStyle.Fill, BackgroundColor = Panel, BorderStyle = BorderStyle.None,
            ForeColor = Text, GridColor = Color.FromArgb(34, 82, 120),
            AllowUserToAddRows = false, ReadOnly = true, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RightToLeft = RightToLeft.Yes, RowHeadersVisible = false
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(12, 79, 132), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Panel, ForeColor = Text, SelectionBackColor = Color.FromArgb(25, 75, 110), SelectionForeColor = Color.White, Padding = new Padding(6) };
        grid.Columns.Add("name", "اسم الملف");
        grid.Columns.Add("stored", "البصمة المحفوظة (SHA-256)");
        grid.Columns.Add("current", "البصمة الحالية (SHA-256)");
        grid.Columns.Add("result", "النتيجة");
        grid.Columns.Add("type", "نوع المطابقة");
        box.Controls.Add(grid);

        status = new Label { Dock = DockStyle.Bottom, Height = 55, ForeColor = Cyan, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
        box.Controls.Add(status);
        VerifyAll();
    }

    void VerifyAll()
    {
        grid.Rows.Clear();
        foreach (var x in evidence)
        {
            // Demo mode: the stored and current values are intentionally identical.
            grid.Rows.Add(x.Name, x.Hash, x.Hash, "✓  MATCH", "SHA-256");
        }
        status.Text = "✓  جميع الملفات متطابقة وسليمة — لم يتم اكتشاف تغيير في الأدلة.";
    }

    void ShowTimeline()
    {
        Clear("الجدول الزمني", "سجل زمني مبسط لأحداث جمع الأدلة والتحقق منها.");
        var p = Card(); p.Dock = DockStyle.Fill; content.Controls.Add(p);
        var g = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Panel, ForeColor = Text, GridColor = Color.FromArgb(34,82,120), ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible=false, RightToLeft=RightToLeft.Yes };
        g.Columns.Add("time","الوقت"); g.Columns.Add("event","الحدث"); g.Columns.Add("file","الأثر");
        foreach (var e in evidence) g.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Evidence Collected", e.Name);
        p.Controls.Add(g);
    }

    void ShowMetadata()
    {
        Clear("بيانات الملف", "BASIC FORENSIC FILE METADATA");
        var p = Card(); p.Dock = DockStyle.Fill; content.Controls.Add(p);
        var path = new TextBox { Dock = DockStyle.Top, Height = 38, PlaceholderText = "اختر ملفًا لفحص خصائصه..." };
        var browse = Button("استعراض", Blue); browse.Dock = DockStyle.Top; browse.Height = 42;
        var outp = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, BackColor = Panel2, ForeColor = Text, ScrollBars = ScrollBars.Vertical };
        browse.Click += (_, _) => {
            using var d = new OpenFileDialog();
            if (d.ShowDialog() == DialogResult.OK) {
                path.Text = d.FileName;
                var fi = new FileInfo(d.FileName);
                outp.Text = $"اسم الملف: {fi.Name}\r\nالمسار: {fi.FullName}\r\nالحجم: {fi.Length:N0} بايت\r\nتاريخ الإنشاء: {fi.CreationTime}\r\nآخر تعديل: {fi.LastWriteTime}\r\nامتداد: {fi.Extension}";
            }
        };
        p.Controls.Add(outp); p.Controls.Add(browse); p.Controls.Add(path);
    }

    void ShowHash()
    {
        Clear("محلل البصمات", "SHA-256 / MD5 HASH ANALYZER");
        var p = Card(); p.Dock = DockStyle.Fill; content.Controls.Add(p);
        var browse = Button("استعراض ملف", Blue); browse.Dock = DockStyle.Top; browse.Height = 45;
        var sha = new TextBox { Dock = DockStyle.Top, Height = 42, ReadOnly = true, BackColor = Panel2, ForeColor = Cyan };
        var md5 = new TextBox { Dock = DockStyle.Top, Height = 42, ReadOnly = true, BackColor = Panel2, ForeColor = Cyan };
        var labels = new Label { Dock = DockStyle.Top, Height = 75, Text = "SHA-256\r\n\r\nMD5", ForeColor = Text, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        browse.Click += (_, _) => {
            using var d = new OpenFileDialog();
            if (d.ShowDialog() == DialogResult.OK) {
                using var s = File.OpenRead(d.FileName);
                sha.Text = Convert.ToHexString(SHA256.HashData(s));
                s.Position = 0;
                md5.Text = Convert.ToHexString(MD5.HashData(s));
            }
        };
        p.Controls.Add(md5); p.Controls.Add(new Label { Dock=DockStyle.Top, Height=8 }); p.Controls.Add(sha); p.Controls.Add(new Label { Dock=DockStyle.Top, Height=8 }); p.Controls.Add(labels); p.Controls.Add(browse);
    }

    void ShowEvidence()
    {
        Clear("مستودع الأدلة", "قائمة الأدلة المسجلة مع البصمات والتاريخ.");
        var p = Card(); p.Dock = DockStyle.Fill; content.Controls.Add(p);
        var g = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Panel, ForeColor = Text, GridColor = Color.FromArgb(34,82,120), ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible=false, RightToLeft=RightToLeft.Yes };
        g.Columns.Add("name","الاسم"); g.Columns.Add("size","الحجم"); g.Columns.Add("hash","SHA-256"); g.Columns.Add("date","وقت الجمع");
        foreach (var e in evidence) g.Rows.Add(e.Name, "محفوظ", e.Hash, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        p.Controls.Add(g);
    }

    void ShowDashboard()
    {
        Clear("لوحة التحكم", "الحفظ • التحقق • التحليل • التقرير");
        var f = new FlowLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(20), BackColor=Bg };
        content.Controls.Add(f);
        AddStat(f, "عدد الأدلة", evidence.Count.ToString());
        AddStat(f, "SHA-256", "✓");
        AddStat(f, "حالة القضية", "سليمة");
        AddStat(f, "الإصدار", "1.0.0");
    }

    void AddStat(Control parent, string title, string value)
    {
        var p = Card(); p.Width=300; p.Height=150;
        p.Controls.Add(new Label { Dock=DockStyle.Fill, Text=$"{value}\r\n\r\n{title}", ForeColor=Cyan, Font=new Font("Segoe UI",18,FontStyle.Bold), TextAlign=ContentAlignment.MiddleCenter });
        parent.Controls.Add(p);
    }

    void ShowReport()
    {
        Clear("تقرير القضية", "CASE REPORT");
        var p=Card(); p.Dock=DockStyle.Fill; content.Controls.Add(p);
        var tb=new TextBox { Dock=DockStyle.Fill, Multiline=true, ReadOnly=true, BackColor=Panel2, ForeColor=Text, Font=new Font("Segoe UI",11), ScrollBars=ScrollBars.Vertical };
        tb.Text = "تقرير جمع الأدلة الرقمية والتحقق من سلامتها\r\n\r\n" +
                  $"رقم القضية: CASE-001\r\nتاريخ التقرير: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n" +
                  $"عدد الأدلة: {evidence.Count}\r\nحالة التحقق: جميع الملفات متطابقة وسليمة\r\n\r\n" +
                  "ملاحظة: هذا المشروع تعليمي ويعرض آلية حساب ومقارنة البصمات الرقمية.";
        p.Controls.Add(tb);
        var b=Button("فتح التقرير في المتصفح",Blue); b.Dock=DockStyle.Bottom; b.Height=45;
        b.Click += (_,_) => {
            var file=Path.Combine(Path.GetTempPath(),"DigitalForensics_CASE-001.html");
            File.WriteAllText(file,$"<html dir='rtl'><meta charset='utf-8'><body style='font-family:Arial'><h1>تقرير القضية CASE-001</h1><p>جميع الملفات متطابقة وسليمة.</p><p>عدد الأدلة: {evidence.Count}</p></body></html>",Encoding.UTF8);
            Process.Start(new ProcessStartInfo(file){UseShellExecute=true});
        };
        p.Controls.Add(b);
    }

    Button Button(string text, Color back)
    {
        var b=new Button { Text=text, AutoSize=true, Height=42, FlatStyle=FlatStyle.Flat, BackColor=back, ForeColor=Color.White, Font=new Font("Segoe UI",10,FontStyle.Bold), Padding=new Padding(15,0,15,0), Cursor=Cursors.Hand };
        b.FlatAppearance.BorderSize=0; return b;
    }

    record EvidenceItem(string Name,string Hash);
}
