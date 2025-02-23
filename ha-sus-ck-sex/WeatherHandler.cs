using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ha_sus_ck_sex
{
    public class WeatherHandler
    {
        private static readonly HttpClient client = new HttpClient();
        private SpeechBubble speechBubble;
        private string[] weatherArray;

        public WeatherHandler(SpeechBubble speechBubble)
        {
            this.speechBubble = speechBubble;
        }

        public void OutputData(string city)
        {
            GetTempData(city);
        }

        public async void GetTempData(string city)
        {
            //string city = Console.ReadLine();
            var (latitude, longitude) = await GetCoordinatesAsync(city);

            if (latitude != null && longitude != null)
            {
                string tempData = await GetJsonAsync($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,rain,snowfall&forecast_days=1");
                if (tempData != null)
                {
                    // Format the JSON data
                    string formattedJson = FormatJson(tempData);

                    // Parse the JSON data and store it in a dictionary
                    Dictionary<string, (double Temperature, double Rain, double Snowfall)> tempDictionary = ParseWeatherData(tempData);

                    // Print the dictionary contents to the console
                    foreach (var entry in tempDictionary)
                    {
                        speechBubble.StartTextAnimation($"Time: {entry.Key}, Temperature: {entry.Value.Temperature}°C, Rain: {entry.Value.Rain}mm, Snow: {entry.Value.Snowfall}mm");
                    }
                    ;
                }
            }
            else
            {
                Console.WriteLine("Address not found.");
            }
        }

        public static async Task<string> GetJsonAsync(string url)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.1; x64; en-US) AppleWebKit/534.11 (KHTML, like Gecko) Chrome/47.0.2412.179 Safari/602");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string jsonData = await response.Content.ReadAsStringAsync();
                return jsonData;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }

        public static async Task<(double?, double?)> GetCoordinatesAsync(string city)
        {
            string addressUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(city)}&format=json&limit=1";
            string responseContent = await GetJsonAsync(addressUrl);

            if (responseContent != null)
            {
                JArray json = JArray.Parse(responseContent);
                if (json.Count > 0)
                {
                    double latitude = (double)json[0]["lat"];
                    double longitude = (double)json[0]["lon"];
                    return (latitude, longitude);
                }
            }
            return (null, null);
        }

        private string FormatJson(string json)
        {
            try
            {
                var parsedJson = JToken.Parse(json);
                return parsedJson.ToString(Formatting.Indented);
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine($"JSON format error: {e.Message}");
                return json;
            }
        }

        private Dictionary<string, (double Temperature, double Rain, double Snowfall)> ParseWeatherData(string jsonData)
        {
            var weatherDictionary = new Dictionary<string, (double Temperature, double Rain, double Snowfall)>();

            try
            {
                // Parse the JSON data
                JObject json = JObject.Parse(jsonData);

                // Extract the time and temperature information
                var times = json["hourly"]?["time"]?.Values<string>();
                var temperatures = json["hourly"]?["temperature_2m"]?.Values<double>();
                var rain = json["hourly"]?["rain"]?.Values<double>();
                var snowfall = json["hourly"]?["snowfall"]?.Values<double>();
                if (times == null || temperatures == null || rain == null || snowfall == null)
                {
                    Console.WriteLine("Error: Missing 'time', 'temperature_2m', 'rain' or 'snowfall' data in JSON response.");
                    return weatherDictionary;
                }

                // Store the time and temperature data in the dictionary
                using (var timeEnumerator = times.GetEnumerator())
                using (var tempEnumerator = temperatures.GetEnumerator())
                using (var rainEnumerator = rain.GetEnumerator())
                using (var snowfallEnumerator = snowfall.GetEnumerator())
                {
                    while (timeEnumerator.MoveNext() && tempEnumerator.MoveNext() && rainEnumerator.MoveNext() && snowfallEnumerator.MoveNext())
                    {
                        weatherDictionary[timeEnumerator.Current] = (tempEnumerator.Current, rainEnumerator.Current, snowfallEnumerator.Current);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"JSON parsing error: {e.Message}");
            }

            return weatherDictionary;
        }
    }
}
