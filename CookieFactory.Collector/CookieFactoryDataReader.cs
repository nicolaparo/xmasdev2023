using CookieFactory.Collector.Models;
using CookieFactory.Minecraft;

namespace CookieFactory.Collector
{
    public class CookieFactoryDataReader(MinecraftRconClient client)
    {
        private static IEnumerable<(int x, int y, int z)> GetOutputChestsPositions()
        {
            return Enumerable.Range(1012, 5).SelectMany(x =>
                Enumerable.Range(79, 2).SelectMany(y =>
                    Enumerable.Range(629, 2).Select(z =>
                        (x, y, z)
            )));
        }
        private static (int x, int y, int z) GetCookieCrafterPosition()
        {
            return (1009, 72, 628);
        }
        private static (int x, int y, int z) GetHayBaleCrafterPosition()
        {
            return (1009, 75, 634);
        }
        private static IEnumerable<(int x, int y, int z)> GetCocoaBeansChestPositions()
        {
            yield return (1002, 76, 635);
            yield return (1003, 76, 635);
        }

        public async Task<IEnumerable<InventoryEntry>> GetOuputChestsContentAsync()
        {
            List<InventoryEntry> entries = new();

            foreach (var (x, y, z) in GetOutputChestsPositions())
                entries.AddRange(await client.GetChestItemsAsync(x, y, z));

            return entries;
        }
        public async Task<IEnumerable<InventoryEntry>> GetCocoaBeansChestContentAsync()
        {
            List<InventoryEntry> entries = new();

            foreach (var (x, y, z) in GetCocoaBeansChestPositions())
                entries.AddRange(await client.GetChestItemsAsync(x, y, z));

            return entries;
        }
        public async Task<IEnumerable<InventoryEntry>> GetCookieCrafterContentAsync()
        {
            var (x, y, z) = GetCookieCrafterPosition();
            return await client.GetChestItemsAsync(x, y, z);
        }
        public async Task<IEnumerable<InventoryEntry>> GetHayBaleCrafterContentAsync()
        {
            var (x, y, z) = GetHayBaleCrafterPosition();
            return await client.GetChestItemsAsync(x, y, z);
        }

    }
}
