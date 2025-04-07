// See https://aka.ms/new-console-template for more information
using carenirvana.bre;

Console.WriteLine("Hello, World!");

var startTime = DateTime.Now;
Console.WriteLine($"start time: {startTime}");
var rEngine = new RuleEngine(File.ReadAllText(@"D:\repos\carenirvana-bre\src\carenirvana.bre.testapp\BREConfigDataEx.json"));
// rEngine.ExecuteRules("");
var output = rEngine.ExecuteRule("CouldBeDiabetic", [null, DateTime.Parse("April-22-1979"), 110]);
Console.WriteLine($"run completed...");
var endTime = DateTime.Now;
Console.WriteLine($"end time: {endTime}");
Console.WriteLine($"Time taken to process : {new TimeSpan(DateTime.Now.Ticks - startTime.Ticks)}");

Console.ReadLine();

// 1 mil (input table)
// batches
// read (threads) 1st batch, 2nd batch, 3rd batch
// rule transformation -- threads
// 

// member id : 1 CouldBeDiabetic : true  -- output (add activity into a table)
// member id : 2 CouldBeDiabetic : false - no action