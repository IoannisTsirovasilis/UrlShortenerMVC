using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UrlShortenerMVC.Helpers
{
    public class CaptchaResponseHelper
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorMessage { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime Challenge { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}