using System.Collections.ObjectModel;

namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class inherits from ObservableCollection&lt;&gt; and overrides the Add() method. This custom 
    /// collection was necessary so that the Add() method can take care of the details of setting up 
    /// the child node in the tree linking it properly to its Parent and Root node, whereas a regular 
    /// collection is not aware of any hierarchical structure. It is used as the data type for the 
    /// ITreeNode&lt;&gt;.Children property and the ITreeNode&lt;&gt;.Siblings property. 
    /// </summary>
    /// <remarks>
    /// The TreeNode&lt;&gt; creates the TreeList objects internally for the Children property and the 
    /// Siblings property. No need to create this collection yourself unless you are creating your own 
    /// implementation of ITreeNode&lt;&gt;
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
    /// ITreeNode&lt;ITreeNode&lt;Person&gt;&gt; familyTree = TreeNode&lt;ITreeNode&lt;Person&gt;&gt;.CreateTree(familyMembers);
    ///
    /// // You do not have to do anything yourself to get a TreeList&lt;T&gt; object. 
    /// // By creating a tree in the above fashion, these collections are created 
    /// // for you. 
    /// // grandpa.Children &lt;-- this is a TreeList&lt;Person&gt; object
    /// // me.Siblings &lt;-- this is also a TreeList&lt;Person&gt; object
    /// </code>
    /// </remarks>
    /// <typeparam name="T">The data type of an ITreeNode&lt;&gt; object. This collection will be a collection of this type of object.</typeparam>
    public class TreeList<T> : ObservableCollection<ITreeNode<T>> where T : ITreeNode<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The object that is a parent to this colleciton of child objects</param>
        public TreeList(ITreeNode<T> parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Adds the object to the list. The list of objects are children of <see cref="Parent"/>.
        /// </summary>
        /// <param name="child">The object being added as another child in the list</param>
        public new void Add(ITreeNode<T> child)
        {
            child.Parent = Parent;
            child.ParentId = Parent.Id;
            child.Root = Parent.Root ?? Parent;
            base.Add(child);
        }

        /// <summary>
        /// The object that serves as a Parent to this collection of child objects
        /// </summary>
        public ITreeNode<T> Parent { get; }
    }
    
}
