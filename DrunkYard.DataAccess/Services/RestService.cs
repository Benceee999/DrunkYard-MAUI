using System.Text.Json;
using System.Text.Json.Serialization;
using DrunkYard.DataAccess.Models;

namespace DrunkYard.DataAccess.Services
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        private const string Url = "https://693d35d1f55f1be793022db3.mockapi.io/games";

        private const string CandidatesUrl = "https://693d35d1f55f1be793022db3.mockapi.io/candidates";

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,

                Converters = { new JsonStringEnumConverter() },

                NumberHandling = JsonNumberHandling.AllowReadingFromString,

                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<DrinkGame>> RefreshDataAsync()
        {
            List<DrinkGame> items = new List<DrinkGame>();

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                System.Diagnostics.Debug.WriteLine("Nincs net, offline mód.");
                return items;
            }

            try
            {
                HttpResponseMessage response = await _client.GetAsync(Url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<DrinkGame>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Hiba: {ex.Message}");
            }

            return items;
        }

        public async Task SaveGameAsync(DrinkGame game)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;

            try
            {
                string json = JsonSerializer.Serialize(game, _serializerOptions);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                await _client.PostAsync(Url, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API POST Hiba: {ex.Message}");
            }
        }

        public async Task<List<CandidateGame>> GetCandidatesAsync()
        {
            List<CandidateGame> items = new List<CandidateGame>();

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return items;

            try
            {
                HttpResponseMessage response = await _client.GetAsync(CandidatesUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<CandidateGame>>(content, _serializerOptions);
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"GetCandidates Hiba: {ex.Message}"); }

            return items;
        }

        public async Task SubmitCandidateGameAsync(CandidateGame game)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;
            try
            {
                string json = JsonSerializer.Serialize(game, _serializerOptions);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _client.PostAsync(CandidatesUrl, content);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"POST Candidate Hiba: {ex.Message}"); }
        }

        public async Task VoteForCandidateAsync(CandidateGame candidate)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;
            try
            {
                string editUrl = $"{CandidatesUrl}/{candidate.Id}";
                string json = JsonSerializer.Serialize(candidate, _serializerOptions);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _client.PutAsync(editUrl, content);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Vote Hiba: {ex.Message}"); }
        }
    }
}