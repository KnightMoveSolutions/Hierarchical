namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class is provided as a wrapper for entity classes that cannot directly inherit from 
    /// <see cref="TreeNode{TId, T}"/>
    /// </summary>
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
