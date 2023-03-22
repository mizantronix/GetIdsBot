using System.Text;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

var token = Environment.GetEnvironmentVariable("SECRET_TOKEN");

// start the bot
var bot = new TelegramBotClient(token);
var me = await bot.GetMeAsync();
using var cts = new CancellationTokenSource();
bot.StartReceiving(HandleUpdateAsync, PollingErrorHandler, null, cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
while (true)
{
    Thread.Sleep(300);
}
Console.ReadLine();

// stop the bot
cts.Cancel();


Task PollingErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine($"Exception while polling for updates: {ex}");
    return Task.CompletedTask;
}


async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    try
    {
        await (update.Type switch
        {
            UpdateType.Message => HandleMessage(bot, update.Message),
            UpdateType.InlineQuery => BotOnInlineQueryReceived(bot, update.InlineQuery!),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(bot, update.ChosenInlineResult!),
            _ => Task.CompletedTask
        });
    }
#pragma warning disable CA1031
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while handling {update.Type}: {ex}");
    }
#pragma warning restore CA1031
}

async Task HandleMessage(ITelegramBotClient bot, Message? msg)
{
    if (msg.ViaBot?.Id != me.Id || msg.Text != "Getting chat info...")
    {
        return;
    }

    var sb = new StringBuilder();
    sb.AppendLine($"Chat ID: {msg.Chat.Id}");
    sb.AppendLine($"Chat title: {msg.Chat.Title}");
    sb.AppendLine($"Chat type: {msg.Chat.Type}");
    sb.AppendLine($"Chat invite link: {msg.Chat.InviteLink}");

    await bot.SendTextMessageAsync(msg.Chat.Id, sb.ToString());
}

async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
{
    var results = new List<InlineQueryResult>
    {
        new InlineQueryResultArticle("0",
            "Get my id info",
            new InputTextMessageContent(GetIdInfo(inlineQuery))),
        new InlineQueryResultArticle("1",
            "Get chat info",
            new InputTextMessageContent("Getting chat info..."))
    };
    
    await botClient.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 2, isPersonal: true);
}

string GetIdInfo(InlineQuery inlineQuery)
{
    var sb = new StringBuilder();
    sb.AppendLine($"Your ID: {inlineQuery.From.Id}");
    sb.AppendLine($"Your Username: {inlineQuery.From.Username}");
    sb.AppendLine($"You are {inlineQuery.From.FirstName} {inlineQuery.From.LastName}");

    return sb.ToString();
}

Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
{
    if (uint.TryParse(chosenInlineResult.ResultId, out var index))
    {
        Console.WriteLine($"{DateTime.Now} User {chosenInlineResult.From} has used bot");
    }

    return Task.CompletedTask;
}
