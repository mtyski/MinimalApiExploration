using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using Minimal.Model.Base;
using StackExchange.Redis;

namespace Minimal.Redis;

public class RedisRepository
{
    private readonly IConnectionMultiplexer multiplexer;

    public RedisRepository(IConnectionMultiplexer multiplexer)
    {
        this.multiplexer = multiplexer;
    }

    private static JsonSerializerOptions Options => BuildJsonSerializerOptions();

    public async Task<Result<TModel>> GetModelAsync<TAggregate, TModel>(long id)
        where TAggregate : BaseEntity
    {
        var item = await multiplexer.GetDatabase()
            .HashGetAsync(typeof(TAggregate).Name, id);

        return !item.IsNullOrEmpty ?
            Result.Ok(JsonSerializer.Deserialize<TModel>(item, Options)!) :
            Result.Fail("Not found");
    }

    public async Task<IReadOnlyCollection<TModel>> GetAllAsync<TAggregate, TModel>()
        where TAggregate : BaseEntity =>
        JsonSerializer.Deserialize<List<TModel>>(
                @$"[{string.Join(
                    ", ",
                    await multiplexer.GetDatabase()
                        .HashGetAllAsync(typeof(TAggregate).Name)
                        .ContinueWith(static t => t.Result.Select(static hv => hv.Value)))}]",
                Options)
            !.AsReadOnly();

    public async Task SaveModelAsync<TAggregate, TModel>(long id, TModel model)
        where TAggregate : BaseEntity =>
        await multiplexer.GetDatabase().HashSetAsync(
            typeof(TAggregate).Name,
            id,
            JsonSerializer.Serialize(model, Options));

    public async Task DeleteModelAsync<TAggregate>(long id)
        where TAggregate : BaseEntity =>
        await multiplexer.GetDatabase().HashDeleteAsync(
            typeof(TAggregate).Name,
            id);

    private static JsonSerializerOptions BuildJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.PropertyNameCaseInsensitive = true;
        return options;
    }
}