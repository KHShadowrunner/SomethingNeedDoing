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
internal class SendCommand : MacroCommand
{
    private static readonly Regex Regex = new(@"^/send\s+(?<name>.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    // Experimental Code
    private readonly int hold = 1;
    // End Code
    private readonly VirtualKey[] vkCodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendCommand"/> class.
    /// </summary>
    /// <param name="text">Original text.</param>
    /// <param name="vkCodes">VirtualKey codes.</param>
    /// <param name="wait">Wait value.</param>
    /// <param name="seconds">Hold value for keypress.</param>
    private SendCommand(string text, VirtualKey[] vkCodes, WaitModifier wait, int seconds)
        : base(text, wait)
    {
        this.vkCodes = vkCodes;
        // Experimental Code
        this.hold = seconds;
        // End Code
    }

    /// <summary>
    /// Parse the text as a command.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>A parsed command.</returns>
    public static SendCommand Parse(string text)
    {
        _ = WaitModifier.TryParse(ref text, out var waitModifier);

        // Experimental Code
        text = text.Trim();
        var seconds = 1;
        var firstSpace = text.IndexOf(' ');
        var moreSpace = text.LastIndexOf(' ');
        var textHold = text.Substring(moreSpace + 1);
        if (firstSpace != moreSpace && char.IsDigit(text[moreSpace + 1]))
        {
            // var textHold = text.Substring(moreSpace + 1);
            seconds = Convert.ToInt32(textHold);
            text = text.Substring(0, moreSpace);
            PluginLog.Debug($"Int value seconds = {seconds * 100}");
        }
        // End Code

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

        return new SendCommand(text, vkCodes, waitModifier, seconds);
    }

    /// <inheritdoc/>
    public async override Task Execute(ActiveMacro macro, CancellationToken token)
    {
        PluginLog.Debug($"Executing: {this.Text}");

        if (this.vkCodes.Length == 1)
        {
            Keyboard.Send(this.vkCodes[0], null, this.hold);
        }
        else
        {
            var key = this.vkCodes.Last();
            var mods = this.vkCodes.SkipLast(1);
            Keyboard.Send(key, mods);
        }

        await this.PerformWait(token);
    }
}
