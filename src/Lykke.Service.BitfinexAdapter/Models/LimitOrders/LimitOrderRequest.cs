﻿using Lykke.Service.BitfinexAdapter.Core.Domain.Trading.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.BitfinexAdapter.Models.LimitOrders
{
    public class LimitOrderRequest
    {
        /// <summary>
        /// name of instrument (asset pair)
        /// </summary>
        [JsonProperty("instrument")]
        [Required]
        public string Instrument { get; set; }

        /// <summary>
        /// price of order
        /// </summary>
        [JsonProperty("price")]
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// volume of order
        /// </summary>
        [JsonProperty("volume")]
        [Required]
        public decimal Volume { get; set; }

        /// <summary>
        /// side of trade: Buy, Sell
        /// </summary>
        [JsonProperty("tradeSide")]
        [Required]
        public TradeSide TradeSide { get; set; } //needs validation

    }
}
