using Microsoft.AspNetCore.SignalR;
using StepChat.Hubs;
using StepChat.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using StepChat.Contexts;
using Newtonsoft.Json.Linq;
using StepChat.Classes.Provider;
using StepChat.Classes.Auth;
using StepChat.Classes.Configuration;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net;
using System;
using StepChat.Services;


//                              :-= +*****************************************+=
//                          :+%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%
//                        -%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=
//                      :%@@@@@@#=-:..........................................              
//                     -@@@@@%=
//                    :@@@@@#                                                               
//                    #@@@@%                                                                
//                   :@@@@@-
//                   *@@@@@                                                                 
//                  .@@@@@=
//                  +@@@@@       .--------------------------.          :-:                  
//                  @@@@@+      =@@@@@@@@@@@@@@@@@@@@@@@@@@@@*        #@@@%.                
//                 =@@@@@.      +@@@@@@@@@@@@@@@@@@@@@@@@@@@@#       -@@@@@.                
//                 %@@@@*        .--------------------------:        %@@@@#                 
//                -@@@@@:                                           :@@@@@:                 
//                #@@@@#                                            #@@@@%                  
//               .@@@@@-                                           .@@@@@=
//               *@@@@%                                            *@@@@@                   
//              .@@@@@=                                           .@@@@@+
//              +@@@@@                                            +@@@@@.                   
//              @@@@@+                                           -@@@@@+
//             =@@@@@.                                          +@@@@@#                     
//             %@@@@*                                       .- *@@@@@@*
//            -@@@@@.   =#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@@@@@@@@@#:                       
//            *@@@@#   :@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%+:                         
//            .*%% *.    =#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#*+-.                            


namespace StepChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddTransient<IConfigService>(param => new ConfigService("appsettings.json"));

            builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddSingleton<MessengerDataDbContext>();
            builder.Services.AddSingleton<EmailSender>();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            // если запрос направлен хабу
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                            {
                                // получаем токен из строки запроса
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            builder.Services.AddDataProtection()
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

            builder.Services.AddSession();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Authorization}/{action=LoginPage}/{id?}");
            app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }
    }
}