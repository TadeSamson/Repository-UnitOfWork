using System;
using System.Collections.Generic;
using SampleArchitecture.Core.Interfaces;

namespace SampleArchitecture.Core.Models
{
    public partial class BlogPost:IEntity
    {
        public BlogPost()
        {
            this.BlogPostTagIds = new List<string>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public string Intro { get; set; }
        public string Content { get; set; }
        public string BlogId { get; set; }
        public string PostedDate { get; set; }
        public string PublishedDate { get; set; }
        public string FeaturedImage { get; set; }
        public string FeaturedImageThumbnail { get; set; }
        public string HeaderImage { get; set; }
        public string HeaderImageThumbnail { get; set; }
        public virtual List<string> BlogPostTagIds { get; set; }
    }
}
