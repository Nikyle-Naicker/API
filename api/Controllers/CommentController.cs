using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments(){
            var comments = await _commentRepo.GetAllCommentsAsync();

            var commentDto = comments.Select(x => x.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null){
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("/{stockId}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, CreateCommentDto commentDto){
            if(!await _stockRepo.StockExists(stockId)){
                return BadRequest("Stock does not exist");
            }

        var commentModel = commentDto.ToCommentFromCreate(stockId);
        await _commentRepo.CreateCommentAsync(commentModel);
        return CreatedAtAction(nameof(GetById), new { id = commentModel.Id}, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateCommentDto){
            var comment = await _commentRepo.UpdateCommentAsync(id, updateCommentDto.ToCommentFromUpdate());

            if (comment == null){
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());

        }

    }
}