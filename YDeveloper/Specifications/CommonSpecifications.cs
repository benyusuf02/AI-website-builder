namespace YDeveloper.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
    }

    public class ActiveSiteSpecification : ISpecification<Models.Site>
    {
        public bool IsSatisfiedBy(Models.Site site) => site.IsActive;
    }

    public class PublishedPageSpecification : ISpecification<Models.Page>
    {
        public bool IsSatisfiedBy(Models.Page page) => page.IsPublished;
    }
}
