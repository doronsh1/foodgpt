using OpenAI.Chat;
using FoodAi.AiService.Models;
using Microsoft.Extensions.Options;
using FoodAi.AiService.Configuration;
using FoodAi.Persistence.Documents;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

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
            var prompt = "How many calories are are in this meal? put the whole answer in a json format";
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
                var completion = await _openAIClient.CompleteChatAsync(messages);
                if(completion.Value == null)
                {
                    return null!;
                }
                OpenAIResponse openAIResponse = new OpenAIResponse()
                {
                    Id = completion.Value.Id,
                    Content = completion.Value.Content.First().Text,
                    Model = completion.Value.Model,
                    Usage = new Usage()
                    {
                        InputTokens = completion.Value.Usage.InputTokens,
                        OutputTokens = completion.Value.Usage.OutputTokens,
                        TotalTokens = completion.Value.Usage.TotalTokens
                    }
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
    }  
}