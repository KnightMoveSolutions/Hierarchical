using Xunit;

namespace KnightMoves.Hierarchical.UnitTests
{

    public class TreeListTests
    {
        // This is our Mock entity. It has a Name property but can have all kind of other things.
        // It can have address, age, soc sec nbr, etc. Inheriting from TreeNode<T> makes it a 
        // tree node capable of having children, parents, etc. 
        private class Person : TreeNode<string, Person>
        {
            public string Name { get; set; }
        }

        private readonly Person _grandpa;
        private readonly Person _dad;
        private readonly Person _uncle;
        
        public TreeListTests()
        {
            // ARRANGE FOR ALL
            _grandpa = new Person { Id = "Grandpa", Name = "Richard" };
            _dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
            _uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
        }

        [Fact(DisplayName = "TestCreate")]
        public void TestCreate()
        {
            // ACTION
            var treeList = new TreeList<string, Person>(_grandpa);

            // ASSERT
            Assert.NotNull(treeList);
            Assert.True(treeList.Count == 0);
            Assert.True(treeList.Parent == _grandpa);
        }

        [Fact(DisplayName = "TestAddChildren")]
        public void TestAddChildren()
        {
            // ARRANGE
            var treeList = new TreeList<string, Person>(_grandpa);

            // ACTION
            treeList.Add(_dad);
            treeList.Add(_uncle);

            // ASSERT
            Assert.True(treeList.Count == 2);
            Assert.True(treeList[0].Id == "Dad");
            Assert.True(treeList[1].Id == "UncleJohn");
        }

        [Fact(DisplayName = "TestRemoveChildren")]
        public void TestRemoveChildren()
        {
            // ARRANGE
            var treeList = new TreeList<string, Person>(_grandpa);
            treeList.Add(_dad);
            treeList.Add(_uncle);

            // ACTION
            treeList.Remove(_dad);

            // ASSERT
            Assert.True(treeList.Count == 1);
            Assert.True(treeList[0].Id == "UncleJohn");
        }
    }
}
