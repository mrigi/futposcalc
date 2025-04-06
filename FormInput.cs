using System.Text;

namespace FutPosCalc;

public class FormInputRaw
{
    public string LiquidationPrice { get; set; } = String.Empty;
    public string EntryPrice { get; set; } = String.Empty;
    public string Leverage { get; set; } = String.Empty;
    public string TradeAmount { get; set; } = String.Empty;
    public string TakeProfitAt { get; set; } = String.Empty;
}

public class FormInput
{
    public decimal LiquidationPrice = 0m;
    public decimal EntryPrice = 0m;
    public decimal Leverage = 0m;
    public decimal TradeAmount = 0m;
    public decimal TakeProfitAt = 0m;
    public bool HasTakeProfitAt = false;
    public bool HasLiquidation = false;
}
