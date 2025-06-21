using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trendy.Data;
using Trendy.Interfaces;
using Trendy.Models;

namespace Trendy.Services
{
    public class TopicService: ITopicService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // <summary>
        /// Initializes a new instance of the TopicService class.
        /// </summary>
        /// <param name="context">The database context used for accessing topic data.</param>
        public TopicService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves a list of all topics, including associated categories.
        /// </summary>
        /// <returns>
        /// An asynchronous task that returns a collection of TopicDto objects.
        /// </returns>
        public async Task<IEnumerable<TopicDto>> ListTopics()
        {
            var topics = await _context.Topics
         .Include(t => t.CategoryTopics)
         .ThenInclude(ct => ct.Category)
         .Include(t=> t.Comments)
         .ToListAsync();

            var topicDtos = new List<TopicDto>();

            //get the admin user to display created by admin
            foreach (var topic in topics)
            {
                string? createdBy = null;

                if (!string.IsNullOrEmpty(topic.UserId))
                {
                    var user = await _userManager.FindByIdAsync(topic.UserId);
                    createdBy = user?.UserName; // or user?.Email
                }

                //get the user who made the comment
                var commentDtos = new List<CommentDto>();
                foreach (var comment in topic.Comments)
                {
                    string userName = "Anonymous";
                    if (!string.IsNullOrEmpty(comment.UserId))
                    {
                        var commentUser = await _userManager.FindByIdAsync(comment.UserId);
                        userName = commentUser?.UserName ?? "Anonymous";
                    }

                    commentDtos.Add(new CommentDto
                    {
                        CommentId = comment.CommentId,
                        CommentText = comment.CommentText,
                        CreatedAt = comment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserName = userName
                    });
                }


                topicDtos.Add(new TopicDto
                {
                    TopicId = topic.TopicId,
                    TopicTitle = topic.TopicTitle,
                    TopicDescription = topic.TopicDescription,
                    TopicCategory = topic.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),
                    CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedBy = createdBy,
                    Comments = commentDtos
                });
            }

            return topicDtos;
        }




        /// <summary>
        /// Retrieves a specific topic by its id, including category details.
        /// </summary>
        /// <param name="topicId">The ID of the topic to retrieve.</param>
        /// <returns>
        /// A ServiceResponse object containing retrieval status and messages.
        /// </returns>
        /// <remarks>
        /// If a topic is found, the response status will be 'Found'.
        /// If the topic does not exist, the response status will be 'NotFound'.
        /// </remarks>

        public async Task<ServiceResponse> GetTopicById(int topicId)
        {
            var response = new ServiceResponse();

            var topic = await _context.Topics
                .Include(t => t.CategoryTopics)
                .ThenInclude(ct =>ct.Category)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.TopicId == topicId);

            // if no topic found
            if (topic == null) {

                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"No topic found with ID {topicId}");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Found;
            response.Messages.Add("Topic retrieved successfully.");

            //get the topic creator username(admin)
            string? createdBy = null;
            if (!string.IsNullOrEmpty(topic.UserId))
            {
                var user = await _userManager.FindByIdAsync(topic.UserId);
                createdBy = user?.UserName;
            }

            //get comments usernames
            var commentDtos = new List<CommentDto>();
            foreach (var comment in topic.Comments)
            {
                string userName = "Anonymous";
                if (!string.IsNullOrEmpty(comment.UserId))
                {
                    var commentUser = await _userManager.FindByIdAsync(comment.UserId);
                    userName = commentUser?.UserName ?? "Anonymous";
                }

                commentDtos.Add(new CommentDto
                {
                    CommentId = comment.CommentId,
                    CommentText = comment.CommentText,
                    CreatedAt = comment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserName = userName
                });
            }

                //store topic details inside the response
                response.TopicData = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicTitle = topic.TopicTitle,
                TopicDescription = topic.TopicDescription,
                TopicCategory = topic.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),                  
                CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                CreatedBy = createdBy,
                Comments = commentDtos
                };

            return response;

           
        }
        /// <summary>
        /// Retrieves all topics associated with a given category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category for which topics should be retrieved.</param>
        /// <returns>
        /// A ServiceResponse containing a list of topics if found, or a NotFound status if no topics exist.
        /// </returns>
        
        public async Task<ServiceResponse> ListTopicsByCategoryId(int categoryId)
        {
            var response = new ServiceResponse();

            var topics = await _context.CategoryTopics
                 .Include(ct => ct.Topic) // Includes the related Topic entity from the CategoryTopics bridge table.
                 .ThenInclude(t => t.CategoryTopics) // Also include the list of all category links for that topic
                 .ThenInclude(ct => ct.Category)  // And include the Category details for those links
                 .Where(ct => ct.CategoryId == categoryId) // Filters the CategoryTopics table to only retrieve records that match the provided categoryId.
                 .Select(ct => ct.Topic) // Extracts only the Topic entity from the filtered CategoryTopics entries.

                 .ToListAsync(); // Executes the query asynchronously and converts the results into a list.



            //if no topics found

            if (!topics.Any())
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"No topics found for this category id {categoryId}");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Found;
            response.Messages.Add($"Found list of topics for this category {categoryId}");

            //Convert topics into TopicDto

            response.TopicDataList = topics.Select(topic => new TopicDto
            {
                TopicId = topic.TopicId,
                TopicTitle = topic.TopicTitle,
                TopicDescription = topic.TopicDescription,
                TopicCategory = topic.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),
                CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),

            }).ToList();
           

            return response;
        }


        /// <summary>
        /// Adds a new topic to the database.
        /// </summary>
        /// <param name="createTopicDto">The DTO containing topic details.</param>
        /// <param name="userId">The ID of the user creating the topic.</param>
        /// <returns>
        /// A ServiceResponse object indicating the result of the operation.
        /// </returns>

        public async Task<ServiceResponse> AddNewTopic(CreateTopicDto createTopicDto, string userId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();


            //create instance of topic

            Topic topic = new Topic()
            {
                TopicTitle = createTopicDto.TopicTitle,
                TopicDescription = createTopicDto.TopicDescription,
                CreatedAt = createTopicDto.CreatedAt,
                UserId = userId // passed in from controller
                

            };
            // Loop through each inserted category ID by the admin to create links
            foreach (int categoryId in createTopicDto.CategoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId); // Look up the category in the categories table
                if(category != null)
                {
                    //if found, create a link between topic and category
                    topic.CategoryTopics.Add(new CategoryTopic
                    {
                        //For this topic, I want to associate it with this category
                        Topic = topic, //link to current topic
                        Category = category
                    });
                }
                else
                {
                    // If category not found, add message to response
                    serviceResponse.Messages.Add($"Category with ID {categoryId} not found.");
                }
            }

                try
                {
                    _context.Topics.Add(topic); // Add topic to DbContext
                     await _context.SaveChangesAsync(); // Save all changes to database

                
                }
                catch (Exception ex)
                {
                // Handle any errors and return early
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                    serviceResponse.Messages.Add("There was an error adding the topic.");
                    serviceResponse.Messages.Add(ex.Message);

                    return serviceResponse; // Return early to prevent further execution
                }
            // Lookup username from UserId
            var user = await _userManager.FindByIdAsync(userId);
            string username = user?.UserName ?? "Unknown";

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.Messages.Add("Topic created successfully.");
            serviceResponse.CreatedId = topic.TopicId;

            //to include the created topic in the response
            serviceResponse.TopicData = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicTitle = topic.TopicTitle,
                TopicDescription = topic.TopicDescription,
                TopicCategory = topic.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),
                CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                CreatedBy = username
            }; 
            return serviceResponse;

        }

        /// <summary>
        /// Updates an existing topic in the database.
        /// </summary>
        /// <param name="updateTopicDto">A DTO containing the updated topic details, including the topic ID, title, description, and category IDs.</param>
        /// <returns>
        /// A ServiceResponse indicating the result of the update operation:
        /// - 'Found' if the topic was successfully updated.
        /// - 'NotFound' if the topic with the given ID does not exist.
        /// - 'Error' if an exception occurred during the update process.
        /// </returns>
        /// <remarks>
        /// This method replaces the topic's title, description, and category links with the values provided in the DTO.
        /// </remarks>


        public async Task<ServiceResponse> UpdateTopic(UpdateTopicDto updateTopicDto)
        {
            ServiceResponse response = new ServiceResponse();

            //Step1: find the existing topic
            var topic = await _context.Topics
                .Include(t => t.CategoryTopics)
                .FirstOrDefaultAsync(t => t.TopicId == updateTopicDto.TopicId);
            if(topic == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"No topic found with ID {updateTopicDto.TopicId}");
                return response;

            }

            //step 2: update title and description

            topic.TopicTitle = updateTopicDto.TopicTitle;
            topic.TopicDescription = updateTopicDto.TopicDescription;

            //step 3 : remove existing category links
            topic.CategoryTopics.Clear();

            //step 4: add new category links

            foreach (int categoryId in updateTopicDto.CategoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if(category != null)
                {
                    topic.CategoryTopics.Add(new CategoryTopic
                    {
                        Topic = topic,
                        Category = category

                    });

                }
                else
                {
                    response.Messages.Add($"Category with ID {categoryId} not found.");
                }
            }

            //step 5: save changes to db

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add("Topic updated successfully.");
                response.CreatedId = topic.TopicId;

                // Populate the updated topic info in the response
                response.TopicData = new TopicDto
                {
                    TopicId = topic.TopicId,
                    TopicTitle = topic.TopicTitle,
                    TopicDescription = topic.TopicDescription,
                    TopicCategory = topic.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),
                    CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("There was an error updating the topic.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }


        /// <summary>
        /// Deletes a topic and its associated category links from the database.
        /// </summary>
        /// <param name="topicId">The ID of the topic to delete.</param>
        /// <returns>
        /// A ServiceResponse object:
        /// - Status 'Deleted' if deletion succeeds.
        /// - Status 'NotFound' if the topic doesn't exist.
        /// - Status 'Error' if deletion fails.
        /// </returns>
        public async Task<ServiceResponse> DeleteTopic(int topicId)
        {
            ServiceResponse response = new ServiceResponse();

            var topic = await _context.Topics
                .Include(t => t.CategoryTopics).FirstOrDefaultAsync(t => t.TopicId == topicId);

            if(topic == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"No topic found with ID {topicId}");
                return response;
            }

            try
            {
                 //remove category links first
                _context.CategoryTopics.RemoveRange(topic.CategoryTopics);

                //remove the topic

                _context.Topics.Remove(topic);

                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
                response.Messages.Add("Topic deleted successfully.");


            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("There was an error deleting the topic.");
                response.Messages.Add(ex.Message);
            }

            return response;

        }

        /// <summary>
        /// Searches for topics by keyword in the title or description.
        /// </summary>
        /// <param name="keyword">The keyword to search for in topic titles and descriptions.</param>
        /// <returns>
        /// A list of TopicDto objects that match the keyword.
        /// </returns>
        public async Task<IEnumerable<TopicDto>> SearchTopicsByKeyword(string keyword)
        {
            var topics = await _context.Topics
                .Include(t => t.CategoryTopics)
                .ThenInclude(ct => ct.Category)
                .Where(t => t.TopicTitle.Contains(keyword) || t.TopicDescription.Contains(keyword))
                .ToListAsync();

            //map the topics to topicDto for output

            return topics.Select(t => new TopicDto
            {
                TopicId = t.TopicId,
                TopicTitle = t.TopicTitle,
                TopicDescription = t.TopicDescription,
                TopicCategory = t.CategoryTopics.Select(ct => ct.Category.CategoryName).ToList(),
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}
