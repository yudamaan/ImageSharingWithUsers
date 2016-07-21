using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageSharing.Data;
using UserAuth.Data;

namespace ImageSharingWithUsers.Models
{
    public class IndexViewModel
    {
       public IEnumerable<Image> MostViewed { get; set; }
        public IEnumerable<Image> MostRecent { get; set; }
        public IEnumerable<ImageWithLikes> MostLiked { get; set; }
       // public IEnumerable<MostViewed> MostViewed { get; set; }
       // public IEnumerable<MostRecent> RecentImages { get; set; }
        public string Url { get; set; }
        public User User { get; set; }
    }
}