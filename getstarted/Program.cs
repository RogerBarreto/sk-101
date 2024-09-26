using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json.Serialization;


#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var api = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["ApiKey"]!;

var builder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", api);

builder.Plugins.AddFromType<TimeInformation>();
builder.Plugins.AddFromType<WidgetFactory>();

var kernel = builder.Build();
var settings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

Console.WriteLine(await kernel.InvokePromptAsync("Create a handy lime colored widget for me.", new(settings)));
Console.WriteLine(await kernel.InvokePromptAsync("Create a beautiful scarlet colored widget for me.", new(settings)));
Console.WriteLine(await kernel.InvokePromptAsync("Create an attractive maroon and navy colored widget for me.", new(settings)));

//Console.WriteLine(await kernel.InvokePromptAsync("How many days until Christmas?", new KernelArguments(openAISettings)));

//var result = await kernel.InvokePromptAsync("Hello!");

//var result = await kernel.InvokePromptAsync("What color is the {{$topic}}?", arguments);

//Console.WriteLine(result);
/*var openAISettings = new OpenAIPromptExecutionSettings { ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonObjectFormat(), MaxTokens = 50 };

KernelArguments arguments = new(openAISettings) { { "topic", "sea" } };

await foreach (var chunk in kernel.InvokePromptStreamingAsync(
    "What color is the {{$topic}}?", arguments))
{
    Console.Write(chunk);
}*/

public class TimeInformation
{
    [KernelFunction, Description("Returns the current time in UTC")]
    public string GetCurrentTime() 
        => DateTime.Now.ToString("R");
}


public class WidgetFactory
{
    [KernelFunction]
    [Description("Creates a new widget of the specified type and colors")]
    public WidgetDetails CreateWidget(
        [Description("The type of widget to be created")] WidgetType widgetType, 
        [Description("The colors of the widget to be created")] WidgetColor[] widgetColors)
    {
        var colors = string.Join('-', widgetColors.Select(c => c.GetDisplayName()).ToArray());
        return new()
        {
            SerialNumber = $"{widgetType}-{colors}-{Guid.NewGuid()}",
            Type = widgetType,
            Colors = widgetColors
        };
    }
}

 /// <summary>
    /// A <see cref="JsonConverter"/> is required to correctly convert enum values.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WidgetType
    {
        [Description("A widget that is useful.")]
        Useful,

        [Description("A widget that is decorative.")]
        Decorative
    }

    /// <summary>
    /// A <see cref="JsonConverter"/> is required to correctly convert enum values.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WidgetColor
    {
        [Description("Use when creating a red item.")]
        Red,

        [Description("Use when creating a green item.")]
        Green,

        [Description("Use when creating a blue item.")]
        Blue
    }

    public class WidgetDetails
    {
        public string SerialNumber { get; init; }
        public WidgetType Type { get; init; }
        public WidgetColor[] Colors { get; init; }
    }
