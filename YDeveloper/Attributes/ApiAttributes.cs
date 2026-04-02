namespace YDeveloper.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoRateLimitAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminOnlyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousApiAttribute : Attribute { }
}
