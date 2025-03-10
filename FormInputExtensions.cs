﻿namespace FutPosCalc;

public static class FormInputExtensions
{
    private static (bool, string) ValidateDecimal(string decimalStr, ref decimal value, string name, bool enabled = true)
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

        foreach (var (b, e) in new[]{
                ValidateDecimal(input.LiquidationPrice, ref result.LiquidationPrice, "Liquidation Price"),
                ValidateDecimal(input.EntryPrice, ref result.EntryPrice, "Entry Price"),
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

        return (true, String.Empty, result);
    }

}