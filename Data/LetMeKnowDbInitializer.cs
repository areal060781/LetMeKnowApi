using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.Model;
using LetMeKnowApi.Core;

namespace LetMeKnowApi.Data
{
    public class LetMeKnowDbInitializer
    {
        private static LetMeKnowContext context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = (LetMeKnowContext)serviceProvider.GetService(typeof(LetMeKnowContext));

            InitializeLetMeKnow();
        }

        private static void InitializeLetMeKnow()
        {
            if (!context.Areas.Any())
            {
                var areas = new Area[]{
                    new Area{ Name = "51 area" },
                    new Area{ Name = "Unauthorized area" },
                    new Area{ Name = "Unknown area" },
                    new Area{ Name = "Spatial area" },               
                };

                foreach (Area s in areas){
                    context.Areas.Add(s);
                }

                context.SaveChanges();
            }

            if (!context.Roles.Any())
            {
                var roles = new Role[]{
                    new Role{ Name = "Administrator" },
                    new Role{ Name = "Really charming rol" }                              
                };

                foreach (Role s in roles){
                    context.Roles.Add(s);
                }

                context.SaveChanges();
            }

            if(!context.Users.Any())
            {
                string salt_01 = Extensions.CreateSalt();
                string password_01 = Extensions.EncryptPassword("Aida123", salt_01);
                User user_01 = new User { UserName = "Aida", PasswordHash = password_01, Salt = salt_01, Email = "areal060781@gmail.com"};

                string salt_02 = Extensions.CreateSalt();
                string password_02 = Extensions.EncryptPassword("Salvador123", salt_02);
                User user_02 = new User { UserName = "Salvador", PasswordHash = password_02, Salt = salt_02, Email = "jspl840210@gmail.com" };

                string salt_03 = Extensions.CreateSalt();
                string password_03 = Extensions.EncryptPassword("Sandra123", salt_03);
                User user_03 = new User { UserName = "Sandra", PasswordHash = password_03, Salt = salt_03, Email = "sandra_something@gmail.com" };

                string salt_04 = Extensions.CreateSalt();
                string password_04 = Extensions.EncryptPassword("Hugo123", salt_04);
                User user_04 = new User { UserName = "Hugo", PasswordHash = password_04, Salt = salt_04, Email = "ouragan1810@gmail.com" };

                string salt_05 = Extensions.CreateSalt();
                string password_05 = Extensions.EncryptPassword("Carmen123", salt_05);
                User user_05 = new User { UserName = "Carmen", PasswordHash = password_05, Salt = salt_05, Email = "carmiux@gmail.com" };

                context.Users.Add(user_01); context.Users.Add(user_02); 
                context.Users.Add(user_03); context.Users.Add(user_04);
                context.Users.Add(user_05);                

                context.SaveChanges();
            }

            if (!context.UserRoles.Any())
            {
                var userRoles = new UserRole[]{
                    new UserRole{ UserId = 1, RoleId = 1},
                    new UserRole{ UserId = 2, RoleId = 2},
                    new UserRole{ UserId = 3, RoleId = 2},
                    new UserRole{ UserId = 4, RoleId = 2},
                    new UserRole{ UserId = 5, RoleId = 2},                              
                };

                foreach (UserRole s in userRoles){
                    context.UserRoles.Add(s);
                }

                context.SaveChanges();
            }

            if (!context.Suggestions.Any())
            {
                Suggestion sugggestion_01 = new Suggestion{
                    Title = "Nice", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 1,
                    CreatorId = 2
                };

                Suggestion sugggestion_02 = new Suggestion{
                    Title = "Thanks", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 2,
                    CreatorId = 3
                };

                Suggestion sugggestion_03 = new Suggestion{
                    Title = "Yay", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 3,
                    CreatorId = 4  
                };

                Suggestion sugggestion_04 = new Suggestion{
                    Title = "Horrible", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 1,
                    CreatorId = 2
                };

                Suggestion sugggestion_05 = new Suggestion{
                    Title = "Saaaad", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Resuelto, 
                    AreaId = 2,
                    CreatorId = 3
                };

                Suggestion sugggestion_06 = new Suggestion{
                    Title = "Boring", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 3,
                    CreatorId = 4  
                };

                Suggestion sugggestion_07 = new Suggestion{
                    Title = "Amazing", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Nuevo, 
                    AreaId = 1,
                    CreatorId = 1  
                };

                Suggestion sugggestion_08 = new Suggestion{
                    Title = "Fantastic", 
                    Content = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..", 
                    Image = "http://photos.google.com/user/photo1.jpg", 
                    Status = SuggestionStatus.Resuelto, 
                    AreaId = 3,
                    CreatorId = 1  
                };   

                context.Suggestions.Add(sugggestion_01); context.Suggestions.Add(sugggestion_02);
                context.Suggestions.Add(sugggestion_03); context.Suggestions.Add(sugggestion_04);
                context.Suggestions.Add(sugggestion_05); context.Suggestions.Add(sugggestion_06);
                context.Suggestions.Add(sugggestion_07); context.Suggestions.Add(sugggestion_08);

                context.SaveChanges();
            }
        }
    }
}
