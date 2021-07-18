namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// Classes that implement this interface can be used to feed into the <see cref="TreeNode{TId, T}.ProcessTree(ITreeNodeProcessor{TId, T})"/> or 
    /// the <see cref="TreeNode{TId, T}.ProcessChildren(ITreeNodeProcessor{TId, T})"/> methods.
    /// </summary>
    /// <typeparam name="TId">The type of the <see cref="ITreeNode{TId, T}.Id"/> property to accommodate different types of identifiers such as string, int, or Guid</typeparam>
    /// <typeparam name="T">The type of object that implements <see cref="ITreeNode{TId, T}"/> or which inherits from either <see cref="TreeNode{TId, T}"/> or <see cref="TreeNodeEF{TId, T}"/>. It will be processed by this <see cref="ITreeNodeProcessor{TId, T}"/> object.</typeparam>
    public interface ITreeNodeProcessor<TId, T> where T : ITreeNode<TId, T>
    {
        /// <summary>
        /// Classes that implement this interface accept an <see cref="ITreeNode{TId, T}"/> 
        /// object and uses it to perform desired process 
        /// </summary>
        /// <param name="node">The <see cref="ITreeNode{TId, T}"/> object that the method depends on for its process</param>
        /// <returns>True if execution was successful, false if not.</returns>
        bool ProcessNode(ITreeNode<TId, T> node);
    }
}
