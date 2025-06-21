using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trendy.Interfaces;
using Trendy.Models;
using Trendy.Services;

namespace Trendy.Controllers
{
    public class TopicPageController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;

        public TopicPageController(ITopicService topicService, ICategoryService categoryService, ICommentService commentService)
        {
            _topicService = topicService;
            _categoryService = categoryService;
            _commentService = commentService;
        }

        // Redirect to List by default
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        //GET: /TopicPage/List

        public async Task<IActionResult> List()
        {
            var topics = await _topicService.ListTopics();
            return View(topics);
        }

        //GET:/TopicPage/Details/3
        public async Task<IActionResult> Details(int id)
        {
            var response = await _topicService.GetTopicById(id);

            if(response.TopicData == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Topic not found." } });

            }
            return View (response.TopicData);
        }

        //GET:/TopicPage/New
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New()
        {

            var categories = await _categoryService.ListCategories();
            ViewBag.CategoryList = categories; //send to the view

            return View();
        }

        // POST: /TopicPage/Add
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(CreateTopicDto createTopicDto)
        {
            // Get the loggedin user Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _topicService.AddNewTopic(createTopicDto, userId); // passing both args

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _topicService.GetTopicById(id);

            if (response.TopicData == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Topic not found." } });
            }

            var categories = await _categoryService.ListCategories();
            ViewBag.CategoryList = categories;

            var topicDto = new UpdateTopicDto
            {
                TopicId = response.TopicData.TopicId,
                TopicTitle = response.TopicData.TopicTitle,
                TopicDescription = response.TopicData.TopicDescription,
                CategoryIds = await _categoryService.GetCategoryIdsForTopic(response.TopicData.TopicId)
            };

            return View(topicDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var response = await _topicService.GetTopicById(id);

            if (response.TopicData == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Topic not found." } });
            }

            return View(response.TopicData);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _topicService.DeleteTopic(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateCommentDto createCommentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _commentService.AddComment(createCommentDto, userId);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = createCommentDto.TopicId });
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }


    }
}
