using HalfWerk.CommonModels;
using HalfWerk.CommonModels.Auditlog;
using HalfWerk.DsBestelService.Listeners;
using Microsoft.Extensions.DependencyInjection;
using Minor.Nijn;
using Minor.Nijn.WebScale.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HalfWerk.DsBestelService.Services
{
    public class EventReplayService : IDisposable
    {
        private bool _disposed;
        private readonly int _timeout;
        private readonly IServiceCollection _services;

        private readonly Stack<string> _queues;
        private readonly IBusContext<IConnection> _context;
        private readonly IModel _channel;

        public EventReplayService(IBusContext<IConnection> context, IServiceCollection services, int timeout)
        {
            _queues = new Stack<string>();
            _services = services;
            _context = context;
            _timeout = timeout;

            _channel = _context.Connection.CreateModel();
        }

        public void StartEventReplay()
        {
            var listener = CreateEventListenerInstance<EventListener>();

            CreateReceiver(
                listener.HandleArtikelToegevoegdEvent, 
                NameConstants.BestelServiceEventReplayQueue,
                new List<string> { NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent }
            );

            SendReplayCommand();
            CheckConnection();
        }

        private void CreateReceiver(EventMessageReceivedCallback handle, string queueName, List<string> expressions)
        {
            _queues.Push(queueName);

            var messageReceiver = _context.CreateMessageReceiver(
                queueName: queueName,
                topicExpressions: expressions
            );

            messageReceiver.DeclareQueue();
            messageReceiver.StartReceivingMessages(handle);
        }

        private void SendReplayCommand()
        {
            ICommandSender commandSender = _context.CreateCommandSender();
            ReplayEventsCommand replayEventsCommand = new ReplayEventsCommand()
            {
                ExchangeName = NameConstants.BestelServiceEventReplayExchange,
                EventType = NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent
            };

            commandSender.SendCommandAsync(new RequestCommandMessage(
                message: JsonConvert.SerializeObject(replayEventsCommand),
                type: NameConstants.AuditlogReplayCommandType,
                correlationId: "",
                routingKey: NameConstants.AuditlogQueue
            ));
        }

        private T CreateEventListenerInstance<T>()
        {
            var serviceProvider = _services.BuildServiceProvider();
            return ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }

        private void CheckConnection()
        {
            while (!_context.IsConnectionIdle())
            {
                Thread.Sleep(_timeout);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EventReplayService()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _queues.ForEach(q => _channel?.QueueDelete(q, false, false));
                _channel.ExchangeDelete(NameConstants.BestelServiceEventReplayExchange, false);

                _channel?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
