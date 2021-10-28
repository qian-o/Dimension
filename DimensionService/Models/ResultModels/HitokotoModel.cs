namespace DimensionService.Models.ResultModels
{
    public class HitokotoModel
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public string Hitokoto { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string FromWho { get; set; }
        public string Creator { get; set; }
        public int CreatorUid { get; set; }
        public int Reviewer { get; set; }
        public string CommitFrom { get; set; }
        public string CreatedAt { get; set; }
        public int Length { get; set; }
    }
}
