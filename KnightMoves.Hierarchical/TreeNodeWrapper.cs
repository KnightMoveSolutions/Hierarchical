namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class is provided as a wrapper for entity classes that cannot directly inherit from 
    /// <see cref="TreeNode{TId, T}"/>
    /// </summary>
    /// <remarks>
    /// This class is provided as a wrapper for entity classes that cannot directly inherit from 
    /// <see cref="TreeNode{TId, T}"/> since there is no multiple inheritance in .NET (thankfully). Instead of 
    /// inheriting from <see cref="TreeNode{TId, T}"/> the entity object can be wrapped by this class, which itself 
    /// inherits from <see cref="TreeNode{TId, T}"/> on the entity object's behalf. Binding to values of the entity
    /// object requires accessing the object via the <see cref="Entity"/> property.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public sealed class TreeNodeWrapper<TId, T> : TreeNode<TId, TreeNodeWrapper<TId, T>>
    {
        /// <summary>
        /// Takes the <paramref name="entity"/> of type <see cref="T"/>, along with the ID of the 
        /// <paramref name="entity"/> and the <paramref name="entityParentId"/> of its parent and wraps the <paramref name="entity"/> so it can function like a 
        /// <see cref="TreeNode{TId, T}"/> object. 
        /// </summary>
        /// <remarks>
        /// The <paramref name="entityId"/> and <paramref name="entityParentId"/> arguments are provided separately because they may 
        /// not be of type <see cref="TId"/> in the original <paramref name="entity"/> object. They have to be converted to type <see cref="TId"/> 
        /// by the calling code and provided through those arguments.
        /// </remarks>
        /// <param name="entity">The entity being wrapped</param>
        /// <param name="entityId">The identifier value of the <paramref name="entity"/> as type <see cref="TId"/></param>
        /// <param name="entityParentId">The identifier value of the <paramref name="entity"/> object's parent as type <see cref="TId"/></param>
        public TreeNodeWrapper(T entity, TId entityId, TId entityParentId)
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
