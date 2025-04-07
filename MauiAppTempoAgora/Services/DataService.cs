using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "128c2b461a476be9ee174898a30e2777";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&lang=pt_br&appid={chave}";

            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JObject.Parse(json);

                    DateTime sunrise = DateTimeOffset
                        .FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"])
                        .ToLocalTime().DateTime;

                    DateTime sunset = DateTimeOffset
                        .FromUnixTimeSeconds((long)rascunho["sys"]["sunset"])
                        .ToLocalTime().DateTime;

                    t = new Tempo
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString("dd/MM/yyyy HH:mm:ss"),
                        sunset = sunset.ToString("dd/MM/yyyy HH:mm:ss"),
                    };
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Cidade não encontrada. Verifique o nome digitado.");
                }
                else
                {
                    throw new Exception($"Erro na requisição: {resp.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception("Sem conexão com a internet. Verifique sua rede.");
            }

            return t;
        }
    }
}
