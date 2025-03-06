using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace PNE_tests
{
    public static class MockExtensions
    {
        // cette classe permet de mock tout les behaviors d'une table d'une BD
        // les fonctionnalites SQL fait sur ce moq auront leurs effets voulues.

        // basiquement ; IQueryable => table de bd

        public static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(items.GetEnumerator()));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(items.Provider));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            //mock FindAsync
            dbSetMock.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => FindEntityByIds(items,ids));

            return dbSetMock;
        }

        //Implementation mock de plusieurs fonctionalites de recherches/tables pour SQL
        //ne fait que servir entre eux et pour CreateDbSetMock. Pas d'utilisations externe 

        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

            public T Current => _inner.Current;
        }

        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }

        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider innerQueryProvider;

            internal TestAsyncQueryProvider(IQueryProvider innerQueryProvider)
            {
                this.innerQueryProvider = innerQueryProvider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return this.innerQueryProvider.Execute(expression)!;
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return this.innerQueryProvider.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = ((IQueryProvider)this).Execute(expression);

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult })!;
            }
        }

        private static T? FindEntityByIds<T>(IQueryable<T> items, object[] ids) where T : class
        {
            var type = typeof(T);
            var idProperty = type.GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                     p.Name.Equals("Id" + type.Name, StringComparison.OrdinalIgnoreCase) ||
                                     p.Name == "CodeCertification" ||
                                     p.Name == "IdMiseEau" ||
                                     p.Name == "Idnote" ||
                                     p.Name == "NomRole");

            foreach (var item in items)
            {
                var value = idProperty!.GetValue(item);
                if (value != null && ids.Length == 1 && value.Equals(ids[0]))
                {
                    return item;
                }
            }

            return null;
        }
    }
}
