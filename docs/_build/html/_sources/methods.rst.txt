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

Example:

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male 
            Id: Dad, Name: Richard Jr, Gender: Male 
                Id: SisterJane, Name: Jane, Gender: Female
                Id: Me, Name: MeMyselfAndI, Gender: Male 
            Id: UndleJohn, Name: John, Gender: Male 
                Id: CousinAnn, Name: Ann, Gender: Female 
    */

    var containsFemale = familyTree.Contains(p => p.Gender == Gender.Female); // true
    var containsOnlyChild = familyTree.Contains(p => p.Children.Count == 1); // true
    var containsMother = familyTree.Contains(p => p.HasChildren && p.Gender == Gender.Female); // false 

DeepCopy
--------
.. code-block:: csharp 

    T DeepCopy()

Makes a deep copy (clone) of the tree. If this method is called on a node that is in the middle 
of an existing tree, then a copy of the branch will be returned where the branch node becomes 
the root object. It works like an unmount in a Linux file system except that the unmounted branch 
is returned. Another way to look at it is that it functions like the spread operator (...) in 
Javascript, except that it is a recursive spread.

.. warning:: 

    DeepCopy() serializes and deserializes the tree in order to create a full copy of every object. If 
    your class contains references to anything that references it back then you will see a circular 
    reference exception. Before calling DeepCopy(), you must ensure that there are no circular referencing
    properties in your class.  

Example:

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    // Make a copy of the whole tree
    var familyTreeCopy = familyTree.DeepCopy();

    // Make a copy of a branch 
    var branchCopy = familyTree.Children[0].DeepCopy();

    /*
        branchCopy looks like this 

        Id: Dad, Name: Richard Jr, Gender: Male, ParentId: null 
            Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
            Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad  
    */

Filter
------
.. code-block:: csharp 

    T Filter(Func<T, bool> filter)

Returns a pruned tree where only the nodes that match the filter are returned with their complete ancestor 
paths up to the root. Nodes that are not matched are not included. If a branch of nodes does not have a
matching node in it, then the entire branch of nodes will be excluded from the returned tree.

.. warning:: 

    Filter() uses the DeepCopy() method, which serializes and deserializes the tree in order to create a full 
    copy of the objects it needs to produce the result. If your class contains references to anything that 
    references it back then you will see a circular reference exception. Before calling DeepCopy(), you must 
    ensure that there are no circular referencing properties in your class.  

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinSteve, Name: Steve, Gender: Male, ParentId: John
            Id: UncleLuke, Name: Luke, Gender: Male, ParentId: Grandpa
                Id: CousinMary, Name: Mary, Gender: Female, ParentId: UncleLuke 
    */

    var femaleTree = familyTree.Filter(p => p.Gender === Gender.Female);

    /*
        femaleTree will look like this 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
            Id: UncleLuke, Name: Luke, Gender: Male, ParentId: Grandpa
                Id: CousinMary, Name: Mary, Gender: Female, ParentId: UncleLuke 
    */

In the example above note the following:

* Grandpa, Dad, and UncleLuke are all included because they have a matching node in their descendants
* MeMyselfAndI is not included in the list of Children for Dad because it doesn't match the filter condition
* UndleJohn and its descendants are entirely excluded because there are not matching nodes in that branch

FindById
--------
.. code-block:: csharp

    T FindById(TId nodeId)

Finds and returns the ``T`` object where the ``Id`` value is equal to the ``nodeId`` 
value provided as an argument. It will search the tree recursively until it is found. ``TId`` is 
the type of the ``Id`` property. 

Example:

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    var targetNode = familyTree.FindById("UndleJohn");

In the example above note the following: 

    * ``targetNode`` will contain the "UncleJohn" branch in the tree above
    * ``targetNode.Parent.Id`` is "Grandpa"
    * ``targetNode.Children[0].Id`` is "CousinAnn"

Find
----

.. code-block:: csharp 

    T Find(Func<T, bool> condition)

Finds and returns the object of type ``T`` where the ``condition`` implemented by the lambda function returns 
true. It will search the tree recursively until the first item is found ignoring any other nodes that match 
the ``condition``.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    var targetNode = familyTree.Find(p => p.Gender == Gender.Female);

In the example above note the following: 

    * ``targetNode.Id`` is "SisterJane"
    * The node with ``Id == "CousinAnn"`` also satisfies the filter condition but it is ignored because it 
      will find "SisterJane" first 

IsAncestor
----------
.. code-block:: csharp

    bool IsAncestor(T treeNode)

Determines if the node provided as the ``treeNode`` is an ancestor of this node up the tree.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    var grandpa familyTree.FindById("Grandpa");
    var dad = familyTree.FindById("Dad");
    var uncle = familyTree.FindById("UncleJohn");
    var me = familyTree.FindById("Me");

    bool result; 

    // result == true 
    result = me.IsAncestor(grandpa);

    // result == true
    result = me.IsAncestor(dad);

    // result == false 
    result = me.IsAncestor(uncle);

IsDescendent
------------
.. code-block:: csharp

    bool IsDescendent(T treeNode)

Determines if the node provided as the ``treeNode`` is a descendant of this node down the tree
recursively.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    var grandpa familyTree.FindById("Grandpa");
    var dad = familyTree.FindById("Dad");
    var uncle = familyTree.FindById("UncleJohn");
    var me = familyTree.FindById("Me");

    bool result; 

    // result == true 
    result = grandpa.IsDescendent(dad);

    // result == true
    result = grandpa.IsDescendent(me);

    // result == false 
    result = uncle.IsDescendent(me);

IsSibling
---------
.. code-block:: csharp

    bool IsSibling(T treeNode)

Determines if the node provided as the ``treeNode`` is a sibling of this node (i.e. they share 
the same ``Parent`` and are members of the same ``Children`` collection).

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */

    var dad = familyTree.FindById("Dad");
    var uncle = familyTree.FindById("UncleJohn");
    var sister = familyTree.FindById("SisterJane");
    var me = familyTree.FindById("Me");
    var cousin = familyTree.FindById("CousinAnn");

    bool result; 

    // result == true 
    result = dad.IsSibling(uncle);

    // result == true
    result = me.IsSibling(sister);

    // result == false 
    result = me.IsSibling(cousin);

MakeChildOfUpperSibling 
-----------------------
.. code-block:: csharp

    void MakeChildOfUpperSibling()

Makes this node a child of the sibling higher in the order of the ``Children`` collection of its 
parent. In a user-interface this method could be wired to a **right-arrow** that moves the node to 
the right in the tree causing it to be indented under the item above it.

Example: 

.. code-block:: csharp

    /* 
        Given the tree below: CousinAnn is misplaced as a child of Grandpa 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
            Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var cousin = familyTree.FindById("CousinAnn");

    cousin.MakeChildOfUpperSibling();

    /* 
        The tree now looks like this  

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */    

MakeSiblingOfParent 
-------------------

.. code-block:: csharp 

    void MakeSiblingOfParent(bool takeLowerSiblingsAsChildren = false)

Makes this node a sibling of its parent. In a user-interface this method could be wired to a 
**left-arrow** that moves the node to the left in the tree causing it to be indented at the same 
level of the item above it and thereby converting it into a sibling of its parent. The 
``takeLowerSiblingsAsChildren``, if ``true``, will cause any existing siblings under it to 
be converted into its children. The default is for the lower siblings to remain children of 
the upper (formerly ``Parent``) item. 

Example: 

.. code-block:: csharp

    /* 
        Given the tree below: UndleJohn is misplaced as a child of Dad 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
                Id: UndleJohn, Name: John, Gender: Male, ParentId: Dad
                Id: SisterMary, Name: Mary, Gender: Female, ParentId: Dad
    */

    var uncle = familyTree.FindById("UndleJohn");

    uncle.MakeSiblingOfParent();

    /* 
        The tree now looks like this  

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
                Id: SisterMary, Name: Mary, Gender: Female, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
    */  

Note the following in the example above 

    * ``SisterMary`` remains a child of ``Dad`` 

Example using ``takeLowerSiblingsAsChildren = true``:

.. code-block:: csharp

    /* 
        Given the tree below: UndleJohn is misplaced as a child of Dad 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
                Id: UndleJohn, Name: John, Gender: Male, ParentId: Dad
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Dad
    */

    var uncle = familyTree.FindById("UndleJohn");

    uncle.MakeSiblingOfParent(true);

    /* 
        The tree now looks like this  

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: John
    */ 

Note the following in the example above 

    * ``CousinAnn`` becomes a child of ``UndleJohn`` 


MoveDownInSiblingOrder 
----------------------
.. code-block:: csharp 

    void MoveDownInSiblingOrder()

Moves this node down in the sibling order. In a user-interface this method could be wired 
to a **down-arrow** causing the node to be moved down in the sibling order. It will stop moving
down when it gets to the bottom of the sibling order at its current level in the tree.

Example: 

.. code-block:: csharp

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var sister = familyTree.FindById("SisterJane");

    sister.MoveDownInSiblingOrder();

     /* 
        The tree now looks like this  

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */    

MoveUpInSiblingOrder 
--------------------
.. code-block:: csharp 

    void MoveUpInSiblingOrder()

Moves this node up in the sibling order. In a user-interface this method could be wired 
to an **up-arrow** causing the node to be moved up in the sibling order. It will stop moving
up when it gets to the top of the sibling order at its current level in the tree remaining
under its current parent.

Example: 

.. code-block:: csharp

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var me = familyTree.FindById("Me");

    me.MoveUpInSiblingOrder();

     /* 
        The tree now looks like this  

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */    

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

Example:

.. code-block:: csharp

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    familyTree.ProcessTree(p => Console.WriteLine(p.IndentString + p.Id));

    /*
        Output: 

        Grandpa
         Dad
          SisterJane
          Me 
         UncleJohn
          CousinAnn 
    */

Note the following in the example above:

    * The root node with Id="Grandpa" is included in the output because processing begins 
      with the calling node itself

    * To process nodes down the tree excluding the current node use the ProcessChildren() method below 

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

Example:

.. code-block:: csharp

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    familyTree.ProcessChildren(p => Console.WriteLine(p.IndentString + p.Id));

    /*
        Output: 

         Dad
          SisterJane
          Me 
         UncleJohn
          CousinAnn 
    */

Note the following in the example above:

    * The root node with Id="Grandpa" is not in the output because processing begins 
      with the Children

ProcessAncestors
----------------

**Using Lambda with maxLevel Parameter**

.. code-block:: csharp

    void ProcessAncestors(
        Action<T> nodeProcessor, 
        T startNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with ``startNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var me = familyTree.FindById("Me");

    familyTree.ProcessAncestors(p => Console.WriteLine(p.Id), me); 

    /*
        Output: 

        Me
        Dad
        Grandpa
    */

    familyTree.ProcessAncestors(p => Console.WriteLine(p.Id), me, 2);

    /*
        Output:

        Me 
        Dad 
    */

Note the following in the example above:

    * In both calls the starting node is ``me`` so the Id "Me" is output first 

    * In the first call to ProcessAncestors() the default ``maxLevel = 1`` was used, which means 
      that by default it will go all the way to the root node. That is why ``Grandpa`` is in the 
      output 

    * In the second call to ProcessAncestors() it uses ``maxLevel = 2``, so it stops processing 
      at the ancestor with ``DepthFromRoot == 2``, which is ``Dad`` 

**Using Lambda with Stop Function** 

.. code-block:: csharp 

    void ProcessAncestors(
        Action<T> nodeProcessor, 
        T startNode, 
        Func<T, bool> stopFunction
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` function starting with 
``startNode`` and stopping at the node that causes the stopFunction to return true.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var me = familyTree.FindById("Me");

    familyTree.ProcessAncestors(
        nodeProcessor: p => Console.WriteLine(p.Id), 
        startNode: me, 
        stopFunction: p => p.Name.Contains("Jr")
    );

    /*
        Output:

        Me 
        Dad 
    */

Note the following in the example above:

    * In both calls the starting node is ``me`` so the Id "Me" is output first 

    * The stop function is more complex than a level number
    
    * You can make the stop function as sophisticated as is necessary

**Using ITreeNodeProcessor Class with maxLevel**

.. code-block:: csharp 

    void ProcessAncestors(
        ITreeNodeProcessor<TId, T> nodeProcessor, 
        T startNode, 
        int maxLevel = 1
    )

Passes each node up the tree of ancestors to the ``nodeProcessor`` starting with ``startNode`` 
and stopping at the node where ``DepthFromRoot`` = 1 (i.e. the top) or the specified ``maxLevel``.

Example: 

.. code-block:: csharp 

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    public class MyTreeNodeProcessor : ITreeNodeProcessor<string, Person>
    {
        public void ProcessNode(Person node) => Console.WriteLine(node.Id);
    }

    var myNodeProcessor = new MyTreeNodeProcessor();
    
    var me = familyTree.FindById("Me");

    familyTree.ProcessAncestors( myNodeProcessor, me );

    /*
        Output: 

        Me
        Dad
        Grandpa
    */

Note the following in the example above:

    * In both calls the starting node is ``me`` so the Id "Me" is output first 

    * The default ``maxLevel = 1`` was used, which means that by default it will go all the way 
      to the root node, which is why ``Grandpa`` is in the output 

    * While not demonstrated here, when using a class you can also specify an integer ``maxLevel`` 
      or a ``stopFunction`` lambda to make it stop climbing the ancestor tree at the defined level 

ToList
------

.. code-block:: csharp 

    List<T> ToList()

Returns all nodes in the tree as a flattened collection of type ``List<T>`` sorted by the ``SortableTreePath`` 
property. If this method is called on a branch then it will return only the nodes in that branch where the top 
of the branch is the root and will be the first element in the list.

.. code-block:: csharp 

    /* 
        Given the tree below: 

        Id: Grandpa, Name: Richard, Gender: Male, ParentId: null
            Id: Dad, Name: Richard Jr, Gender: Male, ParentId: Grandpa
                Id: SisterJane, Name: Jane, Gender: Female, ParentId: Dad
                Id: Me, Name: MeMyselfAndI, Gender: Male, ParentId: Dad
            Id: UndleJohn, Name: John, Gender: Male, ParentId: Grandpa
                Id: CousinAnn, Name: Ann, Gender: Female, ParentId: Grandpa
    */

    var flatList = familyTree.ToList();

    flatList.ForEach(p => p.Console.WriteLine(p.SortableTreePath));

    /*
        Output: 

        1 
        1.1
        1.1.1
        1.1.2
        1.2
        1.2.1 
    */

    flatList.ForEach(p => p.Console.WriteLine(p.IndentString + p.Id));

     /*
        Output: 

        Grandpa 
         Dad
          SisterJane
          Me 
         UncleJohn
          CousinAnn 
    */

Note the following in the example above:

    * The first call above shows how the ``SortableTreePath`` property can be used to display the 
      tree items in the property order without having to recurse the tree even through the list is 
      flat. 

    * The second call above demonstrates the fact that the tree can be displayed in nested fashion 
      using the ``IndentString`` property without recursing by virtue of the fact that the list 
      is sorted by the ``SortableTreePath`` property. 
