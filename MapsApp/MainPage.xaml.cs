using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;


namespace MapsApp;
public partial class MainPage : ContentPage
{
private AirportViewModel viewModel;

    public MainPage()
    {
        InitializeComponent();
        viewModel = new AirportViewModel();
        BindingContext = viewModel;
    }

    public async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue.ToLower();
        if (string.IsNullOrWhiteSpace(query))
        {
            SuggestionsCollectionView.IsVisible = false;
            return;
        }

        if (viewModel.AirportsList != null)
        {
            var suggestions = viewModel.AirportsList
                .Where(a => a.Name.ToLower().Contains(query) || a.ICAO.ToLower().Contains(query)) // Filter by name or ICAO code
                .Select(a => $"{a.Name} ({a.ICAO})")
                .ToList();

            if (suggestions.Any())
            {
                SuggestionsCollectionView.ItemsSource = suggestions;
                SuggestionsCollectionView.IsVisible = true;
            }
            else
            {
                SuggestionsCollectionView.IsVisible = false;
            }
        }
    }

    public void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            var selectedSuggestion = e.CurrentSelection[0].ToString();
            SearchEntry.Text = selectedSuggestion;
            SuggestionsCollectionView.IsVisible = false;
            SuggestionsCollectionView.SelectedItem = null; 

            var selectedAirportName = selectedSuggestion.Split('(')[0].Trim().ToLower();
            Console.WriteLine(selectedAirportName);

            var matchingAirport = viewModel.AirportsList
                .FirstOrDefault(a => a.Name.ToLower().Contains(selectedAirportName));

            if (matchingAirport != null)
            {
                var longitude = matchingAirport.Longitude;
                var latitude = matchingAirport.Latitude;
                Console.WriteLine("Name"+matchingAirport.Longitude.ToString());

                if (double.TryParse(latitude, out double lat) && double.TryParse(longitude, out double lon))
                {
                    Location location = new Location(lat, lon);
                    MapSpan mapSpan = new MapSpan(location, 0.2, 0.2);
                    map.MoveToRegion(mapSpan);
                    var pin = new Pin
                    {
                        Location = location,
                        Label = matchingAirport.Name,
                        Address = matchingAirport.ICAO
                    };
                    map.Pins.Add(pin);

                    SearchEntry.Unfocus();

                }
                else
                {
                    Console.WriteLine("Invalid latitude or longitude.");
                }
            }
            else
            {
                Console.WriteLine("No matching airport found.");
            }
        }
    }
}
