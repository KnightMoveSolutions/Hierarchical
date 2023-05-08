using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.HashFunction;
using System.Linq;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class inherits from <see cref="TreeNode{TId, T}"/> and implements code required to make the tree node 
    /// compatible with <see cref="Microsoft.EntityFrameworkCore"/>. 
    /// </summary>
    /// <typeparam name="TId">The type of the <see cref="Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The type of the object that is being proxied into a <see cref="TreeNode{TId, T}"/> object</typeparam>
    public class TreeNodeEF<TId, T> : TreeNode<TId, T> where T : ITreeNode<TId, T>
    {
        [ForeignKey("ParentId")]
        public override T Parent { get => base.Parent; set => base.Parent = value; }

        [ForeignKey("RootId")]
        public override T Root { get => base.Root; set => base.Root = value; }

        [NotMapped]
        public override IHashFunction HashProvider
        {
            get { return base.HashProvider; }
            set { base.HashProvider = value; }
        }

        [NotMapped]
        public override TreeList<TId, T> Children => base.Children;

        public IList<T> ChildrenList
        {
            get
            {
                IList<T> tList = new List<T>();
                Children.ToList().ForEach(t => tList.Add((T)t));
                return tList;
            }
            set { ; }
        }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/> <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        public new int DepthFromRoot
        {
            get { return base.DepthFromRoot; }
            set { ; }
        }

        /// <summary>
        /// True if this node has Children false if not.
        /// </summary>
        public new bool HasChildren
        {
            get { return base.HasChildren; }
            set { ; }
        }

        [NotMapped]
        public override char IndentCharacter
        {
            get { return base.IndentCharacter; }
            set { base.IndentCharacter = value; }
        }

        /// <summary>
        /// The string version of <see cref="IndentCharacter"/> that is compatible with Entity Framework
        /// </summary>
        [MaxLength(1)]
        public string IndentCharacterStr
        {
            get { return IndentCharacter.ToString(); }
            set { IndentCharacter = string.IsNullOrEmpty(value) ? ' ' : value.ToCharArray()[0]; }
        }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/> <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        public new string IndentString
        {
            get { return base.IndentString; }
            set { ; }
        }

        /// <summary>
        /// A string representation of the location of this object in the tree in the form of a "Path".
        /// </summary>
        public new string SortableTreePath
        {
            get { return base.SortableTreePath; }
            set { ; }
        }

        public override Guid TreeNodeId
        {
            get { return base.TreeNodeId; }
            set { base.TreeNodeId = value; }
        }

    }
}
