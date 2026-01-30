using ApiGymAutomationN8N.Models;

namespace ApiGymAutomationN8N.Interface
{
    public interface IN8nService
    {
        Task<N8nResponse?> SendRequestToN8N(WorkoutRequest workoutRequest);
    }
}
