
Ario API Design Document and Lessons Learned

Developed using C# and ASP.NET with Visual Studio 2017 for Mac
Implements Microsoft Entity Framework (EF) v2.2

Published to Microsoft Azure at: 
http://arioplatform.azurewebsites.net/api/

TODO

- polish security and authentication protocols (Okta)
- document all code and develop Github Wiki
- add sanity checking for appropriate data entry
- add node visibility via teams (using NodeTeamJoin table)
- QA test all work

KNOWN BUG

- querying database for numerical fields equal to 0 will have unexpected results


CONSTUCT TYPES

MODELS

- C# class representation of SQL database table
- public fields in class represent columns in the data table (with appropriate data types)
	- column and class field should be named identically for ease of use
- fields initialized with { get; set; } to allow full control
- “display” models were created in order to create a distinction between the raw data in the table and the data that the user sees after a GET call

MODELS.CONVERTERS

- takes a JSON string and forms it to an object class
- required for object classes containing abstract data types

CONTEXTS

- Microsoft Entity Framework (EF) allows a list of services to be configured at the beginning of runtime in the Startup.cs script
- A context is the link between a SQL database table and an ASP.NET construct
- Contexts are added using a default connection string (located in appsettings.json)
- each context contains an IEnumerable DbSet<~> data structure which contains pointers to the live SQL table
- By default, Microsoft EF declares that each table must have a PRIMARY KEY index (highly recommended for best practices) and that key by default is named “ID”
	- PRIMARY KEY name can be overridden using override void OnModelCreating(ModelBuilder modelBuilder)

CONTROLLERS

- Each controller directly corresponds to an API call suffix using the class-level attribute [Route(“api/[controller]”)]
- IMPORTANT: the suffix that replaces “[controller]” in the HTML query is equal to the name of the controller class minus the end “Controller”
	- ex/ to call the ShowNodesController API call, you would type “http://[yourwebsitehere]/api/shownodes”
- Each method in the controller class must have an associated attribute corresponding to the desired REST command (e.g. HttpGet, HttpPost, HttpDelete, etc.)
- In order to put additional suffixes in the query string, add the suffix string as a parameter to the attribute
	- ex/ the attribute [HttpGet(“all”)] will be called with the query “http://[yourwebsitehere]/api/[controller]/all”
- variable suffixes can be placed using {}, which contains a string that matches the variable name of a function parameter
	- ex/ [HttpGet(“{id}”)] public IActionResult GetById(int id) {} is called by “http://[yourwebsitehere]/api/[controller]/{id}” where {id} is the integer input into GetById
- controller classes call repositories to carry out the internal code of the API call (e.g. GetAll(), Create(), Update(), etc.)
- GET calls should return an IEnumerable data type, which will be automatically converted to a JSON string by Microsoft EF
- all other calls should return an appropriate IActionResult 

REPOSITORIES

- Repos carry out internal logic for the API calls
- They read from or manipulate context variables, which are linked to the live SQL databases
- DbSets can be converted into List structures temporarily for ease of use
- Repos are linked to interfaces for data integrity
- Each repo must be matched to its corresponding interface at Startup.cs explicitly as a singleton



