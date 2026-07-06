using InventoryManagement.Application.InventoryItems.Processing;
using InventoryManagement.Domain.Abstractions;
using InventoryManagement.Domain.InventoryItems;

namespace InventoryManagement.Application.InventoryItems.Orchestration;

public class InventoryItemOrchestrationService
{
    private readonly InventoryItemProcessingService processingService;
    private readonly IUnitOfWork unitOfWork;

    public InventoryItemOrchestrationService(
        InventoryItemProcessingService processingService,
        IUnitOfWork unitOfWork)
    {
        this.processingService = processingService;
        this.unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<InventoryItemEntity>> ListAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        return processingService.ListAsync(search, cancellationToken);
    }

    public Task<InventoryItemEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return processingService.GetByIdAsync(id, cancellationToken);
    }

    public Task<InventoryItemEntity> CreateAsync(
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        return ExecuteInTransactionAsync(
            () => processingService.CreateAsync(
                name,
                sku,
                quantity,
                cancellationToken),
            cancellationToken);
    }

    public Task<InventoryItemEntity?> UpdateAsync(
        Guid id,
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        return ExecuteInTransactionAsync(
            () => processingService.UpdateAsync(
                id,
                name,
                sku,
                quantity,
                cancellationToken),
            cancellationToken);
    }

    public Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return ExecuteInTransactionAsync(
            () => processingService.DeleteAsync(id, cancellationToken),
            cancellationToken);
    }

    private async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await operation();

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
