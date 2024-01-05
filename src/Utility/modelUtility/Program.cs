using modelUtility;

string[]? parameters;

#if DEBUG
    parameters = new string[3]
    {
        "CreateModelDocument","PurchaseOrder","Purchase Order to extract key information"
    };
#else
 parameters = Environment.GetCommandLineArgs();
#endif

if (parameters.Length ==  0)
    throw new Exception("parameters cannot be null");

string command = parameters[1];

Console.WriteLine($"Command: {command}");

var bootStrapper = Utility.CreateBoostrapInstance();

Operation? operation = Utility.ConvertStringToEnum(command);

if (operation == null)
    throw new Exception("Command invalid");

switch (operation)
{
    case Operation.CreateModelDocument:
        // Get ModelId and Description
        string modelId = parameters[2];
        string description = parameters[3];
        await bootStrapper.CreateModelDocument(modelId, description);
        break;
    default:
        break;
}
