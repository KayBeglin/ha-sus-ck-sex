using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ha_sus_ck_sex
{
    public class PythonHandling : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();
        private string _sessionId;
        private const string BaseUrl = "http://localhost:5000/";

        public PythonHandling()
        {
            doStuff();
        }

        public async void doStuff()
        {
            await StartSession("You are a cat. Your name is Todd.");
            await SendMessage("Hello, Todd!");
        }

        public async Task StartSession(string systemPrompt = null)
        {
            var request = new
            {
                system_prompt = systemPrompt
            };

            var response = await PostAsync("start", request);
            var result = JsonConvert.DeserializeObject<SessionResponse>(response);
            _sessionId = result.session_id;
        }

        public async Task<string> SendMessage(string message)
        {
            if (string.IsNullOrEmpty(_sessionId))
                throw new InvalidOperationException("Session not started. Call StartSession first.");

            var request = new
            {
                session_id = _sessionId,
                message = message
            };

            var response = await PostAsync("chat", request);
            var result = JsonConvert.DeserializeObject<ChatResponse>(response);
            Console.WriteLine(result.response);
            return result.response;
        }

        public async Task EndSession()
        {
            if (string.IsNullOrEmpty(_sessionId)) return;

            var request = new
            {
                session_id = _sessionId
            };

            await PostAsync("end", request);
            _sessionId = null;
        }

        private async Task<string> PostAsync(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{BaseUrl}{endpoint}", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        // Response classes
        private class SessionResponse
        {
            public string session_id { get; set; }
        }

        private class ChatResponse
        {
            public string response { get; set; }
            public string session_id { get; set; }
        }
    }
}
