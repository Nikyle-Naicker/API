using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService; 

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager, IFMPService fmpService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        //Displays all User Comments

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllComments([FromQuery]CommentQueryObject queryObject){

            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var comments = await _commentRepo.GetAllCommentsAsync(queryObject);

            var commentDto = comments.Select(x => x.ToCommentDto());

            return Ok(commentDto);
        }

        //Displays user comment by id

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null){
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        //Creates new user comment

        [HttpPost]
        [Route("{symbol:alpha}")]
        public async Task<IActionResult> CreateComment([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if(stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if(stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                else
                {
                    await _stockRepo.CreateStock(stock);
                }
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);


            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserId = appUser.Id;
            await _commentRepo.CreateCommentAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id}, commentModel.ToCommentDto());
        }

        //Updates user comment

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateCommentDto){

            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.UpdateCommentAsync(id, updateCommentDto.ToCommentFromUpdate());

            if (comment == null){
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());

        }

        //Deletes user comment

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id){
            
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            var commentModel = await _commentRepo.DeleteCommentAsync(id);

            if (commentModel == null){
                return NotFound("Comment not found");
            }

            return Ok(commentModel);
        }

    }
}