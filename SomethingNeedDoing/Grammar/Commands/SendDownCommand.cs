using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;
using SomethingNeedDoing.Exceptions;
using SomethingNeedDoing.Grammar.Modifiers;
using SomethingNeedDoing.Misc;

namespace SomethingNeedDoing.Grammar.Commands;

/// <summary>
/// The /send command.
/// </summary>
internal class SendDownCommand : MacroCommand
{
    private static readonly Regex Regex = new(@"^/senddown\s+(?<name>.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly VirtualKey[] vkCodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendDownCommand"/> class.
    /// </summary>
    /// <param name="text">Original text.</param>
    /// <param name="vkCodes">VirtualKey codes.</param>
    /// <param name="wait">Wait value.</param>
    private SendDownCommand(string text, VirtualKey[] vkCodes, WaitModifier wait)
        : base(text, wait)
    {
        this.vkCodes = vkCodes;
    }

    /// <summary>
    /// Parse the text as a command.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>A parsed command.</returns>
    public static SendDownCommand Parse(string text)
    {
        _ = WaitModifier.TryParse(ref text, out var waitModifier);
        var match = Regex.Match(text);
        if (!match.Success)
            throw new MacroSyntaxError(text);

        var nameValue = ExtractAndUnquote(match, "name");
        var vkCodes = nameValue.Split("+")
            .Select(name =>
            {
                if (!Enum.TryParse<VirtualKey>(name, true, out var vkCode))
                    throw new MacroCommandError("Invalid virtual key");

                return vkCode;
            })
            .ToArray();

        return new SendDownCommand(text, vkCodes, waitModifier);
    }

    /// <inheritdoc/>
    public async override Task Execute(ActiveMacro macro, CancellationToken token)
    {
        PluginLog.Debug($"Executing: {this.Text}");

        if (this.vkCodes.Length == 1)
        {
            Keyboard.SendDown(this.vkCodes[0], null);
        }
        else
        {
            var key = this.vkCodes.Last();
            var mods = this.vkCodes.SkipLast(1);
            Keyboard.SendDown(key, mods);
        }

        await this.PerformWait(token);
    }
}
