using Azure.Messaging.ServiceBus;
using System.Net.Mail;

Console.WriteLine("Hello, World! i ammmm");
var conn = "Endpoint=sb://irfestivalservicebusabd.servicebus.windows.net/;SharedAccessKeyName=listener;SharedAccessKey=ExE2PtKm9m1/yBokd8J27DvbWZ+KnlhSMO8KGTiyEEI=;EntityPath=mails";
var qeueName = "mails";
await using (var cl = new ServiceBusClient(conn))
{
    var proc = cl.CreateProcessor(qeueName, new ServiceBusProcessorOptions());

    proc.ProcessMessageAsync += MessageHandler;
    proc.ProcessErrorAsync += ErrorHandler;

    await proc.StartProcessingAsync();
    Console.WriteLine("wait and press any key");
    Console.ReadKey();

    Console.WriteLine("\n stopping rec");
    await proc.StopProcessingAsync();
    Console.WriteLine("stopped");

}

static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"mail to send : {body}");
    string _sender = "Abdelrahman.hazem@inetum-realdolmen.world";
    string _password = "dummy";

    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

    client.Port = 587;
    client.DeliveryMethod = SmtpDeliveryMethod.Network;
    client.UseDefaultCredentials = false;
    System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
    client.EnableSsl = true;
    client.Credentials = credentials;

    MailMessage message = new MailMessage(_sender, "sander.lacaeyse@inetum-realdolmen.world");
    message.Subject = "SMTP";
    message.Body = args.Message.Body.ToString();
    client.Send(message);
    await args.CompleteMessageAsync(args.Message);

}
static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());

    return Task.CompletedTask;

}