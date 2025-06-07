using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trendy.Models;

namespace Trendy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly TrendyDbContext _context;

        public TopicsController(TrendyDbContext context)
        {
            _context = context;
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
            List<Topic> topics = await _context.Topics
                .Include(t => t.Categories)
                .ToListAsync();


            List<TopicDto> topicDtos = new List<TopicDto>();

            //foreach topic in the database

            foreach(Topic topic in topics)
            {
                //create new instance of topicDto, add to list
                topicDtos.Add(new TopicDto()
                {
                    TopicId = topic.TopicId,
                    TopicCategory = topic.Categories.Select(c => c.CategoryName).ToList(),
                    TopicDescription= topic.TopicDescription,
                    TopicTitle = topic.TopicTitle,
                    CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd")

                });
            }
            //return 200 OK with topicDtos
            return Ok(topicDtos);

        }

        /// <summary>
        /// returns single topic specified by {id}, represented by TopicDto
        /// </summary>
        /// <param name="id">Tipic id</param>
        /// <returns>
        /// 200 OK
        /// {topicDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Topic/FindTopic/1 - {TopicDto}
        /// </example>
        [HttpGet(template:"FindTopic/{id}")]

        public async Task<ActionResult<TopicDto>> FindTopic(int id)
        {
            //include will join (topic) with Categories

            var topic = await _context.Topics
                .Include(t => t.Categories)
                .FirstOrDefaultAsync(t => t.TopicId == id);
            //if topic could not be located, return 404 Not Found
            if(topic == null)
            {
                return NotFound();
            }

            //create instance of TopicDto
            TopicDto topicDto = new TopicDto()
            {
                TopicId = topic.TopicId,
                TopicCategory = topic.Categories.Select(t => t.CategoryName).ToList(),
                TopicDescription = topic.TopicDescription,
                TopicTitle = topic.TopicTitle,
                CreatedAt = topic.CreatedAt.ToString("yyyy-MM-dd")

            };
            //return 200 Ok with topicDto
            return Ok(topicDto);
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
        
        [HttpPost(template:"Add")]

        public async Task<ActionResult<Topic>> AddNewTopic(CreateTopicDto createTopicDto)
        {
            //Create anew topic object
            var topic = new Topic
            {
                TopicTitle = createTopicDto.TopicTitle,
                TopicDescription = createTopicDto.TopicDescription,
                CreatedAt = DateTime.Now

            };
            var categories = await _context.Categories
                //SOURCE: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select?view=net-9.0
                .Where(c => createTopicDto.TopicCategory.Contains(c.CategoryId))
                .ToListAsync();
            topic.Categories = categories;

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return Created($"api/Topics/{topic.TopicId}", createTopicDto);


        }

        // GET: api/Topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
        {
            return await _context.Topics.ToListAsync();
        }

        // GET: api/Topics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }

            return topic;
        }

        // PUT: api/Topics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopic(int id, Topic topic)
        {
            if (id != topic.TopicId)
            {
                return BadRequest();
            }

            _context.Entry(topic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Topics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Topic>> PostTopic(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTopic", new { id = topic.TopicId }, topic);
        }

        // DELETE: api/Topics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.TopicId == id);
        }
    }
}
