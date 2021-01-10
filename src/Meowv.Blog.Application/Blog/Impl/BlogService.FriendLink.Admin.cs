﻿using Meowv.Blog.Domain.Blog;
using Meowv.Blog.Dto.Blog;
using Meowv.Blog.Dto.Blog.Params;
using Meowv.Blog.Extensions;
using Meowv.Blog.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meowv.Blog.Blog.Impl
{
    public partial class BlogService
    {
        /// <summary>
        /// Create a friendLink.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BlogResponse> CreateFriendlinkAsync(CreateFriendLinkInput input)
        {
            var response = new BlogResponse();

            var friendLink = await _friendLinks.FindAsync(x => x.Name == input.Name);
            if (friendLink is not null)
            {
                response.IsFailed($"The friendLink:{input.Name} already exists.");
                return response;
            }

            await _friendLinks.InsertAsync(new FriendLink
            {
                Name = input.Name,
                Url = input.Url
            });

            return response;
        }

        /// <summary>
        /// Delete friendLink by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogResponse> DeleteFriendlinkAsync(string id)
        {
            var response = new BlogResponse();

            var friendLink = await _friendLinks.FindAsync(id.ToObjectId());
            if (friendLink is null)
            {
                response.IsFailed($"The friendLink id not exists.");
                return response;
            }

            await _friendLinks.DeleteAsync(id.ToObjectId());

            return response;
        }

        /// <summary>
        /// Update friendLink by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BlogResponse> UpdateFriendlinkAsync(string id, UpdateFriendLinkInput input)
        {
            var response = new BlogResponse();

            var friendLink = await _friendLinks.FindAsync(id.ToObjectId());
            if (friendLink is null)
            {
                response.IsFailed($"The friendLink id not exists.");
                return response;
            }

            friendLink.Name = input.Name;
            friendLink.Url = input.Url;

            await _friendLinks.UpdateAsync(friendLink);

            return response;
        }

        /// <summary>
        /// Get admin friendLink list.
        /// </summary>
        /// <returns></returns>
        public async Task<BlogResponse<List<GetAdminFriendLinkDto>>> GetAdminFriendlinksAsync()
        {
            var response = new BlogResponse<List<GetAdminFriendLinkDto>>();

            var friendLinks = await _friendLinks.GetListAsync();

            var result = ObjectMapper.Map<List<FriendLink>, List<GetAdminFriendLinkDto>>(friendLinks);

            response.Result = result;
            return response;
        }
    }
}