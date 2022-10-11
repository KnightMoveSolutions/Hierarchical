# KnightMoves.Pipelines

The Pipelines library is a framework for decomposing and executing processes (i.e. PipelineOperations) 
that are highly dependent on each other and which must be executed in a particular order.

## License

MIT License

## What problem does it solve?

This framework simplifies the creation of procedural code that is made up of many steps that involve a 
combination of reaching out to other APIs, pulling data from a database, saving data to a database, calculations, 
validation logic, sorting/grouping, data transformations, and crafting of the output format.

Without a framework like this, inevitably the code becomes a monolith with a mix of steps executing the 
types of logic above, exception handling throughout, logging interweaved with the business logic, if/else 
blocks, async operations, and early returns.

This framework makes it easy to separate (i.e. decompose) portions of logic (i.e. steps in the procedure) 
into their own classes so that each step adheres to the Single Responsibility Principle and can be 
independently managed.

Full documentation at the URL below

## Documentation

https://docs.knightmovesolutions.com/pipelines/index.html
