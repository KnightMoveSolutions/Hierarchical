using System;
using System.Collections.Generic;
using System.Data.HashFunction;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// Classes that implement this interface provide all the functionality to give classes of type T the ability 
    /// to function as a node in a tree structure so that tree node functionality is generic. Entity classes that 
    /// are organized in a tree do not have to implement the tree structure. 
    /// </summary>
    /// <typeparam name="TId">The type of the <see cref="Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The type of the object that is being proxied into a Tree Node object (ITreeNode).</typeparam>
    public interface ITreeNode<TId, T> where T : ITreeNode<TId, T>
    {
        string TypeName { get; set; }

        /// <summary>
        /// Returns true if the tree contains a node that satisfied the condition 
        /// provided by the lambda function.
        /// </summary>
        /// <param name="condition">A boolean function that tests each node for the given condition</param>
        /// <returns>True if ANY node in the tree satisfies the condition</returns>
        bool Contains(Func<T, bool> condition);

        /// <summary>
        /// Makes a deep copy (clone) of the tree. If this method is called on a node 
        /// that is in the middle of an existing tree, then a copy of the branch will 
        /// be returned where the branch node becomes the root object. It works like an 
        /// unmount in a Linux file system except that the unmounted branch is returned.
        /// </summary>
        /// <returns>A deep copy clone of this node</returns>
        T DeepCopy();

        /// <summary>
        /// Returns a new tree where only the nodes that match the filter are returned 
        /// with their complete ancestor paths up to the root. Nodes that are not matched
        /// are not included. If a path of nodes does not have a matching node in it then
        /// the entire path of nodes will not be included.
        /// </summary>
        /// <param name="filter">A matching function that returns a boolean</param>
        /// <returns>A pruned tree with nodes that match the filter</returns>
        T Filter(Func<T, bool> filter);

        /// <summary>
        /// For classes that implement this interface, this method finds and returns the object of type 
        /// <typeparamref name="T"/> where the <see cref="Id"/> value is equal to the <paramref name="nodeId"/> 
        /// value provided as an argument. It will search the tree recursively until it is found.
        /// </summary>
        /// <param name="nodeId">The <see cref="Id"/> of the node to search for.</param>
        /// <returns>The node of type <typeparamref name="T"/> that matches the search ID or null if it is not found</returns>
        T FindById(TId nodeId);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the 
        /// <paramref name="treeNode"/> is an ancestor of this node up the tree.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is an ancestor of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        bool IsAncestor(T treeNode);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the 
        /// <paramref name="treeNode"/> is a descendant of this node down the tree.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is a descendant of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        bool IsDescendent(T treeNode);

        /// <summary>
        /// For classes that implement this interface, this method determines if the node provided as the <paramref name="treeNode"/> 
        /// is a sibling of this node.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is a sibling of this object.</param>
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
        /// that up-arrow functionality in the object model. 
        /// </remarks>
        void MoveUpInSiblingOrder();

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. It does not include this node.
        /// </summary>
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        void ProcessChildren(ITreeNodeProcessor<TId, T> nodeProcessor) ;

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. It does not include this node.
        /// </summary>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        void ProcessChildren(Action<T> nodeProcessor);

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. Unlike <see cref="ProcessChildren(ITreeNodeProcessor{TId, T})"/>, 
        /// this method will start with (include) this node.
        /// </summary>
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        void ProcessTree(ITreeNodeProcessor<TId, T> nodeProcessor);

        /// <summary>
        /// For classes that implement this interface, this method passes each child of this node to the 
        /// <paramref name="nodeProcessor"/> provided recursively down the tree. Unlike <see cref="ProcessChildren(Func{ITreeNode{TId, T}, bool})"/>, this method 
        /// will start with (include) this node.
        /// </summary>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        void ProcessTree(Action<T> nodeProcessor);

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> starting with the <paramref name="startNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by <paramref name="maxLevel"/>
        /// </summary>
        /// <param name="nodeProcessor">The <see cref="ITreeNodeProcessor{TId, T}"/> object used to process the nodes up the ancestor tree</param>
        /// <param name="startNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="maxLevel">(Optional) If defined will be used to check the <see cref="DepthFromRoot"/> property and will stop when it gets to that level</param>
        void ProcessAncestors(ITreeNodeProcessor<TId, T> nodeProcessor, T startNode, int maxLevel = 1);

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> function starting with the <paramref name="startNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by <paramref name="maxLevel"/>
        /// </summary>
        /// <param name="nodeProcessor">The function used to process the nodes up the ancestor tree</param>
        /// <param name="startNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="maxLevel"></param>
        void ProcessAncestors(Action<T> nodeProcessor, T startNode, int maxLevel = 1);

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> function starting with the <paramref name="startNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by the <paramref name="stopFunction"/>
        /// </summary>
        /// <param name="nodeProcessor">The function used to process the nodes up the ancestor tree</param>
        /// <param name="startNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="stopFunction">The function used to process the nodes up the ancestor tree to determine if it should stop recursing</param>
        void ProcessAncestors(Action<T> nodeProcessor, T startNode, Func<T, bool> stopFunction);

        /// <summary>
        /// Marks the <see cref="IsSerializable"/> flag as true for all nodes in the tree
        /// </summary>
        void MarkAsSerializable();

        /// <summary>
        /// Returns all nodes in the tree as a flattened collection of type <see cref="IList{T}"/> 
        /// sorted by the <see cref="SortableTreePath"/> property. If this method is called 
        /// on a branch then it will return only the nodes in that branch where the top of the  
        /// branch is the root.
        /// </summary>
        /// <returns><see cref="IList{T}"/></returns>
        List<T> ToList();

        /// <summary>
        /// Marks the <see cref="IsSerializable"/> flag as false for all nodes in the tree
        /// </summary>
        void UnMarkAsSerializable();

        // Properties

        /// <summary>
        /// The Child objects of this node in the hierarchy
        /// </summary>
        TreeList<TId, T> Children { get; }

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
        TId Id { get; set; }

        /// <summary>
        /// Hash Function provider used to produce the PathId
        /// </summary>
        IHashFunction HashProvider { get; set; }

        /// <summary>
        /// A unique identifier that represents the unique path from the root to this node
        /// </summary>
        string PathId { get; set; }

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
        ITreeNode<TId, T> Parent { get; set; }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Parent"/> object of this node.
        /// </summary>
        /// <remarks>
        /// This value is required for all objects except the root object in order for the 
        /// <see cref="TreeNode{TId, T}.CreateTree(System.Collections.Generic.List{ITreeNode{TId, T}})"/> method to successfully build the tree.
        /// </remarks>
        TId ParentId { get; set; }

        /// <summary>
        /// A reference to the Root object in the tree. All objects in the tree will have 
        /// the same Root.
        /// </summary>
        ITreeNode<TId, T> Root { get; set; }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Root"/> object of this node
        /// </summary>
        TId RootId { get; set; }

        /// <summary>
        /// The other objects in the <see cref="Parent"/> objects <see cref="Children"/> 
        /// collection. It returns all objects in that collection except for this node.
        /// </summary>
        TreeList<TId, T> Siblings { get; }

        /// <summary>
        /// A string representation of the location of this object in the tree in the form of a "Path".
        /// </summary>
        string SortableTreePath { get; }

        /// <summary>
        /// A unique identifer for the node. Not required but can be useful.
        /// </summary>
        Guid TreeNodeId { get; set; }

        /// <summary>
        /// When true it will cause the Root and Parent properties to return null in order
        /// to avoid circular reference errors during JSON serialization. When false, Root and 
        /// Parent references are respected. Set to true only when serializing. Ensure this 
        /// value is false if using the tree object model.
        /// </summary>
        bool IsSerializable { get; set; }
    }
}
