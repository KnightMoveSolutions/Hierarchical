using System;
using System.Collections.Generic;
using System.Linq;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class implements all of the ITreeNode&lt;&gt; interface members but is declared abstract so other 
    /// entity objects are required to inherit from it. Inheriting from this class gives the new class 
    /// all the functionality of a Tree Node. Any classes that already inherit from another class can use 
    /// the TreeNodeWrapper&lt;&gt; as wrapper that inherits this abstract class on behalf of the entity 
    /// object it is wrapping. In that way it works around the multiple inheritance problem and allows the 
    /// entity to participate in a tree as an ITreeNode&lt;&gt; object. 
    /// </summary>
    /// <remarks>
    /// <code>
    /// namespace MyApp
    /// {
    ///     // Just inherit from TreeNode&lt;T&gt;
    ///     public class Person : TreeNode&lt;Person&gt;
    ///     {
    ///         // Person is now able to function as a tree node
    ///         // ID and ParentID are inherited properties
    ///         
    ///         public string Name { get; set; }
    ///     }
    ///     
    ///     public class MyFamilyTreeApp
    ///     {
    ///         public static void Main(string[] args)
    ///         {
    ///             Person grandpa = new Person { Id = "Grandpa", Name = "Richard" };
    ///             Person dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
    ///             Person uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
    ///             Person cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
    ///             Person sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
    ///             Person me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };
    ///             
    ///             List&lt;Person&gt; familyMembers = new List&lt;Person&gt;();
    ///             
    ///             // Let's add them in random order just to prove a point
    ///             familyMembers.Add(sister);
    ///             familyMembers.Add(dad);
    ///             familyMembers.Add(uncle);
    ///             familyMembers.Add(grandpa);
    ///             familyMembers.Add(me);
    ///             familyMembers.Add(cousin);
    ///             
    ///             // Not linked in tree structure
    ///             Console.WriteLine(grandpa.Children.Count);      // prints 0
    ///             Console.WriteLine(dad.Parent == null);          // prints true
    ///             Console.WriteLine(me.Parent == null);           // prints true
    /// 
    ///             // Creates tree structure automatically by using ID and ParentID of objects.
    ///             // Even though the family members were added in random order to the collection 
    ///             // the tree is still created properly.
    ///             ITreeNode&lt;ITreeNode&lt;Person&gt;&gt; familyTree = TreeNode&lt;ITreeNode&lt;Person&gt;&gt;.CreateTree(familyMembers);
    ///             
    ///             Console.WriteLine(familyTree == grandpa);                           // prints true
    ///             Console.WriteLine(familyTree.Children.Count);                       // prints 2
    ///             Console.WriteLine(familyTree.Children[0] == dad);                   // prints true
    ///             Console.WriteLine(familyTree.Children[1] == uncle);                 // prints true
    ///             Console.WriteLine(familyTree.Children[0].Children.Count);           // prints 2
    ///             Console.WriteLine(familyTree.Children[0].Children[0] == sister);    // prints true
    ///             Console.WriteLine(familyTree.Children[0].Children[1] == me);        // prints true
    ///             Console.WriteLine(familyTree.Children[1].Children.Count);           // prints 1
    ///             Console.WriteLine(familyTree.Children[1].Children[0] == cousin);    // prints true
    ///             
    ///             // Linked in tree now
    ///             Console.WriteLine(grandpa.Children.Count);      // prints 2
    ///             Console.WriteLine(dad.Parent == grandpa);       // prints true
    ///             Console.WriteLine(me.Parent == dad);            // prints true
    ///         }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <typeparam name="T">The type of the object that is being proxied into a Tree Node object (TreeNode).</typeparam>
    public abstract class TreeNode<T> : ITreeNode<T> where T : ITreeNode<T>
    {
        private string _id;

        /// <summary>
        /// Constructor setting some reasonable defaults
        /// </summary>
        protected TreeNode()
        {
            _id = string.Empty;
            TreeNodeId = Guid.Empty;
            Children = new TreeList<T>(this);
            IndentCharacter = ' ';
        }

        private static void AddChildren(List<ITreeNode<T>> treeNodeCollection, ITreeNode<T> node)
        {
            treeNodeCollection
                .FindAll(n => n.ParentId == node.Id)
                .ForEach(n =>
                {
                    node.Children.Add(n);
                    AddChildren(treeNodeCollection, n);
                });
        }

        /// <summary>
        /// Accepts a regular collection of ITreeNode&lt;T&gt; objects in the form of a generic List&lt;T&gt; and builds a 
        /// hierarchical object model.
        /// </summary>
        /// <remarks>
        /// Accepts a regular collection of ITreeNode&lt;T&gt; objects in the form of a List&lt;T&gt; collection and builds a 
        /// hierarchical object model. The collection of objects are assumed to have a single root object, which the method
        /// identifies as the object with a null <see cref="ParentId"/>. If it does not find the root node by this criteria
        /// it will throw an ArgumentException. All other objects will be put in their respective place in the hierarchy as 
        /// long as their <see cref="ParentId"/> values are specified. 
        /// </remarks>
        /// <param name="treeNodeCollection">A collection of objects that are part of the same hierarchy (i.e. share the same root)</param>
        /// <returns>
        /// An ITreeNode&lt;T&gt; object representing the root node from the <paramref name="treeNodeCollection"/> argument. 
        /// All other objects will be added as Children of their respective Parent objects in the object graph.
        /// </returns>
        public static ITreeNode<T> CreateTree(List<ITreeNode<T>> treeNodeCollection)
        {
            var rootNode = treeNodeCollection.Find(n => n.ParentId == null);

            if (rootNode == null)
            {
                throw new ArgumentException("Could not find the root node in the treeNodeCollection. The treeNodeCollection must contain a *single* root node identified by having a null ParentId.");
            }

            AddChildren(treeNodeCollection, rootNode);

            return rootNode;
        }

        /// <summary>
        /// Finds and returns the ITreeNode&lt;T&gt; object where the <see cref="Id"/> value is equal to the <paramref name="nodeId"/> 
        /// value provided as an argument. It will search the tree recursively until it is found.
        /// </summary>
        /// <param name="nodeId">The <see cref="Id"/> of the node to search for.</param>
        /// <returns>The ITreeNode&lt;T&gt; that matches the search ID or null if it is not found</returns>
        public ITreeNode<T> FindById(string nodeId)
        {
            ITreeNode<T> targetNode = null;

            ProcessTree(delegate(ITreeNode<T> t)
            {
                if (t.Id == nodeId)
                {
                    targetNode = t;
                }
                return true;
            });

            return targetNode;
        }

        /// <summary>
        /// Determines if the node provided as the <paramref name="treeNode"/> is an ancestor of this node up the tree.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is an ancestor of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        public bool IsAncestor(T treeNode)
        {
            return SortableTreePath.StartsWith(treeNode.SortableTreePath);
        }

        /// <summary>
        /// Determines if the node provided as the <paramref name="treeNode"/> is an descendant of this node down the tree.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is a descendant of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        public bool IsDescendent(T treeNode)
        {
            return treeNode.SortableTreePath.StartsWith(SortableTreePath);
        }

        /// <summary>
        /// Determines if the node provided as the <paramref name="treeNode"/> is a sibling of this node.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is a sibling of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is a sibling of this object, false if not</returns>
        public bool IsSibling(T treeNode)
        {
            return Siblings.Contains(treeNode);
        }

        /// <summary>
        /// Makes this node a child of the sibling higher in the order of the Children collection of its parent. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to a [RIGHT-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the second item to highlight it, then click the right-arrow, the item will 
        /// look like it moved to the right and became a child of the sibling just above it in the tree. 
        /// Executing this method provides that right-arrow functionality in the object model. 
        /// </remarks>
        public void MakeChildOfUpperSibling()
        {
            int index = Parent.Children.IndexOf(this);

            if (index > 0)
            {
                ITreeNode<T> node = Parent.Children[index - 1];
                Parent.Children.Remove(this);
                node.Children.Add(this);
            }
        }

        /// <summary>
        /// Makes this node a sibling of its parent. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is most useful when wiring up to a [LEFT-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see an item listed under 
        /// a parent, you click the item to highlight it, then click the left-arrow, the item will look like 
        /// is moved to the left and become a sibling what used to be its parent in the tree. Executing this 
        /// method provides that left-arrow functionality in the object model. 
        /// </para>
        /// <para>
        /// When executing this functionality, 
        /// however, there are two possibilities for other siblings of the parent. If the parent has siblings 
        /// then the default behavior is the item being moved to the left gets inserted as a sibling up a level 
        /// by itself. The items that were siblings below it remain children of the parent. If the 
        /// <paramref name="takeLowerSiblingsAsChildren"/> parameter is true, then the items that were siblings 
        /// under the item being moved will become children of the item being moved. 
        /// </para>
        /// <para>
        /// For example, given the following hierarchy:
        /// </para>
        /// <pre>
        ///   ItemA
        ///     ItemA1
        ///     ItemA2
        ///     ItemA3
        ///   ItemB
        ///   ItemC
        /// </pre>
        /// <code language="csharp">ItemA1.MakeSiblingOfParent(false)</code>
        /// <para>
        /// becomes:
        /// </para>
        /// <pre>
        ///   ItemA
        ///     ItemA2
        ///     ItemA3
        ///   ItemA1
        ///   ItemB
        ///   ItemC
        /// </pre>
        /// <para>
        /// else if you do this:
        /// </para>
        /// <code language="csharp">ItemA1.MakeSiblingOfParent(true)</code>
        /// <para>
        /// then it would do this:
        /// </para>
        /// <pre>
        ///   ItemA
        ///   ItemA1
        ///     ItemA2
        ///     ItemA3
        ///   ItemB
        ///   ItemC
        /// </pre>
        /// </remarks>
        /// <param name="takeLowerSiblingsAsChildren">If true, makes the lower siblings children of this object, otherwise it moves this object by itself.</param>
        public void MakeSiblingOfParent(bool takeLowerSiblingsAsChildren = false)
        {
            if (takeLowerSiblingsAsChildren)
            {
                for (int i = Parent.Children.IndexOf(this) + 1; i < Parent.Children.Count; i++)
                {
                    ITreeNode<T> item = Parent.Children[i];
                    Parent.Children.Remove(item);
                    Children.Add(item);
                }
            }

            Parent.Children.Remove(this);
            Parent.Parent.Children.Add(this);
        }

        /// <summary>
        /// Moves this node down in the sibling order. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to a [DOWN-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the first item to highlight it, then click the down-arrow, the item will 
        /// look like it moved under the sibling that was previously below it. Executing this method provides
        ///  that down-arrow functionality in the object model. 
        /// </remarks>
        public void MoveDownInSiblingOrder()
        {
            int index = Parent.Children.IndexOf(this);
            if ((index + 1) < Parent.Children.Count)
            {
                Parent.Children.Remove(this);
                Parent.Children.Insert(index + 1, this);
            }
        }

        /// <summary>
        /// Moves this node up in the sibling order. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to an [UP-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the second item to highlight it, then click the up-arrow, the item will 
        /// look like it moved on top of the sibling that was previously above it. Executing this method provides
        ///  that up-arrow functionality in the object model. 
        /// </remarks>
        public void MoveUpInSiblingOrder()
        {
            int index = Parent.Children.IndexOf(this);
            if (index > 0)
            {
                Parent.Children.Remove(this);
                Parent.Children.Insert(index - 1, this);
            }
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. It does not include this node.
        /// </summary>
        /// <remarks>
        /// This method performs recursion for you. Provide a node processor of type ITreeNodeProcessor&lt;T&gt; 
        /// and this method will recursively travel down the children of this node passing each child node it 
        /// encounters down the tree into the ProcessNode method of the processor. The processor can then do its 
        /// thing against each node. This method will NOT include this node. If you need to include this node as 
        /// well then use <see cref="ProcessTree(KnightMoves.Hierarchical.ITreeNodeProcessor{T})"/> instead.
        /// </remarks>
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        /// <returns>True if every execution of the ProcessNode method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessChildren(ITreeNodeProcessor<T> nodeProcessor)
        {
            bool flag = true;

            foreach (ITreeNode<T> local in Children)
            {
                flag = (flag & nodeProcessor.ProcessNode(local)) & local.ProcessChildren(nodeProcessor);
            }

            return flag;
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. It does not include this node.
        /// </summary>
        /// <remarks>
        /// This method performs recursion for you. Provide a delegate as a node processor of type that accepts 
        /// an ITreeNode&lt;T&gt; object (which is for the nodes of the tree) and returns a bool as true if the 
        /// execution of the delegate is successful or false if not. This method will recursively travel down the 
        /// children of this node passing each child node it encounters down the tree into the delegate method. 
        /// The delegate method can then do its thing against each node. This method will NOT include this node. 
        /// If you need to include this node as well then use 
        /// <see cref="ProcessTree(Func&lt;ITreeNode&lt;T&gt;, bool&gt;)"/> instead.
        /// </remarks>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        /// <returns>True if every execution of the delegate method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessChildren(Func<ITreeNode<T>, bool> nodeProcessor)
        {
            bool flag = true;

            foreach (ITreeNode<T> local in Children)
            {
                flag = (flag & nodeProcessor(local)) & local.ProcessChildren(nodeProcessor);
            }

            return flag;
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. Unlike ProcessChildren, this method will start with (include) this node.
        /// </summary>
        /// <remarks>
        /// This method performs recursion for you. Provide a node processor of type ITreeNodeProcessor&lt;T&gt; 
        /// and this method will recursively travel down the children of this node passing each child node it 
        /// encounters down the tree into the ProcessNode method of the processor. The processor can then do its 
        /// thing against each node. Unlike ProcessChildren, this method will start with (include) this node. If 
        /// you need to exclude this node then use 
        /// <see cref="ProcessChildren(KnightMoves.Hierarchical.ITreeNodeProcessor{T})"/> instead. 
        /// </remarks>
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        /// <returns>True if every execution of the ProcessNode method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessTree(ITreeNodeProcessor<T> nodeProcessor)
        {
            return (nodeProcessor.ProcessNode(this) & ProcessChildren(nodeProcessor));
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. Unlike ProcessChildren, this method will start with (include) this node.
        /// </summary>
        /// <remarks>
        /// This method performs recursion for you. Provide a delegate as a node processor of type that accepts 
        /// an ITreeNode&lt;T&gt; object (which is for the nodes of the tree) and returns a bool as true if the 
        /// execution of the delegate is successful or false if not. This method will recursively travel down the 
        /// children of this node passing each child node it encounters down the tree into the delegate method. 
        /// The delegate method can then do its thing against each node. Unlike ProcessChildren, this method will 
        /// start with (include) this node. If you need to exclude this node then use 
        /// <see cref="ProcessChildren(Func&lt;ITreeNode&lt;T&gt;, bool&gt;)"/> instead.
        /// </remarks>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        /// <returns>True if every execution of the delegate method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessTree(Func<ITreeNode<T>, bool> nodeProcessor)
        {
            return (nodeProcessor(this) & ProcessChildren(nodeProcessor));
        }

        // Properties

        /// <summary>
        /// The Child objects of this node in the hierarchy
        /// </summary>
        public TreeList<T> Children { get; }

        /// <summary>
        /// The number of levels in the tree where the ROOT node is Level 1. 
        /// Whatever contains the Root (i.e. whole tree) is Level 0. 
        /// </summary>
        public int DepthFromRoot
        {
            get
            {
                if (Parent != null)
                {
                    return (Parent.DepthFromRoot + 1);
                }

                return 1;
            }
        }

        /// <summary>
        /// True if this node has Children false if not.
        /// </summary>
        public bool HasChildren => (Children.Count > 0);

        /// <summary>
        /// The ID of this node
        /// </summary>
        public virtual string Id
        {
            get
            {
                if (_id == string.Empty)
                {
                    _id = TreeNodeId.ToString();
                }

                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The character used for indentation. It will be repeated once for every Level 
        /// in the tree, which is reported by the <see cref="DepthFromRoot"/> property by
        /// the <see cref="IndentString"/> property.
        /// </summary>
        public char IndentCharacter { get; set; }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/> <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        public string IndentString => new string(IndentCharacter, DepthFromRoot);

        /// <summary>
        /// The Parent node of this node
        /// </summary>
        public ITreeNode<T> Parent { get; set; }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Parent"/> object of this node.
        /// </summary>
        /// <remarks>
        /// This value is required for all objects except the root object in order for the 
        /// <see cref="CreateTree"/> method to successfully build the tree.
        /// </remarks>
        public string ParentId { get; set; }

        /// <summary>
        /// A reference to the Root object in the tree. All objects in the tree will have 
        /// the same Root.
        /// </summary>
        public ITreeNode<T> Root { get; set; }

        /// <summary>
        /// The other objects in the <see cref="Parent"/> objects <see cref="Children"/> 
        /// collection. It returns all objects in that collection except for this node.
        /// </summary>
        public TreeList<T> Siblings
        {
            get
            {
                if (Parent == null)
                    return null;
                
                var list = new TreeList<T>(Parent);

                Parent
                    .Children
                    .Where(n => n != this)
                    .ToList()
                    .ForEach(n => list.Add(n));

                return list;
            }
        }

        /// <summary>
        /// A string representation of the location of this object in the tree in the form of a "Path".
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property implements the Materialized Path Pattern. A well-known example of Materialized Path 
        /// is the common file system folder path. C:\Users\Bob\Documents is an example of a Materialized Path. 
        /// </para>
        /// <para>
        /// The SortableTreePath produces a path using integers starting with 1. See the example below.
        /// </para>
        /// <pre>
        /// Tree                     SortableTreePath
        /// ----------------------   ---------------------------
        /// RootNode                 1
        ///   ItemA                  1.1
        ///     ItemA1               1.1.1
        ///     ItemA2               1.1.2
        ///   ItemB                  1.2
        ///     ItemB1               1.2.1
        ///       ItemBA             1.2.1.1
        ///   ItemC                  1.3
        ///   ItemD                  1.4
        ///     ItemD1               1.4.1
        ///     ItemD2               1.4.2
        ///   ...
        ///   etc.
        /// </pre>
        /// <para>
        /// You can see that if you sort by the SortableTreePath it will result in a list where every object
        /// is in order by its position in the tree.
        /// </para>
        /// <para>
        /// This value is incredibly useful when persisting to the database. It allows for the use of very simple 
        /// SQL queries to get nodes or segments of the tree. 
        /// </para>
        /// <para>
        /// Using the dataset above you can do the following:
        /// </para>
        /// <para>
        /// Get all nodes in the tree in hierarchical order:
        /// </para>
        /// <code language="sql">SELECT IndentString + Name FROM MyTreeTable ORDER BY SortableTreePath</code>
        /// <para>Result:</para>
        /// <pre>
        /// Name
        /// -------------
        /// RootNode  
        ///   ItemA 
        ///     ItemA1 
        ///     ItemA2
        ///   ItemB
        ///     ItemB1
        ///       ItemBA
        ///   ItemC 
        ///   ItemD
        ///     ItemD1
        ///     ItemD2   
        /// </pre>
        /// <para>
        /// Get the ItemB tree
        /// </para>
        /// <code language="sql">SELECT IndentString + Name FROM MyTreeTable WHERE SortableTreePath LIKE '1.2%' ORDER BY SortableTreePath</code> 
        /// <para>Result:</para>
        /// <pre>
        /// Name
        /// -------------
        ///   ItemB
        ///     ItemB1
        ///       ItemBA
        /// </pre>
        /// <para>
        /// Get children of ItemD:
        /// </para>
        /// <code language="sql">SELECT Name FROM MyTreeTable WHERE SortableTreePath LIKE '1.4.%' ORDER BY SortableTreePath</code>
        /// <para>Result:</para>
        /// <pre>
        /// Name
        /// -------------
        /// ItemD1
        /// ItemD2   
        /// </pre>
        /// <para>
        /// Get Ancestors of ItemBA:
        /// </para>
        /// <code language="sql">
        /// SELECT @itemBAPath = SortableTreePath FROM MyTreeTable WHERE Name = 'ItemBA' -- Noramlly use ID but you get the idea
        /// SELECT Name FROM MyTreeTable WHERE @itemBAPath LIKE SortableTreePath + '%' ORDER BY SortableTreePath 
        /// </code>
        /// <para>Result:</para>
        /// <pre>
        /// Name
        /// -------------
        /// RootNode  
        ///   ItemB
        ///     ItemB1
        ///       ItemBA
        /// </pre>
        /// </remarks>
        public string SortableTreePath
        {
            get
            {
                string str = Parent == null ? string.Empty : Parent.SortableTreePath + ".";
                
                string str2 = (Parent?.Children.IndexOf(this) + 1)?.ToString() ?? "1";

                if (Parent == null || Root == null)
                    return str2;

                int num2 = Parent.Children.Count + 1;

                if (str2.Length < num2.ToString().Length)
                {
                    int num3 = Parent.Children.Count + 1;
                    str2 = new string('0', num3.ToString().Length - str2.Length) + str2;
                }

                return (str + str2);
            }
        }

        /// <summary>
        /// A unique identifer for the node. Not required but can be useful.
        /// </summary>
        public Guid TreeNodeId { get; set; }
    }
}
