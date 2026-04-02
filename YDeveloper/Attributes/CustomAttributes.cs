namespace YDeveloper.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Permission { get; }
        public RequirePermissionAttribute(string permission) => Permission = permission;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveDataAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AuditableAttribute : Attribute { }
}
