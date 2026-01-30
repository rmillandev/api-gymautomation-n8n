using ApiGymAutomationN8N.Config;
using ApiGymAutomationN8N.Interface;
using ApiGymAutomationN8N.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace ApiGymAutomationN8N.Service
{
    public class N8nService : IN8nService
    {
        private readonly HttpClient httpClient;
        private readonly HeaderAuthConfig headerAuthConfig;
        private readonly N8nConfig n8NConfig;

        public N8nService(HttpClient httpClient, IOptions<HeaderAuthConfig> optionsHeader, IOptions<N8nConfig> optionsN8N)
        {
            this.httpClient = httpClient;
            headerAuthConfig = optionsHeader.Value;
            n8NConfig = optionsN8N.Value;
        }

        public async Task<N8nResponse?> SendRequestToN8N(WorkoutRequest workoutRequest)
        {
            if (workoutRequest == null) return new N8nResponse { Status = "Error", Message = "The object cannot be empty." };

            if (string.IsNullOrWhiteSpace(workoutRequest.Prompt)) return new N8nResponse { Status = "Error", Message = "The prompt is empty." };

            if (workoutRequest.Muscles == null || workoutRequest.Muscles.Length == 0) return new N8nResponse { Status = "Error", Message = "The exercises list is empty." };

            try
            {
                httpClient.DefaultRequestHeaders.Remove(headerAuthConfig.AuthHeader);
                httpClient.DefaultRequestHeaders.Add(headerAuthConfig.AuthHeader, headerAuthConfig.AuthToken);

                var response = await httpClient.PostAsJsonAsync(n8NConfig.BaseUrl, workoutRequest);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<N8nResponse>();
                }

                var errorBody = await response.Content.ReadFromJsonAsync<ErrorN8N>();

                return new N8nResponse
                {
                    Status = "Error",
                    Message = errorBody?.Error ?? $"Automation failure: {response.StatusCode}"
                };
            }
            catch (HttpRequestException ex)
            {
                return new N8nResponse { Status = "Error", Message = $"The automation server could not be contacted. {ex.Message}" };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
