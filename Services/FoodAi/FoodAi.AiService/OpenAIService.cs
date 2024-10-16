using OpenAI.Chat;
using FoodAi.AiService.Models;
using Microsoft.Extensions.Options;
using FoodAi.AiService.Configuration;
using FoodAi.Persistence.Documents;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace FoodAi.AiService
{
    public class OpenAIService
    {
        private readonly ChatClient _openAIClient;

        private readonly string _gptModel = "gpt-4.0-turbo";
        private readonly ILogger<OpenAIService> _logger;

        //public OpenAIService(string apiKey, string gptModel)
        //{
        //    _gptModel = gptModel;
        //    _openAIClient = new(_gptModel,
        //        new System.ClientModel.ApiKeyCredential(apiKey));
        //}

        public OpenAIService(IOptions<OpenAISettings> openAISettings, ILogger<OpenAIService> logger)
        {
            var settings = openAISettings.Value ?? throw new ArgumentNullException(nameof(openAISettings));
            _logger = logger;
            _gptModel = openAISettings.Value.OpenAIModel!;

            _openAIClient = new(_gptModel,
                new System.ClientModel.ApiKeyCredential(openAISettings.Value.ApiKey!));

        }

        public async Task<string> GeneratePrompt()
        {
            var prompt = "How many calories, Proteins, Carbs and fats, are are in this meal? give me a json that looks like this {calories: number, protien: number, carbs: number, fat: number, break-down: text }";
            return await Task.FromResult<string>(prompt);
        }
        
        public async Task<OpenAIResponse> RunChatCompletionAsync(string prompt, byte[] imageData)
        {
            try
            {         
                var usermessages = new List<ChatMessageContentPart>()
            {
                ChatMessageContentPart.CreateTextMessageContentPart(prompt),
                ChatMessageContentPart.CreateImageMessageContentPart(new BinaryData(imageData),"image/jpg")
            };
                var messages = new List<ChatMessage>()
            {
              // new SystemChatMessage(""),
               new UserChatMessage(usermessages)
            };
                DateTime start = DateTime.Now;
                var completion = await _openAIClient.CompleteChatAsync(messages);
                DateTime end = DateTime.Now;
                if(completion.Value == null)
                {
                    return null!;
                }
                var time = end - start;
                _logger.LogInformation($"Time taken to complete chat: {end - start}");
                var content = ExtractJson(completion.Value.Content.First().Text);
                OpenAIResponse openAIResponse = new OpenAIResponse()
                {
                    Id = completion.Value.Id,
                    Content = content.ToString()!,
                    Model = completion.Value.Model,
                    Usage = new Usage()
                    {
                        InputTokens = completion.Value.Usage.InputTokens,
                        OutputTokens = completion.Value.Usage.OutputTokens,
                        TotalTokens = completion.Value.Usage.TotalTokens
                    },
                    ResponseTime = time
                };
                return openAIResponse!;
                //            var chatCompletionRequest = await _openAIClient.ChatEndpoint
                //            .GetCompletionAsync(new ChatRequest(messages, maxTokens: 500));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null!;
            }
        }
        private   string ExtractJson(string input)
        {
            string pattern = @"(\{(?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!))\})";

            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                string json = match.Value;
                //Console.WriteLine("Found JSON: " + json);
                return json;
                // Parse the JSON
                //var obj = JObject.Parse(json);
                //Console.WriteLine("Parsed JSON: " + obj.ToString());
            }
            return null!;
        }
    }  
}