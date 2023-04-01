using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KnightMoves.Hierarchical.UnitTests
{
    public class TreeNodeWrapperTests
    {
        // This is our Mock entity. It has a Name property but can have all kind of other things.
        // It can have address, age, soc sec nbr, etc. It does not inherit from TreeNode<T> 
        // so we use the TreeNodeWrapper instead making it a tree node capable of having children, 
        // parents, etc. 
        internal class Person 
        {
            public string Name { get; set; }
        }

        private readonly TreeNodeWrapper<string, Person> _grandpa;
        private readonly TreeNodeWrapper<string, Person> _dad;
        private readonly TreeNodeWrapper<string, Person> _uncle;
        private readonly TreeNodeWrapper<string, Person> _cousin;
        private readonly TreeNodeWrapper<string, Person> _sister;
        private readonly TreeNodeWrapper<string, Person> _me;
        private readonly List<TreeNodeWrapper<string, Person>> _familyMembers;
        private readonly ITreeNode<string, TreeNodeWrapper<string, Person>> _familyTree;

        public TreeNodeWrapperTests()
        {
            // ARRANGE FOR ALL
            _grandpa = new TreeNodeWrapper<string, Person>(new Person { Name = "Richard" }, "Grandpa", null);
            _dad = new TreeNodeWrapper<string, Person>(new Person { Name = "Richard Jr." }, "Dad", "Grandpa");
            _uncle = new TreeNodeWrapper<string, Person>(new Person { Name = "John" }, "UncleJohn", "Grandpa");
            _cousin = new TreeNodeWrapper<string, Person>(new Person { Name = "Ann" }, "CousinAnn", "UncleJohn");
            _sister = new TreeNodeWrapper<string, Person>(new Person { Name = "Jane" }, "SisterJane", "Dad");
            _me = new TreeNodeWrapper<string, Person>(new Person { Name = "MeMyselfAndI" }, "Me", "Dad");

            _familyMembers = new List<TreeNodeWrapper<string, Person>>
            {
                // Let's add them in random order to test that they'll 
                // be added in the correct order and hierarchy
                _sister,
                _dad,
                _uncle,
                _grandpa,
                _me,
                _cousin
            };

            _familyTree = TreeNode<string, TreeNodeWrapper<string, Person>>.CreateTree(_familyMembers);
        }

        [Fact(DisplayName = "TestCreateTree")]
        public void TestCreateTree()
        {
            // ASSERT
            Assert.NotNull(_familyTree);
        }

        [Fact(DisplayName = "TestCreateNoRootException")]
        public void TestCreateNoRootException()
        {
            // ARRANGE
            // Root node is identified by a null or empty ParentId. This should throw an exception
            _grandpa.ParentId = "GreatGrandpa";

            // ASSERT
            Assert.Throws<ArgumentException>(() => TreeNode<string, TreeNodeWrapper<string, Person>>.CreateTree(_familyMembers));
        }

        [Fact(DisplayName = "TestHierarchy")]
        public void TestHierarchy()
        {
            // ASSERT

            // _grandpa is the root Person
            Assert.True(_familyTree == _grandpa);

            // _grandpa has two children (_dad and _uncle)
            Assert.True(_familyTree.Children.Count == 2);
            Assert.True(_familyTree.Children[0] == _dad);
            Assert.True(_familyTree.Children[0].Parent == _grandpa);
            Assert.True(_familyTree.Children[1] == _uncle);
            Assert.True(_familyTree.Children[1].Parent == _grandpa);

            // _dad has two children (_me and _sister)
            Assert.True(_familyTree.Children[0].Children.Count == 2);
            Assert.True(_familyTree.Children[0].Children[0] == _sister);
            Assert.True(_familyTree.Children[0].Children[0].Parent == _dad);
            Assert.True(_familyTree.Children[0].Children[1] == _me);
            Assert.True(_familyTree.Children[0].Children[1].Parent == _dad);

            // _uncle has one child (_cousin)
            Assert.True(_familyTree.Children[1].Children.Count == 1);
            Assert.True(_familyTree.Children[1].Children[0] == _cousin);
            Assert.True(_familyTree.Children[1].Children[0].Parent == _uncle);
        }

        [Fact(DisplayName = "TestAddChild")]
        public void TestAddChild()
        {
            // ARRANGE
            // My sister had a baby
            TreeNodeWrapper<string, Person> nephew = new TreeNodeWrapper<string, Person>(new Person { Name = "Johnny" }, "LittleJohnny", "SisterJane");

            // ACTION 
            _sister.Children.Add(nephew);

            // ASSERT
            Assert.True(_sister.HasChildren);
            Assert.True(_sister.Children[0] == nephew);
        }

        [Fact(DisplayName = "TestFindById")]
        public void TestFindById()
        {
            // ACTION
            var searchNode = _familyTree.FindById("UncleJohn");

            // ASSERT
            Assert.True(searchNode == _uncle);
        }

        [Fact(DisplayName = "TestIsAncestor")]
        public void TestIsAncestor()
        {
            // ACTION
            var isAncestor = _me.IsAncestor(_grandpa);
            var isNotAncestor = _me.IsAncestor(_sister);

            // ASSERT
            Assert.True(isAncestor);
            Assert.False(isNotAncestor);
        }

        [Fact(DisplayName = "TestIsDescendant")]
        public void TestIsDescendant()
        {
            // ACTION
            var isDescendant = _grandpa.IsDescendent(_me);
            var isNotDescendant = _dad.IsDescendent(_cousin);

            // ASSERT
            Assert.True(isDescendant);
            Assert.False(isNotDescendant);
        }

        [Fact(DisplayName = "TestIsSibling")]
        public void TestIsSibling()
        {
            // ACTION
            var isSibling = _me.IsSibling(_sister);
            var isNotSibling = _me.IsSibling(_cousin);

            // ASSERT
            Assert.True(isSibling);
            Assert.False(isNotSibling);
        }

        [Fact(DisplayName = "TestMakeChildOfUpperSibling")]
        public void TestMakeChildOfUpperSibling()
        {
            // ACTION
            _me.MakeChildOfUpperSibling(); // Right-arrow functionality
            var isChildOfSisterNow = _sister.IsDescendent(_me);
            var isParentOfMeNow = _me.Parent == _sister;

            // ASSERT
            Assert.True(isChildOfSisterNow);
            Assert.True(isParentOfMeNow);
        }

        [Fact(DisplayName = "TestMakeSiblingOfParent")]
        public void TestMakeSiblingOfParent()
        {
            // ACTION
            _me.MakeSiblingOfParent(); // Left-arrow functionality
            var amDadsBroNow = _me.IsSibling(_dad);
            var dadHasOneChildNow = _dad.Children.Count == 1;

            // ASSERT
            Assert.True(amDadsBroNow);
            Assert.True(dadHasOneChildNow);
        }

        [Fact(DisplayName = "TestMakeSiblingOfParentWhereCurrentSiblingsBecomeChildren")]
        public void TestMakeSiblingOfParentWhereCurrentSiblingsBecomeChildren()
        {
            // ACTION
            _sister.MakeSiblingOfParent(true); // Left-arrow functionality
            var sisIsDadsSisNow = _sister.IsSibling(_dad);
            var dadHasNoChildrenNow = _dad.Children.Count == 0;
            var sisHasMeAsChildNow = _me.Parent == _sister;

            // ASSERT
            Assert.True(sisIsDadsSisNow);
            Assert.True(dadHasNoChildrenNow);
            Assert.True(sisHasMeAsChildNow);
        }

        [Fact(DisplayName = "TestMoveUpInSiblingOrder")]
        public void TestMoveUpInSiblingOrder()
        {
            // ACTION
            _me.MoveUpInSiblingOrder(); // Up-arrow functionality
            var amBigBroNow = _dad.Children[0] == _me;
            var sisIsLittleSisNow = _dad.Children[1] == _sister;

            // ASSERT
            Assert.True(amBigBroNow);
            Assert.True(sisIsLittleSisNow);
        }

        [Fact(DisplayName = "TestMoveDownInSiblingOrder")]
        public void TestMoveDownInSiblingOrder()
        {
            // ACTION
            _sister.MoveDownInSiblingOrder(); // Down-arrow functionality
            var sisIsLittleSisNow = _dad.Children[1] == _sister;
            var amBigBroNow = _dad.Children[0] == _me;

            // ASSERT
            Assert.True(sisIsLittleSisNow);
            Assert.True(amBigBroNow);
        }

        [Fact(DisplayName = "TestProcessChildrenWithClass")]
        public void TestProcessChildrenWithClass()
        {
            // ACTION
            var mockProcessor = new MockTreeNodeProcessor();
            _grandpa.ProcessChildren(mockProcessor);

            // ASSERT
            Assert.True(mockProcessor.IsProcessed(_dad));
            Assert.True(mockProcessor.IsProcessed(_sister));
            Assert.True(mockProcessor.IsProcessed(_me));
            Assert.True(mockProcessor.IsProcessed(_uncle));
            Assert.True(mockProcessor.IsProcessed(_cousin));
        }

        [Fact(DisplayName = "TestProcessTreeWithClass")]
        public void TestProcessTreeWithClass()
        {
            // ACTION
            var mockProcessor = new MockTreeNodeProcessor();
            _grandpa.ProcessTree(mockProcessor);

            // ASSERT
            Assert.True(mockProcessor.IsProcessed(_grandpa));
            Assert.True(mockProcessor.IsProcessed(_dad));
            Assert.True(mockProcessor.IsProcessed(_sister));
            Assert.True(mockProcessor.IsProcessed(_me));
            Assert.True(mockProcessor.IsProcessed(_uncle));
            Assert.True(mockProcessor.IsProcessed(_cousin));
        }

        private class MockTreeNodeProcessor : ITreeNodeProcessor<string, TreeNodeWrapper<string, Person>>
        {
            private readonly IList<string> _testOutput;

            public MockTreeNodeProcessor()
            {
                _testOutput = new List<string>();
            }

            public void ProcessNode(TreeNodeWrapper<string, Person> node)
            {
                _testOutput.Add(node.Id + " Processed");
            }

            public bool IsProcessed(ITreeNode<string, TreeNodeWrapper<string, Person>> node)
            {
                var processed = _testOutput.FirstOrDefault(n => n == node.Id + " Processed");
                return processed != null;
            }
        }

        [Fact(DisplayName = "TestProcessChildrenWithDelegate")]
        public void TestProcessChildrenWithDelegate()
        {
            // ARRANGE
            IList<string> testOutput = new List<string>();

            // ACTION
            _grandpa.ProcessChildren(delegate (TreeNodeWrapper<string, Person> t)
            {
                testOutput.Add(t.Id + " Processed");
            });

            // ASSERT
            Assert.True(testOutput.FirstOrDefault(n => n == _dad.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _sister.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _me.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _uncle.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _cousin.Id + " Processed") != null);
        }

        [Fact(DisplayName = "TestProcessTreeWithDelegate")]
        public void TestProcessTreeWithDelegate()
        {
            // ARRANGE
            IList<string> testOutput = new List<string>();

            // ACTION
            _grandpa.ProcessTree(delegate (TreeNodeWrapper<string, Person> t)
            {
                testOutput.Add(t.Id + " Processed");
            });

            // ASSERT
            Assert.True(testOutput.FirstOrDefault(n => n == _grandpa.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _dad.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _sister.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _me.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _uncle.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _cousin.Id + " Processed") != null);
        }

        [Fact(DisplayName = "TestDepthFromRoot")]
        public void TestDepthFromRoot()
        {
            // ASSERT
            Assert.True(_grandpa.DepthFromRoot == 1);
            Assert.True(_dad.DepthFromRoot == 2);
            Assert.True(_me.DepthFromRoot == 3);
        }

        [Fact(DisplayName = "TestHasChildren")]
        public void TestHasChildren()
        {
            // ASSERT
            Assert.True(_grandpa.HasChildren);
            Assert.True(_dad.HasChildren);
            Assert.True(_uncle.HasChildren);
            Assert.False(_sister.HasChildren);
            Assert.False(_me.HasChildren);
            Assert.False(_cousin.HasChildren);
        }

        [Fact(DisplayName = "TestId")]
        public void TestId()
        {
            // Assert (<sigh> just for code coverage)
            Assert.True(_me.Id == "Me");
        }

        [Fact(DisplayName = "TestIndentCharacter")]
        public void TestIndentCharacter()
        {
            // Assert (<sigh> just for code coverage)
            Assert.True(_familyTree.IndentCharacter == ' ');
        }

        [Fact(DisplayName = "TestIndentString")]
        public void TestIndentString()
        {
            // ASSERT
            Assert.True(_me.IndentString == "   ");
        }

        [Fact(DisplayName = "TestParent")]
        public void TestParent()
        {
            // ASSERT
            Assert.True(_grandpa.Parent == null);
            Assert.True(_dad.Parent == _grandpa);
            Assert.True(_uncle.Parent == _grandpa);
            Assert.True(_sister.Parent == _dad);
            Assert.True(_me.Parent == _dad);
            Assert.True(_cousin.Parent == _uncle);
        }

        [Fact(DisplayName = "TestParentId")]
        public void TestParentId()
        {
            // ASSERT
            Assert.True(_grandpa.ParentId == null);
            Assert.True(_dad.ParentId == "Grandpa");
            Assert.True(_uncle.ParentId == "Grandpa");
            Assert.True(_sister.ParentId == "Dad");
            Assert.True(_me.ParentId == "Dad");
            Assert.True(_cousin.ParentId == "UncleJohn");
        }

        [Fact(DisplayName = "TestRoot")]
        public void TestRoot()
        {
            // ASSERT
            Assert.True(_grandpa.Root == null);
            Assert.True(_dad.Root == _grandpa);
            Assert.True(_uncle.Root == _grandpa);
            Assert.True(_sister.Root == _grandpa);
            Assert.True(_me.Root == _grandpa);
            Assert.True(_cousin.Root == _grandpa);
        }

        [Fact(DisplayName = "TestSortableTreePath")]
        public void TestSortableTreePath()
        {
            // ASSERT
            Assert.True(_grandpa.SortableTreePath == "1");
            Assert.True(_dad.SortableTreePath == "1.1");
            Assert.True(_uncle.SortableTreePath == "1.2");
            Assert.True(_sister.SortableTreePath == "1.1.1");
            Assert.True(_me.SortableTreePath == "1.1.2");
            Assert.True(_cousin.SortableTreePath == "1.2.1");
        }

        [Fact(DisplayName = "TestTreeNodeIdGet")]
        public void TestTreeNodeIdGet()
        {
            // ASSERT
            Assert.True(_grandpa.TreeNodeId == Guid.Empty);
        }

        [Fact(DisplayName = "TestTreeNodeIdSet")]
        public void TestTreeNodeIdSet()
        {
            // ARRANGE
            Guid testGuid = Guid.NewGuid();
            _grandpa.TreeNodeId = testGuid;

            // ASSERT
            Assert.True(_grandpa.TreeNodeId == testGuid);
        }

    }
}
