using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public TodoController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            return Ok(await _dbContext.Items.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ItemData itemData)
        {

            if (ModelState.IsValid)
            {
                await _dbContext.AddAsync(itemData);
                await _dbContext.SaveChangesAsync();

                return Created("GetItem", itemData);
            }

            return new JsonResult("Invlid Data") { StatusCode = 500 };

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item != null)
                return Ok(item);

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemData itemData)
        {
            if (id != itemData.Id)
                return BadRequest();

            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                return NotFound();

            item.Title = itemData.Title;
            item.Description = itemData.Description;
            item.Done = itemData.Done;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                return BadRequest();

            _dbContext.Remove(item);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        } 
        
    }
}
