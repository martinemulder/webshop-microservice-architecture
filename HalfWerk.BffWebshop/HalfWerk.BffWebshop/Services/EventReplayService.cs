using HalfWerk.BffWebshop.Listeners;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.Auditlog;
using Microsoft.Extensions.DependencyInjection;
using Minor.Nijn;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HalfWerk.BffWebshop.Services
{
    public class EventReplayService : IDisposable
    {
        private bool _disposed;
        private readonly int _timeout;
        private readonly ServiceProvider _serviceProvider;

        private readonly IBusContext<IConnection> _context;
        private readonly IModel _channel;

        public EventReplayService(IBusContext<IConnection> context, IServiceCollection services, int timeout)
        {
            _context = context;
            _timeout = timeout;

            _serviceProvider = services.BuildServiceProvider();
            _channel = _context.Connection.CreateModel();
        }

        public void StartEventReplay()
        {
            StartArtikelReplay();
            CheckConnection(NameConstants.BffWebshopEventReplayQueue);

            StartVoorraadReplay();
            CheckConnection(NameConstants.BffWebshopEventReplayQueue);
        }

        private void StartArtikelReplay()
        {
            var listener = CreateEventListenerInstance<EventListener>();

            CreateReceiver(
                handle: listener.ReceiveAddArtikelToCatalogus,
                queueName: NameConstants.BffWebshopEventReplayQueue,
                expressions: new List<string> { NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent }
            );

            SendReplayCommand(NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent);
        }

        private void StartVoorraadReplay()
        {
            var listener = CreateEventListenerInstance<EventListener>();

            CreateReceiver(
                handle: listener.HandleVoorraadChanged,
                queueName: NameConstants.BffWebshopEventReplayQueue,
                expressions: new List<string> { NameConstants.MagazijnServiceVoorraadChangedEvent }
            );

            SendReplayCommand(NameConstants.MagazijnServiceVoorraadChangedEvent);
        }

        private void CreateReceiver(EventMessageReceivedCallback handle, string queueName, List<string> expressions)
        {
            var messageReceiver = _context.CreateMessageReceiver(
                queueName: queueName,
                topicExpressions: expressions
            );

            messageReceiver.DeclareQueue();
            messageReceiver.StartReceivingMessages(handle);
        }

        private void SendReplayCommand(string eventType)
        {
            ICommandSender commandSender = _context.CreateCommandSender();
            ReplayEventsCommand replayEventsCommand = new ReplayEventsCommand()
            {
                ExchangeName = NameConstants.BffWebshopEventReplayExchange,
                Topic = eventType
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
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        private void CheckConnection(string queueName)
        {
            Thread.Sleep(_timeout);

            while (!_context.IsConnectionIdle())
            {
                Thread.Sleep(_timeout);
            }

            _channel.QueueDelete(queueName, false, false);
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
                _channel.ExchangeDelete(NameConstants.BffWebshopEventReplayExchange, false);

                _channel?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
