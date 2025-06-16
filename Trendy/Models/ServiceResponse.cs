namespace Trendy.Models
{

    /// <summary>
    /// Standard response package for service operations (Create, Update, Delete).
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// Enum representing service operation results.
        /// </summary>
        public enum ServiceStatus { Found, NotFound, Created, Updated, Deleted, Error}

        /// <summary>
        /// the result status of the operation
        /// </summary>
        public ServiceStatus Status { get; set; }

        /// <summary>
        /// the id of the newly created or modified entity
        /// </summary>
        public int CreatedId { get; set; }

        /// <summary>
        /// messages for success or error handling
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the topic data associated with the current context.
        /// </summary>

        /// <summary>
        /// Holds a single topic's data, used in find-by-ID responses.
        /// </summary>
        public TopicDto TopicData { get; set; }

        /// <summary>
        /// Holds a list of topics,  used find all topics by categoryId.
        /// </summary>
        public List<TopicDto> TopicDataList { get; set; } = new List<TopicDto>();

        public CategoryDto CategoryData { get; set; }
    }

}

