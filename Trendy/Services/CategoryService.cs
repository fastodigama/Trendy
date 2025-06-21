using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Trendy.Data;
using Trendy.Interfaces;
using Trendy.Models;

namespace Trendy.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> ListCategories()
        {
            //list all categories
            List<Category> Categories = await _context.Categories.ToListAsync();

            List<CategoryDto> categoryDtos = new List<CategoryDto>();

            foreach(Category category in Categories)
            {

                categoryDtos.Add(new CategoryDto()
                {
                    categoryId = category.CategoryId,
                    categoryName = category.CategoryName

                });
            }
            return categoryDtos;
        }

        public async Task<CategoryDto> GetCategoryById(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return null;
            }

            CategoryDto categoryDto = new CategoryDto()
            {
                categoryId = category.CategoryId,
                categoryName = category.CategoryName
            };

            return categoryDto;
        }

        /// <summary>
        /// Updates an existing category's name.
        /// </summary>
        /// <param name="categoryDto">The DTO containing the category ID and new name.</param>
        /// <returns>A ServiceResponse indicating the result of the operation.</returns>
        public async Task<ServiceResponse> UpdateCategory(CategoryDto categoryDto)
        {
            ServiceResponse response = new();

            var category = await _context.Categories.FindAsync(categoryDto.categoryId);
            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Category not found.");
                return response;
            }

            category.CategoryName = categoryDto.categoryName;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add("Category updated successfully.");
                response.CategoryData = new CategoryDto
                {
                    categoryId = category.CategoryId,
                    categoryName = category.CategoryName
                };
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating category.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }




        public async Task<ServiceResponse> AddNewCategory(CreateCategoryDto createCategoryDto)
            {
            ServiceResponse serviceResponse = new ServiceResponse();

            // Check if a category with the same name already exists (case-insensitive)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == createCategoryDto.categoryName.ToLower());

            if (existingCategory != null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Found;
                serviceResponse.Messages.Add("Category already exists.");
                serviceResponse.CategoryData = new CategoryDto
                {
                    categoryId = existingCategory.CategoryId,
                    categoryName = existingCategory.CategoryName
                };
                return serviceResponse;
            }

            //create new categ
            Category category = new Category()
            {
                CategoryName = createCategoryDto.categoryName,
               
            };
            
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Category.");
                serviceResponse.Messages.Add(ex.Message);

            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = category.CategoryId;
            serviceResponse.Messages.Add("Category created successfully.");
            // Include the new category details in the response
            serviceResponse.CategoryData = new CategoryDto
            {
                categoryId = category.CategoryId,
                categoryName = category.CategoryName
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            ServiceResponse response = new ServiceResponse();
            // topic must exist in the first place
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("No category with this id.");
                return response;
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the category");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            response.Messages.Add("Category deleted successfully.");
            return response;

        }


        public async Task<ServiceResponse> LinkCategoryToTopic(int topicId, List<int> categoryIds)
        {
            ServiceResponse serviceResponse = new();

            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Topic not found.");
                return serviceResponse;
            }

            foreach (int categoryId in categoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    serviceResponse.Messages.Add($"Category {categoryId} not found.");
                    continue;
                }

                bool exists = await _context.CategoryTopics
                    .AnyAsync(ct => ct.TopicId == topicId && ct.CategoryId == categoryId);

                if (exists)
                {
                    serviceResponse.Messages.Add($"Category {categoryId} is already linked.");
                    continue;
                }

                _context.CategoryTopics.Add(new CategoryTopic
                {
                    TopicId = topicId,
                    CategoryId = categoryId,
                    Category= category,
                    Topic = topic
                });
            }

            try
            {
                await _context.SaveChangesAsync();

                // Fetch updated topic with categories
                var updatedTopic = await _context.Topics
                    .Include(t => t.CategoryTopics)
                    .ThenInclude(ct => ct.Category)
                    .FirstOrDefaultAsync(t => t.TopicId == topicId);

                if (updatedTopic != null)
                {
                    serviceResponse.TopicData = new TopicDto
                    {
                        TopicId = updatedTopic.TopicId,
                        TopicTitle = updatedTopic.TopicTitle,
                        TopicDescription = updatedTopic.TopicDescription,
                        CreatedAt = updatedTopic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        TopicCategory = updatedTopic.CategoryTopics
                            .Select(ct => ct.Category.CategoryName)
                            .ToList()
                    };
                }


                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.Messages.Add("Categories linked successfully.");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error linking categories.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }




      
        public async Task<ServiceResponse> UnlinkCategoryFromTopic(int topicId, int categoryId)
        {
            ServiceResponse serviceResponse = new();

            // Check if the link exists
            var link = await _context.CategoryTopics
                .FirstOrDefaultAsync(ct => ct.TopicId == topicId && ct.CategoryId == categoryId);

            if (link == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("This topic is not linked with the specified category.");
                return serviceResponse;
            }

            _context.CategoryTopics.Remove(link);

            try
            {
                await _context.SaveChangesAsync();

                // Optionally return the updated topic info
                var updatedTopic = await _context.Topics
                    .Include(t => t.CategoryTopics)
                    .ThenInclude(ct => ct.Category)
                    .FirstOrDefaultAsync(t => t.TopicId == topicId);

                if (updatedTopic != null)
                {
                    serviceResponse.TopicData = new TopicDto
                    {
                        TopicId = updatedTopic.TopicId,
                        TopicTitle = updatedTopic.TopicTitle,
                        TopicDescription = updatedTopic.TopicDescription,
                        CreatedAt = updatedTopic.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        TopicCategory = updatedTopic.CategoryTopics
                            .Select(ct => ct.Category.CategoryName)
                            .ToList()
                    };
                }

                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
                serviceResponse.Messages.Add("Category successfully unlinked from topic.");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error occurred while unlinking.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }
        public async Task<List<int>> GetCategoryIdsForTopic(int topicId)
        {
            return await _context.CategoryTopics
                .Where(ct => ct.TopicId == topicId)
                .Select(ct => ct.CategoryId)
                .ToListAsync();
        }


    }
}
