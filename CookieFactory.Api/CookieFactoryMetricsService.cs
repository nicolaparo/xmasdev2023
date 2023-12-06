using CookieFactory.Shared;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookieFactory.Api
{
    public static class CookieFactoryMetricsEndpoints
    {
        public static void MapCookieFactoryEndpoints(this WebApplication app)
        {
            app.MapGet("/metrics", async (CookieFactoryMetricsService metricsService, DateTimeOffset? fromDate, DateTimeOffset? toDate) =>
            {
                var fromDateValue = fromDate ?? DateTimeOffset.UtcNow.AddHours(-1);

                var events = await metricsService.GetEventsAsync(fromDateValue, toDate);

                return events;
            });
            app.MapGet("/cookies", async (CookieFactoryMetricsService metricsService) =>
            {
                var producedCookies = await metricsService.GetProducedCookiesAsync();

                return producedCookies;
            });
        }
    }

    public class CookieFactoryMetricsService(SqlConnection connection)
    {
        public async Task<IEnumerable<CookieFactoryEvent>> GetEventsAsync(DateTimeOffset fromDate, DateTimeOffset? toDate = null)
        {
            var from = fromDate.UtcDateTime;
            var to = toDate?.UtcDateTime ?? DateTime.UtcNow;

            var query = """
                SELECT [Timestamp], [Type], [Severity], [Message], [Data]
                FROM [dbo].[CookieFactoryEvents]
                WHERE [Timestamp] >= @from AND [Timestamp] <= @to
                ORDER BY [Timestamp] DESC
                """;

            return await connection.QueryAsync<CookieFactoryEvent>(query, new { from, to });
        }

        public async Task<IEnumerable<CookieFactoryEvent>> GetEventsBySeverityAsync(CookieFactoryEventSeverity severity)
        {
            var query = """
                SELECT [Timestamp], [Type], [Severity], [Message], [Data]
                FROM [dbo].[CookieFactoryEvents]
                WHERE [Severity] = @severity
                ORDER BY [Timestamp] DESC
                """;

            return await connection.QueryAsync<CookieFactoryEvent>(query, new { severity });
        }

        public async Task<int> GetProducedCookiesAsync()
        {
            var query = """
                select SUM(CONVERT(int, json_value(data, '$.ProducedCookies')))
                from CookieFactoryEvents
                Where Type = 'cookie.halted' and Data is not null
                """;

            return await connection.ExecuteScalarAsync<int>(query);
        }
    }
}
