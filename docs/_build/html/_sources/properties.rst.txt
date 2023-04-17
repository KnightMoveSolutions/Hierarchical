`Documentation Home <https://docs.knightmovesolutions.com>`_

==========
Properties
==========

Id
--

.. code-block:: csharp 

   TId Id 

The ID of this node. It's data type is customized by the ``TId`` generic type specification.

Children 
--------

.. code-block:: csharp 

    TreeList<TId, T> Children

The Child objects of this node in the hierarchy as a collection of type ``TreeList<TId, T>``, which is a 
custom implementation that derives from ``System.Collections.ObjectModel.Collection<T>``. A node is considered
a "branch" if the Children collection contains at least one item. A node is considered a "leaf" if the 
Children collection is empty.

DepthFromRoot
-------------

.. code-block:: csharp 

    int DepthFromRoot

Represents the number of levels in the tree that the node is in where the ROOT node is Level 1 
(i.e. not zero-based like arrays). Whatever code artifact contains the Root node (i.e. whole tree) 
is considered Level 0 but is not a node of the tree. ``DepthOfRoot`` is used to calculate the 
``IndentString`` documented below.

HasChildren
-----------

.. code-block:: csharp 

    bool HasChildren

True if this node has Children false if not. If true then this node is considered a "branch". If false 
then this node is considered a "leaf".

IndentCharacter
---------------

.. code-block:: csharp 

    char IndentCharacter

The character used for indentation. It will be repeated once for every Level in the tree. It is  
repeated ``DepthFromRoot`` times and returned by the ``IndentString`` property.

IndentString
------------

.. code-block:: csharp 

    string IndentString

Returns the ``IndentCharacter`` repeated ``DepthFromRoot`` times to produce the indentation string 
for pretty display in text output.

Parent
------

.. code-block:: csharp 

    T Parent

A reference to the Parent node object of this node.

ParentId
--------

.. code-block:: csharp

    TId ParentId

The ``Id`` of the ``Parent`` object of this node. It's data type is customized by the ``TId`` generic type specification.

PathId
------

.. code-block:: csharp 

    string PathId

A unique identifier representing the unique path from the root to the current node. Essentially, it 
is a hash of the chain of hashes of ancestor Ids.

Root
----

.. code-block:: csharp 

    T Root

A reference to the Root object in the tree. All objects in the tree will have the same Root.

RoodId
------

.. code-block:: csharp 

    TId RootId

The ``Id`` of the ``Root`` object. Useful for persisting to the database when there are multiple 
hierarchies in the same table differentiated by their shared ``Root``. It's data type is customized 
by the ``TId`` generic type specification.

Siblings
--------

.. code-block:: csharp 

    TreeList<TId, T>

The other objects in the ``Parent`` object's ``Children`` collection. It returns all objects in that 
collection except for this node.

SortableTreePath
----------------

.. code-block:: csharp 

    string SortableTreePath

A string representation of the location of this object in the tree in the form of a "Path". The 
``SortableTreePath`` is extensively documented in the :doc:`sortable-tree-path` page.

TreeNodeId
----------

.. code-block:: csharp 

    Guid TreeNodeId

A unique identifer for the node. It is provided as a placeholder that is not required but can be useful.

