using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // Mapping objects/ members without automapper
            // return await _context.Users
            //         .Where(x => x.UserName == username)
            //         .Select(user => new MemberDto
            //         {
            //             Id = user.Id,
            //             UserName = user.UserName,


            //         }).SingleOrDefaultAsync();

            return await _context.Users
                    .Where(x => x.UserName == username)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
           var query =  _context.Users.AsQueryable();

           query = query.Where(u => u.UserName != userParams.CurrentUserName);
           query = query.Where(u => u.Gender == userParams.Gender);

            //minDob gives value of the youngest year of a person -- 1997 born person is younger than 1980. minDob has 1997
            // Today's year - maxage - 1 -- substracting the present year
            //maxDob gives the oldest year of a person -- 1980 
           var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
           var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));


            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u=> u.AccountCreated),
                _ => query.OrderByDescending(u=> u.LastActive)
            };
            
            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userParams.pageNumber,
                 userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id); 
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)            
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username )
                .Select(x => x.Gender).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0;
        // }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}