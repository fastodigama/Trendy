﻿using System;
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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Adds a new comment to a topic.
        /// </summary>
        /// <param name="createCommentDto">The comment data to add.</param>
        /// <returns>A ServiceResponse indicating the result of the operation.</returns>
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto createCommentDto)
        {
            var response = await _commentService.AddComment(createCommentDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all comments associated with a specific topic.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <returns>A list of CommentDto objects for the given topic.</returns>
        [HttpGet("GetCommentsForTopic/{topicId}")]
        public async Task<IActionResult> GetCommentsForTopic(int topicId)
        {
            var comments = await _commentService.GetCommentsForTopic(topicId);
            return Ok(comments);
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to update.</param>
        /// <param name="updatedText">The new text of the comment.</param>
        /// <returns>A ServiceResponse indicating the result of the update.</returns>
        [HttpPut("UpdateComment/{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] string updatedText)
        {
            var response = await _commentService.UpdateComment(commentId, updatedText);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }
        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        [HttpDelete("DeleteComment/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var response = await _commentService.DeleteComment(commentId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Ok(response);
        }

    }
}