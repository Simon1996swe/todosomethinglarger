using Microsoft.EntityFrameworkCore;
using todosomethinglarger.Models;

namespace todosomethinglarger.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
