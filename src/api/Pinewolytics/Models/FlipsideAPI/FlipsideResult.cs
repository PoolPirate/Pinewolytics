using Pinewolytics.Models.FlipsideAPI.Results;
using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Models.FlipsideAPI;

public class FlipsideResult<T> where T : class, IFlipsideRequestResult
{
    public required string Jsonrpc { get; init; }

    public object? Error { get; init; } = null;
    public T? Result { get; init; } = null;
}
