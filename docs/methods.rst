`Documentation Home <https://docs.knightmovesolutions.com>`_

=======
Methods
=======

Contains
--------
.. code-block:: csharp 

    bool Contains(Func<T, bool> condition)
    
Returns true if the tree contains AT LEAST ONE node that satisfied the condition provided 
by the lambda function.

DeepCopy
--------
.. code-block:: csharp 

    T DeepCopy()

Makes a deep copy (clone) of the tree. If this method is called on a node that is in the middle 
of an existing tree, then a copy of the branch will be returned where the branch node becomes 
the root object. It works like an unmount in a Linux file system except that the unmounted branch 
is returned.

Filter
------
.. code-block:: csharp 

    T Filter(Func<T, bool> filter)

Returns a pruned tree where only the nodes that match the filter are returned with their complete ancestor 
paths up to the root. Nodes that are not matched are not included. If a branch of nodes does not have a
matching node in it, then the entire branch of nodes will be excluded from the returned tree.

FindById
--------
.. code-block:: csharp

    T FindById(TId nodeId)

Finds and returns the ``T`` object where the ``Id`` value is equal to the ``nodeId`` 
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

    void ProcessChildren(Action<T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree, where 
the ``nodeProcessor`` is a lambda function used as the handler. The method does not include 
this node when processing the tree.

.. code-block:: csharp 

    void ProcessChildren(ITreeNodeProcessor<TId, T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree, where the 
``nodeProcessor`` is a handler object that implements ``ITreeNodeProcessor<TId, T>``. The method 
does not include this node when processing the tree.

ProcessTree
-----------

.. code-block:: csharp

    void ProcessTree(Action<T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree,
where the ``nodeProcessor`` is a lambda function used as the handler. Unlike 
``ProcessChildren``, this method will start with (include) this node.

.. code-block:: csharp

    void ProcessTree(ITreeNodeProcessor<TId, T> nodeProcessor)

Passes each child of this node to the ``nodeProcessor`` recursively down the tree, where the 
``nodeProcessor`` is a handler object that implements ``ITreeNodeProcessor<TId, T>``. Unlike 
``ProcessChildren``, this method will start with (include) this node.

ProcessAncestors
----------------

.. code-block:: csharp 

    void ProcessAncestors(
        ITreeNodeProcessor<TId, T> nodeProcessor, 
        T startNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` starting with ``startNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.

.. code-block:: csharp

    void ProcessAncestors(
        Action<T> nodeProcessor, 
        T startNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with ``startNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.

.. code-block:: csharp 

    void ProcessAncestors(
        Action<T> nodeProcessor, 
        T startNode, 
        Func<T, bool> stopFunction
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with 
``startNode`` and stopping at the node that causes the stopFunction to return true.

ToList
------

.. code-block:: csharp 

    List<T> ToList()

Returns all nodes in the tree as a flattened collection of type ``List<T>`` sorted by the ``SortableTreePath`` 
property. If this method is called on a branch then it will return only the nodes in that branch where the top 
of the branch is the root and will be the first element in the list.
