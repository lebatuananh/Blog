﻿using System.Text.Json.Serialization;

namespace Meowv.Blog.Dto.Blog
{
    public class PostBriefDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [JsonIgnore]
        public int Year { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }
    }
}