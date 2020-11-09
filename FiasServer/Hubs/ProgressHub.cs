using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace FiasServer.Hubs
{
    public class ProgressHub : Hub
    {
        public async Task Progress(decimal progress, string session)
        {
            await Clients.All.SendAsync("Progress", progress, session);
        }
    }
}