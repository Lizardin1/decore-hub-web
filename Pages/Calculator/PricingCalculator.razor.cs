using decorehubweb.Pages.Calculator.Models;
using Newtonsoft.Json;

namespace decorehubweb.Pages.Calculator;
public partial class PricingCalculator
{
    public List<Channels>? channels { get; set; } = new();
    private Channels? channel;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = await client.GetAsync("channels");
            var content = await request.Content.ReadAsStringAsync();
            channels = JsonConvert.DeserializeObject<List<Channels>>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void OnCanalValueChanged(string newValue)
    {
        channel = channels.FirstOrDefault(x => x.Channel == newValue);
        StateHasChanged();
    }
}