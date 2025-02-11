using decorehubweb.Pages.Calculator.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Text;

namespace decorehubweb.Pages.Calculator;

public partial class CalculateSalePrice
{
    [Parameter]
    public Channels channel { get; set; } = null!;
    public bool comissionIsVariable { get; set; }
    public bool AdClassOrPremium;

    CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");

    decimal salePrice;
    decimal marginPercent;
    decimal margin;
    decimal buy_cost;
    decimal comission;
    decimal comissionPercent;
    decimal adsPercent;
    decimal taxPercent;
    decimal ads;
    decimal freight_cost;
    decimal cupon;
    decimal ship_cost;
    decimal tax;
    string pc = string.Empty;

    string adsHelper = string.Empty;
    string taxHelper = string.Empty;
    string comissionHelper = string.Empty;


    protected override void OnInitialized()
    {
        adsHelper = channel.adsPercent.ToString() + "%";
        taxHelper = channel.taxPercent.ToString() + "%";
        comissionHelper = channel.comissionPercent.ToString() + "%";

        adsPercent = channel.adsPercent;
        taxPercent = channel.taxPercent;
        comissionPercent = channel.comissionPercent;
    }

    private async void CalculateWithoutVariableComission()
    {
        comissionHelper = comissionPercent.ToString() + "%";
        await Calculate(comissionPercent);
    }

    private Task<bool> Calculate(decimal comissionParameter)
    {
        if (marginPercent == 0)
        {
            SnackBar.Add("Informe a margem que deseja obter!", Severity.Error);
            return Task.FromResult(false);
        }

        var fixCost = buy_cost + freight_cost + cupon + ship_cost;
        if (fixCost == 0) return Task.FromResult(false);

        var percent = 1 - ((adsPercent + taxPercent + comissionParameter + marginPercent)) / 100;

        salePrice = fixCost / percent;

        ads = salePrice * (adsPercent / 100);
        tax = salePrice * (taxPercent / 100);
        comission = salePrice * (comissionParameter / 100);

        margin = salePrice - fixCost - ads - tax - comission;

        pc = $"{((margin / buy_cost) * 100).ToString("F2")}";

        return Task.FromResult(true);
    }

    private void Clear()
    {
        salePrice = 0;
        marginPercent = 0;
        margin = 0;
        buy_cost = 0;
        comission = 0;
        ads = 0;
        freight_cost = 0;
        cupon = 0;
        ship_cost = 0;
        tax = 0;
        pc = string.Empty;
        //quotations = new();
        //width = 0;
        //height = 0;
        //length = 0;
        //weight = 0;
        //cep = new();
        //cepValue = string.Empty;
        OnInitialized();
    }

    private string? downloadUrl;
    private async Task DownloadFile()
    {
        string fileContent = CreateFileTxt();

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);
        string base64File = Convert.ToBase64String(fileBytes);

        downloadUrl = $"data:text/plain;base64,{base64File}";

        await JS.InvokeVoidAsync("clickDownloadLink", downloadUrl);
    }

    private string CreateFileTxt()
    {
        var sb = new StringBuilder();

        int leftColumnWidth = 32;
        int rightColumnWidth = 56;

        string borderLine = "|" + new string('-', leftColumnWidth) + "|" + new string('-', rightColumnWidth + 1) + "|";

        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Canal", channel.Channel, leftColumnWidth, rightColumnWidth));  // Canal
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Margem desejada", $"{marginPercent}%", leftColumnWidth, rightColumnWidth));  // Margem desejada
        sb.AppendLine(borderLine);

        sb.AppendLine(FormatRow("Preço de compra", $"R$ {buy_cost}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Preço de frete", $"R$ {freight_cost}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Cupom", $"R$ {cupon}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Custo de envio", $"R$ {ship_cost}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);

        sb.AppendLine(FormatRow("Comissão (15%)", $"R$ {comission:F2}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Ads (10%)", $"R$ {ads:F2}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Imposto (15%)", $"R$ {tax:F2}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("P.C (Margem / Preço de compra)", $"{pc}%", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);

        //sb.AppendLine(FormatRow("Transportadora", offerSelected?.carrier.company_name ?? " ", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        //sb.AppendLine(FormatRow("Prazo", $"{offerSelected?.delivery_time.days} dias", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        //sb.AppendLine(FormatRow("Preço", $"R$ {offerSelected?.final_price}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        sb.AppendLine(FormatRow("Estimativa de entrega", "20/01/2025", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);

        sb.AppendLine(FormatRow("Preço de venda", "R$ 870,75", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);
        //sb.AppendLine(FormatRow("Margem", $"{Convert.ToDateTime(offerSelected?.delivery_time.estimated_date).ToString("dd/MM/yyyy")}", leftColumnWidth, rightColumnWidth));
        sb.AppendLine(borderLine);

        return sb.ToString();
    }

    string FormatRow(string label, string value, int labelWidth, int valueWidth)
    {
        return $"|{label.PadRight(labelWidth)}| {value.PadRight(valueWidth)}|";
    }
}
