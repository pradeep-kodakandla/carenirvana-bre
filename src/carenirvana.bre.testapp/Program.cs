// See https://aka.ms/new-console-template for more information
using carenirvana.bre;

Console.WriteLine("Hello, World!");

var rEngine = new RuleEngine(File.ReadAllText(@"D:\repos\carenirvana-bre\src\carenirvana.bre.testapp\BREConfigData.json"));
rEngine.ExecuteRules("");
Console.WriteLine($"run completed...");
Console.ReadLine();

// 1 mil (input table)
// batches
// read (threads) 1st batch, 2nd batch, 3rd batch
// rule transformation -- threads
// 

// member id : 1 CouldBeDiabetic : true  -- output (add activity into a table)
// member id : 2 CouldBeDiabetic : false - no action