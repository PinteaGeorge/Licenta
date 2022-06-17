using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _cotext;
        public LikesRepository(DataContext context)
        {
            _cotext = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserID, int likedUserId)
        {
            return await _cotext.Likes.FindAsync(sourceUserID, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _cotext.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _cotext.Likes.AsQueryable();
            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(u => u.SourceUserId == likesParams.UserId);
                users = likes.Select(u => u.LikedUser);
            }
            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(u => u.LikedUserId == likesParams.UserId);
                users = likes.Select(u => u.SourceUser);
            }
            var likedUsers = users.Select(x => new LikeDto
            {
                Username = x.UserName,
                KnownAs = x.KnownAs,
                Age = x.DateOfBirth.CalculateAge(),
                PhotoUrl = x.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = x.City,
                Id = x.Id

            });
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNo, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _cotext.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
