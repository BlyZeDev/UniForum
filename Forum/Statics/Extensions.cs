namespace Forum.Statics;

public static class Extensions
{
    public static bool Equals(this Password left, Password right)
    {
        ReadOnlySpan<byte> leftSpan = left;
        ReadOnlySpan<byte> rightSpan = right;

        return leftSpan.SequenceEqual(rightSpan);
    }
}