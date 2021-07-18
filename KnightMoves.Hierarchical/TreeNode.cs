using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.CRC;
using System.Linq;
using System.Runtime.Serialization;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class implements all of the <see cref="ITreeNode{TId, T}"/> interface members but is declared abstract so other 
    /// entity objects are required to inherit from it. Inheriting from this class gives the new class 
    /// all the functionality of a Tree Node. Any classes that already inherit from another class can use 
    /// the <see cref="TreeNodeWrapper{TId, T}"/> as wrapper that inherits this abstract class on behalf of the entity 
    /// object it is wrapping. In that way it works around the multiple inheritance problem and allows the 
    /// entity to participate in a tree as an <see cref="ITreeNode{TId, T}"/> object. 
    /// </summary>
    /// <typeparam name="TId">The type of the <see cref="Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The type of the object that is being proxied into a <see cref="TreeNode{TId, T}"/> object</typeparam>
    public class TreeNode<TId, T> : ITreeNode<TId, T> where T : ITreeNode<TId, T> 
    {
        private const int MIN_DEPTH_VALUE = 1;

        /// <summary>
        /// Constructor setting some reasonable defaults
        /// </summary>
        protected TreeNode()
        {
            HashProvider = CRCFactory.Instance.Create();
            TreeNodeId = Guid.Empty;
            Children = new TreeList<TId, T>(this);
            IndentCharacter = ' ';
        }

        private static void AddChildren(List<T> treeNodeCollection, ITreeNode<TId, T> node)
        {
            treeNodeCollection
                .FindAll(n => n.ParentId?.ToString() == node.Id.ToString())
                .ForEach(n =>
                {
                    node.Children.Add(n);
                    AddChildren(treeNodeCollection, n);
                });
        }

        /// <summary>
        /// Accepts a regular collection of <see cref="ITreeNode{TId, T}"/> objects in the form of a generic <see cref="List{T}"/> and builds a 
        /// hierarchical object model.
        /// </summary>
        /// <param name="treeNodeCollection">A collection of objects that are part of the same hierarchy (i.e. share the same root)</param>
        /// <returns>
        /// An <see cref="T"/> object representing the root node from the <paramref name="treeNodeCollection"/> argument. 
        /// All other objects will be added as <see cref="Children"/> of their respective <see cref="Parent"/> objects in the object graph.
        /// </returns>
        public static T CreateTree(List<T> treeNodeCollection) 
        {
            var rootNode = treeNodeCollection.Find(n => {
                return
                    string.IsNullOrEmpty(n.ParentId?.ToString()) ||
                    (
                        typeof(TId) == typeof(Guid) &&
                        Guid.Parse(n.ParentId.ToString()) == Guid.Empty
                    );
            });

            if (rootNode == null)
            {
                throw new ArgumentException($"Could not find the root node in the {nameof(treeNodeCollection)}. The {nameof(treeNodeCollection)} must contain a *single* root node identified by having a null {nameof(ParentId)}.");
            }

            AddChildren(treeNodeCollection, rootNode);

            return (T) rootNode;
        }

        /// <summary>
        /// Finds and returns the <see cref="ITreeNode{TId, T}"/> object where the <see cref="Id"/> value is equal to the <paramref name="nodeId"/> 
        /// value provided as an argument. It will search the tree recursively until it is found.
        /// </summary>
        /// <param name="nodeId">The <see cref="Id"/> of the node to search for.</param>
        /// <returns>The <see cref="ITreeNode{TId, T}"/> that matches the search ID or null if it is not found</returns>
        public ITreeNode<TId, T> FindById(TId nodeId)
        {
            ITreeNode<TId, T> targetNode = null;

            ProcessTree(delegate(ITreeNode<TId, T> t)
            {
                if (t.Id.ToString() == nodeId.ToString())
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
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is an ancestor of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        public bool IsAncestor(T treeNode)
        {
            return SortableTreePath.StartsWith(treeNode.SortableTreePath);
        }

        /// <summary>
        /// Determines if the node provided as the <paramref name="treeNode"/> is an descendant of this node down the tree.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is a descendant of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is an ancestor of this object, false if not.</returns>
        public bool IsDescendent(T treeNode)
        {
            return treeNode.SortableTreePath.StartsWith(SortableTreePath);
        }

        /// <summary>
        /// Determines if the node provided as the <paramref name="treeNode"/> is a sibling of this node.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object that is being checked if it is a sibling of this object.</param>
        /// <returns>True if <paramref name="treeNode"/> is a sibling of this object, false if not</returns>
        public bool IsSibling(T treeNode)
        {
            return Siblings.Contains(treeNode);
        }

        /// <summary>
        /// Makes this node a child of the sibling higher in the order of the <see cref="Children"/> collection of its parent. 
        /// </summary>
        public void MakeChildOfUpperSibling()
        {
            int index = Parent.Children.IndexOf(this);

            if (index > 0)
            {
                ITreeNode<TId, T> node = Parent.Children[index - 1];
                Parent.Children.Remove(this);
                node.Children.Add(this);
            }
        }

        /// <summary>
        /// Makes this node a sibling of its parent. 
        /// </summary>
        /// <param name="takeLowerSiblingsAsChildren">If true, makes the lower siblings children of this object, otherwise it moves this object by itself.</param>
        public void MakeSiblingOfParent(bool takeLowerSiblingsAsChildren = false)
        {
            if (takeLowerSiblingsAsChildren)
            {
                for (int i = Parent.Children.IndexOf(this) + 1; i < Parent.Children.Count(); i++)
                {
                    ITreeNode<TId, T> item = Parent.Children[i];
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
        public void MoveDownInSiblingOrder()
        {
            int index = Parent.Children.IndexOf(this);
            if ((index + 1) < Parent.Children.Count())
            {
                Parent.Children.Remove(this);
                Parent.Children.Insert(index + 1, this);
            }
        }

        /// <summary>
        /// Moves this node up in the sibling order. 
        /// </summary>
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
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        /// <returns>True if every execution of the <see cref="ITreeNodeProcessor{TId, T}.ProcessNode(ITreeNode{TId, T})"/> method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessChildren(ITreeNodeProcessor<TId, T> nodeProcessor)
        {
            bool flag = true;

            foreach (ITreeNode<TId, T> local in Children)
            {
                flag = (flag & nodeProcessor.ProcessNode(local)) & local.ProcessChildren(nodeProcessor);
            }

            return flag;
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. It does not include this node.
        /// </summary>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        /// <returns>True if every execution of the delegate method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessChildren(Func<ITreeNode<TId, T>, bool> nodeProcessor)
        {
            bool flag = true;

            foreach (T local in Children)
            {
                flag = (flag & nodeProcessor(local)) & local.ProcessChildren(nodeProcessor);
            }

            return flag;
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. Unlike <see cref="ProcessChildren(ITreeNodeProcessor{TId, T})"/>, this method will start with (include) this node.
        /// </summary>
        /// <param name="nodeProcessor">The object that will process each node down the tree.</param>
        /// <returns>True if every execution of the <see cref="ITreeNodeProcessor{TId, T}.ProcessNode(ITreeNode{TId, T})"/> method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessTree(ITreeNodeProcessor<TId, T> nodeProcessor)
        {
            return nodeProcessor.ProcessNode(this) & ProcessChildren(nodeProcessor);
        }

        /// <summary>
        /// Passes each child of this node to the <paramref name="nodeProcessor"/> provided recursively down the
        /// tree. Unlike <see cref="ProcessChildren(Func{ITreeNode{TId, T}, bool})"/>, this method will start with (include) this node.
        /// </summary>
        /// <param name="nodeProcessor">The delegate method that will process each node down the tree.</param>
        /// <returns>True if every execution of the delegate method returns true, false if at least one of the executions returns false.</returns>
        public bool ProcessTree(Func<ITreeNode<TId, T>, bool> nodeProcessor)
        {
            return nodeProcessor(this) & ProcessChildren(nodeProcessor);
        }

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> starting with the <paramref name="treeNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by <paramref name="maxLevel"/>
        /// </summary>
        /// <param name="nodeProcessor">The <see cref="ITreeNodeProcessor{TId, T}"/> object used to process the nodes up the ancestor tree</param>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="maxLevel">(Optional) If defined will be used to check the <see cref="DepthFromRoot"/> property and will stop when it gets to that level</param>
        /// <returns></returns>
        public bool ProcessAncestors(ITreeNodeProcessor<TId, T> nodeProcessor, ITreeNode<TId, T> treeNode, int maxLevel = MIN_DEPTH_VALUE)
        {
            bool flag = true;

            flag = flag & 
                   nodeProcessor.ProcessNode(treeNode) &                        // Process this node
                   treeNode.Parent == null ||                                   // There is no parent so evaluate to true
                   treeNode.DepthFromRoot == maxLevel ||                        // Instructed to stop here so evalutate to true
                   ProcessAncestors(nodeProcessor, treeNode.Parent, maxLevel);  // Go up the tree and return the result
 
            return flag;
        }

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> function starting with the <paramref name="treeNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by <paramref name="maxLevel"/>
        /// </summary>
        /// <param name="nodeProcessor">The function used to process the nodes up the ancestor tree</param>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public bool ProcessAncestors(Func<ITreeNode<TId, T>, bool> nodeProcessor, ITreeNode<TId, T> treeNode, int maxLevel = MIN_DEPTH_VALUE)
        {
            bool flag = true;

            flag = flag &
                   nodeProcessor(treeNode) &                                    // Process this node
                   treeNode.Parent == null ||                                   // There is no parent so evaluate to true
                   treeNode.DepthFromRoot == maxLevel ||                        // Instructed to stop here so evalutate to true
                   ProcessAncestors(nodeProcessor, treeNode.Parent, maxLevel);  // Go up the tree and return the result

            return flag;
        }

        /// <summary>
        /// Executes the <paramref name="nodeProcessor"/> function starting with the <paramref name="treeNode"/> object 
        /// and then recursively up the ancestor tree until the root or the node defined by the <paramref name="stopFunction"/>
        /// </summary>
        /// <param name="nodeProcessor">The function used to process the nodes up the ancestor tree</param>
        /// <param name="treeNode">The <see cref="ITreeNode{TId, T}"/> object used as the starting point for processing up the ancestor tree</param>
        /// <param name="stopFunction">The function used to process the nodes up the ancestor tree to determine if it should stop recursing</param>
        /// <returns></returns>
        public bool ProcessAncestors(Func<ITreeNode<TId, T>, bool> nodeProcessor, ITreeNode<TId, T> treeNode, Func<ITreeNode<TId, T>, bool> stopFunction)
        {
            bool flag = true;

            flag = flag &
                   nodeProcessor(treeNode) &                                        // Process this node
                   treeNode.Parent == null ||                                       // There is no parent so evaluate to true
                   stopFunction(treeNode) ||                                        // Instructed to stop here if it returns true
                   ProcessAncestors(nodeProcessor, treeNode.Parent, stopFunction);  // Go up the tree and return the result

            return flag;
        }

        /// <summary>
        /// When true it will cause the Root and Parent properties to return null in order
        /// to avoid circular reference errors during JSON serialization. When false, Root and 
        /// Parent references are respected. Set to true only when serializing. Ensure this 
        /// value is false if using the tree object model.
        /// </summary>
        public bool IsSerializable { get; set; }

        /// <summary>
        /// Marks the <see cref="IsSerializable"/> flag as true for all nodes in the tree
        /// </summary>
        public void MarkAsSerializable()
        {
            var root = Root == null ? this : Root;
            root.ProcessTree(node => node.IsSerializable = true);
        }

        /// <summary>
        /// Marks the <see cref="IsSerializable"/> flag as false for all nodes in the tree
        /// </summary>
        public void UnMarkAsSerializable()
        {
            var root = Root == null ? this : Root;
            root.ProcessTree(node => node.IsSerializable = false);
        }

        // Properties

        /// <summary>
        /// The Child objects of this node in the hierarchy
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(InterfaceToConcreteGenericJsonConverter), ItemConverterParameters = new object[] { typeof(TreeNode<,>) })]
        public virtual TreeList<TId, T> Children { get; private set; }

        /// <summary>
        /// The number of levels in the tree where the ROOT node is Level 1. 
        /// Whatever contains the Root (i.e. whole tree) is Level 0. 
        /// </summary>
        public int DepthFromRoot
        {
            get
            {
                return _parent == null ? MIN_DEPTH_VALUE : (_parent.DepthFromRoot + 1);
            }
        }

        /// <summary>
        /// True if this node has Children false if not.
        /// </summary>
        public bool HasChildren => (Children.Count > 0);

        /// <summary>
        /// The ID of this node
        /// </summary>
        public virtual TId Id { get; set; }

        /// <summary>
        /// Hash Function provider used to produce the <see cref="PathId"/>. The default Hash Function is 
        /// an instance of <see cref="System.Data.HashFunction.CRC.ICRC"/>
        /// </summary>
        public virtual IHashFunction HashProvider { get; set; }

        /// <summary>
        /// A unique identifier that represents the unique path from the root to this node
        /// </summary>
        public string PathId
        {
            get
            {
                if (HashProvider == null)
                    return null;

                var rawString = _parent == null ? Id.ToString() : _parent.PathId + Id.ToString();

                byte[] hashBytes = HashProvider.ComputeHash(rawString).Hash;

                return Convert.ToBase64String(hashBytes);
            }
            set
            {
                var val = value; // do nothing for EF to work
            }
        }

        /// <summary>
        /// The character used for indentation. It will be repeated once for every Level 
        /// in the tree, which is reported by the <see cref="DepthFromRoot"/> property and 
        /// used to produce the <see cref="IndentString"/> property.
        /// </summary>
        public virtual char IndentCharacter { get; set; }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/> <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        public string IndentString => new string(IndentCharacter, DepthFromRoot);

        private ITreeNode<TId, T> _parent;
        /// <summary>
        /// The Parent node of this node
        /// </summary>
        public virtual ITreeNode<TId, T> Parent 
        {
            get { return IsSerializable ? null : _parent; }
            set { _parent = value; } 
        }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Parent"/> object of this node.
        /// </summary>
        /// <remarks>
        /// This value is required for all objects except the root object in order for the 
        /// <see cref="CreateTree"/> method to successfully build the tree.
        /// </remarks>
        public virtual TId ParentId { get; set; }

        private ITreeNode<TId, T> _root;
        /// <summary>
        /// A reference to the Root object in the tree. All objects in the tree will have 
        /// the same Root.
        /// </summary>
        public virtual ITreeNode<TId, T> Root 
        { 
            get { return IsSerializable ? null : _root; }
            set { _root = value; }
        }

        /// <summary>
        /// The <see cref="Id"/> of the <see cref="Root"/> object of this node
        /// </summary>
        public virtual TId RootId { get; set; }

        /// <summary>
        /// The other objects in the <see cref="Parent"/> objects <see cref="Children"/> 
        /// collection. It returns all objects in that collection except for this node.
        /// </summary>
        public TreeList<TId, T> Siblings
        {
            get
            {
                if (IsSerializable)
                    return null;

                if (Parent == null)
                    return null;
                
                var list = new TreeList<TId, T>(Parent);

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
        public string SortableTreePath
        {
            get
            {
                string str = _parent == null ? string.Empty : _parent.SortableTreePath + ".";
                
                string str2 = (_parent?.Children.IndexOf(this) + 1)?.ToString() ?? "1";

                if (_parent == null || _root == null)
                    return str2;

                int num2 = _parent.Children.Count() + 1;

                if (str2.Length < num2.ToString().Length)
                {
                    int num3 = _parent.Children.Count() + 1;
                    str2 = new string('0', num3.ToString().Length - str2.Length) + str2;
                }

                return (str + str2);
            }
        }

        /// <summary>
        /// A unique identifer for the node that can be used in addition to or as an 
        /// alternative to the <see cref="Id"/> when defining the <see cref="TId"/> as something other than <see cref="Guid"/>. 
        /// This is Not required but can be useful.
        /// </summary>
        public virtual Guid TreeNodeId { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // We do this to restore the Root and Parent 
            // references that were lost upon serialization
            if (ParentId == null)
            {
                IsSerializable = false;
                var root = this;
                var nodeIndex = new List<ITreeNode<TId, T>> { this };
                
                ProcessChildren(n =>
                {
                    n.IsSerializable = false;
                    n.Root = root;
                    n.Parent = nodeIndex.Single(p => p.Id.Equals(n.ParentId));
                    nodeIndex.Add(n);
                    return true;
                });
            }
        }
    }
}
