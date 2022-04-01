namespace DimensionService.Common
{
    public class AppPath
    {
        // 附件路径
        public static readonly string AttachmentsPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Attachments");
        // 一言句子路径
        public static readonly string SentencesPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Sentences");
        // 模板路径
        public static readonly string TemplatesPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Templates");
    }
}
