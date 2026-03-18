using System.Text;
using System.Text.Json;
using StudyHelperAPI.Models.Gemini;
using StudyHelperAPI.Services.Interfaces;

namespace StudyHelperAPI.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
            _model = configuration["Gemini:Model"] ?? "gemini-2.5-flash";
        }

        public async Task<string> AskAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("Gemini API key não configurada. Verifique Gemini:ApiKey.");

            var request = new GeminiRequest
            {
                Contents = new List<GeminiContent>
                {
                    new GeminiContent
                    {
                        Parts = new List<GeminiPart>
                        {
                            new GeminiPart { Text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/{_model}:generateContent?key={_apiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Gemini API erro {(int)response.StatusCode}: {responseString}");

            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var answer = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            if (string.IsNullOrWhiteSpace(answer))
                throw new InvalidOperationException($"Gemini API retornou vazio: {responseString}");

            return answer;
        }

        public async Task<SummaryResult> SummarizeMaterialAsync(string materialId, string content)
        {
            try
            {
                var prompt = $@"Você é um assistente de estudos. 
                Faça um resumo claro, organizado e didático do seguinte conteúdo acadêmico.
                Use tópicos, destaque os pontos principais e facilite o aprendizado, não faça o resumo muito grande.
                
                Conteúdo:
                {content}";

                var summary = await AskAsync(prompt);

                return new SummaryResult
                {
                    MateriaId = materialId,
                    Summary = summary,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new SummaryResult
                {
                    MateriaId = materialId,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<string> AnswerActivityAsync(string question, string context)
        {
            try
            {
                var prompt = $@"Você é um assistente de estudos inteligente.
            Responda a seguinte atividade acadêmica de forma completa e bem explicada.
            
            Contexto do material: {context}
            
            Atividade/Pergunta: {question}
            
            Forneça uma resposta detalhada, clara e acadêmica.";

                return await AskAsync(prompt);
            }
            catch (Exception ex)
            {
                return $"Erro ao gerar resposta: {ex.Message}";
            }
        }
    }
}
