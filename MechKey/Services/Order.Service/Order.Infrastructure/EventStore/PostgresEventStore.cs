using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Order.Application.Interfaces;
using System.Data;

namespace Order.Infrastructure.EventStore
{
    public class PostgresEventStore : IEventStore
    {
        private readonly string _connectionString;

        public PostgresEventStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<object> events)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var tx = await connection.BeginTransactionAsync();

            foreach (var @event in events)
            {
                var eventType = @event.GetType().FullName!;
                var data = JsonConvert.SerializeObject(@event);

                var sql = @"INSERT INTO event_store (aggregate_id, event_type, data)
                        VALUES (@AggregateId, @EventType, @Data::jsonb)";
                await connection.ExecuteAsync(sql, new
                {
                    AggregateId = aggregateId,
                    EventType = eventType,
                    Data = data
                }, tx as IDbTransaction);
            }

            await tx.CommitAsync();
        }

        public async Task<List<object>> GetEventsAsync(Guid aggregateId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT event_type, data FROM event_store
                    WHERE aggregate_id = @AggregateId
                    ORDER BY id ASC";

            var rows = await connection.QueryAsync<(string EventType, string Data)>(sql, new { AggregateId = aggregateId });

            var events = new List<object>();
            foreach (var row in rows)
            {
                var type = GetEventType(row.EventType);
                if (type == null) continue;

                var @event = JsonConvert.DeserializeObject(row.Data, type);
                if (@event != null)
                    events.Add(@event);
            }

            return events;
        }

        private static Type? GetEventType(string typeName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
