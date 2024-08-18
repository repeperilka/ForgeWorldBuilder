using System.Net;
using Newtonsoft.Json;
namespace Scryfall{




    public class ScryfallList{
        public string next_page;
        public bool has_more;
        public List<ScryfallCard> data;
    }
    public class ScryfallCard{
        public string name;
    }
    public static class ScryfallManager{
        public static HttpClient client = new HttpClient();
        public static string scryfallApi = "https://api.scryfall.com";
        public static async Task<List<ScryfallCard>> SearchCards(string command){
            
            string finalUrl = scryfallApi + "/cards/search?q=" + command.Replace(" ", "%20");
            List<ScryfallCard> returnedCards = new List<ScryfallCard>();
            
            while(finalUrl != ""){
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, finalUrl);
                message.Headers.Add("Accept", "*/*");
                message.Headers.Add("User-Agent", "ForgeEditions/0.4");
                HttpResponseMessage response = await client.SendAsync(message);
                if(response.IsSuccessStatusCode){
                    Console.WriteLine("Success");
                }
                ScryfallList list = JsonConvert.DeserializeObject<ScryfallList>(await response.Content.ReadAsStringAsync());
                returnedCards.AddRange(list.data);
                if(list.has_more){
                    finalUrl = list.next_page;
                }else{
                    finalUrl = "";
                }
            }
            return returnedCards;
        }
    }
}