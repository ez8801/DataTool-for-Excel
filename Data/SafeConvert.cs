using System.Linq;

public static class SafeConvert
{
    public static byte ToByte(string value)
    {
        if (byte.TryParse(value, out var result))
            return result;
        return default;
    }

    public static bool ToBoolean(string value)
    {
        if (bool.TryParse(value, out var result))
            return result;
        return default;
    }

    public static short ToInt16(string value)
    {
        if (short.TryParse(value, out var result))
            return result;
        return default;
    }

    public static int ToInt32(string value)
    {
        if (int.TryParse(value, out var result))
            return result;
        return default;
    }

    public static long ToInt64(string value)
    {
        if (long.TryParse(value, out var result))
            return result;
        return default;
    }

    public static float ToSingle(string value)
    {
        if (float.TryParse(value, out var result))
            return result;
        return default;
    }

    private static string Simplify(string value)
    {
        if (value.StartsWith("["))
            value = value.Remove(0, 1);
        if (value.EndsWith("]"))
            value = value.Remove(value.Length - 1);
        return value;
    }

    public static bool[] ToBoolArray(string value)
    {
        if (false == string.IsNullOrEmpty(value))
        {
            value = Simplify(value);

            bool[] result = value.Split(',')
                .Select(x => bool.TryParse(x, out var b) ? b : default(bool))
                .ToArray();
            return result;
        }
        return new bool[0];
    }

    public static int[] ToInt32Array(string value)
    {
        if (false == string.IsNullOrEmpty(value))
        {
            value = Simplify(value);

            int[] result = value.Split(',')
                .Select(x => int.TryParse(x, out var i) ? i : default(int))
                .ToArray();
            return result;
        }
        return new int[0];
    }

    public static float[] ToSingleArray(string value)
    {
        if (false == string.IsNullOrEmpty(value))
        {
            value = Simplify(value);

            float[] result = value.Split(',')
                .Select(x => float.TryParse(x, out var f) ? f : default(float))
                .ToArray();
            return result;
        }
        return new float[0];
    }

    public static string[] ToStringArray(string value)
    {
        if (false == string.IsNullOrEmpty(value))
        {
            value = Simplify(value);

            string[] result = value.Split(',');
            return result;
        }
        return new string[0];
    }
}
