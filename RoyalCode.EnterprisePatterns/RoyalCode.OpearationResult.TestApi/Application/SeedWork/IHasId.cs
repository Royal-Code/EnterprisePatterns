namespace RoyalCode.OpearationResult.TestApi.Application.SeedWork;

public interface IHasId<out TId>
{
    TId Id { get; }
}
