using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UWCBridgeServer.Models;
using UWPBridge.Shared;

namespace UWCBridgeServer.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private IConfiguration _config;
        private readonly UserContext _context;
        /// <summary>
        /// コンストラクタにはサービスにAddしたインジェクション対象を引数にできる.
        /// </summary>
        /// <param name="config">設定</param>
        /// <param name="context">データベースコンテキスト</param>
        public TokenController(IConfiguration config, UserContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();

            // 初期起動時はユーザが無いためログインできない
            if (_context.Users.Count() == 0)
            {
                // デフォルトユーザアカウントを追加する
                _context.Users.Add(new User { Name = "admin", Password = "admin", Email="admin@admin", BirthDate=DateTime.Now });
                _context.SaveChanges();
            }

            var user = Authenticate(login);

            if(user!= null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }
        /// <summary>
        /// トークンを作る.
        /// </summary>
        /// <param name="user">データベースのユーザレコード</param>
        /// <returns>トークン</returns>
        private string BuildToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.BirthDate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// ユーザ認証する.
        /// </summary>
        /// <param name="login">ログインフォームの入力値</param>
        /// <returns>認証した場合はデータベースのユーザレコード</returns>
        private User Authenticate(LoginModel login)
        {
            try
            {
                var user = _context.Users.First(e => e.Name.Equals(login.Username));
                if (user.Password.Equals(login.Password))
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}