`Documentation Home <https://docs.knightmovesolutions.com>`_

=======
Methods
=======

FindById
--------
.. code-block:: csharp

    ITreeNode<TId, T> FindById(TId nodeId)

Finds and returns the ``ITreeNode<TId, T>`` object where the ``Id`` value is equal to the ``nodeId`` 
value provided as an argument. It will search the tree recursively until it is found. ``TId`` is 
the type of the ``Id`` property. 

IsAncestor
----------
.. code-block:: csharp

    bool IsAncestor(T treeNode)

Determines if the node provided as the ``treeNode`` is an ancestor of this node up the tree.

IsDescendent
------------
.. code-block:: csharp

    bool IsDescendent(T treeNode)

Determines if the node provided as the ``treeNode`` is a descendant of this node down the tree
recursively.

IsSibling
---------
.. code-block:: csharp

    bool IsSibling(T treeNode)

Determines if the node provided as the ``treeNode`` is a sibling of this node (i.e. they share 
the same ``Parent`` and are members of the same ``Children`` collection).

MakeChildOfUpperSibling 
-----------------------
.. code-block:: csharp

    void MakeChildOfUpperSibling()

Makes this node a child of the sibling higher in the order of the ``Children`` collection of its 
parent. In a user-interface this method could be wired to a right-arrow that moves the node to 
the right in the tree causing it to be indented under the item above it.

MakeSiblingOfParent 
-------------------
.. code-block:: csharp 

    void MakeSiblingOfParent(bool takeLowerSiblingsAsChildren = false)

Makes this node a sibling of its parent. In a user-interface this method could be wired to a 
left-arrow that moves the node to the left in the tree causing it to be indented at the same 
level of the item above it and thereby converting it into a sibling of its parent. The 
``takeLowerSiblingsAsChildren``, if ``true``, will cause any existing siblings under it to 
be converted into its children. The default is for the lower siblings to remain children of 
the upper (formerly ``Parent``) item. 

MoveDownInSiblingOrder 
----------------------
.. code-block:: csharp 

    void MoveDownInSiblingOrder()

Moves this node down in the sibling order. In a user-interface this method could be wired 
to a down-arrow causing the node to be moved down in the sibling order. It will stop moving
down when it gets to the bottom of the sibling order at its current level in the tree.

MoveUpInSiblingOrder 
--------------------
.. code-block:: csharp 

    void MoveUpInSiblingOrder()

Moves this node up in the sibling order. In a user-interface this method could be wired 
to an up-arrow causing the node to be moved up in the sibling order. It will stop moving
up when it gets to the top of the sibling order at its current level in the tree remaining
under its current parent.

ProcessChildren
---------------

.. code-block:: csharp

    bool ProcessChildren(Func<ITreeNode<TId, T>, bool> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree,
where the ``nodeProcessor`` is a lambda function used as the handler. The ``ProcessChildren``
method will return ``true`` if the ``nodeProcessor`` returns ``true`` for **all** nodes and ``false`` 
if the ``nodeProcessor`` returns ``false`` for **any** of the nodes. The method does not include 
this node when processing the tree.

.. code-block:: csharp 

    bool ProcessChildren(ITreeNodeProcessor<TId, T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree, where the 
``nodeProcessor`` is a handler object that implements ``ITreeNodeProcessor<TId, T>``. The 
``ProcessChildren`` method will return ``true`` if the ``nodeProcessor`` returns ``true`` for 
**all** nodes and ``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes. 
The method does not include this node when processing the tree.

ProcessTree
-----------

.. code-block:: csharp

    bool ProcessTree(Func<ITreeNode<TId, T>, bool> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree,
where the ``nodeProcessor`` is a lambda function used as the handler. The ``ProcessTree``
method will return ``true`` if the ``nodeProcessor`` returns ``true`` for **all** nodes and 
``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes. Unlike 
``ProcessChildren``, this method will start with (include) this node.

.. code-block:: csharp

    bool ProcessTree(ITreeNodeProcessor<TId, T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree, where the 
``nodeProcessor`` is a handler object that implements ``ITreeNodeProcessor<TId, T>``. The 
``ProcessTree`` method will return ``true`` if the ``nodeProcessor`` returns ``true`` for 
**all** nodes and ``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes. 
Unlike ``ProcessChildren``, this method will start with (include) this node.

ProcessAncestors
----------------

.. code-block:: csharp 

    bool ProcessAncestors(
        ITreeNodeProcessor<TId, T> nodeProcessor, 
        ITreeNode<TId, T> treeNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` starting with ``treeNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.
The ``ProcessAncestors`` method will return ``true`` if the ``nodeProcessor`` returns ``true`` for 
**all** nodes and ``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes. 

.. code-block:: csharp

    bool ProcessAncestors(
        Func<ITreeNode<TId, T>, bool> nodeProcessor, 
        ITreeNode<TId, T> treeNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with ``treeNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.
The ``ProcessAncestors`` method will return ``true`` if the ``nodeProcessor`` returns ``true`` for 
**all** nodes and ``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes.

.. code-block:: csharp 

    bool ProcessAncestors(
        Func<ITreeNode<TId, T>, bool> nodeProcessor, 
        ITreeNode<TId, T> treeNode, 
        Func<ITreeNode<TId, T>, bool> stopFunction
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with 
``treeNode`` and stopping at the node that causes the stopFunction to return true.
The ``ProcessAncestors`` method will return ``true`` if the ``nodeProcessor`` returns ``true`` for 
**all** nodes and ``false`` if the ``nodeProcessor`` returns ``false`` for **any** of the nodes.
