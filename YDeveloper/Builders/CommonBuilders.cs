namespace YDeveloper.Builders
{
    public class EmailBuilder
    {
        private string _to = string.Empty;
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private List<string> _cc = new();

        public EmailBuilder To(string email) { _to = email; return this; }
        public EmailBuilder Subject(string subject) { _subject = subject; return this; }
        public EmailBuilder Body(string body) { _body = body; return this; }
        public EmailBuilder Cc(string email) { _cc.Add(email); return this; }

        public object Build() => new { To = _to, Subject = _subject, Body = _body, Cc = _cc };
    }

    public class QueryBuilder<T>
    {
        private IQueryable<T> _query;

        public QueryBuilder(IQueryable<T> query) => _query = query;

        public QueryBuilder<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public QueryBuilder<T> OrderBy<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            _query = _query.OrderBy(keySelector);
            return this;
        }

        public IQueryable<T> Build() => _query;
    }
}
