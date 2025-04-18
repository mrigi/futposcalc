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
        ctOutput.Text = position.ToString(input);

        ctSide.Visibility = Visibility.Visible;

        if (position.IsLong)
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

    private void ProcessInput()
    {
        ctSide.Visibility = Visibility.Hidden;

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