using Pinewolytics.Entities;
using Pinewolytics.Models.Entities;

namespace Pinewolytics.Models.DTOs.ICNS;

public class ICNSNameDTO : ICNSName, IFlipsideObject<ICNSNameDTO>
{
    public static ICNSNameDTO Parse(string[] rawValues)
    {
        return new ICNSNameDTO()
        {
            Name = rawValues[0],
            OSMOAddress = rawValues[1]
        };
    }
}
