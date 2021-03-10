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
    /// <remarks>
    /// It marks the <see cref="Id"/> property as the Primary Key column, the <see cref="ParentId"/> as a self-referencing 
    /// foreign key column and marks a number of columns with the <see cref="NotMappedAttribute"/>. 
    /// </remarks>
    /// <typeparam name="TId">The type of the <see cref="Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The type of the object that is being proxied into a <see cref="TreeNode{TId, T}"/> object</typeparam>
    public abstract class TreeNodeEF<TId, T> : TreeNode<TId, T> where T : ITreeNode<TId, T>
    {
        [NotMapped]
        public override IHashFunction HashProvider
        {
            get { return base.HashProvider; }
            set { base.HashProvider = value; }
        }

        [ForeignKey("ParentEntity")]
        public override TId ParentId
        {
            get { return base.ParentId; }
            set { base.ParentId = value; }
        }

        [NotMapped]
        public override ITreeNode<TId, T> Parent
        {
            get { return base.Parent; }
            set { base.Parent = value; }
        }

        public T ParentEntity
        {
            get { return (T)base.Parent; }
            set { base.Parent = value; }
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
            set
            {
                IList<T> val = value;
            }
        }

        [NotMapped]
        public override ITreeNode<TId, T> Root
        {
            get { return base.Root; }
            set 
            { 
                base.Root = value; // Fake it so EF persists read-only value to DB
            }
        }

        public override TId RootId
        {
            get { return base.RootId; }
            set { base.RootId = value; }
        }

        /// <summary>
        /// Repeats the <see cref="IndentCharacter"/> <see cref="DepthFromRoot"/> number of 
        /// times to produce the indentation string for pretty display in text output.
        /// </summary>
        public new int DepthFromRoot
        {
            get { return base.DepthFromRoot; }
            set
            {
                int val = value; // Fake it so EF persists read-only value to DB
            }
        }

        /// <summary>
        /// True if this node has Children false if not.
        /// </summary>
        public new bool HasChildren
        {
            get { return base.HasChildren; }
            set
            {
                bool val = value; // Fake it so EF persists read-only value to DB
            }
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
            set
            {
                string val = value; // Fake it so EF persists read-only value to DB
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
        public new string SortableTreePath
        {
            get { return base.SortableTreePath; }
            set
            {
                string val = value; // Fake it so EF persists read-only value to DB
            }
        }

        public override Guid TreeNodeId
        {
            get { return base.TreeNodeId; }
            set { base.TreeNodeId = value; }
        }

    }
}
