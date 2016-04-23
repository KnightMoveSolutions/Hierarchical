namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class is provided as a wrapper for entity classes that cannot directly inherit from 
    /// TreeNode&lt;&gt;
    /// </summary>
    /// <remarks>
    /// This class is provided as a wrapper for entity classes that cannot directly inherit from 
    /// TreeNode&lt;&gt; since there is no multiple inheritance in .NET (thankfully). Instead of 
    /// inheriting from TreeNode&lt;&gt; the entity object can be wrapped by this class, which itself 
    /// inherits from TreeNode&lt;&gt; on the entity object's behalf. Binding to values of the entity
    /// object requires accessing the object via the <see cref="Entity"/> property.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public sealed class TreeNodeWrapper<T> : TreeNode<TreeNodeWrapper<T>>
    {
        /// <summary>
        /// Constructor. Takes the <paramref name="entity"/> of type T, along with the ID of the 
        /// entity and the ID of its parent and wraps the entity so it can function like a 
        /// TreeNode&lt;T&gt; object. 
        /// </summary>
        /// <remarks>
        /// The entityId and entityParentId arguments are provided separately because they may 
        /// not be strings in the original entity object. They have to be converted to strings 
        /// by the calling code and provided in those arguments.
        /// </remarks>
        /// <param name="entity"></param>
        /// <param name="entityId"></param>
        /// <param name="entityParentId"></param>
        public TreeNodeWrapper(T entity, string entityId, string entityParentId)
        {
            Entity = entity;
            Id = entityId;
            ParentId = entityParentId;
        }

        /// <summary>
        /// The domain entity being wrapped
        /// </summary>
        public T Entity { get; set; }

    }
}
