using System.CommandLine;

namespace Didot.Cli;

public class RegisterExtensionCommand : Command
{
    public RegisterExtensionCommand(RegisterExtensionOptions options, RegisterExtensionCommandHandler handler)
        : base("register", "Register an extension in Didot's extension registry.")
    {
        Arguments.Add(options.Reference);
        Options.Add(options.Name);

        Validators.Add(result =>
        {
            var reference = result.GetValue(options.Reference);
            if (string.IsNullOrWhiteSpace(reference))
                result.AddError("The <reference> argument cannot be empty.");

            var nameResult = result.GetResult(options.Name);
            if (nameResult is not null && nameResult.Tokens.Count > 0)
            {
                var name = result.GetValue(options.Name);
                if (string.IsNullOrWhiteSpace(name))
                    result.AddError("The --name option cannot be empty when provided.");
            }
        });

        this.SetAction(parseResult =>
            handler.Execute(
                parseResult.GetValue(options.Reference)!,
                parseResult.GetValue(options.Name)
            )
        );
    }
}
