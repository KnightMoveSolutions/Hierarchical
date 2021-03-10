using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KnightMoves.Hierarchical.UnitTests
{

    public class PersonEF : TreeNodeEF<int, PersonEF>
    {
        public string Name { get; set; }
    }

    public class TestDBContext : DbContext
    {
        public TestDBContext(DbContextOptions<TestDBContext> options) : base(options)
        {

        }

        public DbSet<PersonEF> Persons { get; set; }
    }

    public class TreeNodeEFTests
    {
        [Fact]
        public async Task TestEFFunctions()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<TestDBContext>()
                                .UseInMemoryDatabase("TestDB")
                                .Options;

            PersonEF person;

            // ACTION
            using (var context = new TestDBContext(options))
            {
                context.Database.EnsureCreated();

                context.Persons.Add(new PersonEF { Id = 123, Name = "Joe" });
                context.SaveChanges();

                person = await context.Persons.FirstOrDefaultAsync();
            }

            // ASSERT 
            Assert.NotNull(person);
            Assert.Equal(123, person.Id);
            Assert.Equal("Joe", person.Name);
        }
    }
}
