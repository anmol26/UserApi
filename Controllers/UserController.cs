using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Services;

namespace UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_userService.GetAllUsers());
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
            return BadRequest(new { Message = "Name is required" });

        var newUser = _userService.AddUser(user);
        
        return CreatedAtAction(nameof(GetUser), new { id = newUser?.Id }, newUser);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        bool updated = _userService.UpdateUser(id, updatedUser);

        if (!updated)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        bool deleted = _userService.DeleteUser(id);

        if (!deleted)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "User deleted successfully" });
    }
}
