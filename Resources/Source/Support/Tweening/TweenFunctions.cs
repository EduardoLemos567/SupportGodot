namespace Support.Tweening;

public static class TweenFunctions
{
    public static double Linear(double p)
    {
        p = double.Clamp(p, 0, 1);
        return p;
    }
    public static double EaseInOutQuint(double p)
    {
        p = double.Clamp(p, 0, 1);
        var a = p < 0.5 ? 16 * p * p * p * p * p : 1;
        var b = double.Pow(-2 * p + 2, 5);
        return a - b / 2;
    }
    public static double EaseInOutCubic(double p)
    {
        p = double.Clamp(p, 0, 1);
        var a = p < 0.5 ? 4 * p * p * p : 1;
        var b = double.Pow(-2 * p + 2, 3);
        return a - b / 2;
    }
    public static double EaseInOut(double p)
    {
        p = double.Clamp(p, 0, 1);
        var a = p < 0.5 ? 1 - double.Cos(p * double.Pi) : 1;
        var b = double.Cos((p - 1) * double.Pi);
        return a - b / 2;
    }
}
