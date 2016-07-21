using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageSharing.Data;

namespace ImageSharingWithUsers.Models
{
    public class MostViewed
    {
        public Image Image { get; set; }
        public int LikesCount { get; set; }
    }
}