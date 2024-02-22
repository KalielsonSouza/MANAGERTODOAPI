using MANAGERTODOAPI.Utils;
using MANAGERTODOAPI.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project1.Data;
using Project1.Models;
using Project1.Utils;
using Project1.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Project1.Controllers
{
    [ApiController]
    [Route("v1")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetAsync(
                        [FromServices] AppDbContext context)
        {
            var CoberturauserList = await context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(CoberturauserList);
        }
        [HttpGet]
        [Route("Tasks")]
        public async Task<IActionResult> GetTaskAsync(
                       [FromServices] AppDbContext context)
        {
            var taskList = await context
                .Tarefas
                .AsNoTracking()
                .ToListAsync();

            return Ok(taskList);
        }
        [HttpPut]
        [Route("Tasks/{taskId}")]
        public async Task<IActionResult> UpdateTaskAsync(
        [FromServices] AppDbContext context,
        [FromRoute] Guid taskId,
        [FromBody] string taskName)
        {
            var task = await context.Tarefas.FindAsync(taskId);

            if (task == null || task.Dono == null)
            {
                return NotFound();
            }

            if (task.DataCriacao > DateTime.Now)
            {
                return Conflict("Data de criação inválida");
            }
            else
            {
                Dictionary<TaskStatus, Action> taskActions = new Dictionary<TaskStatus, Action>
                {
                    { TaskStatus.Faulted, () => HandleFaulted() },
                    { TaskStatus.Canceled, () => HandleCanceled() },
                    { TaskStatus.RanToCompletion, () => HandleRanToCompletion() },
                    { TaskStatus.Created, () => HandleCreated() },
                    { TaskStatus.Running, () => HandleRunning() },
                    { TaskStatus.WaitingToRun, () => HandleWaitingToRun() },
                    { TaskStatus.WaitingForChildrenToComplete, () => HandleWaitingForChildrenToComplete() },
                    { TaskStatus.WaitingForActivation, () => HandleWaitingForActivation() }
                };
            }

            task.Nome = taskName;
            

            await context.SaveChangesAsync();

            return Ok(task);
        }
        void HandleFaulted()
        {
            // Lidar com o estado Faulted, se necessário
        }

        void HandleCanceled()
        {
            // Lidar com o estado Canceled, se necessário
        }

        void HandleRanToCompletion()
        {
            // Lidar com o estado RanToCompletion, se necessário
        }
        void HandleCreated()
        {
            // Lidar com o estado Created, se necessário
        }

        void HandleRunning()
        {
            // Lidar com o estado Running, se necessário
        }

        void HandleWaitingToRun()
        {
            // Lidar com o estado WaitingToRun, se necessário
        }

        void HandleWaitingForChildrenToComplete()
        {
            // Lidar com o estado WaitingForChildrenToComplete, se necessário
        }

        void HandleWaitingForActivation()
        {
            // Lidar com o estado WaitingForActivation, se necessário
        }

        void HandleUnknownStatus()
        {
            // Lidar com qualquer outro estado desconhecido, se necessário
        }
        [HttpPost]
        [Route("Users")]
        public async Task<IActionResult> PostAsync(
                       [FromServices] AppDbContext context,
                       [FromBody] CreateUserVM userVM)
        {

            PasswordEncryptor passwordEncryptor = new PasswordEncryptor();
            if (!ModelState.IsValid)
                return BadRequest();
            bool emailExists = await context.Users.AnyAsync(u => u.Email == userVM.Email);
            if (emailExists)
                return Conflict("Já existe um usuário com o e-mail fornecido.");            
            User user = new User
            {
                Email = userVM.Email,
                FullName = userVM.FullName,
                Password = passwordEncryptor.HashPassword(userVM.Password),
                //Password = userVM.Password,
                PermissionType = "Common"
            };          
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpPost]
        [Route("UsersLogin")]
        public async Task<IActionResult> GetAsyncLogin(
                    [FromServices] AppDbContext context,
                    [FromBody] LoginUserVM userLoginVM)
        {

            var user = await context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == userLoginVM.Email);
            PasswordEncryptor passwordEncryptor = new PasswordEncryptor();
            if (!ModelState.IsValid)
                return BadRequest();
            if (passwordEncryptor.VerifyPassword(userLoginVM.Password, user.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(servicesKey.jwtKey());
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "UsersLogin",
                    Audience = "user",
                    Subject = new ClaimsIdentity(new[]
                    {                       
                       new Claim(ClaimTypes.Email, user.Email),
                       new Claim(ClaimTypes.Role, user.PermissionType)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }
            return BadRequest();

        }

        [HttpPost]
        [Route("TokenVerification")]
        public IActionResult TokenVerification([FromHeader(Name = "Authorization")] string token)
        {
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(servicesKey.jwtKey());

                try
                {
                    tokenHandler.ValidateToken(token.Substring(7), new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = "UsersLogin",
                        ValidateAudience = true,
                        ValidAudience = "user",
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var email = jwtToken.Claims.FirstOrDefault(e => e.Type == "email").Value;

                    return Ok("{\n \"Authorized\": true " +
                        "\n \"Email\":"+email+" \n}");
                }
                catch (Exception ex)
                {                  
                    return Unauthorized("Invalid or expired token.");
                }
            }

            return Unauthorized("Token is required.");
        }


    }
}
