using Trendy.Models;

namespace Trendy.Interfaces
{
    /// <summary>
    /// Defines the contract for category-related operations.
    /// </summary>
        public interface ICategoryService
        {

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="createCategoryDto">DTO containing category details.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> AddNewCategory(CreateCategoryDto createCategoryDto);

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve.</param>
        Task<CategoryDto> GetCategoryById(int categoryId);

        /// <summary>
        /// Lists all categories available in the system.
        /// </summary>
        Task<IEnumerable<CategoryDto>> ListCategories();

        /// <summary>
        /// Associates a topic with one or more categories.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <param name="categoryIds">List of category IDs to link to the topic.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> LinkCategoryToTopic(int topicId, List<int> categoryIds);


        /// <summary>
        /// Removes the association between a topic and a category.
        /// </summary>
        /// <param name="categoryId">The ID of the category to unlink.</param>
        /// <param name="topicId">The ID of the topic to unlink from the category.</param>
        /// <returns>
        /// A ServiceResponse indicating success ('Removed') or failure ('Error').
        /// </returns>
        /// <remarks>
        /// If the association exists, it will be removed.
        /// If the category or topic does not exist, an appropriate error response will be returned.
        /// </remarks>
        Task<ServiceResponse> UnlinkCategoryFromTopic(int topicId, int categoryId);

        /// <summary>
        /// Updates an existing category's name.
        /// </summary>
        /// <param name="categoryDto">DTO containing the updated category details.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> UpdateCategory(CategoryDto categoryDto);





        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> DeleteCategory(int id);


        Task<List<int>> GetCategoryIdsForTopic(int topicId);

    }
}
