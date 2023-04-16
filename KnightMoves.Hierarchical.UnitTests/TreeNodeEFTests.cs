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

                var joe = new PersonEF { Id = 123, Name = "Joe" };
                var jr = new PersonEF { Id = 1234, Name = "Joe Jr" };

                joe.Children.Add(jr);

                context.Persons.Add(joe);
                context.Persons.Add(jr);
                context.SaveChanges();

                person = await context.Persons.FirstOrDefaultAsync();
            }

            // ASSERT 
            Assert.NotNull(person);
            Assert.Equal(123, person.Id);
            Assert.Equal("Joe", person.Name);
            Assert.NotEmpty(person.Children);
            Assert.Equal(1234, person.Children[0].Id);
            Assert.NotNull(person.Children[0].Parent);
            Assert.Equal(123, person.Children[0].ParentId);
            Assert.Equal(123, person.Children[0].Parent.Id);
            Assert.NotNull(person.Children[0].Root);
            Assert.Equal(123, person.Children[0].RootId);
            Assert.Equal(123, person.Children[0].Root.Id);
        }
    }
}
