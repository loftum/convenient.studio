using Xunit.Sdk;

namespace Convenient.Studio.Tests.Assertions;

public delegate bool AssertionDelegateWithMessage(out string errorMessage);

public static class Verify
{
    public static void That(AssertionDelegateWithMessage del)
    {
        if (!del(out var errorMessage))
        {
            throw FailException.ForFailure(errorMessage);
        }
    }
}