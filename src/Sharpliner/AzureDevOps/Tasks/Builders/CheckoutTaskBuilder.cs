namespace Sharpliner.AzureDevOps.Tasks
{
    public class CheckoutTaskBuilder
    {
        public SelfCheckoutTask Self => new(null);

        public NoneCheckoutTask None => new(null);

        public RepositoryCheckoutTask Repository(string repository) => new(null, repository);

        public RepositoryCheckoutTask Repository(string displayName, string repository) => new(displayName, repository);

        internal CheckoutTaskBuilder()
        {
        }
    }
}
