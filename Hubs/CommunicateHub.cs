using Microsoft.AspNetCore.SignalR;

namespace ScalaxServer.Hubs;

public class CommunicateHub : Hub
{

    private static readonly string activeConsDataPath = Environment.CurrentDirectory + "/active_cons.txt";
    //private static readonly string activeConsData = File.ReadAllText(activeConsDataPath);


    public Task Init(string clientId, string conId, bool isAdmin)
    {

        string prevConsData = File.ReadAllText(activeConsDataPath);
        File.WriteAllText(activeConsDataPath, prevConsData + clientId + ':' + (isAdmin ? $"%{conId}%" : conId) + ';');

        return Task.CompletedTask;
    }




    // # ADMIN ONLY!

    public Task FetchActiveCons(string conId)
    {
        Console.Write("\n\n FETCHED DATA. \n\n");
        //string adminId = (File.ReadAllText(activeConsDataPath).Split(';').ToList().Find(conId => conId.Contains("-a-"))).Replace("-a-", "");
        string txt = File.ReadAllText(activeConsDataPath.Replace(";", "\n"));
        return Clients.Client(conId).SendAsync("GetActiveCons", txt);
    }

    public Task ExecCmdOnOne (string targetConId, string cmd)
    {
        Console.WriteLine("\n\n Sent command \n\n");
        return Clients.Client(targetConId).SendAsync("ExecCmdOnOne", cmd);
    }

    ////
    ///



    public Task SendExecCmdOnOneRes (string conId, string cmdResOutput)
    {
        Console.Write($"\n\n {cmdResOutput} \n\n");

        return Clients.Client(File.ReadAllText(activeConsDataPath).Split(';').ToList().Find(id => id.Contains('%')).Split(':').ElementAt(1).Replace("%", String.Empty)).SendAsync("GetExecCmdOnOneRes", $"{conId} :: \n {cmdResOutput} \n\n");
    }



    //public Task SendCmd(string sender, string cmd)
    //{
    //    return Clients.All.SendAsync("GetCmd", cmd);
    //}
}

