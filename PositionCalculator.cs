namespace FutPosCalc;


public class FuturesPositionCalculator
{
    public FuturesPosition Calculate(FormInput input)
    {
        decimal liquidationPriceAt = input.LiquidationPrice;
        decimal entryPrice = input.EntryPrice;
        decimal takeProfitAt = input.TakeProfitAt;

        decimal leverage = input.Leverage;
        decimal tradeAmount = input.TradeAmount;

        decimal enterFee = 0.00m;
        decimal exitFee = 0.00m;

        decimal positionCost;
        decimal additionalMargin;
        decimal totalMargin;
        decimal positionLeveraged;
        decimal enterFeeCost;
        decimal costLeveraged;
        decimal liquidationPrice;
        decimal halfLossAt;
        decimal liquidationPercentage;
        decimal profit = 0m;
        decimal exitFeeCost;


        bool isLong = false;


        if (input.HasLiquidation)
        {
            isLong = input.LiquidationPrice < entryPrice;
        }

        else if (input.HasTakeProfitAt)
        {
            isLong = input.TakeProfitAt > entryPrice;

            liquidationPriceAt = entryPrice * (1 + 1/leverage);
        }

            var tradeType = isLong ? "Long" : "Short";
        var diff = 0m;

        do
        {
            positionCost = tradeAmount - diff;
            additionalMargin = tradeAmount - positionCost;

            totalMargin = positionCost + additionalMargin;

            positionLeveraged = positionCost * leverage;

            enterFeeCost = positionLeveraged * enterFee / 100;
            costLeveraged = positionLeveraged - enterFeeCost;


            liquidationPrice = isLong
                ? entryPrice * (1 - (totalMargin / (positionCost * leverage)))
                : entryPrice * (1 + (totalMargin / (positionCost * leverage)));

            halfLossAt = isLong
                ? entryPrice - ((entryPrice - liquidationPrice) / 2)
                : entryPrice + ((liquidationPrice - entryPrice) / 2);

            liquidationPercentage = isLong
                ? ((entryPrice - liquidationPrice) / entryPrice) * 100
                : ((liquidationPrice - entryPrice) / entryPrice) * 100;

            if (input.HasTakeProfitAt)
            {
                profit = isLong
                    ? (takeProfitAt - entryPrice) / entryPrice * costLeveraged
                    : (entryPrice - takeProfitAt) / entryPrice * costLeveraged;

                exitFeeCost = (costLeveraged + profit) * exitFee / 100;
                profit -= exitFeeCost;
            }

            diff += 0.01m;
        }
        while (
            (diff < tradeAmount)
            &&
            (isLong ? (liquidationPrice > liquidationPriceAt) : (liquidationPrice < liquidationPriceAt))
        );
/*
        Console.WriteLine($"Entry Price: {entryPrice}");
        Console.WriteLine($"Liquidation Price: {liquidationPriceAt:F2}");
        Console.WriteLine($"Leverage: {leverage}");
        Console.WriteLine($"Amount: {tradeAmount}");
        Console.WriteLine($"Take Profit At: {takeProfitAt}");
        // Console.WriteLine($"Trade Type: {tradeType}");
        Console.WriteLine($"Position: {positionCost:F2} + {additionalMargin:F2} (additional margin)");
        //Console.WriteLine($"Liquidation Price Calculated: {liquidationPrice:F2}");
        Console.WriteLine($"Half Loss Price: {halfLossAt:F2}");
        Console.WriteLine($"Volatility Allowed: {liquidationPercentage:F2}%");
        //Console.WriteLine($"Enter Fee: {enterFeeCost:F2}");
        //Console.WriteLine($"Exit Fee: {exitFeeCost:F2}");
        Console.WriteLine($"Profit at Take Profit: {profit:F2}");
*/
        var result = new FuturesPosition
        {
            PositionSize = positionCost,
            AdditionalMargin = additionalMargin,
            EstimatedProfit = profit,
            Volatility = liquidationPercentage,
            HalfLossPrice = halfLossAt,
            LossPrice = liquidationPrice,
            IsLong = isLong,
        };

        return result;
    }
}