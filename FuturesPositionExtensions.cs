using System.Text;

namespace FutPosCalc;

static class FuturesPositionExtensions
{
    public static string ToString(this FuturesPosition position, FormInput input)
    {
        var sb = new StringBuilder();

        var padLeft = 19;
        var padRight = 10;

        sb.Append("Position Size:".PadLeft(padLeft));
        sb.AppendLine($"{position.PositionSize:F2}".PadLeft(padRight));

        if (position.AdditionalMargin != 0m)
        {
            sb.Append("Additional Margin:".PadLeft(padLeft));
            sb.AppendLine($"{position.AdditionalMargin:F2}".PadLeft(padRight));
        }

        if (input.HasTakeProfitAt)
        {
            sb.Append("Estimated Profit:".PadLeft(padLeft));
            sb.AppendLine($"{position.EstimatedProfit:F2}".PadLeft(padRight));
        }

        sb.AppendLine();

        sb.Append("Volatility Allowed:".PadLeft(padLeft));
        sb.AppendLine($"{position.Volatility:F2}%".PadLeft(padRight));

        //sb.Append("Half Loss At:".PadLeft(padLeft));
        //sb.AppendLine($"{position.HalfLossPrice:F2}".PadLeft(padRight));

        var priceDiffPct = 0m;

        try
        {
            priceDiffPct = Math.Abs((input.LiquidationPrice - position.LossPrice) / position.LossPrice);
        }
        catch {}

        if (!input.HasLiquidation | priceDiffPct > 0.001m)
        {
            sb.Append("Liquidation At:".PadLeft(padLeft));
            sb.AppendLine($"{position.LossPrice:F2}".PadLeft(padRight));
        }

        return sb.ToString();
    }

}
