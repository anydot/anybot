using System;

namespace RoumenBot
{
    public sealed class RoumenImage<T> : IEquatable<RoumenImage<T>> where T : ITag
    {
        public RoumenImage(string imageUrl, string description, string commentLink)
        {
            ImageUrl = imageUrl;
            Description = description;
            CommentLink = commentLink;
        }

        public string ImageUrl { get; }
        public string Description { get; }
        public string CommentLink { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RoumenImage<T>);
        }

        public bool Equals(RoumenImage<T>? other)
        {
            return other != null &&
                ImageUrl == other.ImageUrl &&
                Description == other.Description &&
                CommentLink == other.CommentLink;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ImageUrl, Description, CommentLink);
        }

        public override string ToString()
        {
            return $"ImageUrl {ImageUrl}\nDescription {Description}\nCommentLink {CommentLink}";
        }
    }
}
