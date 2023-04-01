`Documentation Home <https://docs.knightmovesolutions.com>`_

======================
Using Entity Framework
======================

Utilizing an domain model object that inherits from ``TreeNode<TId, T>`` is problematic due to 
the particulars of how Entity Framwork functions. To accommodate the use of EF so that the 
``ITreeNode<TId, T>`` properties can be persisted to the database, another abstract class was 
created to satisfy this need.

.. code-block:: csharp 

    TreeNodeEF<TId, T>

If your domain entity is to be persisted to the database using Entity Framework, then it should 
inherit from ``TreeNodeEF<TId, T>`` instead of ``TreeNode<TId, T>``. Certain properties of the 
``ITreeNode<TId, T>`` model have been decorated with data annotations to instruct EF on what to 
do with those properties such as annotating properties with ``[NotMapped]`` to prevent EF from 
persisting them. 

It is recommended to annotate the ``Id`` property with the ``[Key]`` attribute 
or configure it as the primary key using the Fluent API. 

If the primary key of your entity is not named ``Id`` then you can override the ``Id`` property 
with get and set code that wires the ``Id`` property to your entity's primary key property. You 
should then mark the ``ParentId`` as a foreign key fluently or with the ``[ForeignKey("ParentEntity")]`` 
annotation where the ``Parent`` Entity has been provided to serve as the navigation property for 
the parent. This makes it a self-referencing foreign key implementing the hierarchy in the relational 
model.

There's some other trickery done under the hood to get it to work, particularly with properties 
that are computed at runtime such as the ``SortableTreePath`` property. If you're curious you can 
look at the source code but it basically takes care of the tree node properties that are persistable 
to the database for you. The rest of your own properties in your subclass are up to you to manage 
as you see fit but whatever you want to persist to the database will go along with the underlying 
inherited properties managed by ``TreeNodeEF<TId, T>``.

Lastly, inheriting from ``TreeNodeEF<TId, T>`` will work with migrations as long as your subclass 
is defined as a ``DbSet<T>`` property in your application's DbContext.
