using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageSharing.Data;
using UserAuth.Data;

namespace ImageSharingWithUsers.Models
{
    public class ImageViewModel
    {
        public Image Image { get; set; }
        public User User { get; set; }
        public bool Liked { get; set; }
        public int LikesCount { get; set; }
    }
}