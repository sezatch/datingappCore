using System;
using System.Collections.Generic;
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
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }
        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                        .OrderByDescending(x=>x.MessageSent)
                        .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u=> u.RecipientUsername == messageParams.Username 
                                            && u.RecipientDeleted == false),
                "Outbox" => query.Where(u=> u.SenderUsername == messageParams.Username
                                            && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username 
                                            && u.RecipientDeleted == false && u.DateRead == null )
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                            .CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
           var query =  _context.Messages
                                .Where(
                                    m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false &&
                                    m.SenderUsername == RecipientUsername || 
                                    m.RecipientUsername == RecipientUsername && m.SenderDeleted == false &&
                                    m.SenderUsername == currentUsername
                                )
                                .OrderBy(m=> m.MessageSent)
                                .AsQueryable();
        // Using projection for optimising queries
        //    var messages = await _context.Messages
        //                         .Include(u => u.Sender).ThenInclude(p => p.Photos)
        //                         .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        //                         .Where(
        //                             m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false &&
        //                             m.SenderUsername == RecipientUsername || 
        //                             m.RecipientUsername == RecipientUsername && m.SenderDeleted == false &&
        //                             m.SenderUsername == currentUsername
        //                         )
        //                         .OrderBy(m=> m.MessageSent)
        //                         .ToListAsync();

            var unreadMessages = query.Where(m => m.DateRead == null 
                                            && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                // await _context.SaveChangesAsync();
            }

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);   
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0; 
        // }
    }
}