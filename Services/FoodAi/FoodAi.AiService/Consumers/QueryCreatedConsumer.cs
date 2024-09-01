using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using FoodAi.AiService.Models;
using FoodAi.MessageQueue.Events;
using FoodAi.Persistence;
using FoodAi.Persistence.Documents;
using FoodAi.Persistence.Service;
using FoodAi.Persistence.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodAi.AiService.Consumers
{
    public class QueryCreatedConsumer(OpenAIService _openAIService,
        ILogger<QueryCreatedConsumer> _logger,
        AzureBlobStorageService _blobStorageService,
        MongoDbService mongoDbService) : IConsumer<QueryCreatedEvent>
    {
        public async Task Consume(ConsumeContext<QueryCreatedEvent> context)
        {           
            await Complete(context.Message, context.CancellationToken);
            _logger.LogDebug("Query created event consumed");             
        }

        public async Task Complete(QueryCreatedEvent queryCreatedEvent, CancellationToken cancellationToken = default)
        {
            string prompt = await _openAIService.GeneratePrompt();

            var imageBytes = await _blobStorageService.DownloadImageAsync(queryCreatedEvent.ImageUrl);
            
            var result = await _openAIService.RunChatCompletionAsync(prompt, imageBytes);
            
            var openAiTransaction = new OpenAiTransaction
            {
                //Id = Guid.NewGuid(),
                UserId = queryCreatedEvent.UserId,
                CreatedAt = queryCreatedEvent.CreatedAt,
                Prompt = prompt,                
                ImageName = queryCreatedEvent.ImageUrl,
                OperationId = queryCreatedEvent.OperationId,
                OpenAIResponse = new OpenAIResponse
                {
                    Content = result.Content,
                    Model = result.Model,
                    Id = result.Id,
                    Usage = new Usage
                    {
                        TotalTokens = result.Usage.TotalTokens,
                        InputTokens = result.Usage.InputTokens,
                        OutputTokens = result.Usage.OutputTokens
                    }
                }
            };

            await SaveTransaction(openAiTransaction, cancellationToken);            
        }

        private async Task SaveTransaction(OpenAiTransaction openAiTransaction, CancellationToken cancellationToken)
        {
            await mongoDbService.CreateQureyAsync(openAiTransaction, cancellationToken);
        }
    }
}
