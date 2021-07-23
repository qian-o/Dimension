namespace DimensionService.Models.ResultModels
{
    public class HitokotoModel
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public string Hitokoto { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string From_who { get; set; }
        public string Creator { get; set; }
        public int Creator_uid { get; set; }
        public int Reviewer { get; set; }
        public string Commit_from { get; set; }
        public string Created_at { get; set; }
        public int Length { get; set; }
    }
}
