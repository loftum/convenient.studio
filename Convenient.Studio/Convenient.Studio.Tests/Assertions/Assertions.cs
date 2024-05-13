namespace Convenient.Studio.Tests.Assertions;

public static class Assertions
{
    public static AssertionDelegateWithMessage IsStringEqualTo(this string actual, string expected, StringComparison comparison = StringComparison.Ordinal)
    {
        return IsStringEqualToWithMessage;
        bool IsStringEqualToWithMessage(out string error)
        {
            error = default;
            if (actual == null)
            {
                if (expected != null)
                {
                    error = $"Expected '{expected}', but got null";
                    return false;
                }

                return true;
            }

            if (!string.Equals(actual, expected, comparison))
            {
                error = $"Expected '{expected}', but got '{actual}'";
                return false;
            }

            return true;
        }
    }
}