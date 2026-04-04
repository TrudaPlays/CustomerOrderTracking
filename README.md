“How did EF Core simplify data access compared to manual SQL?
What trade-offs did you notice?”
EF Core simplify data access compared to manual SQL in that I could use more natural sounding code to query and add things to the database,
instead of having to write the SQL to C# variables and make the database tables manually. By using EFCore I was able to cut
down production time by not having to wade through the database-making procedures and having to figure out how to make my tables
and get it connected to my C# program. By having each table represented by classes and having one class specifically creating and
managing the database for me, it became much more simple.
The biggest trade-off I noticed was it was practically impossible to actually 'see' my tables, which was relatively easy when
using the classic local database setup of my previous assignments. Basically the only way I could look inside the database
was to run my application and go through my menu, which left the question open what if my program wasn't working quite correctly
and the true contents of the database were not being displayed?
Despite this, I would personally go with EF Core for projects like this, as it takes a great deal of headache out of 
creating and managing the database and makes it much more easier and intuitive by having it all be in C# classes and using
more native-feeling language to query the database.
