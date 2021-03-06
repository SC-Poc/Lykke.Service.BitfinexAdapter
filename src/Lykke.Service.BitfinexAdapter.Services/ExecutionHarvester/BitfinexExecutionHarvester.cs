﻿using System.Threading;
using Common.Log;
using Lykke.Service.BitfinexAdapter.Core.Domain.Trading;
using Lykke.Service.BitfinexAdapter.Core.Domain.WebSocketClient;
using Lykke.Service.BitfinexAdapter.Core.Handlers;
using Lykke.Service.BitfinexAdapter.Core.Utils;
using Lykke.Service.BitfinexAdapter.Core.WebSocketClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.BitfinexAdapter.Services.ExecutionHarvester
{
    public sealed class BitfinexExecutionHarvester : IHostedService
    {
        private readonly IBitfinexWebSocketSubscriber _socketSubscriber;
        private readonly BitfinexModelConverter _bitfinexModelConverter;
        private readonly IHandler<ExecutionReport> _handler;
        private readonly ILog _log;

        public BitfinexExecutionHarvester(IBitfinexWebSocketSubscriber socketSubscriber, 
            BitfinexModelConverter bitfinexModelConverter, 
            IHandler<ExecutionReport> handler, 
            ILog log
           )
        {
            _socketSubscriber = socketSubscriber;
            _bitfinexModelConverter = bitfinexModelConverter;
            _handler = handler;
            _log = log;
        }

        private Task MessageHandler(TradeExecutionUpdate tradeUpdate)
        {
            var execution = _bitfinexModelConverter.ToOrderStatusUpdate(tradeUpdate);
            return _handler.Handle(execution);
        }

        private Task MessageDispatcher(dynamic message)
        {
            return MessageHandler(message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _socketSubscriber.Subscribe(MessageDispatcher);
            _socketSubscriber.Start();
            _log.WriteInfoAsync(GetType().Name, "Initialization", "Started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _socketSubscriber.Stop();
            _log.WriteInfoAsync(GetType().Name, "Cleanup", "Stopped");
            return Task.CompletedTask;
        }
    }
}
