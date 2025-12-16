
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Compliance.Redaction;

//TODO : Switch here to repro fault
using Meeze.Classfications;

// nb: error occurs when the "Web" part here matches the root of the classification attribute namespace. 
namespace Meeze.Web.App;

public static partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddLogging(b => b.EnableRedaction())
            .AddRedaction(redactionBuilder =>
        {
            redactionBuilder.SetRedactor<Base64Redactor>(MyClassifications.Secret);
            redactionBuilder.SetRedactor<NullRedactor>(MyClassifications.Known);
            redactionBuilder.SetFallbackRedactor<Base64Redactor>();
        });
        var app = builder.Build();

        app.MapGet("/", (
                [FromServices] ILogger<Dummy> logger,
                [FromQuery] string? secret, 
                [FromQuery] string? known 
                ) =>
        {
            LogAction(logger, secret ?? "defaultSecret", known ?? "defaultKnown");
            return "OK";
        });

        app.Run();
    }
    
    [LoggerMessage(LogLevel.Information, "Action for {Secret},{Known}")]
    static partial void LogAction(ILogger<Dummy> logger, [Secret] string secret, [Known] string known);
    
    /*
       This builds to the below.  note "new Web.Classfications.SecretAttribute" not "new global::Web.Classfications.SecretAttribute"

       [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Gen.Logging", "10.1.0.0")]
       private static readonly Microsoft.Extensions.Compliance.Classification.DataClassification _Web_Classfications_SecretAttribute = new Web.Classfications.SecretAttribute().Classification;
       
       [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Gen.Logging", "10.1.0.0")]
       private static readonly Microsoft.Extensions.Compliance.Classification.DataClassification _Web_Classfications_KnownAttribute = new Web.Classfications.KnownAttribute().Classification;

     */
}

public class Dummy{}



