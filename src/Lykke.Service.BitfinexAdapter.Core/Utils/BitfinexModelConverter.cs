﻿using Lykke.Service.BitfinexAdapter.Core.Domain;
using Lykke.Service.BitfinexAdapter.Core.Domain.OrderBooks;
using Lykke.Service.BitfinexAdapter.Core.Domain.Settings;
using Lykke.Service.BitfinexAdapter.Core.Domain.Trading;
using Lykke.Service.BitfinexAdapter.Core.Domain.Trading.Enums;
using Lykke.Service.BitfinexAdapter.Core.Domain.WebSocketClient;
using System;
using System.Globalization;

namespace Lykke.Service.BitfinexAdapter.Core.Utils
{
    public sealed class BitfinexModelConverter : ExchangeConverters
    {

        public BitfinexModelConverter(BitfinexAdapterSettings configuration) : base(configuration.SupportedCurrencySymbols, Constants.BitfinexExchangeName, configuration.UseSupportedCurrencySymbolsAsFilter)
        {
        }

        public static OrderBookItem ToOrderBookItem(OrderBookItemResponse response)
        {
            return new OrderBookItem
            {
                Id = response.Id.ToString(CultureInfo.InvariantCulture),
                IsBuy = response.Amount > 0,
                Price = response.Price,
                Symbol = response.Pair,
                Size = response.Amount
            };
        }

        public ExecutionReport ToOrderStatusUpdate(TradeExecutionUpdate eu)
        {
            var instrument = ExchangeSymbolToLykkeInstrument(eu.AssetPair);
            var transactionTime = eu.TimeStamp;
            var tradeType = ConvertTradeType(eu.Volume);
            var orderId = eu.OrderId.ToString();
            return new ExecutionReport(instrument, transactionTime, eu.Price, eu.Volume, tradeType, orderId, OrderExecutionStatus.Fill)
            {
                Message = eu.OrderType,
                Fee = eu.Fee,
                ExecType = ExecType.Trade
            };
        }

        public string ConvertOrderType(OrderType type)
        {
            switch (type)
            {
                case OrderType.Market:
                    return "market";
                case OrderType.Limit:
                    return "limit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public string ConvertTradeType(TradeType signalTradeType)
        {
            switch (signalTradeType)
            {
                case TradeType.Buy:
                    return "buy";
                case TradeType.Sell:
                    return "sell";
                default:
                    throw new ArgumentOutOfRangeException(nameof(signalTradeType), signalTradeType, null);
            }
        }

        public static TradeType ConvertTradeType(string signalTradeType)
        {
            switch (signalTradeType)
            {
                case "buy":
                    return TradeType.Buy;
                case "sell":
                    return TradeType.Sell;
                default:
                    throw new ArgumentOutOfRangeException(nameof(signalTradeType), signalTradeType, null);
            }
        }

        public static TradeType ConvertTradeType(decimal amount)
        {
            return amount > 0 ? TradeType.Buy : TradeType.Sell;
        }

    }
}