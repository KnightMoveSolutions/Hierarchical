using System.Collections.ObjectModel;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class inherits from <see cref="Collection{T}"/> and overrides the <see cref="Collection{T}.Add(T)"/> method. This custom 
    /// collection was necessary so that the <see cref="Add(ITreeNode{TId, T})"/> method can take care of the details of setting up 
    /// the child node in the tree linking it properly to its <see cref="ITreeNode{TId, T}.Parent"/> and <see cref="ITreeNode{TId, T}.Root"/> node, whereas a regular 
    /// collection is not aware of any hierarchical structure. It is used as the data type for the 
    /// <see cref="ITreeNode{TId, T}.Children"/> property and the <see cref="ITreeNode{TId, T}.Siblings"/> property. 
    /// </summary>
    /// <remarks>
    /// The <see cref="TreeNode{TId, T}"/> creates the <see cref="TreeList{TId, T}"/> objects internally for the <see cref="ITreeNode{TId, T}.Children"/> property and the 
    /// <see cref="ITreeNode{TId, T}.Siblings"/> property. No need to create this collection yourself unless you are creating your own 
    /// implementation of <see cref="ITreeNode{TId, T}"/>
    /// <code>
    /// Person grandpa = new Person { ID = "Grandpa", Name = "Richard" };
    /// Person dad = new Person { ID = "Dad", ParentID = "Grandpa", Name = "Richard Jr." };
    /// Person uncle = new Person { ID = "UncleJohn", ParentID = "Grandpa", Name = "John" };
    /// Person cousin = new Person { ID = "CousinAnn", ParentID = "UncleJohn", Name = "Ann" };
    /// Person sister = new Person { ID = "SisterJane", ParentID = "Dad", Name = "Jane" };
    /// Person me = new Person { ID = "Me", ParentID = "Dad", Name = "MeMyselfAndI" };
    ///        
    /// List&lt;Person&gt; familyMembers = new List&lt;Person&gt;();
    ///        
    /// // Let's add them in random order just to prove a point
    /// familyMembers.Add(sister);
    /// familyMembers.Add(dad);
    /// familyMembers.Add(uncle);
    /// familyMembers.Add(grandpa);
    /// familyMembers.Add(me);
    /// familyMembers.Add(cousin);
    ///        
    /// // Creates tree structure automatically by using ID and ParentID of objects.
    /// // Even though the family members were added in random order to the collection 
    /// // the tree is still created properly.
    /// ITreeNode&lt;string, Person&gt; familyTree = TreeNode&lt;string, Person&gt;.CreateTree(familyMembers);
    ///
    /// // You do not have to do anything yourself to get a <see cref="TreeList{TId, T}"/> object. 
    /// // By creating a tree in the above fashion, these collections are created 
    /// // for you. 
    /// // grandpa.Children &lt;-- this is a TreeList&lt;string, Person&gt; object
    /// // me.Siblings &lt;-- this is also a TreeList&lt;string, Person&gt; object
    /// </code>
    /// </remarks>
    /// <typeparam name="TId">The type of the <see cref="ITreeNode{TId, T}.Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The data type of an <see cref="ITreeNode{TId, T}"/> object. This collection will be a collection of this type of object.</typeparam>
    public class TreeList<TId, T> : Collection<ITreeNode<TId, T>> where T : ITreeNode<TId, T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The object that is a parent to this colleciton of child objects</param>
        public TreeList(ITreeNode<TId, T> parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Adds the object to the list. The list of objects are children of <see cref="Parent"/>.
        /// </summary>
        /// <param name="child">The object being added as another child in the list</param>
        public new void Add(ITreeNode<TId, T> child)
        {
            child.Parent = Parent;
            child.ParentId = Parent.Id;
            child.Root = Parent.Root ?? Parent;
            child.RootId = Parent.Root != null ? Parent.RootId : Parent.Id;
            base.Add(child);
        }

        /// <summary>
        /// The object that serves as a Parent to this collection of child objects
        /// </summary>
        public ITreeNode<TId, T> Parent { get; }
    }
    
}
