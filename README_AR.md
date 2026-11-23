# Digital Forensics Pro

مشروع التحليل الجنائي الرقمي بلغة C# لبيئة Visual Studio 2026 Community.

## الإصدار
- Target Framework: .NET 10 (net10.0-windows)
- SDK محدد في global.json: 10.0.302
- Windows Forms
- C#

## الوظائف
1. Dashboard تفاعلي.
2. Evidence Vault لإضافة الأدلة الرقمية.
3. SHA-256 و MD5 Hash Analyzer.
4. تحليل Metadata الأساسي للملفات.
5. Forensic Timeline.
6. Integrity Check لاكتشاف تغير الدليل.
7. Case Report وتوليد تقرير HTML.
8. حفظ الأدلة في Documents/DigitalForensicsPro/evidence.json.

## التشغيل في Visual Studio 2026 Community
1. ثبّت .NET 10 SDK 10.0.302 و workload الخاص بـ Desktop development with .NET.
2. افتح DigitalForensicsPro.sln.
3. من Solution Explorer اضغط بالزر الأيمن على DigitalForensicsPro واختر Set as Startup Project.
4. اختر Debug أو Release.
5. نفّذ Build > Rebuild Solution.
6. اضغط F5.

المشروع تعليمي، ويجب استخدامه فقط على الملفات والأدلة التي لديك صلاحية تحليلها.


## النسخة Premium
تم تحسين توزيع الواجهة لتجنب تداخل العناصر:
- Header منظم بدون تداخل الساعة وحالة النظام.
- بطاقات Dashboard متساوية ومتباعدة.
- مسافات داخلية وخارجية موحدة.
- أحجام عناصر محسنة للشاشات الكبيرة.
- دعم أفضل لتغيير حجم نافذة البرنامج.
- Minimum window size مناسب لمنع ضغط الواجهة.
