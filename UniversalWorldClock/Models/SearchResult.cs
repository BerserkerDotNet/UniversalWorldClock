using System;

namespace UniversalWorldClock.Models
{
    public sealed class SearchResult
    {
        public int Id { get; set; }
        public Uri Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
    }
}