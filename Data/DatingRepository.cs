using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipentId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipentId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where( x=> x.UserId == userId ).FirstOrDefaultAsync(x => x.IsMain);

        }

        public async Task<Photo> GetPhoto(int id)
        {
           var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
           return photo;
        }

        public async Task<User> GetUser(int id)
        {
           return await  _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).OrderByDescending(s=> s.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId );
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                users = users.Where( p=> p.DateOfBirth.CalculateAge() >= userParams.MinAge && p.DateOfBirth.CalculateAge() <= userParams.MaxAge );
            }
            
            if(userParams.Likers)
            {
                users = users.Where(u => u.Liker.Any(x => x.LikerId == u.Id));
            }
            if(userParams.Likees)
            {
                users = users.Where(u => u.Likee.Any(x => x.LikeeId == u.Id));
            }
            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                         users = users.OrderByDescending( p=> p.Created);
                         break;
                    default:
                             users = users.OrderByDescending( p=> p.LastActive);
                             break;
                }
            }

            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
           return await _context.SaveChangesAsync() >0;
        }
    }
}