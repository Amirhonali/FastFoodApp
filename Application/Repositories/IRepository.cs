using System;
using System.Linq.Expressions;

namespace Application.Repositories
{
	public interface IRepository<T>
	{
		public IEnumerable<T> GetAsync(Expression<Func<T, bool>> predicate);

		public Task<T> GetByIdAsync(int Id);

		public Task<T> AddAsync(T sync);

		public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> sync);

		public Task<T> UpdateAsync(T sync);

		public Task<bool> DeleteAsync(int Id);
	}
}

