using decorehubweb.Pages.Calculator.Models;
using MudBlazor;
using Newtonsoft.Json;
using System.Globalization;

namespace decorehubweb.Pages.Calculator;

public partial class Configurations
{
    public List<Channels> channels { get; set; } = new();

    CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");
    private Dictionary<int, bool> collapseStates = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = await client.GetAsync("channels");
            var content = await request.Content.ReadAsStringAsync();
            channels = JsonConvert.DeserializeObject<List<Channels>>(content) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void OnAdsChanged(decimal newValue, int Id)
    {
        var channel = channels.Where(x => x.Id == Id).First();
        channel.adsPercent = newValue;
    }
    private void OnComissionChanged(decimal newValue, int Id)
    {
        var channel = channels.Where(x => x.Id == Id).First();
        channel.comissionPercent = newValue;
    }
    private void OnTaxChanged(decimal newValue)
    {
        channels.ForEach(x => x.taxPercent = newValue);
    }
    private void OnStartValueChanged(decimal newValue, Guid comissionId, int channelId)
    {
        var comission = channels
            .First(x => x.Id == channelId).variableComission
            .First(x => x.Id == comissionId);


        if (newValue > comission.endValue && comission.endValue != 0)
        {
            SnackBar.Add("O valor inicial não pode ser maior que o valor final!", Severity.Error);
            return;
        }

        comission.startValue = newValue;
    }
    private void OnEndValueChanged(decimal newValue, Guid comissionId, int channelId)
    {
        var comission = channels
            .First(x => x.Id == channelId).variableComission
            .First(x => x.Id == comissionId);

        if (newValue < comission.startValue)
        {
            SnackBar.Add("O valor final não pode ser menor que o valor inicial!", Severity.Error);
            return;
        }

        comission.endValue = newValue;
    }
    private void OnPercentChanged(decimal newValue, Guid comissionId, int channelId)
    {
        var comission = channels
            .First(x => x.Id == channelId).variableComission
            .First(x => x.Id == comissionId);
        comission.percent = newValue / 100;
    }
    private async Task SaveChanges()
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(channels), null, "application/json");
            var request = await client.PutAsync("channels", content);
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SnackBar.Add("Valores atualizados", Severity.Success, config =>
                {
                    SnackBar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void ToggleCollapse(int Id)
    {
        if (!collapseStates.ContainsKey(Id))
        {
            collapseStates[Id] = true;
            return;
        }
        collapseStates[Id] = !collapseStates[Id];
    }
    private bool IsCollapseOpen(int id)
    {
        return collapseStates.TryGetValue(id, out var isOpen) && isOpen;
    }
    private void AddNewItem(int Id)
    {
        channels.First(x => x.Id == Id).variableComission.Add(new VariableComission());
    }
    private void DeleteItem(Guid comissionId, int channelId)
    {
        var channel = channels.First(x => x.Id == channelId);
        var comission = channel.variableComission.First(x => x.Id == comissionId);
        channel.variableComission.Remove(comission);
    }
}
