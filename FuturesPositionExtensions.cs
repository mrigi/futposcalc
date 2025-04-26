using System.Diagnostics;
using System.Text;

namespace FutPosCalc;

static class FuturesPositionExtensions
{
    private static string FormatNumber(decimal number)
    {
        int decimals = 2;

        var t = number;
        while (t < 1)
        {
            t *= 10;
            decimals += 1;
        }

        return number.ToString("#,##0" + (number < 100 ? ".".PadRight(decimals + 1, '0') : ""));
    }

    public static string ToString(this FuturesPosition position, FormInput input)
    {
        var sb = new StringBuilder();

        var padLeft = 19;
        var padRight = 13;

        int priceDecimals = 2;

        var t = position.LossPrice;
        while (t < 1)
        {
            t *= 10;
            priceDecimals += 1;
        }


        sb.Append("Position Size:".PadLeft(padLeft));
        sb.AppendLine(FormatNumber(position.PositionSize).PadLeft(padRight));

        if (position.AdditionalMargin != 0m)
        {
            sb.Append("Additional Margin:".PadLeft(padLeft));
            sb.AppendLine(FormatNumber(position.AdditionalMargin).PadLeft(padRight));
        }

        if (input.HasTakeProfitAt)
        {
            sb.Append("Estimated Profit:".PadLeft(padLeft));
            sb.AppendLine(FormatNumber(position.EstimatedProfit).PadLeft(padRight));
        }

        sb.AppendLine();


        var priceDiffPct = 0m;

        try
        {
            priceDiffPct = Math.Abs((input.LiquidationPrice - position.LossPrice) / position.LossPrice);
        }
        catch
        {
            Debug.WriteLine($"LossPrice={FormatNumber(position.LossPrice)}");
        }

        if (!input.HasLiquidation | priceDiffPct > 0.001m)
        {
            sb.Append("Liquidation At:".PadLeft(padLeft));
            sb.AppendLine(FormatNumber(position.LossPrice).PadLeft(padRight));
        }

        sb.Append("Reclaim body price:".PadLeft(padLeft));
        sb.AppendLine(FormatNumber(position.PositionSizePrice).PadLeft(padRight));

        sb.AppendLine();

        sb.Append("Volatility Allowed:".PadLeft(padLeft));
        sb.AppendLine($"{FormatNumber(position.Volatility)}%".PadLeft(padRight));
        //sb.Append("Half Loss At:".PadLeft(padLeft));
        //sb.AppendLine($"{position.HalfLossPrice:F2}".PadLeft(padRight));

        return sb.ToString();
    }

}
