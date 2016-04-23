namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// Classes that implement this interface can be used to feed into the TreeNode&lt;&gt;.ProcessTree() or 
    /// the TreeNode&lt;&gt;.ProcessChildren() methods.
    /// </summary>
    /// <remarks>
    /// <code language="csharp">
    /// namespace MyPersonApp
    /// {
    ///     public static void Main(string[] args)
    ///     {
    ///         Person grandpa = new Person { ID = "Grandpa", Name = "Richard" };
    ///         Person dad = new Person { ID = "Dad", ParentID = "Grandpa", Name = "Richard Jr." };
    ///         Person uncle = new Person { ID = "UncleJohn", ParentID = "Grandpa", Name = "John" };
    ///         Person cousin = new Person { ID = "CousinAnn", ParentID = "UncleJohn", Name = "Ann" };
    ///         Person sister = new Person { ID = "SisterJane", ParentID = "Dad", Name = "Jane" };
    ///         Person me = new Person { ID = "Me", ParentID = "Dad", Name = "MeMyselfAndI" };
    ///         
    ///         List&lt;Person&gt; familyMembers = new List&lt;Person&gt;();
    ///        
    ///         // Let's add them in random order just to prove a point
    ///         familyMembers.Add(sister);
    ///         familyMembers.Add(dad);
    ///         familyMembers.Add(uncle);
    ///         familyMembers.Add(grandpa);
    ///         familyMembers.Add(me);
    ///         familyMembers.Add(cousin);
    ///        
    ///         // Creates tree structure automatically by using ID and ParentID of objects.
    ///         // Even though the family members were added in random order to the collection 
    ///         // the tree is still created properly.
    ///         ITreeNode&lt;ITreeNode&lt;Person&gt;&gt; familyTree = TreeNode&lt;ITreeNode&lt;Person&gt;&gt;.CreateTree(familyMembers);
    ///    
    ///         PrintNamePersonProcessor processor = new PrintNamePersonProcessor();
    ///    
    ///         familyTree.ProcessTree(processor);
    ///    
    ///         // Output:
    ///         // ---------------------------------
    ///         //  Richard (1)
    ///         //   Richard Jr. (1.1)
    ///         //    Jane (1.1.1)
    ///         //    MeMyselfAndI (1.1.2)
    ///         //   John (1.2)
    ///         //    Ann (1.2.1)
    ///     }
    /// }
    ///     
    ///     public class Person : TreeNode&lt;Person&gt;
    ///     {
    ///         public string Name { get; set; }
    ///     }
    /// 
    ///     public class PrintNamePersonProcessor() : ITreeNodeProcessor&lt;Person&gt;
    ///     {
    ///         public bool ProcessNode(ITreeNode&lt;T&gt; node)
    ///         {
    ///             Console.WriteLine(node.IndentString + node.Name + " (" + node.SortableTreePath + ")");
    ///             return true;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <typeparam name="T">The type of object that implements <see cref="ITreeNode&lt;T&gt;"/> directly or indirectly by inheriting from <see cref="TreeNode&lt;T&gt;"/> and which this ITreeNodeProcessor object will be processing.</typeparam>
    public interface ITreeNodeProcessor<T> where T : ITreeNode<T>
    {
        /// <summary>
        /// Classes that implement this interface accept an ITreeNode&lt;T&gt; 
        /// object and uses it to perform desired process 
        /// </summary>
        /// <param name="node">The ITreeNode&lt;T&gt; that the method depends on for its process</param>
        /// <returns>True if execution was successful, false if not.</returns>
        bool ProcessNode(ITreeNode<T> node);
    }
}
