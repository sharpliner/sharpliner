using System;

namespace Sharpliner.GitHubActions
{
    /// <summary>
    /// Represents the service container that will be used for the job
    /// </summary>
    public record Service
    {
        /// <summary>
        /// The service with the given Id will be used for the the job.
        /// </summary>
        /// <param name="id">Id of the service.</param>
        /// <exception cref="ArgumentNullException">Raised if the service Id is null or empty.</exception>
        public Service(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException((nameof(id)));
            Id = id;
        }

        /// <summary>
        /// The service with the given Id and container will be used for the job.
        /// </summary>
        /// <param name="id">Id of the service.</param>
        /// <param name="container">Container of the service.</param>
        public Service(string id, Container container) : this(id)
        {
            Container = container;
        }

        /// <summary>
        /// Get the Id of the service.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get/Set the container of the service.
        /// </summary>
        public Container Container { get; set; }
    }
}
