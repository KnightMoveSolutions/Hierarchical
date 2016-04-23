using System;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// Classes that implement this interface provide all the functionality to give classes of type T the ability 
    /// to function as a node in a tree structure so that tree node functionality is generic. Entity classes that 
    /// are organized in a tree do not have to implement the tree structure. 
    /// </summary>
    /// <typeparam name="T">The type of the object that is being proxied into a Tree Node object (ITreeNode).</typeparam>
    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        /// <summary>
        /// For classes that implement this interface, this method finds and returns the ITreeNode&lt;T&gt; object 
        /// where the <see cref="Id"/> value is equal to the <paramref name="nodeId"/> value provided as
        /// an argument. It will search the tree recursively until it is found.
        /// </summary>
        /// <param name="nodeId">The <see cref="Id"/> of the node to search for.</param>
        /// <returns>The ITreeNode&lt;T&gt; that matches the search ID or null if it is not found</returns>
        ITreeNode<T> FindById(string nodeId);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the 
        /// <paramref name="treeNode"/> is an ancestor of this node up the tree.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is an ancestor of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        bool IsAncestor(T treeNode);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the 
        /// <paramref name="treeNode"/> is a descendant of this node down the tree.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is a descendant of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        bool IsDescendent(T treeNode);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the <paramref name="treeNode"/> 
        /// is a sibling of this node.
        /// </summary>
        /// <param name="treeNode">The ITreeNode&lt;T&gt; object that is being checked if it is a sibling of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is a sibling of this object, false if not</returns>
        bool IsSibling(T treeNode);

        /// <summary>
        /// For classes that implement this interface, this method makes this node a child of the sibling higher in the order of the 
        /// Children collection of its parent. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to a [RIGHT-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the second item to highlight it, then click the right-arrow, the item will 
        /// look like it moved to the right and became a child of the sibling just above it in the tree. 
        /// Executing this method provides that right-arrow functionality in the object model. 
        /// </remarks>
        void MakeChildOfUpperSibling();

        /// <summary>
        /// For classes that implement this interface, this method makes this node a sibling of its parent. 
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
        void MakeSiblingOfParent(bool takeLowerSiblingsAsChildren = false);

        /// <summary>
        /// For classes that implement this interface, this method moves this node down in the sibling order. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to a [DOWN-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the first item to highlight it, then click the down-arrow, the item will 
        /// look like it moved under the sibling that was previously below it. Executing this method provides
        ///  that down-arrow functionality in the object model. 
        /// </remarks>
        void MoveDownInSiblingOrder();

        /// <summary>
        /// For classes that implement this interface, this method moves this node up in the sibling order. 
        /// </summary>
        /// <remarks>
        /// This method is most useful when wiring up to an [UP-ARROW] button that is common on many user 
        /// interfaces that edit trees. For example, if you are viewing a tree and you see two items listed under 
        /// the same parent, you click the second item to highlight it, then click the up-arrow, the item will 
        /// look like it moved on top of the sibling that was previously above it. Executing this method provides
        ///  that up-arrow functionality in the object model. 
        /// </remarks>
        void MoveUpInSiblingOrder();

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. It does not include this node.
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
        bool ProcessChildren(ITreeNodeProcessor<T> nodeProcessor);

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. It does not include this node.
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
        bool ProcessChildren(Func<ITreeNode<T>, bool> nodeProcessor);

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. Unlike ProcessChildren, 
        /// this method will start with (include) this node.
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
        bool ProcessTree(ITreeNodeProcessor<T> nodeProcessor);

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. Unlike ProcessChildren, this method 
        /// will start with (include) this node.
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
        bool ProcessTree(Func<ITreeNode<T>, bool> nodeProcessor);

        // Properties

        /// <summary>
        /// The Child objects of this node in the hierarchy
        /// </summary>
        TreeList<T> Children { get; }

        /// <summary>
        /// The number of levels in the tree where the ROOT node is Level 1. 
        /// Whatever contains the Root (i.e. whole tree) is Level 0. 
        /// </summary>
        int DepthFromRoot { get; }

        /// <summary>
        /// True if this node has Children false if not.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// The ID of this node
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The character used for indentation. It will be repeated (by the <see cref="IndentString"/> property) once 
        /// for every Level (i.e. <see cref="DepthFromRoot"/> property) in the tree.
        /// </summary>
        char IndentCharacter { get; set; }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/>, <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        string IndentString { get; }

        /// <summary>
        /// The Parent node of this node
        /// </summary>
        ITreeNode<T> Parent { get; set; }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Parent"/> object of this node.
        /// </summary>
        /// <remarks>
        /// This value is required for all objects except the root object in order for the 
        /// <see cref="KnightMoves.Hierarchical.TreeNode&lt;T&gt;.CreateTree"/> method to successfully build the tree.
        /// </remarks>
        string ParentId { get; set; }

        /// <summary>
        /// A reference to the Root object in the tree. All objects in the tree will have 
        /// the same Root.
        /// </summary>
        ITreeNode<T> Root { get; set; }

        /// <summary>
        /// The other objects in the <see cref="Parent"/> objects <see cref="Children"/> 
        /// collection. It returns all objects in that collection except for this node.
        /// </summary>
        TreeList<T> Siblings { get; }

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
        string SortableTreePath { get; }

        /// <summary>
        /// A unique identifer for the node. Not required but can be useful.
        /// </summary>
        Guid TreeNodeId { get; set; }
    }
}
