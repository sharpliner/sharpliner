namespace Sharpliner.AzureDevOps.Tasks;

public class CheckoutTaskBuilder
{
    public SelfCheckoutTask Self => new();

    public NoneCheckoutTask None => new();

    public RepositoryCheckoutTask Repository(string repository) => new(repository);

    internal CheckoutTaskBuilder()
    {
    }
}
