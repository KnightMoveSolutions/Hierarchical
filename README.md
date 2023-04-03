# KnightMoves.Hierarchical

A lightweight library of a handful of interfaces/classes that implement everything an entity needs to participate as a node in a tree. 
The purpose of this implementation is to take care of all things necessary for creating tree structures and recursively processing the
nodes in the tree. Some scenarios where trees are useful can be category trees, account/subaccount trees, organization trees, management/employee 
hierarchies, etc. 

## License

MIT License

## Getting Started

You can pull the code from the repository and build it yourself or consume the NuGet package in your solution.

	PM> Install-Package KnightMoves.Hierarchical

It is basically just a single DLL. Here's what you get. 

Suppose you have the following entity class in your application.

    public class Person
    {
        public string Name { get; set; }
        // And a bunch of other properties not included for brevity
    }

All you have to do is inherit from `TreeNode<TId, T>` and your `Person` class becomes a fully capable node in a hierarchy. 

	// Just inherit from TreeNode<TId, T>
    // TId is the type of the entity's Id field. If the name of your entity's identifier property
    // is "Id" then you should override the inherited "Id" property since it is virtual or remove it

    public class Person : TreeNode<string, Person>
    {
        // Person is now able to function as a tree node
        // Id and ParentID are inherited properties 

        public string Name { get; set; }
    }

One instance of your class is not useful as a tree. So to illustrate, we start with a collection of `Person` objects.

	List<Person> persons = _personRepository.GetPersons();

The persons collection can contain the `Person` objects in any order. As long as they have an `Id` and a `ParentId` with a single 
'Person' object having a `null` `ParentId` representing the root node, you can create the tree like this.

	var hierarchy = Person.CreateTree(persons)

The `hierarchy` object is a single `Person` object of type `ITreeNode<TId, T>` object that contains all other objects 
in its `Children` collection. Some of those children will have child objects in their `Children` collection so on and so forth. 

That's all you need at its most basic level.

## A Complete Example

Here is an example console application.

	namespace MyApp
	{
        // Just inherit from TreeNode<TId, T>
        public class Person : TreeNode<string, Person>
        {
	        // Person is now able to function as a tree node
	        // Id and ParentId are inherited properties

	        public string Name { get; set; }
        }

        public class MyFamilyTreeApp
        {
            public static void Main(string[] args)
            {
                var grandpa = new Person { Id = "Grandpa", Name = "Richard" };
                var dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
                var uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
                var cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
                var sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
                var me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };

                var familyMembers = new List<Person>();

                // Let's add them in random order just to prove a point
                familyMembers.Add(sister);
                familyMembers.Add(dad);
                familyMembers.Add(uncle);
                familyMembers.Add(grandpa);
                familyMembers.Add(me);
                familyMembers.Add(cousin);

                // Not linked in tree structure
                Console.WriteLine(grandpa.Children.Count);      // prints 0
                Console.WriteLine(dad.Parent == null);          // prints true
                Console.WriteLine(me.Parent == null);           // prints true

                // Creates tree structure automatically by using Id and ParentId of objects.
                // Even though the family members were added in random order to the collection 
                // the tree is still created properly. It returns a single Person object. In this 
                // case the root Person object is grandpa and it will return it with all other 
                // child objects arranged under it in the hierarchy.

                var familyTree = Person.CreateTree(familyMembers);

                Console.WriteLine(familyTree == grandpa);                           // prints true
                Console.WriteLine(familyTree.Children.Count);                       // prints 2
                Console.WriteLine(familyTree.Children[0] == dad);                   // prints true
                Console.WriteLine(familyTree.Children[1] == uncle);                 // prints true
                Console.WriteLine(familyTree.Children[0].Children.Count);           // prints 2
                Console.WriteLine(familyTree.Children[0].Children[0] == sister);    // prints true
                Console.WriteLine(familyTree.Children[0].Children[1] == me);        // prints true
                Console.WriteLine(familyTree.Children[1].Children.Count);           // prints 1
                Console.WriteLine(familyTree.Children[1].Children[0] == cousin);    // prints true

                // Linked in tree now
                Console.WriteLine(grandpa.Children.Count);      // prints 2
                Console.WriteLine(dad.Parent == grandpa);       // prints true
                Console.WriteLine(me.Parent == dad);            // prints true
            }
        }
	}

## Full Documentation 

https://docs.knightmovesolutions.com/hierarchical/index.html




