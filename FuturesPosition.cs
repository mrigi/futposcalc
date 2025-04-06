
using System.Text;

namespace FutPosCalc;

public class FuturesPosition
{
    public bool IsLong { get; set; }
    public decimal PositionSize { get; set; }
    public decimal AdditionalMargin { get; set; }
    public decimal Volatility { get; set; }
    public decimal HalfLossPrice { get; set; }
    public decimal LossPrice { get; set; }
    public decimal EstimatedProfit { get; set; }
}
