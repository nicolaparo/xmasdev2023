using CookieFactory.Shared;

namespace CookieFactory.Collector
{
    public class CookieFactoryMonitorService(CookieFactoryDataReader reader, NotifierService notifier)
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private Task worker;

        public bool IsRunning { get; private set; }

        public async Task<bool> StartAsync()
        {
            IsRunning = true;
            worker = Task.Run(DoMonitorLoopAsync);
            return true;
        }

        public async Task StopAsync()
        {
            if (worker is not null && IsRunning && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();

                if (worker is not null)
                    await worker;

                worker = null;
                IsRunning = false;
            }
        }

        public async Task RunAsync()
        {
            await StartAsync();
            await worker;
        }

        private async Task DoMonitorLoopAsync()
        {
            while (IsRunning && !cancellationTokenSource.IsCancellationRequested)
            {
                await DoMonitorAsync();
                await Task.Delay(TimeSpan.FromSeconds(.5), cancellationTokenSource.Token);
            }
        }

        private const string Cookie = "minecraft:cookie";
        private const string Wheat = "minecraft:wheat";
        private const string CocoaBeans = "minecraft:cocoa_beans";

        private int previousCookiesCount;
        private int previousCocoaBeansCount;
        private bool invalidCookieRecipeSent;
        private int productionHaltedCooldown = -1;
        private int producedCookies = 0;
        private bool firstLoop = true;

        private async Task DoMonitorAsync()
        {
            var cookiesCount = (await reader.GetOuputChestsContentAsync())
                .Where(x => x.ItemId is Cookie)
                .Sum(x => x.Count);

            var cocoaBeansCount = (await reader.GetCocoaBeansChestContentAsync())
                .Where(x => x.ItemId is CocoaBeans)
                .Sum(x => x.Count);

            if (!firstLoop)
            {
                var currentCookieRecipe = (await reader.GetCookieCrafterContentAsync()).Select(x => x.ItemId).ToArray();

                if (cookiesCount == previousCookiesCount && productionHaltedCooldown >= 0)
                {
                    if (productionHaltedCooldown == 0)
                        notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Warning, "cookie.halted", $"Cookie production halted", new { ProducedCookies = producedCookies }));

                    productionHaltedCooldown--;
                }

                if (previousCocoaBeansCount < cocoaBeansCount)
                {
                    notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Success, "cocoa.load", $"Loaded {cocoaBeansCount} Cocoa Beans", new { LoadedCocoaBeans = cocoaBeansCount }));
                }

                if (cocoaBeansCount <= 0 && previousCocoaBeansCount > 0)
                    notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Warning, "cocoa.empty", $"Cocoa Beans' chest is empty", new { }));

                if (currentCookieRecipe.Zip(new[] { Wheat, CocoaBeans, Wheat }).Any(x => x.First != x.Second))
                {
                    if (!invalidCookieRecipeSent)
                    {
                        notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Error, "cookie.recipe", $"Cookie recipe is not correct!", new { Recipe = currentCookieRecipe }));
                        invalidCookieRecipeSent = true;
                    }
                }
                else
                {
                    if (invalidCookieRecipeSent)
                    {
                        notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Success, "cookie.recipe", $"Cookie recipe was fixed", new { }));
                        invalidCookieRecipeSent = false;
                    }
                }

                if (previousCookiesCount < cookiesCount)
                {
                    if (productionHaltedCooldown < 0)
                    {
                        notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Success, "cookie.started", $"Cookie production started", new { }));
                        producedCookies = 0;
                    }

                    producedCookies += cookiesCount - previousCookiesCount;

                    productionHaltedCooldown = 3;
                }

                if (cookiesCount < previousCookiesCount)
                    notifier.NotifyAsync(new CookieFactoryEvent(CookieFactoryEventSeverity.Warning, "cookie.withdrawn", $"Withdrawn {previousCookiesCount - cookiesCount} Cookies", new { WithdrawnCookies = previousCookiesCount - cookiesCount }));

            }

            firstLoop = false;

            previousCookiesCount = cookiesCount;
            previousCocoaBeansCount = cocoaBeansCount;
        }
    }
}
