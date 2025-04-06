namespace FutPosCalc;

public static class FormInputExtensions
{
    private static (bool, string) ValidateDecimal(
        string decimalStr,
        ref decimal value,
        string name,
        bool enabled = true)
    {
        if (!enabled)
        {
            return (true, String.Empty);
        }

        decimalStr = decimalStr.Trim();

        if (string.IsNullOrEmpty(decimalStr))
        {
            return (false, $"{name} is empty");
        }

        if (!decimal.TryParse(decimalStr, out value))
        {
            return (false, $"{name} is not a number");
        }

        if (value <= 0)
        {
            return (false, $"{name} must be > 0");
        }

        return (true, String.Empty);
    }

    public static (bool, string, FormInput) IsValid(this FormInputRaw input)
    {
        FormInput result = new FormInput();

        var resultType = result.GetType();

        result.HasTakeProfitAt = !string.IsNullOrEmpty(input.TakeProfitAt);
        result.HasLiquidation = !string.IsNullOrEmpty(input.LiquidationPrice);

        foreach (var (b, e) in new[]{
                ValidateDecimal(input.EntryPrice, ref result.EntryPrice, "Entry Price"),
                ValidateDecimal(input.LiquidationPrice, ref result.LiquidationPrice, "Liquidation Price", result.HasLiquidation),
                ValidateDecimal(input.Leverage, ref result.Leverage, "Leverage"),
                ValidateDecimal(input.TradeAmount, ref result.TradeAmount, "Trade Amount"),
                ValidateDecimal(input.TakeProfitAt!, ref result.TakeProfitAt, "Take Profit", result.HasTakeProfitAt),
            })
        {
            if (!b)
            {
                return (b, e, result);
            }
        }

        if (!result.HasTakeProfitAt & !result.HasLiquidation)
        {
            return (false, "Either 'Liquidation' or 'Take profit' must be filled", result);
        }

        if (result.HasTakeProfitAt & result.HasLiquidation)
        {
            if (
                ((result.TakeProfitAt > result.EntryPrice) & (result.LiquidationPrice > result.EntryPrice))
                |
                ((result.TakeProfitAt < result.EntryPrice) & (result.LiquidationPrice < result.EntryPrice))
            )
            {
                return (false, "'Take Profit' contradicts 'Liquidation'", result);
            }
        }

        return (true, String.Empty, result);
    }

}