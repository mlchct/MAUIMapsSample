using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;

namespace MapsApp;

public class AirportViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Airport> AirportsList { get;set; }
    public AirportViewModel()
    {
        AirportsList = new ObservableCollection<Airport>();

        InitializeAsync();

    }

    private async void InitializeAsync()
    {
        await RequestToAPI();
    }
    public async Task RequestToAPI()
    {
        try
        {
            var httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.api-ninjas.com/v1/airports?min_elevation=-1266");
            requestMessage.Headers.Add("X-Api-Key", "");
            var response = await httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode(); 
            var content = await response.Content.ReadFromJsonAsync<List<Airport>>();

            if (content != null)
            {
                foreach (var airport in content)
                {
                    AirportsList.Add (
                        new Airport{
                            ICAO = airport.ICAO,
                            IATA = airport.IATA,
                            Name = airport.Name,
                            Longitude = airport.Longitude,
                            Latitude = airport.Latitude
                        }
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

