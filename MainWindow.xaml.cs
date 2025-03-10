using System.Text;
using System.Windows;
using System.Windows.Media;

namespace FutPosCalc;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitInput();
    }

    private void InitInput()
    {
        ctLiquidationPrice.Text = Properties.Settings.Default.LiquidationPrice;
        ctEntryPrice.Text = Properties.Settings.Default.EntryPrice;
        ctLeverage.Text = Properties.Settings.Default.Leverage;
        ctOutput.Text = "";
        ctTakeProfitAt.Text = Properties.Settings.Default.TakeProfitAt;
        ctTradeAmount.Text = Properties.Settings.Default.TradeAmount;
        ProcessInput();
    }

    private void SaveInput()
    {
        Properties.Settings.Default.LiquidationPrice = ctLiquidationPrice.Text;
        Properties.Settings.Default.EntryPrice = ctEntryPrice.Text;
        Properties.Settings.Default.Leverage = ctLeverage.Text;
        Properties.Settings.Default.TakeProfitAt = ctTakeProfitAt.Text;
        Properties.Settings.Default.TradeAmount = ctTradeAmount.Text;
        Properties.Settings.Default.Save();
    }

    private void RenderResult(FuturesPosition position, FormInput input)
    {
        var sb = new StringBuilder();

        var padLeft = 19;
        var padRight = 10;

        sb.Append("Position Size:".PadLeft(padLeft));
        sb.AppendLine($"{position.PositionSize:F2}".PadLeft(padRight));

        sb.Append("Additional Margin:".PadLeft(padLeft));
        sb.AppendLine($"{position.AdditionalMargin:F2}".PadLeft(padRight));

        if (input.HasTakeProfitAt)
        {
            sb.Append("Estimated Profit:".PadLeft(padLeft));
            sb.AppendLine($"{position.EstimatedProfit:F2}".PadLeft(padRight));
        }

        sb.AppendLine();
        
        sb.Append("Volatility Allowed:".PadLeft(padLeft));
        sb.AppendLine($"{position.Volatility:F2}%".PadLeft(padRight));

        sb.Append("Half Loss At:".PadLeft(padLeft));
        sb.AppendLine($"{position.HalfLossPrice:F2}".PadLeft(padRight));

        ctOutput.Text = sb.ToString();
    }

    private void UpdateSide()
    {
        decimal liquidationPrice;
        decimal entryPrice;
        if (
            decimal.TryParse(ctLiquidationPrice.Text.Trim(), out liquidationPrice)
            &&
            decimal.TryParse(ctEntryPrice.Text.Trim(), out entryPrice)
            &&
            liquidationPrice > 0
            &&
            entryPrice > 0
            && liquidationPrice != entryPrice
        )
        {
            ctSide.Visibility = Visibility.Visible;
            if (liquidationPrice < entryPrice)
            {
                ctSide.Content = "Long";
                ctSide.Foreground = new SolidColorBrush(Colors.Green);

            }
            else
            {
                ctSide.Content = "Short";
                ctSide.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        else
        {
            ctSide.Visibility = Visibility.Hidden;
        }
    }

    private void ProcessInput()
    {
        UpdateSide();

        var input = new FormInputRaw
        {
            LiquidationPrice = ctLiquidationPrice.Text,
            EntryPrice = ctEntryPrice.Text,
            Leverage = ctLeverage.Text,
            TakeProfitAt = ctTakeProfitAt.Text,
            TradeAmount = ctTradeAmount.Text,
        };

        var (isValid, error, validInput) = input.IsValid();

        if (!isValid)
        {
            ctOutput.Text = error;
            ctOutput.Foreground = new SolidColorBrush(Colors.Red);
            return;
        }

        ctOutput.Foreground = new SolidColorBrush(Colors.Black);

        var calc = new FuturesPositionCalculator();

        var result = calc.Calculate(validInput);

        RenderResult(result, validInput);
        SaveInput();
    }

    private void Input_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        ProcessInput();
    }
}