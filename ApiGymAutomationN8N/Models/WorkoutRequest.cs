namespace ApiGymAutomationN8N.Models
{
    public class WorkoutRequest
    {
        public string[] Muscles { get; set; } = Array.Empty<string>();
        public string Prompt { get; set; } = string.Empty;
    }
}
