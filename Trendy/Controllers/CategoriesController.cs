using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trendy.Interfaces;
using Trendy.Models;

namespace Trendy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly TrendyDbContext _context;

        public CategoriesController(ICategoryService CategoryService)
        {
            _categoryService = CategoryService;
        }





        /// <summary>
        /// Retrieves a list of all available categories.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of CategoryDto objects representing all categories.
        /// </returns>

        [HttpGet(template:"ListCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> ListCategories()
        {
            IEnumerable<CategoryDto> categoryDtos = await _categoryService.ListCategories();
            return Ok(categoryDtos);
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>200 OK with CategoryDto, or 404 if not found.</returns>
        
        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound($"No category found with ID {id}");

            return Ok(category);
        }

        /// <summary>
        /// Adds a new category to the system.
        /// </summary>
        /// <param name="categoryDto">The DTO containing category details.</param>
        /// <returns>201 Created with the new category ID, or 500 if there's an error.</returns>
        
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            var response = await _categoryService.AddNewCategory(createCategoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);

        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>
        /// A ServiceResponse indicating:
        /// - 200 OK if the category was deleted successfully.
        /// - 404 Not Found if the category doesn't exist.
        /// - 500 Internal Server Error if deletion failed.
        /// </returns>
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategory(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response);

            return Ok(response);
        }

        /// <summary>
        /// Links one or more categories to a specific topic.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <param name="categoryIds">List of category IDs to link to the topic.</param>
        /// <returns>A ServiceResponse indicating the result.</returns>
        [HttpPost("LinkCategoryToTopic/{topicId}")]
        public async Task<IActionResult> LinkCategoryToTopic(int topicId, [FromBody] List<int> categoryIds)
        {
            var response = await _categoryService.LinkCategoryToTopic(topicId, categoryIds);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }

        /// <summary>
        /// Unlinks a category from a topic.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        [HttpPost("UnlinkCategoryFromTopic")]
        public async Task<IActionResult> UnlinkCategoryFromTopic(int topicId, int categoryId)
        {
            var response = await _categoryService.UnlinkCategoryFromTopic(topicId, categoryId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="categoryDto">The updated category data.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto categoryDto)
        {
            var response = await _categoryService.UpdateCategory(categoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }



    }
}
