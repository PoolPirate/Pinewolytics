namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismGasPriceDTO
{
    public double L1GasPrice { get;  }
    public double L2GasPrice { get; }

    public OptimismGasPriceDTO(double l1GasPrice, double l2GasPrice)
    {
        L1GasPrice = l1GasPrice;
        L2GasPrice = l2GasPrice;
    }
}
