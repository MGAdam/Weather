using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weather
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();



        static async Task Main()
        {
           var locator = new Windows.Devices.Geolocation.Geolocator();

            var position = (await locator.GetGeopositionAsync()).Coordinate.Point.Position;

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={position.Latitude}&lon={position.Longitude}&appid=171dd6e6b0fe8d04a0fd21a1a4330d49&units=metric");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
            }

            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

            }

        }


        
    }

}
