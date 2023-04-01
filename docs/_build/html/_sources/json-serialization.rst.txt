`Documentation Home <https://docs.knightmovesolutions.com>`_

==================
JSON Serialization
==================

Due to the fact that the ``TreeNode<TId, T>`` implementation includes references to the ``Parent``, 
``Root`` and ``Siblings`` objects, it introduces complications when serializing the tree for JSON 
output. It will fail with errors about circular references. To make things worse, for some reason 
at the time of this documentation (July 2021), the ``[JsonIgnore]`` attribute does not work for 
``NewtonSoft`` or the ``System.Text.Json.Serialization`` namespace.

Serialization
-------------

In order to get around this, two methods were implemented to mark the tree as serializable and unmark 
the tree as serializable.

.. code-block:: csharp 

    familyTree.MarkAsSerializable();

    // Safe to serialize to JSON here. Cannot use some methods.

    familyTree.UnMarkAsSerializable();

    // Restored to original state. Safe to use all methods.

The ``MarkAsSerializable()`` method sets the ``IsSerializable`` property to true for all nodes in the 
tree recursively. When ``IsSerializable`` is true then the ``Root``, ``Parent``, and ``Siblings`` 
properties will return null in order to avoid the circular reference exception during serialization. 
During this time, methods that rely on ``Parent`` or ``Root`` will not work. Therefore, you should only 
mark the tree as serializable for the lines of code that produce the JSON string. After that, if you 
want the full functionality of the tree, then you must restore it by calling the ``UnMarkSerializable()`` 
method, which will set the ``IsSerializable`` property to true for all nodes in the tree recursively. 
After that all methods of the tree that depend on ``Parent``, ``Root``, and ``Siblings`` will function 
normally.

Deserialization
---------------

This library supports re-hydrating the tree model from a JSON string. It will also work polymorphically, 
where the type of the tree can be a base object and concrete objects that derive from it will be preserved.

The scenario using the original ``familyTree`` example will work.

.. code-block:: csharp 

    familyTree.MarkAsSerializable();

    var json = Newtonsoft.Json.JsonConvert.SerializeObject(familyTree);

    Console.WriteLine(json); // prints json as expected

    var rebuiltTree = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(json);

    Console.WriteLine(rebuiltTree.Children[0].Children[1].Id == "Me"); // prints true

If the tree was built with nodes that derive from the type supplied for ``T`` it will still work.

Consider the following completely new example, which uses ``Person`` as the base class and uses several 
derived classes for the tree nodes.

.. code-block:: csharp 

    namespace MyApp
    {
        // Base class inherits from TreeNode<TId, T>
        public class Person : TreeNode<string, Person>
        {
            // Person is now able to function as a tree node
            // Id and ParentId are inherited properties

            public string Name { get; set; }
        }

        // The SchoolBudget property is specific to the Principal
        public class Principal : Person
        {
            public decimal SchoolBudget { get; set; }
        }

        // The Subject property is specific to the Teacher 
        public class Teacher : Person
        {
            public string Subject { get; set; }
        }

        // The GPA property is specific to the Student
        public class Student : Person
        {
            public decimal GPA { get; set; }
        }

        public class MyFamilyTreeApp
        {
            public static void Main(string[] args)
            {
                var principal = new Principal { Id = "p", Name = "Mrs. Monroe", SchoolBudget = 1000000.00m };
                var teacher = new Teacher { Id = "t", ParentId = "p", Name = "Mrs. Smith", Subject = "Math" }; 
                var student = new Student { Id = "s", ParentId = "t", Name = "Johnny", GPA = 3.75m };

                // NOTICE this is a collection of Person objects
                var schoolPeople = new List<Person> { principal, student, teacher };

                // NOTICE this tree is of Person objects --> TreeNode<string, Person>
                var schoolTree = TreeNode<string, Person>.CreateTree(schoolPeople);

                schoolTree.MarkAsSerializable();

                // Examine this json string to see that properties particular to the objects above 
                // are preserved in the output (i.e. principal.SchoolBudget, teacher.Subject, and student.GPA)
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(schoolTree);

                // Do this if you want to use the schoolTree object again
                schoolTree.UnMarkAsSerializable();

                // De-Serialize the json into a rebuilt tree object using the base Person type
                var rebuiltTree = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(json);

                Console.WriteLine(rebuiltTree.GetType().Name);                          // Prints Principal
                Console.WriteLine(rebuiltTree.Children[0].GetType().Name);              // Prints Teacher
                Console.WriteLine(rebuiltTree.Children[0].Children[0].GetType().Name);  // Prints Student

                Console.WriteLine(rebuiltTree.SchoolBudget);            // Prints 1000000.00
                Console.WriteLine(rebuiltTree.Children[0].Subject);     // Prints Math
                Console.WriteLine(rebuiltTree.Children[0].GPA);         // Prints 3.75
            }
        }
    }

So you can see that if your tree is made up of different classes that derive from a base class, the tree 
model and the serialization / deserialization of the objects will still work.
