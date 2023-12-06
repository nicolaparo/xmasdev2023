using CookieFactory.Minecraft;
using CookieFactory.Minecraft.Nbt.Tags;
using CookieFactory.Minecraft.Nbt;
using CookieFactory.Collector.Models;
using System.Text.Json;

namespace CookieFactory.Collector
{
    static class MinecraftCommands
    {
        private static async Task<NbtTag> ExecuteDataGetCommandAsync(this MinecraftRconClient client, string command)
        {
            var result = await client.SendCommandAsync(command.Trim());
            var resultParts = result.Message.Body.Split(": ", 2);

            if (resultParts is [_, var snbt])
                return NbtSerializer.Deserialize(snbt);

            throw new Exception(result.Message.Body);
        }

        public static async Task<NbtTag> DataGetEntityAsync(this MinecraftRconClient client, string entitySelector, string entityPath = null)
        {
            return await client.ExecuteDataGetCommandAsync($"data get entity {entitySelector} {entityPath}");
        }
        public static async Task<TResult> DataGetEntityAsync<TResult>(this MinecraftRconClient client, string entitySelector, string entityPath = null)
        {
            var result = await client.DataGetEntityAsync(entitySelector, entityPath);
            return result.ToJson().Deserialize<TResult>();
        }
        public static async Task<InventoryEntry[]> GetEntityInventoryAsync(this MinecraftRconClient client, string entitySelector)
        {
            return await client.DataGetEntityAsync<InventoryEntry[]>(entitySelector, "Inventory");
        }

        public static async Task<NbtTag> DataGetBlockAsync(this MinecraftRconClient client, int x, int y, int z, string entityPath = null)
        {
            return await client.ExecuteDataGetCommandAsync($"data get block {x} {y} {z} {entityPath}");
        }
        public static async Task<TResult> DataGetBlockAsync<TResult>(this MinecraftRconClient client, int x, int y, int z, string entityPath = null)
        {
            var result = await client.DataGetBlockAsync(x, y, z, entityPath);
            return result.ToJson().Deserialize<TResult>();
        }
        public static async Task<InventoryEntry[]> GetChestItemsAsync(this MinecraftRconClient client, int x, int y, int z)
        {
            return await client.DataGetBlockAsync<InventoryEntry[]>(x, y, z, "Items");
        }
    }
}
