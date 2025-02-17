using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API
{
    public class ChatHub: Hub
    {
        public async Task JoinChat(object createdOrder) // can use object createdOrder - dynamic ko dc (OrderDto here instead of object if pass raw object from client - JSON.parse)
        {
            //await Clients.All
            //    .SendAsync("ReceiveMessage", "admin", $"{userName} has joined");

            //var json = JsonConvert.SerializeObject(createdOrder);
            await Clients.All
                  .SendAsync("ReceiveMessage", "admin", createdOrder);
        }
    }
}
