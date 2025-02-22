using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace ha_sus_ck_sex
{
    public class WeatherHandler
    {
        private static readonly HttpClient client = new HttpClient();

        public WeatherHandler()
        {
            GetWeather();
        }

        public async void GetWeather()
        {
            Console.WriteLine(await GetJsonAsync("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m"));
        }

        public static async Task<string> GetJsonAsync(string url)
        {
            try
            {
                // Make an asynchronous GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string jsonData = await response.Content.ReadAsStringAsync();

                return jsonData;
            }
            catch (HttpRequestException e)
            {
                // Handle any errors that occur during the request
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
}