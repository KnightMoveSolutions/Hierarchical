using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KnightMoves.Hierarchical.UnitTests
{

    public class TreeNodeTests
    {
        // This is our Mock entity. It has a Name property but can have all kind of other things.
        // It can have address, age, soc sec nbr, etc. Inheriting from TreeNode<T> makes it a 
        // tree node capable of having children, parents, etc. 
        private class Person : TreeNode<string, Person>
        {
            public override string Id { get; set; }
            public string Name { get; set; }
        }

        private readonly Person _grandpa;
        private readonly Person _dad;
        private readonly Person _uncle;
        private readonly Person _cousin;
        private readonly Person _sister;
        private readonly Person _me;
        private readonly List<Person> _familyMembers;
        private readonly Person _familyTree;

        public TreeNodeTests()
        {
            // ARRANGE FOR ALL
            _grandpa = new Person { Id = "Grandpa", Name = "Richard" };
            _dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
            _uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
            _cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
            _sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
            _me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };

            _familyMembers = new List<Person>
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

            _familyTree = TreeNode<string, Person>.CreateTree(_familyMembers);
        }

        [Fact]
        public void TestCreateTree()
        {
            // ASSERT
            Assert.NotNull(_familyTree);
        }

        [Fact]
        public void TestCreateNoRootException()
        {
            // ARRANGE
            // Root node is identified by a null or empty ParentId. This should throw an exception
            _grandpa.ParentId = "GreatGrandpa";

            // ASSERT
            Assert.Throws<ArgumentException>(() => TreeNode<string, Person>.CreateTree(_familyMembers));
        }

        [Fact]
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

        [Fact]
        public void TestAddChild()
        {
            // ARRANGE
            // My sister had a baby
            Person nephew = new Person { Id = "LittleJohnny", ParentId = "SisterJane", Name = "Johnny" };

            // ACTION 
            _sister.Children.Add(nephew);

            // ASSERT
            Assert.True(_sister.HasChildren);
            Assert.True(_sister.Children[0] == nephew);
        }

        [Fact]
        public void TestFindById()
        {
            // ACTION
            var searchNode = _familyTree.FindById("UncleJohn");

            // ASSERT
            Assert.True(searchNode == _uncle);
        }

        [Fact]
        public void TestIsAncestor()
        {
            // ACTION
            var isAncestor = _me.IsAncestor(_grandpa);
            var isNotAncestor = _me.IsAncestor(_sister);

            // ASSERT
            Assert.True(isAncestor);
            Assert.False(isNotAncestor);
        }

        [Fact]
        public void TestIsDescendant()
        {
            // ACTION
            var isDescendant = _grandpa.IsDescendent(_me);
            var isNotDescendant = _dad.IsDescendent(_cousin);

            // ASSERT
            Assert.True(isDescendant);
            Assert.False(isNotDescendant);
        }

        [Fact]
        public void TestIsSibling()
        {
            // ACTION
            var isSibling = _me.IsSibling(_sister);
            var isNotSibling = _me.IsSibling(_cousin);

            // ASSERT
            Assert.True(isSibling);
            Assert.False(isNotSibling);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        private class MockTreeNodeProcessor : ITreeNodeProcessor<string, Person>
        {
            private readonly IList<string> _testOutput;

            public MockTreeNodeProcessor()
            {
                _testOutput = new List<string>();
            }

            public bool ProcessNode(ITreeNode<string, Person> node)
            {
                _testOutput.Add(node.Id + " Processed");
                return true;
            }

            public bool IsProcessed(ITreeNode<string, Person> node)
            {
                return _testOutput.Any(n => n == node.Id + " Processed");
            }
        }

        [Fact]
        public void TestProcessChildrenWithDelegate()
        {
            // ARRANGE
            IList<string> testOutput = new List<string>();

            // ACTION
            _grandpa.ProcessChildren(delegate (ITreeNode<string, Person> t)
            {
                testOutput.Add(t.Id + " Processed");
                return true;
            });

            // ASSERT
            Assert.True(testOutput.FirstOrDefault(n => n == _dad.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _sister.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _me.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _uncle.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _cousin.Id + " Processed") != null);
        }

        [Fact]
        public void TestProcessTreeWithDelegate()
        {
            // ARRANGE
            IList<string> testOutput = new List<string>();

            // ACTION
            _grandpa.ProcessTree(delegate (ITreeNode<string, Person> t)
            {
                testOutput.Add(t.Id + " Processed");
                return true;
            });

            // ASSERT
            Assert.True(testOutput.FirstOrDefault(n => n == _grandpa.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _dad.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _sister.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _me.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _uncle.Id + " Processed") != null);
            Assert.True(testOutput.FirstOrDefault(n => n == _cousin.Id + " Processed") != null);
        }

        [Fact]
        public void TestDepthFromRoot()
        {
            // ASSERT
            Assert.True(_grandpa.DepthFromRoot == 1);
            Assert.True(_dad.DepthFromRoot == 2);
            Assert.True(_me.DepthFromRoot == 3);
        }

        [Fact]
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

        [Fact]
        public void TestId()
        {
            // Assert (<sigh> just for code coverage)
            Assert.True(_me.Id == "Me");
        }

        [Fact]
        public void TestIndentCharacter()
        {
            // Assert (<sigh> just for code coverage)
            Assert.True(_familyTree.IndentCharacter == ' ');
        }

        [Fact]
        public void TestIndentString()
        {
            // ASSERT
            Assert.True(_me.IndentString == "   ");
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void TestTreeNodeIdGet()
        {
            // ASSERT
            Assert.True(_grandpa.TreeNodeId == Guid.Empty);
        }

        [Fact]
        public void TestTreeNodeIdSet()
        {
            // ARRANGE
            Guid testGuid = Guid.NewGuid();
            _grandpa.TreeNodeId = testGuid;

            // ASSERT
            Assert.True(_grandpa.TreeNodeId == testGuid);
        }

        [Fact]
        public void TestGuidIdType()
        {
            // ARRANGE 
            var grandpaId = Guid.NewGuid();
            var dadId = Guid.NewGuid();
            var uncleId = Guid.NewGuid();
            var cousinId = Guid.NewGuid();
            var sisterId = Guid.NewGuid();
            var meId = Guid.NewGuid();

            var grandpa = new PersonGuid { Id = grandpaId, Name = "Richard" };
            var dad = new PersonGuid { Id = dadId, ParentId = grandpaId, Name = "Richard Jr." };
            var uncle = new PersonGuid { Id = uncleId, ParentId = grandpaId, Name = "John" };
            var cousin = new PersonGuid { Id = cousinId, ParentId = uncleId, Name = "Ann" };
            var sister = new PersonGuid { Id = sisterId, ParentId = dadId, Name = "Jane" };
            var me = new PersonGuid { Id = meId, ParentId = dadId, Name = "MeMyselfAndI" };

            var familyMembers = new List<PersonGuid>
            {
                sister,
                dad,
                uncle,
                grandpa,
                me,
                cousin
            };

            // ACTION
            var familyTree = TreeNode<Guid, PersonGuid>.CreateTree(familyMembers);

            // ASSERT
            Assert.NotNull(familyTree);

            // _grandpa is the root Person
            Assert.True(familyTree == grandpa);

            // _grandpa has two children (_dad and _uncle)
            Assert.True(familyTree.Children.Count == 2);
            Assert.True(familyTree.Children[0] == dad);
            Assert.True(familyTree.Children[0].Parent == grandpa);
            Assert.True(familyTree.Children[1] == uncle);
            Assert.True(familyTree.Children[1].Parent == grandpa);

            // _dad has two children (_me and _sister)
            Assert.True(familyTree.Children[0].Children.Count == 2);
            Assert.True(familyTree.Children[0].Children[0] == sister);
            Assert.True(familyTree.Children[0].Children[0].Parent == dad);
            Assert.True(familyTree.Children[0].Children[1] == me);
            Assert.True(familyTree.Children[0].Children[1].Parent == dad);

            // _uncle has one child (_cousin)
            Assert.True(familyTree.Children[1].Children.Count == 1);
            Assert.True(familyTree.Children[1].Children[0] == cousin);
            Assert.True(familyTree.Children[1].Children[0].Parent == uncle);

            Assert.True(grandpa.Root == null);
            Assert.True(dad.Root == grandpa);
            Assert.True(uncle.Root == grandpa);
            Assert.True(sister.Root == grandpa);
            Assert.True(me.Root == grandpa);
            Assert.True(cousin.Root == grandpa);

            Assert.True(grandpa.ParentId == Guid.Empty);
        }

        // Test class specifying Guid as the data type for the Id property
        class PersonGuid : TreeNode<Guid, PersonGuid>
        {
            public string Name { get; set; }
        }

        [Fact]
        public void TestPathId()
        {
            // ARRANGE 

            // Rebuilding the hierarchy with the same data should produce the same PathId values
            var grandpa = new Person { Id = "Grandpa", Name = "Richard" };
            var dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
            var uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
            var cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
            var sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
            var me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };

            var familyMembers = new List<Person>
            {
                sister,
                dad,
                uncle,
                grandpa,
                me,
                cousin
            };

            // ACTION
            TreeNode<string, Person>.CreateTree(familyMembers);

            // ASSERT 
            Assert.Equal(_grandpa.PathId, grandpa.PathId);
            Assert.Equal(_dad.PathId, dad.PathId);
            Assert.Equal(_uncle.PathId, uncle.PathId);
            Assert.Equal(_cousin.PathId, cousin.PathId);
            Assert.Equal(_sister.PathId, sister.PathId);
            Assert.Equal(_me.PathId, me.PathId);
        }

        [Fact]
        public void TestProcessAncestorsWithClassProcessor()
        {
            // ARRANGE
            var treeNodeProcessor = new TestTreeNodeProcessor();

            // ACTION
            var result = _familyTree.ProcessAncestors(treeNodeProcessor, _me);

            // ASSERT
            Assert.True(result);
            Assert.Equal(3, treeNodeProcessor.TestResults.Count());
            Assert.Equal(_me.Name, treeNodeProcessor.TestResults[0]);
            Assert.Equal(_dad.Name, treeNodeProcessor.TestResults[1]);
            Assert.Equal(_grandpa.Name, treeNodeProcessor.TestResults[2]);
        }

        [Fact]
        public void TestProcessAncestorsWithClassProcessorUsingMaxLevel()
        {
            // ARRANGE
            var treeNodeProcessor = new TestTreeNodeProcessor();

            // ACTION
            var result = _familyTree.ProcessAncestors(treeNodeProcessor, _me, 2);

            // ASSERT
            Assert.True(result);
            Assert.Equal(2, treeNodeProcessor.TestResults.Count());
            Assert.Equal(_me.Name, treeNodeProcessor.TestResults[0]);
            Assert.Equal(_dad.Name, treeNodeProcessor.TestResults[1]);
        }

        [Fact]
        public void TestProcessAncestorsWithFunctionProcessor()
        {
            // ARRANGE
            var testResults = new List<string>();

            // ACTION
            var result = _familyTree.ProcessAncestors((node) =>
            {
                var p = node as Person;
                testResults.Add(p.Name);
                return true;
            }, _me);

            // ASSERT
            Assert.True(result);
            Assert.Equal(3, testResults.Count());
            Assert.Equal(_me.Name, testResults[0]);
            Assert.Equal(_dad.Name, testResults[1]);
            Assert.Equal(_grandpa.Name, testResults[2]);
        }

        [Fact]
        public void TestProcessAncestorsWithFunctionProcessorUsingMaxLevel()
        {
            // ARRANGE
            var testResults = new List<string>();

            // ACTION
            var result = _familyTree.ProcessAncestors((node) =>
            {
                var p = node as Person;
                testResults.Add(p.Name);
                return true;
            }, _me, 2);

            // ASSERT
            Assert.True(result);
            Assert.Equal(2, testResults.Count());
            Assert.Equal(_me.Name, testResults[0]);
            Assert.Equal(_dad.Name, testResults[1]);
        }

        [Fact]
        public void TestProcessAncestorsWithFunctionProcessorUsingStopFunction()
        {
            // ARRANGE
            var testResults = new List<string>();

            // ACTION
            var result = _familyTree.ProcessAncestors((node) =>
            {
                var p = node as Person;
                testResults.Add(p.Name);
                return true;
            }, 
            _me,
            (node) =>
            {
                var p = node as Person;
                return p.Name == "Richard Jr.";
            });

            // ASSERT
            Assert.True(result);
            Assert.Equal(2, testResults.Count());
            Assert.Equal(_me.Name, testResults[0]);
            Assert.Equal(_dad.Name, testResults[1]);
        }

        [Fact]
        public void TestMarkAsSerializable()
        {
            // ARRANGE
            _familyTree.MarkAsSerializable();

            // ACTION
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_familyTree);

            _familyTree.UnMarkAsSerializable();

            // ASSERT
            Assert.False(string.IsNullOrEmpty(json));
        }

        [Fact]
        public void TestUnMarkAsSerializable()
        {
            // ARRANGE
            _familyTree.UnMarkAsSerializable();

            var threwException = false;

            // ACTION
            try
            {
                var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(_familyTree);
            }
            catch 
            {
                threwException = true;
            }

            // ASSERT
            Assert.True(threwException);
        }

        [Fact]
        public void TestDeserialization()
        {
            // ARRANGE
            _familyTree.MarkAsSerializable();

            // ACTION
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_familyTree);

            var familyTree = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(json);

            _familyTree.UnMarkAsSerializable();

            // ASSERT
            Assert.NotNull(familyTree);
            
            // Ensure all objects are of Type T (in this case Person)
            Assert.IsType<Person>(familyTree);
            Assert.IsType<Person>(familyTree.Children[0]);
            Assert.IsType<Person>(familyTree.Children[0].Children[0]);
            Assert.IsType<Person>(familyTree.Children[0].Children[1]);
            Assert.IsType<Person>(familyTree.Children[1]);
            Assert.IsType<Person>(familyTree.Children[1].Children[0]);

            // Ensure deserialization of structure matches pre-serialized object
            Assert.Equal(_familyTree.Id, familyTree.Id);
            Assert.Equal(_familyTree.Children[0].Id, familyTree.Children[0].Id);
            Assert.Equal(_familyTree.Children[0].Children[0].Id, familyTree.Children[0].Children[0].Id);
            Assert.Equal(_familyTree.Children[0].Children[1].Id, familyTree.Children[0].Children[1].Id);
            Assert.Equal(_familyTree.Children[1].Id, familyTree.Children[1].Id);
            Assert.Equal(_familyTree.Children[1].Children[0].Id, familyTree.Children[1].Children[0].Id);

            // Ensure Root references were restored
            Assert.Null(familyTree.Root);
            Assert.Equal(familyTree, familyTree.Children[0].Root);
            Assert.Equal(familyTree, familyTree.Children[0].Children[0].Root);
            Assert.Equal(familyTree, familyTree.Children[0].Children[1].Root);
            Assert.Equal(familyTree, familyTree.Children[1].Root);
            Assert.Equal(familyTree, familyTree.Children[1].Children[0].Root);

            // Ensure Parent references were restored
            Assert.Null(familyTree.Parent);
            Assert.Equal(familyTree, familyTree.Children[0].Parent);
            Assert.Equal(familyTree.Children[0], familyTree.Children[0].Children[0].Parent);
            Assert.Equal(familyTree.Children[0], familyTree.Children[0].Children[1].Parent);
            Assert.Equal(familyTree, familyTree.Children[1].Parent);
            Assert.Equal(familyTree.Children[1], familyTree.Children[1].Children[0].Parent);
        }

        private class TestTreeNodeProcessor : ITreeNodeProcessor<string, Person>
        {
            public IList<string> TestResults { get; set; } = new List<string>();

            public bool ProcessNode(ITreeNode<string, Person> node)
            {
                var p = node as Person;
                TestResults.Add(p.Name);
                return true;
            }
        }
    }
}
