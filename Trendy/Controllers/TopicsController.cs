using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trendy.Interfaces;
using Trendy.Models;

namespace Trendy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        /// <summary>
        /// Returns Topic list
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{TopicDto},{TopicDto}]
        /// </returns>
        /// <example>
        /// GET: api/Topic/ListTopics - > [{TopicDto},{TopicDto}]
        /// </example>

        [HttpGet(template:"ListTopics")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> ListTopics()
        {

            //useing the interface and service to fetch list of topics in the db
            IEnumerable<TopicDto> topicDtos = await _topicService.ListTopics();

            // Return a 200 OK response with the list of TopicDto objects
            return Ok(topicDtos);

        }

        /// <summary>
        /// Retrieves a specific topic by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the topic to retrieve.</param>
        /// <returns>
        /// Returns a 200 OK response with the TopicDto object if found.
        /// Returns a 404 Not Found response if no topic exists with the given ID.
        /// </returns>
        /// <remarks>
        /// This method calls the TopicService to fetch a topic by ID.
        /// If the topic is found, it is returned in the response.
        /// If no topic matches the given ID, a NotFound response is returned.
        /// </remarks>
        /// <example>
        /// GET: api/Topic/2 -> Returns the topic with ID 2.
        /// GET: api/Topic/99 -> Returns 404 Not Found if topic 99 doesn't exist.
        /// </example>

        //[HttpGet(template:"GetTopicById/{id}")]

        [HttpGet("GetTopicById/{id}")]

        public async Task<ActionResult<ServiceResponse>> GetTopicById(int id)
        {
            var response = await _topicService.GetTopicById(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);


        }

        [HttpGet("GetTopicsByCategoryId/{id}")]

        public async Task<IActionResult> GetTopicsByCategoryId(int id)
        {
            var response = await _topicService.ListTopicsByCategoryId(id);
            if(response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);
        }






        /// <summary>
        /// Add new topic to the database
        /// </summary>
        /// <param name="createTopicDto"> pass the required information to add a new topic (TopicTitle,
        /// TopicDescription, CreatedAt, and Category
        /// </param>
        /// <example>
        /// POST api/Topic/Add
        /// </example>
        /// <returns>
        /// 201 Created
        /// Location: api/topic/find/{topicId}
        /// or
        /// 404 Not Found
        /// </returns>

        [HttpPost(template: "Add")]

        public async Task<ActionResult<Topic>> AddNewTopic(CreateTopicDto createTopicDto)
        {
            ServiceResponse response = await _topicService.AddNewTopic(createTopicDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Topics/GetTopicById/{response.CreatedId}",response);


            


        }


        /// <summary>
        /// Updates an existing topic based on the provided topic ID and update details.
        /// </summary>
        /// <param name="id">The ID of the topic to update (from the URL).</param>
        /// <param name="updateTopicDto">An object containing the new topic title, description, and category IDs.</param>
        /// <returns>
        /// Returns:
        /// 
        /// - 404 NotFound if the topic doesn't exist.
        /// - 500 InternalServerError if an error occurs during update.
        /// - 200 OK with a success response if the update succeeds.
        /// </returns>
        /// <remarks>
        /// This endpoint updates the topic's title, description, and category links.
        /// </remarks>


        [HttpPut(template:"UpdateTopic")]

        public async Task<IActionResult> UpdateTopic( UpdateTopicDto updateTopicDto)
        {
           
            // Call the service method to perform the update
            var response = await _topicService.UpdateTopic(updateTopicDto);

            // If the topic wasn't found, return 404 Not Found

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }

            // If there was a server error during update, return 500
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }
               

            // If everything went fine, return 200 OK with the response
            return Ok(response);
        }

        /// <summary>
        /// Deletes a topic by its ID.
        /// </summary>
        /// <param name="id">The ID of the topic to be deleted.</param>
        /// <returns>
        /// Returns 404 if not found, 500 if error occurs, and 200 if successfully deleted.
        /// </returns>
        [HttpDelete(template:"DeleteTopic/{id}")]

        public async Task<IActionResult> DeleteTopic(int id)
        {
            // Call the service to delete the topic
            var response = await _topicService.DeleteTopic(id);

            // Return 404 Not Found if topic doesn't exist
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            // Return 500 Internal Server Error if deletion failed
            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);


            // Return 200 OK with confirmation if deleted successfully
            return Ok(response);

        }


        [HttpGet(template:"SearchByKeyword")]

        public async Task<IActionResult>SeatchTopicsByKeyWord([FromQuery]string keyword)
        {
            var results = await _topicService.SearchTopicsByKeyword(keyword);
            if (!results.Any())
            {
                return NotFound($"No topics found matching keyword: {keyword}");
            }

            return Ok(results);

        }


    }
}
