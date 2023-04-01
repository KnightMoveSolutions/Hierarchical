`Documentation Home <https://docs.knightmovesolutions.com>`_

================
SortableTreePath
================

This property implements the Materialized Path Pattern. A well-known example of Materialized Path 
is the common file system folder path. ``C:\Users\Bob\Documents`` is an example of a Materialized Path. 
The ``SortableTreePath`` produces a path using integers starting with 1. See the example below.

.. code-block:: text

    Tree                     SortableTreePath
    ----------------------   ---------------------------
    RootNode                 1
      ItemA                  1.1
        ItemA1               1.1.1
        ItemA2               1.1.2
      ItemB                  1.2
        ItemB1               1.2.1
        ItemB1A              1.2.1.1
      ItemC                  1.3
      ItemD                  1.4
        ItemD1               1.4.1
        ItemD2               1.4.2
      ...
      etc.

You can see that if you sort by the ``SortableTreePath`` it will result in a list where every object 
is in order by its position in the tree. This value is incredibly useful when persisting to the database. 
It allows for the use of very simple SQL queries to get nodes or segments of the tree.

Using the dataset above you can do the following.

.. note::

   Keep in mind that if you have multiple trees in the table, you will have to add ``WHERE RootId = 'Something' || Id = 'Something'``
   to avoid mixing nodes from different trees. Your data access model would have to persist the ``RootId`` 
   from the entity in order to accommodate this.

Get all nodes in the tree in hierarchical order:

.. code-block:: SQL

    SELECT IndentString + Name FROM MyTreeTable ORDER BY SortableTreePath

Result:

.. code-block:: text

    Name
    -------------
    RootNode
      ItemA
        ItemA1
        ItemA2
      ItemB
        ItemB1
        ItemB1A
      ItemC
      ItemD
        ItemD1
        ItemD2

Get children of ItemD:

.. code-block:: SQL

    SELECT Name FROM MyTreeTable WHERE SortableTreePath LIKE '1.4.%' ORDER BY SortableTreePath

Result:

.. code-block:: text

    Name
    -------------
    ItemD1
    ItemD2   

Get Ancestors of ItemBA:

.. code-block:: SQL

    SELECT @itemBAPath = SortableTreePath 
    FROM MyTreeTable 
    WHERE Name = 'ItemBA' -- Noramlly use ID but you get the idea

    SELECT Name 
    FROM MyTreeTable 
    WHERE @itemBAPath LIKE SortableTreePath + '%' 
    ORDER BY SortableTreePath

Result:

.. code-block:: text

    Name
    -------------
    RootNode  
      ItemB
        ItemB1
        ItemBA

