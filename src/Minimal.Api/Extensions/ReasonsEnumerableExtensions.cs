namespace Minimal.Api.Extensions;

internal static class ReasonsEnumerableExtensions
{
    public static string Stringy<TReason>(this IEnumerable<TReason> reasons)
        where TReason : IReason =>
        string.Join(Environment.NewLine, reasons);

    public static bool Contains<TReason>(this IEnumerable<IReason> reasons)
        where TReason : IReason =>
        reasons.OfType<TReason>().Any();
}
